using System;
using System.Text;

namespace Ape.Netcode
{
    public class NetworkReader
    {
        protected byte[] _data;
        protected int _position;
        protected int _dataSize;
        private int _offset;

        public byte[] RawData => _data;
        public int RawDataSize => _dataSize;
        public int UserDataOffset => _offset;
        public int UserDataSize => _dataSize - _offset;
        public bool IsNull => _data == null;
        public int Position => _position;
        public bool EndOfData => _position == _dataSize;
        public int AvailableBytes => _dataSize - _position;

        // Cache encoding instead of creating it with BinaryWriter each time
        // 1000 readers before: 1MB GC, 30ms
        // 1000 readers after: .8MB GC, 18ms
        private static readonly UTF8Encoding _uTF8Encoding = new UTF8Encoding(false, true);

        public void SkipBytes(int count) => _position += count;

        public void SetPosition(int position) => _position = position;

        public void SetSource(NetworkWriter dataWriter)
        {
            _data = dataWriter.Data;
            _position = 0;
            _offset = 0;
            _dataSize = dataWriter.Length;
        }

        public void SetSource(byte[] source)
        {
            _data = source;
            _position = 0;
            _offset = 0;
            _dataSize = source.Length;
        }

        public void SetSource(byte[] source, int offset, int maxSize)
        {
            _data = source;
            _position = offset;
            _offset = offset;
            _dataSize = maxSize;
        }

        public NetworkReader() { }

        public NetworkReader(NetworkWriter writer) => SetSource(writer);

        public NetworkReader(byte[] source) => SetSource(source);

        public NetworkReader(byte[] source, int offset, int maxSize) =>
            SetSource(source, offset, maxSize);

        #region Read Methods
        public byte ReadByte()
        {
            byte res = _data[_position];
            _position += 1;
            return res;
        }

        public sbyte ReadSByte()
        {
            var b = (sbyte)_data[_position];
            _position++;
            return b;
        }

        public bool[] ReadBoolArray()
        {
            ushort size = BitConverter.ToUInt16(_data, _position);
            _position += 2;
            var arr = new bool[size];
            Buffer.BlockCopy(_data, _position, arr, 0, size);
            _position += size;
            return arr;
        }

        public ushort[] ReadUShortArray()
        {
            ushort size = BitConverter.ToUInt16(_data, _position);
            _position += 2;
            var arr = new ushort[size];
            Buffer.BlockCopy(_data, _position, arr, 0, size * 2);
            _position += size * 2;
            return arr;
        }

        public short[] ReadShortArray()
        {
            ushort size = BitConverter.ToUInt16(_data, _position);
            _position += 2;
            var arr = new short[size];
            Buffer.BlockCopy(_data, _position, arr, 0, size * 2);
            _position += size * 2;
            return arr;
        }

        public long[] ReadLongArray()
        {
            ushort size = BitConverter.ToUInt16(_data, _position);
            _position += 2;
            var arr = new long[size];
            Buffer.BlockCopy(_data, _position, arr, 0, size * 8);
            _position += size * 8;
            return arr;
        }

        public ulong[] ReadULongArray()
        {
            ushort size = BitConverter.ToUInt16(_data, _position);
            _position += 2;
            var arr = new ulong[size];
            Buffer.BlockCopy(_data, _position, arr, 0, size * 8);
            _position += size * 8;
            return arr;
        }

        public int[] ReadIntArray()
        {
            ushort size = BitConverter.ToUInt16(_data, _position);
            _position += 2;
            var arr = new int[size];
            Buffer.BlockCopy(_data, _position, arr, 0, size * 4);
            _position += size * 4;
            return arr;
        }

        public uint[] ReadUIntArray()
        {
            ushort size = BitConverter.ToUInt16(_data, _position);
            _position += 2;
            var arr = new uint[size];
            Buffer.BlockCopy(_data, _position, arr, 0, size * 4);
            _position += size * 4;
            return arr;
        }

        public float[] ReadFloatArray()
        {
            ushort size = BitConverter.ToUInt16(_data, _position);
            _position += 2;
            var arr = new float[size];
            Buffer.BlockCopy(_data, _position, arr, 0, size * 4);
            _position += size * 4;
            return arr;
        }

        public double[] ReadDoubleArray()
        {
            ushort size = BitConverter.ToUInt16(_data, _position);
            _position += 2;
            var arr = new double[size];
            Buffer.BlockCopy(_data, _position, arr, 0, size * 8);
            _position += size * 8;
            return arr;
        }

        public string[] ReadStringArray()
        {
            ushort arraySize = ReadUShort();
            var arr = new string[arraySize];
            for (int i = 0; i < arraySize; i++)
            {
                arr[i] = ReadString();
            }
            return arr;
        }

