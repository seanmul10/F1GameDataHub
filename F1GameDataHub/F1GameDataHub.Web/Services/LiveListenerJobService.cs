using System.Net;
using System.Net.Sockets;
using System.Text;
using OvertakeTest;
using PacketRecording;

namespace F1GameDataHub.Web.Services;

public sealed class LiveListenerJobService(IConfiguration configuration, JobStatusStore statusStore)
{
    private readonly int _udpPort = configuration.GetValue("Listener:UdpPort", 20778);
    private readonly bool _recordRawDump = configuration.GetValue("Listener:RecordRawDump", true);
    private readonly string _dumpDirectory = configuration["Listener:DumpDirectory"]
        ?? @"Logs\PacketDumps";
    private readonly object _gate = new();
    private CancellationTokenSource? _cts;
    private Task? _listenerTask;
    private Task? _monitorTask;

    public bool TryStart(out string message)
    {
        lock (_gate)
        {
            if (_listenerTask is { IsCompleted: false })
            {
                message = "Listener is already running.";
                return false;
            }

            _cts = new CancellationTokenSource();
            statusStore.SetListenerStatus(JobRunState.Running, $"Listening on UDP {_udpPort}...", 0, 0, null, null);
            _listenerTask = Task.Run(() => RunListenerAsync(_cts.Token));
            message = "Listener started.";
            return true;
        }
    }

    public bool TryStop(out string message)
    {
        lock (_gate)
        {
            if (_listenerTask is null || _listenerTask.IsCompleted || _cts is null)
            {
                message = "Listener is not running.";
                return false;
            }

            _cts.Cancel();
            message = "Stop requested.";
            return true;
        }
    }

    private async Task RunListenerAsync(CancellationToken cancellationToken)
    {
        RawPacketDumpWriter? dumpWriter = null;
        try
        {
            using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            using var udpClient = new UdpClient(new IPEndPoint(IPAddress.Any, _udpPort));
            dumpWriter = CreateDumpWriter();
            using (dumpWriter)
            {
            var statusMessage = dumpWriter is null
                ? $"Listening on UDP {_udpPort}"
                : $"Listening on UDP {_udpPort} | Dump: {dumpWriter.OutputPath}";
            var receiver = new CountingPacketReceiver(
                new UdpClientReceiver(udpClient),
                dumpWriter,
                (totalPackets, sourceIpAddress) => statusStore.RecordListenerPacket(totalPackets, statusMessage, sourceIpAddress));
            statusStore.SetListenerStatus(JobRunState.Running, statusMessage, 0, 0, null, null);
            _monitorTask = Task.Run(() => PublishRateAsync(receiver, linkedCts.Token), linkedCts.Token);
            var listener = new TestListener(receiver);
            var recordingTask = listener.StartRecording(linkedCts.Token);
            var telemetryTask = listener.HandleTelemetry(linkedCts.Token);

            // If either pipeline task exits (or faults), stop the other and surface failures.
            await Task.WhenAny(recordingTask, telemetryTask);
            linkedCts.Cancel();

            await Task.WhenAll(recordingTask, telemetryTask);
            if (_monitorTask is not null)
            {
                await _monitorTask;
            }
            statusStore.SetListenerStatus(JobRunState.Completed, BuildStoppedMessage(dumpWriter));
            }
        }
        catch (OperationCanceledException)
        {
            statusStore.SetListenerStatus(JobRunState.Completed, BuildStoppedMessage(dumpWriter));
        }
        catch (Exception ex)
        {
            var failureMessage = dumpWriter is null
                ? $"Listener failed: {ex.Message}"
                : $"Listener failed: {ex.Message} | Dump: {dumpWriter.OutputPath}";
            statusStore.SetListenerStatus(JobRunState.Failed, failureMessage);
        }
    }

    private static string BuildStoppedMessage(RawPacketDumpWriter? dumpWriter)
    {
        return dumpWriter is null
            ? "Listener stopped."
            : $"Listener stopped. Dump written to: {dumpWriter.OutputPath}";
    }

    private async Task PublishRateAsync(CountingPacketReceiver receiver, CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }

