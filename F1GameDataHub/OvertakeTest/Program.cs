// Instantiate the recorder, and start it
using OvertakeTest;
using PacketRecording;
using System.Net;
using System.Net.Sockets;

var udpClient = new UdpClient(new IPEndPoint(IPAddress.Any, 20777));
var packetReceiver = new UdpClientReceiver(udpClient);

var recorder = new TestListener(packetReceiver);

var cts = new CancellationTokenSource();
var recordingTask = recorder.StartRecording(cts.Token);
var telemetryTask = recorder.HandleTelemetry(cts.Token);

Console.WriteLine("Press Enter to stop recording...");
Console.ReadLine();

cts.Cancel();

try
{
    await Task.WhenAll(recordingTask, telemetryTask);
}
catch (OperationCanceledException)
{
}