        public string[] ReadStringArray(int maxStringLength)
        {
            ushort arraySize = ReadUShort();
            var arr = new string[arraySize];
            for (int i = 0; i < arraySize; i++)
            {
                arr[i] = ReadString(maxStringLength);
            }
            return arr;
        }

        public bool ReadBool()
        {
            bool res = _data[_position] > 0;
            _position += 1;
            return res;
        }

        public char ReadChar() => (char)ReadUShort();

        public ushort ReadUShort()
        {
            ushort result = BitConverter.ToUInt16(_data, _position);
            _position += 2;
            return result;
        }

        public short ReadShort()
        {
            short result = BitConverter.ToInt16(_data, _position);
            _position += 2;
            return result;
        }

        public long ReadLong()
        {
            long result = BitConverter.ToInt64(_data, _position);
            _position += 8;
            return result;
        }

        public ulong ReadULong()
        {
            ulong result = BitConverter.ToUInt64(_data, _position);
            _position += 8;
            return result;
        }

        public int ReadInt()
        {
            int result = BitConverter.ToInt32(_data, _position);
            _position += 4;
            return result;
        }

        public uint ReadUInt()
        {
            uint result = BitConverter.ToUInt32(_data, _position);
            _position += 4;
            return result;
        }

        public float ReadFloat()
        {
            float result = BitConverter.ToSingle(_data, _position);
            _position += 4;
            return result;
        }

        public double ReadDouble()
        {
            double result = BitConverter.ToDouble(_data, _position);
            _position += 8;
            return result;
        }

        /// <summary>
        /// Note that "maxLength" only limits the number of characters in a string, not its size in bytes.
        /// </summary>
        /// <returns>"string.Empty" if value > "maxLength"</returns>
        public string ReadString(int maxLength)
        {
            ushort size = ReadUShort();
            if (size == 0)
            {
                return null;
            }

            int actualSize = size - 1;
            if (actualSize >= NetworkWriter.StringBufferMaxLength)
            {
                return null;
            }

            ArraySegment<byte> data = ReadBytesSegment(actualSize);

            return (
                maxLength > 0
                && _uTF8Encoding.GetCharCount(data.Array, data.Offset, data.Count) > maxLength
            )
                ? string.Empty
                : _uTF8Encoding.GetString(data.Array, data.Offset, data.Count);
        }

        public string ReadString()
        {
            ushort size = ReadUShort();
            if (size == 0)
            {
                return null;
            }

            int actualSize = size - 1;
            if (actualSize >= NetworkWriter.StringBufferMaxLength)
            {
                return null;
            }

            ArraySegment<byte> data = ReadBytesSegment(actualSize);

            return _uTF8Encoding.GetString(data.Array, data.Offset, data.Count);
        }

        public ArraySegment<byte> ReadBytesSegment(int count)
        {
            ArraySegment<byte> segment = new ArraySegment<byte>(_data, _position, count);
            _position += count;
            return segment;
        }

        public ArraySegment<byte> ReadRemainingBytesSegment()
        {
            ArraySegment<byte> segment = new ArraySegment<byte>(_data, _position, AvailableBytes);
            _position = _data.Length;
            return segment;
        }

        public byte[] ReadRemainingBytes()
        {
            byte[] outgoingData = new byte[AvailableBytes];
            Buffer.BlockCopy(_data, _position, outgoingData, 0, AvailableBytes);
            _position = _data.Length;
            return outgoingData;
        }

        public void ReadBytes(byte[] destination, int start, int count)
        {
            Buffer.BlockCopy(_data, _position, destination, start, count);
            _position += count;
        }

        public void ReadBytes(byte[] destination, int count)
        {
            Buffer.BlockCopy(_data, _position, destination, 0, count);
            _position += count;
        }

        public sbyte[] ReadSBytesWithLength()
        {
            int length = ReadInt();
            sbyte[] outgoingData = new sbyte[length];
            Buffer.BlockCopy(_data, _position, outgoingData, 0, length);
            _position += length;
            return outgoingData;
        }

        public byte[] ReadBytesWithLength()
        {
            int length = ReadInt();
            byte[] outgoingData = new byte[length];
            Buffer.BlockCopy(_data, _position, outgoingData, 0, length);
            _position += length;
            return outgoingData;
        }

        public T Read<T>() where T : NetworkPacket, new()
        {
            var packet = new T();
            packet.InternalDeserialize(this);

            return packet;
        }
        #endregion

        #region PeekMethods

        public byte PeekByte() => _data[_position];

        public sbyte PeekSByte() => (sbyte)_data[_position];

        public bool PeekBool() => _data[_position] > 0;

        public char PeekChar() => (char)PeekUShort();

        public ushort PeekUShort() => BitConverter.ToUInt16(_data, _position);

