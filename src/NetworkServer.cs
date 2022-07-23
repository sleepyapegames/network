using System;
using System.Net;
using System.Net.Sockets;

namespace Ape.Netcode
{
    public class NetworkServer
    {
        private Socket _socket;
        private EndPoint _remoteEndPoint;
        private byte[] _buffer;

        public NetworkServer()
        {
            _remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
            _buffer = new byte[1024];
        }

        public void Start(int port)
        {
            var endPoint = new IPEndPoint(IPAddress.Any, port);
            _socket = new Socket(endPoint.AddressFamily, SocketType.Dgram, ProtocolType.Udp);
            _socket.Bind(endPoint);
        }

        public void Send(EndPoint endPoint, byte[] data)
        {
            _socket.SendTo(data, endPoint);
        }

        public void Tick()
        {
            while (_socket.Poll(0, SelectMode.SelectRead))
            {
                var receiveLength = _socket.ReceiveFrom(_buffer, ref _remoteEndPoint);
                if (receiveLength > 0)
                    ReceiveData(_remoteEndPoint, _buffer, receiveLength);
            }
        }

        private void ReceiveData(EndPoint endPoint, byte[] buffer, int receiveLength)
        {
            var reader = new NetworkReader(buffer);
            var number = reader.GetInt();
            var message = reader.GetString();
            Console.WriteLine($"[{endPoint}] {number} {message}");

            var writer = new NetworkWriter();
            writer.Put(69);
            writer.Put(420);
            Send(endPoint, writer.Data);
        }
    }
}
