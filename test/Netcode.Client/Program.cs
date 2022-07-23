using System.Text;
using Ape.Netcode;

Console.WriteLine("Hello, Client!");

var client = new NetworkClient();
client.Connect("127.0.0.1", 9991);

while (true)
{
    client.Send(Encoding.UTF8.GetBytes("hello from client"));
    client.Tick();
    Thread.Sleep(60);
}
