namespace F1GameDataHub.Web.Services;

public enum JobRunState
{
    Idle,
    Running,
    Completed,
    Failed
}

public sealed record JobStatusSnapshot(
    JobRunState State,
    string Message,
    DateTimeOffset UpdatedAtUtc,
    double? PacketRateHz = null,
    long? TotalPackets = null,
    DateTimeOffset? LastPacketReceivedAtUtc = null,
    string? SourceIpAddress = null);

public sealed class JobStatusStore
{
    private readonly object _gate = new();
    private JobStatusSnapshot _importStatus = new(JobRunState.Idle, "Not started.", DateTimeOffset.UtcNow);
    private JobStatusSnapshot _listenerStatus = new(JobRunState.Idle, "Not started.", DateTimeOffset.UtcNow);

    public JobStatusSnapshot ImportStatus
    {
        get
        {
            lock (_gate)
            {
                return _importStatus;
            }
        }
    }

    public JobStatusSnapshot ListenerStatus
    {
        get
        {
            lock (_gate)
            {
                return _listenerStatus;
            }
        }
    }

    public void SetImportStatus(JobRunState state, string message)
    {
        lock (_gate)
        {
            _importStatus = new JobStatusSnapshot(state, message, DateTimeOffset.UtcNow);
        }
    }

    public void SetListenerStatus(
        JobRunState state,
        string message,
        double? packetRateHz = null,
        long? totalPackets = null,
        DateTimeOffset? lastPacketReceivedAtUtc = null,
        string? sourceIpAddress = null)
    {
        lock (_gate)
        {
            _listenerStatus = new JobStatusSnapshot(
                state,
                message,
                DateTimeOffset.UtcNow,
                packetRateHz,
                totalPackets,
                lastPacketReceivedAtUtc,
                sourceIpAddress);
        }
    }

    public void RecordListenerPacket(long totalPackets, string? message = null, string? sourceIpAddress = null)
    {
        lock (_gate)
        {
            var effectiveMessage = string.IsNullOrWhiteSpace(message) ? _listenerStatus.Message : message;
            _listenerStatus = new JobStatusSnapshot(
                JobRunState.Running,
                effectiveMessage,
                DateTimeOffset.UtcNow,
                _listenerStatus.PacketRateHz,
                totalPackets,
                DateTimeOffset.UtcNow,
                sourceIpAddress ?? _listenerStatus.SourceIpAddress);
        }
    }
}
