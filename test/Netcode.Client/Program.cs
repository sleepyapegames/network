﻿using Ape.Netcode;

Console.WriteLine("Hello, Client!");

var client = new NetworkClient();
client.Connect("127.0.0.1", 9991);

var writer = new NetworkWriter();
writer.Put(69);
writer.Put("Sixty Nine");

var count = 0;
while (true)
{
    client.Send(new PacketTest { Message = $"client {++count}" });
    client.Tick();
    Thread.Sleep(60);
}
