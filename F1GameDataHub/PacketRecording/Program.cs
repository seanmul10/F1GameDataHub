// Instantiate the recorder, and start it
using PacketRecording;
using System.Net;

var recorder = new UdpPacketRecorder(new IPEndPoint(IPAddress.Loopback, 20777), @"D:\Temp\test_dump");
var cts = new CancellationTokenSource();

await Task.Run(async () =>
{
    await recorder.StartRecording(compressionEnabled: false, cts.Token);
});

Console.WriteLine("Press Enter to stop recording...");
Console.ReadLine();

cts.Cancel();