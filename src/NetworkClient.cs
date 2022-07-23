using System;
using System.Net;
using System.Net.Sockets;

namespace Ape.Netcode
{
    public class NetworkClient
    {
        private Socket _socket;
        private byte[] _buffer;
        private EndPoint _remoteEndPoint;

        public NetworkClient() => _buffer = new byte[1024];

        public void Connect(string address, int port)
        {
            var endPoint = new IPEndPoint(IPAddress.Any, 0);
            _remoteEndPoint = new IPEndPoint(IPAddress.Parse(address), port);
            _socket = new Socket(endPoint.AddressFamily, SocketType.Dgram, ProtocolType.Udp);
        }

        public void Send<T>(T packet) where T : NetworkPacket, new()
        {
            var writer = new NetworkWriter();
            writer.Put(packet);
            _socket.SendTo(writer.Data, _remoteEndPoint);
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
            var header = (NetworkHeader)reader.GetByte();

            Console.WriteLine(header);
        }
    }
}
