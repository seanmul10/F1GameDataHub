using PacketReplaying;
using System.Net;

string compressedFilePath = @"D:\Temp\div_2_australia.gz";

Console.WriteLine("Press Enter to start replaying...");
Console.ReadLine();

var replayer = new UdpPacketReplayer(IPAddress.Loopback, 20777, compressedFilePath);
var cts = new CancellationTokenSource();

await Task.Run(async () =>
{
    await replayer.Replay(cts.Token);
});

Console.WriteLine("Press Enter to stop replaying...");
Console.ReadLine();

cts.Cancel();