        public short PeekShort() => BitConverter.ToInt16(_data, _position);

        public long PeekLong() => BitConverter.ToInt64(_data, _position);

        public ulong PeekULong() => BitConverter.ToUInt64(_data, _position);

        public int PeekInt() => BitConverter.ToInt32(_data, _position);

        public uint PeekUInt() => BitConverter.ToUInt32(_data, _position);

        public float PeekFloat() => BitConverter.ToSingle(_data, _position);

        public double PeekDouble() => BitConverter.ToDouble(_data, _position);

        public string PeekString(int maxLength)
        {
            ushort size = PeekUShort();
            if (size == 0)
            {
                return null;
            }

            int actualSize = size - 1;
            if (actualSize >= NetworkWriter.StringBufferMaxLength)
            {
                return null;
            }

            return (
                maxLength > 0
                && _uTF8Encoding.GetCharCount(_data, _position + 2, actualSize) > maxLength
            )
                ? string.Empty
                : _uTF8Encoding.GetString(_data, _position + 2, actualSize);
        }

        public string PeekString()
        {
            ushort size = PeekUShort();
            if (size == 0)
            {
                return null;
            }

            int actualSize = size - 1;
            if (actualSize >= NetworkWriter.StringBufferMaxLength)
            {
                return null;
            }

            return _uTF8Encoding.GetString(_data, _position + 2, actualSize);
        }
        #endregion

        #region TryReadMethods
        public bool TryReadByte(out byte result)
        {
            if (AvailableBytes >= 1)
            {
                result = ReadByte();
                return true;
            }
            result = 0;
            return false;
        }

        public bool TryReadSByte(out sbyte result)
        {
            if (AvailableBytes >= 1)
            {
                result = ReadSByte();
                return true;
            }
            result = 0;
            return false;
        }

        public bool TryReadBool(out bool result)
        {
            if (AvailableBytes >= 1)
            {
                result = ReadBool();
                return true;
            }
            result = false;
            return false;
        }

        public bool TryReadChar(out char result)
        {
            if (!TryReadUShort(out ushort uShortValue))
            {
                result = '\0';
                return false;
            }
            result = (char)uShortValue;
            return true;
        }

        public bool TryReadShort(out short result)
        {
            if (AvailableBytes >= 2)
            {
                result = ReadShort();
                return true;
            }
            result = 0;
            return false;
        }

        public bool TryReadUShort(out ushort result)
        {
            if (AvailableBytes >= 2)
            {
                result = ReadUShort();
                return true;
            }
            result = 0;
            return false;
        }

        public bool TryReadInt(out int result)
        {
            if (AvailableBytes >= 4)
            {
                result = ReadInt();
                return true;
            }
            result = 0;
            return false;
        }

        public bool TryReadUInt(out uint result)
        {
            if (AvailableBytes >= 4)
            {
                result = ReadUInt();
                return true;
            }
            result = 0;
            return false;
        }

        public bool TryReadLong(out long result)
        {
            if (AvailableBytes >= 8)
            {
                result = ReadLong();
                return true;
            }
            result = 0;
            return false;
        }

        public bool TryReadULong(out ulong result)
        {
            if (AvailableBytes >= 8)
            {
                result = ReadULong();
                return true;
            }
            result = 0;
            return false;
        }

        public bool TryReadFloat(out float result)
        {
            if (AvailableBytes >= 4)
            {
                result = ReadFloat();
                return true;
            }
            result = 0;
            return false;
        }

        public bool TryReadDouble(out double result)
        {
            if (AvailableBytes >= 8)
            {
                result = ReadDouble();
                return true;
            }
            result = 0;
            return false;
        }

        public bool TryReadString(out string result)
        {
            if (AvailableBytes >= 2)
            {
                ushort strSize = PeekUShort();
                if (AvailableBytes >= strSize + 1)
                {
                    result = ReadString();
                    return true;
                }
            }
            result = null;
            return false;
        }

        public bool TryReadStringArray(out string[] result)
        {
            ushort strArrayLength;
            if (!TryReadUShort(out strArrayLength))
            {
                result = null;
                return false;
            }

            result = new string[strArrayLength];
            for (int i = 0; i < strArrayLength; i++)
            {
                if (!TryReadString(out result[i]))
                {
                    result = null;
                    return false;
                }
            }

            return true;
        }

        public bool TryReadBytesWithLength(out byte[] result)
        {
            if (AvailableBytes >= 4)
            {
                var length = PeekInt();
                if (length >= 0 && AvailableBytes >= length + 4)
                {
                    result = ReadBytesWithLength();
                    return true;
                }
            }
            result = null;
            return false;
        }
        #endregion

        public void Clear()
        {
            _position = 0;
            _dataSize = 0;
            _data = null;
        }
    }
}
