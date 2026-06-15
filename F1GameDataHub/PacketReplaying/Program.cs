using PacketReplaying;
using System.Net;

string[] races =
{
    "s18_div_1_cota",
};

//Console.WriteLine("Press Enter to start replaying...");
//Console.ReadLine();

var cts = new CancellationTokenSource();

foreach (var race in races)
{
    var replayer = new UdpPacketReplayer(IPAddress.Loopback, 20777, $@"C:\Users\seanm\Downloads\PacketRecording\{race}.gz");

    await Task.Run(async () =>
    {
        await replayer.Replay(cts.Token);
    });
}

//Console.WriteLine("Press Enter to stop replaying...");
//Console.ReadLine();

cts.Cancel();