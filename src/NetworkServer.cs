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

        public void Send<T>(EndPoint endPoint, T packet) where T : NetworkPacket, new()
        {
            var writer = new NetworkWriter();
            writer.Put(packet);
            _socket.SendTo(writer.Data, endPoint);
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
            var packet = reader.Get<PacketTest>();
            Console.WriteLine($"{header}: {packet.Message}");
        }
    }
}
