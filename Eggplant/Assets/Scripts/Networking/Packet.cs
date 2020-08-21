using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Assets.Scripts.Networking
{
    public enum PacketTypes
    {
        Welcome,
        WelcomeReceived,
        SpawnPlayer,
        PlayerPosition,
        PlayerRotation,
        PlayerMovement
    }

    public class Packet : IDisposable
    {
        private List<byte> Buffer;
        private byte[] ReadableBuffer;
        private int ReadPosition;

        public Packet()
        {
            Buffer = new List<byte>();
            ReadPosition = 0;
        }
        public Packet(int id)
        {
            Buffer = new List<byte>();
            ReadPosition = 0;

            Write(id);
        }
        public Packet(byte[] data)
        {
            Buffer = new List<byte>();
            ReadPosition = 0;

            SetBytes(data);
        }

        #region Functions

        public void SetBytes(byte[] data)
        {
            Write(data);
            ReadableBuffer = Buffer.ToArray();
        }

        public void WriteLength()
        {
            Buffer.InsertRange(0, BitConverter.GetBytes(Buffer.Count));
        }

        public void InsertInt(int value)
        {
            Buffer.InsertRange(0, BitConverter.GetBytes(value));
        }

        public byte[] ToArray()
        {
            ReadableBuffer = Buffer.ToArray();
            return ReadableBuffer;
        }

        public int Length()
        {
            return Buffer.Count;
        }

        public int UnreadLength()
        {
            return Length() - ReadPosition;
        }

        public void Reset(bool shouldReset = true)
        {
            if (shouldReset)
            {
                Buffer.Clear();
                ReadableBuffer = null;
                ReadPosition = 0;
            }
            else
            {
                ReadPosition -= 4;
            }
        }

        #endregion

        #region Write Data

        public void Write(byte value)
        {
            Buffer.Add(value);
        }
        public void Write(byte[] value)
        {
            Buffer.AddRange(value);
        }
        public void Write(short value)
        {
            Buffer.AddRange(BitConverter.GetBytes(value));
        }
        public void Write(int value)
        {
            Buffer.AddRange(BitConverter.GetBytes(value));
        }
        public void Write(long value)
        {
            Buffer.AddRange(BitConverter.GetBytes(value));
        }
        public void Write(float value)
        {
            Buffer.AddRange(BitConverter.GetBytes(value));
        }
        public void Write(bool value)
        {
            Buffer.AddRange(BitConverter.GetBytes(value));
        }
        public void Write(string value)
        {
            Write(value.Length);
            Buffer.AddRange(Encoding.ASCII.GetBytes(value));
        }
        public void Write(Vector3 value)
        {
            Write(value.X);
            Write(value.Y);
            Write(value.Z);
        }
        public void Write(Quaternion value)
        {
            Write(value.X);
            Write(value.Y);
            Write(value.Z);
            Write(value.W);
        }
        
        #endregion

        #region Read Data

        public byte ReadByte(bool moveReadPosition = true)
        {
            if (Buffer.Count > ReadPosition)
            {
                var value = ReadableBuffer[ReadPosition];

                if (moveReadPosition)
                    ReadPosition++;
                
                return value;
            }
            else
            {
                throw new Exception("Could not read value of type 'byte'!");
            }
        }
        public byte[] ReadBytes(int length, bool moveReadPosition = true)
        {
            if (Buffer.Count > ReadPosition)
            {
                var value = Buffer.GetRange(ReadPosition, length).ToArray();
                
                if (moveReadPosition)
                    ReadPosition += length;
                
                return value;
            }
            else
            {
                throw new Exception("Could not read value of type 'byte[]'!");
            }
        }
        public short ReadShort(bool moveReadPos = true)
        {
            if (Buffer.Count > ReadPosition)
            {
                var value = BitConverter.ToInt16(ReadableBuffer, ReadPosition);

                if (moveReadPos)
                    ReadPosition += 2;
                
                return value;
            }
            else
            {
                throw new Exception("Could not read value of type 'short'!");
            }
        }
        public int ReadInt(bool moveReadPosition = true)
        {
            if (Buffer.Count > ReadPosition)
            {
                var value = BitConverter.ToInt32(ReadableBuffer, ReadPosition);

                if (moveReadPosition)
                    ReadPosition += 4;
                
                return value;
            }
            else
            {
                throw new Exception("Could not read value of type 'int'!");
            }
        }
        public long ReadLong(bool moveReadPosition = true)
        {
            if (Buffer.Count > ReadPosition)
            {
                var value = BitConverter.ToInt64(ReadableBuffer, ReadPosition);

                if (moveReadPosition)
                    ReadPosition += 8;
                
                return value;
            }
            else
            {
                throw new Exception("Could not read value of type 'long'!");
            }
        }
        public float ReadFloat(bool moveReadPosition = true)
        {
            if (Buffer.Count > ReadPosition)
            {
                var value = BitConverter.ToSingle(ReadableBuffer, ReadPosition);

                if (moveReadPosition)
                    ReadPosition += 4;
                
                return value;
            }
            else
            {
                throw new Exception("Could not read value of type 'float'!");
            }
        }
        public bool ReadBool(bool moveReadPosition = true)
        {
            if (Buffer.Count > ReadPosition)
            {
                var value = BitConverter.ToBoolean(ReadableBuffer, ReadPosition);
                
                if (moveReadPosition)
                    ReadPosition ++;
                
                return value;
            }
            else
            {
                throw new Exception("Could not read value of type 'bool'!");
            }
        }
        public string ReadString(bool moveReadPosition = true)
        {
            try
            {
                var length = ReadInt();
                var value = Encoding.ASCII.GetString(ReadableBuffer, ReadPosition, length);

                if (moveReadPosition && value.Length > 0)
                    ReadPosition += length;
                
                return value;
            }
            catch
            {
                throw new Exception("Could not read value of type 'string'!");
            }
        }
        public Vector3 ReadVector3(bool moveReadPosition = true)
        {
            return new Vector3(ReadFloat(moveReadPosition), ReadFloat(moveReadPosition), ReadFloat(moveReadPosition));
        }
        public Quaternion ReadQuaternion(bool moveReadPosition = true)
        {
            return new Quaternion(ReadFloat(moveReadPosition), ReadFloat(moveReadPosition), ReadFloat(moveReadPosition), ReadFloat(moveReadPosition));
        }

        #endregion

        private bool Disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!Disposed)
            {
                if (disposing)
                {
                    Buffer = null;
                    ReadableBuffer = null;
                    ReadPosition = 0;
                }

                Disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
