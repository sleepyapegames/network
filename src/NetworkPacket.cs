namespace Ape.Netcode
{
    public abstract class NetworkPacket
    {
        protected abstract void Serialize(NetworkWriter writer);
        protected abstract void Deserialize(NetworkReader reader);

        internal void InternalSerialize(NetworkWriter writer)
        {
            writer.Put((byte)NetworkHeader.Packet);
            Serialize(writer);
        }

        internal void InternalDeserialize(NetworkReader reader) => Deserialize(reader);
    }
}
