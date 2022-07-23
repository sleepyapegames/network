namespace Ape.Netcode
{
    public class PacketTest : NetworkPacket
    {
        public string Message { get; set; }

        protected override void Deserialize(NetworkReader reader) => Message = reader.ReadString();

        protected override void Serialize(NetworkWriter writer) => writer.Put(Message);
    }
}
