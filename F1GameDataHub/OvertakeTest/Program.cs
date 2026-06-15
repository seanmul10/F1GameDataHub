// Instantiate the recorder, and start it
using OvertakeTest;
using PacketRecording;
using System.Net;
using System.Net.Sockets;

var udpClient = new UdpClient(new IPEndPoint(IPAddress.Any, 20777));
var packetReceiver = new UdpClientReceiver(udpClient);

var recorder = new TestListener(packetReceiver);

var cts = new CancellationTokenSource();

var recordingTask = Task.Run(async () =>
{
    try
    {
        await recorder.StartRecording(cts.Token);
    }
    catch (Exception ex)
    {
        Console.Error.WriteLine($"Recording failed: {ex}");
    }
});

var telemetryTask = Task.Run(async () =>
{
    try
    {
        await recorder.HandleTelemetry(cts.Token);
    }
    catch (Exception ex)
    {
        Console.Error.WriteLine($"Telemetry failed: {ex}");
    }
});

await Task.WhenAll(recordingTask, telemetryTask);

Console.WriteLine("Press Enter to stop recording...");
Console.ReadLine();

cts.Cancel();