            var snapshot = receiver.TakeRateSnapshot();
            statusStore.SetListenerStatus(
                JobRunState.Running,
                snapshot.StatusMessage,
                snapshot.RateHz,
                snapshot.TotalPackets,
                snapshot.LastPacketReceivedAtUtc,
                snapshot.SourceIpAddress);
        }
    }

    private RawPacketDumpWriter? CreateDumpWriter()
    {
        if (!_recordRawDump)
        {
            return null;
        }

        var resolvedDumpDirectory = Path.IsPathRooted(_dumpDirectory)
            ? _dumpDirectory
            : Path.GetFullPath(Path.Combine(GetRepositoryRoot(), _dumpDirectory));
        Directory.CreateDirectory(resolvedDumpDirectory);
        var baseFilePath = Path.Combine(resolvedDumpDirectory, $"live_{DateTime.UtcNow:yyyyMMdd_HHmmss}");
        return new RawPacketDumpWriter(baseFilePath);
    }

    private static string GetRepositoryRoot()
    {
        var current = new DirectoryInfo(Directory.GetCurrentDirectory());
        while (current is not null)
        {
            if (Directory.Exists(Path.Combine(current.FullName, ".git")))
            {
                return current.FullName;
            }

            current = current.Parent;
        }

        return Directory.GetCurrentDirectory();
    }

    private sealed class CountingPacketReceiver(
        IPacketReceiver inner,
        RawPacketDumpWriter? dumpWriter,
        Action<long, string?> onPacketReceived) : IPacketReceiver
    {
        private readonly object _gate = new();
        private long _totalPackets;
        private int _windowPackets;
        private DateTimeOffset _windowStartedAt = DateTimeOffset.UtcNow;
        private DateTimeOffset? _lastPacketReceivedAtUtc;
        private string? _sourceIpAddress;
        private readonly string _statusMessage = dumpWriter is null
            ? "Listening"
            : $"Listening | Dump: {dumpWriter.OutputPath}";

        public async Task<UdpReceiveResult> ReceiveAsync(CancellationToken token)
        {
            var result = await inner.ReceiveAsync(token);
            dumpWriter?.Write(result.Buffer, DateTime.UtcNow);
            long total;
            string? sourceIpAddress = result.RemoteEndPoint.Address.ToString();
            lock (_gate)
            {
                _totalPackets++;
                _windowPackets++;
                _lastPacketReceivedAtUtc = DateTimeOffset.UtcNow;
                _sourceIpAddress = sourceIpAddress;
                total = _totalPackets;
            }

            onPacketReceived(total, sourceIpAddress);

            return result;
        }

        public PacketRateSnapshot TakeRateSnapshot()
        {
            lock (_gate)
            {
                var now = DateTimeOffset.UtcNow;
                var elapsed = now - _windowStartedAt;
                var rateHz = elapsed.TotalSeconds <= 0
                    ? 0
                    : _windowPackets / elapsed.TotalSeconds;
                var total = _totalPackets;
                var lastPacket = _lastPacketReceivedAtUtc;
                var sourceIpAddress = _sourceIpAddress;

                _windowPackets = 0;
                _windowStartedAt = now;
                return new PacketRateSnapshot(rateHz, total, lastPacket, sourceIpAddress, _statusMessage);
            }
        }
    }

    private readonly record struct PacketRateSnapshot(
        double RateHz,
        long TotalPackets,
        DateTimeOffset? LastPacketReceivedAtUtc,
        string? SourceIpAddress,
        string StatusMessage);

    private sealed class RawPacketDumpWriter : IDisposable
    {
        private readonly object _gate = new();
        private readonly Stream _stream;
        private readonly BinaryWriter _writer;

        public RawPacketDumpWriter(string baseFilePath)
        {
            var streamFactory = new FileStreamFactory(compressionEnabled: true);
            _stream = streamFactory.CreateStream(baseFilePath);
            _writer = new BinaryWriter(_stream, Encoding.UTF8, leaveOpen: true);
            OutputPath = baseFilePath + ".gz";
        }

        public string OutputPath { get; }

        public void Write(byte[] buffer, DateTime timestampUtc)
        {
            var timeStamp = timestampUtc.ToString("o");
            var timeStampBytes = Encoding.UTF8.GetBytes(timeStamp);

            lock (_gate)
            {
                _writer.Write(timeStampBytes.Length);
                _writer.Write(timeStampBytes);
                _writer.Write(buffer.Length);
                _writer.Write(buffer);
                _writer.Flush();
                _stream.Flush();
            }
        }

        public void Dispose()
        {
            lock (_gate)
            {
                _writer.Dispose();
                _stream.Dispose();
            }
        }
    }
}
