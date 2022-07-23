using Ape.Netcode;

Console.WriteLine("Hello, Server!");

var server = new NetworkServer();
server.Start(9991);

while (true)
{
    server.Tick();
    Thread.Sleep(60);
}
