// Instantiate the recorder, and start it
using PacketRecording;
using System.Net;
using System.Net.Sockets;

var udpClient = new UdpClient(new IPEndPoint(IPAddress.Any, 20777));
var packetReceiver = new UdpClientReceiver(udpClient);

var streamFactory = new FileStreamFactory(compressionEnabled: true);
var recorder = new UdpPacketRecorder(packetReceiver, streamFactory);

var cts = new CancellationTokenSource();
Console.WriteLine("Press Enter to stop recording...");
var recordingTask = recorder.StartRecording(@"D:\Temp\div_1_brazil", cts.Token);

Console.ReadLine();

cts.Cancel();

try
{
    await recordingTask;
}
catch (OperationCanceledException)
{
}