// Instantiate the recorder, and start it
using OvertakeTest;
using PacketRecording;
using System.Net;
using System.Net.Sockets;

var udpPortRaw = Environment.GetEnvironmentVariable("F1_UDP_PORT");
var udpPort = int.TryParse(udpPortRaw, out var parsedUdpPort) ? parsedUdpPort : 20777;

Console.WriteLine($"Listening for UDP telemetry on port {udpPort}...");
var udpClient = new UdpClient(new IPEndPoint(IPAddress.Any, udpPort));
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