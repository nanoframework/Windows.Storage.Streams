//
// Copyright (c) 2017 The nanoFramework project contributors
// See LICENSE file in the project root for full license information.
//

using System;
using System.Text;

namespace Windows.Storage.Streams
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class DataReader : MarshalByRefObject, IDisposable, IDataReader
    {
        private IInputStream _stream;
        private ByteBuffer _buffer;

        private bool _disposed;
        private int _currentReadPosition;
        private InputStreamOptions _inputStreamOptions;

        private const int defaultBufferSize = 512;

        /// <summary>
        /// Creates and initializes a new instance of the data reader.
        /// </summary>
        /// <param name="inputStream">The input stream.</param>
        public DataReader(IInputStream inputStream)
        {
            _stream = inputStream ?? throw new ArgumentNullException("inputStream");
            _disposed = false;

            _buffer = new ByteBuffer(defaultBufferSize);
        }

        /// <summary>
        /// Gets or sets the byte order of the data in the input stream.
        /// </summary>
        /// <value>
        /// One of the enumeration values.
        /// </value>
        /// <remarks>
        /// nanoFramework doesn't this feature. ByteOrder is always <see cref="ByteOrder.LittleEndian"/>.
        /// </remarks>
        public ByteOrder ByteOrder => ByteOrder.LittleEndian;

        /// <summary>
        /// Gets or sets the read options for the input stream.
        /// </summary>
        /// <value>
        /// One of the enumeration values.
        /// </value>
        public InputStreamOptions InputStreamOptions { get { return _inputStreamOptions; } set => _inputStreamOptions = value; }

        /// <summary>
        /// Gets the size of the buffer that has not been read.
        /// </summary>
        /// <value>
        /// The size of the buffer that has not been read, in bytes.
        /// </value>
        public uint UnconsumedBufferLength { get { return (_buffer.Length - (uint)_currentReadPosition); } }

        /// <summary>
        /// Gets or sets the Unicode character encoding for the input stream.
        /// </summary>
        /// <value>
        /// One of the enumeration values.
        /// </value>
        /// <remarks>
        /// nanoFramework doesn't this feature. UnicodeEncoding is always <see cref="UnicodeEncoding.Utf8"/>.
        /// </remarks>
        public UnicodeEncoding UnicodeEncoding => UnicodeEncoding.Utf8;

        /// <summary>
        /// Detaches a stream that was previously attached to the reader.
        /// </summary>
        /// <returns>The detached stream.</returns>
        public IInputStream DetachStream()
        {
            IInputStream inputStream = _stream;
            _stream = null;
            return inputStream;
        }

        /// <summary>
        /// Loads data from the input stream.
        /// </summary>
        /// <param name="count">The count of bytes to load into the intermediate buffer.</param>
        /// <returns>The operation.</returns>
        //public DataReaderLoadOperation LoadAsync(UInt32 count)
        public uint Load(UInt32 count)
        {
            // check the max number of bytes that the backing buffer can hold
            if (count > (_buffer.Capacity - _buffer.Length))
            {
                
                throw new InvalidOperationException();
            }

            // check for (dumb!) 0 bytes request
            if(count == 0)
            {
                return 0;
            }

            // create buffer to hold data read from stream 
            var readBuffer = new ByteBuffer(count);

            var bytesRead = _stream.Read(readBuffer, count, _inputStreamOptions);

            // copy data from read buffer to backing buffer
            Array.Copy(readBuffer.Data, 0, _buffer.Data, (int)_buffer.Length, (int)bytesRead);

            // update counter
            _buffer.Length += bytesRead;

            return bytesRead;
        }

        /// <summary>
        /// Reads a Boolean value from the input stream.
        /// </summary>
        /// <returns>The value.</returns>
        public bool ReadBoolean()
        {
            var value = _buffer.Data[IncreaseReadPosition(1)] > 0;

            CheckReadPosition();

            return value;
        }

        /// <summary>
        /// Reads a buffer from the input stream.
        /// </summary>
        /// <param name="length">The length of the buffer, in bytes.</param>
        /// <returns>The buffer.</returns>
        public IBuffer ReadBuffer(UInt32 length)
        {
            ByteBuffer buffer = new ByteBuffer(length);

            Array.Copy(_buffer.Data, IncreaseReadPosition((int)length), buffer.Data, 0, (int)length);

            CheckReadPosition();

            return buffer;
        }

        /// <summary>
        /// Reads a byte value from the input stream.
        /// </summary>
        /// <returns>The value.</returns>
        public byte ReadByte()
        {
            var value = _buffer.Data[IncreaseReadPosition(1)];

            CheckReadPosition();

            return value;
        }

        /// <summary>
        /// Reads an array of byte values from the input stream.
        /// </summary>
        /// <param name="value">The array of values.</param>
        public void ReadBytes(Byte[] value)
        {
            if (value == null)
            {
                throw new ArgumentNullException();
            }

            Array.Copy(_buffer.Data, IncreaseReadPosition(value.Length), value, 0, value.Length);

            CheckReadPosition();
        }

        /// <summary>
        /// Reads a date and time value from the input stream.
        /// </summary>
        /// <returns>The value.</returns>
        public DateTime ReadDateTime()
        {
            // read position update and check are performed on the call
            return new DateTime(ReadInt64());
        }

        /// <summary>
        /// Reads a floating-point value from the input stream.
        /// </summary>
        /// <returns>The value.</returns>
        public double ReadDouble()
        {
            var value = BitConverter.ToDouble(_buffer.Data, IncreaseReadPosition(8));

            CheckReadPosition();

            return value;
        }

        /// <summary>
        /// Reads a GUID value from the input stream.
        /// </summary>
        /// <returns>The value.</returns>
        public Guid ReadGuid()
        {
            byte[] byteArray = new byte[16];

            // read position update and check are performed on the call
            ReadBytes(byteArray);

            return new Guid(byteArray);
        }

        /// <summary>
        /// Reads a 16-bit integer value from the input stream.
        /// </summary>
        /// <returns>The value.</returns>
        public short ReadInt16()
        {
            var value = BitConverter.ToInt16(_buffer.Data, IncreaseReadPosition(2));

            CheckReadPosition();

            return value;
        }

        /// <summary>
        /// Reads a 32-bit integer value from the input stream.
        /// </summary>
        /// <returns>The value.</returns>
        public int ReadInt32()
        {
            var value = BitConverter.ToInt32(_buffer.Data, IncreaseReadPosition(4));

            CheckReadPosition();

            return value;
        }

        /// <summary>
        /// Reads a 64-bit integer value from the input stream.
        /// </summary>
        /// <returns>The value.</returns>
        public long ReadInt64()
        {
            var value = BitConverter.ToInt64(_buffer.Data, IncreaseReadPosition(8));

            CheckReadPosition();

            return value;
        }

        /// <summary>
        /// Reads a floating-point value from the input stream.
        /// </summary>
        /// <returns>The value.</returns>
        public float ReadSingle()
        {
            var value = BitConverter.ToSingle(_buffer.Data, IncreaseReadPosition(4));

            CheckReadPosition();

            return value;
        }

        /// <summary>
        /// Reads a string value from the input stream.
        /// </summary>
        /// <param name="codeUnitCount">The length of the string.</param>
        /// <returns>The value.</returns>
        public string ReadString(UInt32 codeUnitCount)
        {
            Char[] buffer = new Char[codeUnitCount];

            int readPosition = IncreaseReadPosition((int)codeUnitCount);

            Encoding.UTF8.GetDecoder().Convert(_buffer.Data, readPosition, (int)codeUnitCount, buffer, 0, (int)codeUnitCount, false, out Int32 bytesUsed, out Int32 charsUsed, out Boolean completed);
            var value = new String(buffer, 0, charsUsed);

            CheckReadPosition();

            return value;
        }

        /// <summary>
        /// Reads a time interval from the input stream.
        /// </summary>
        /// <returns>The value.</returns>
        public TimeSpan ReadTimeSpan()
        {
            // read position update and check are performed on the call
            return new TimeSpan(ReadInt64());
        }

        /// <summary>
        /// Reads a 16-bit unsigned integer from the input stream.
        /// </summary>
        /// <returns>The value.</returns>
        public ushort ReadUInt16()
        {
            var value = BitConverter.ToUInt16(_buffer.Data, IncreaseReadPosition(2));

            CheckReadPosition();

            return value;
        }

        /// <summary>
        /// Reads a 32-bit unsigned integer from the input stream.
        /// </summary>
        /// <returns>The value.</returns>
        public uint ReadUInt32()
        {
            var value = BitConverter.ToUInt32(_buffer.Data, IncreaseReadPosition(4));

            CheckReadPosition();

            return value;
        }

        /// <summary>
        /// Reads a 64-bit unsigned integer from the input stream.
        /// </summary>
        /// <returns>The value.</returns>
        public ulong ReadUInt64()
        {
            var value = BitConverter.ToUInt64(_buffer.Data, IncreaseReadPosition(8));

            CheckReadPosition();

            return value;
        }

        /// <summary>
        /// Increases the backing buffer read position.
        /// </summary>
        /// <param name="count">How many bytes to read from the backing buffer.</param>
        /// <returns>
        /// The current buffer position before increasing it by <para>count</para>.
        /// </returns>
        private int IncreaseReadPosition(int count)
        {
            if (UnconsumedBufferLength < count)
            {
                throw new IndexOutOfRangeException();
            }

            // save current read position
            int newPosition = _currentReadPosition;

            // increase by count request
            _currentReadPosition += count;

            return newPosition;
        }

        /// <summary>
        /// Checks current read position and resets the backing buffer if all bytes have been read
        /// </summary>
        private void CheckReadPosition()
        {
            if (_currentReadPosition == _buffer.Length)
            {
                _buffer = new ByteBuffer(defaultBufferSize);
                _currentReadPosition = 0;
            }
        }

        /// <summary>
        /// Throws an <see cref="InvalidOperationException"/> if caller tries to read more bytes than the ones that are available in backing buffer.
        /// </summary>
        /// <param name="count">Number of bytes to be read</param>
        private void CheckAvailableBytes(int count)
        {
            if (count > (_buffer.Length - _currentReadPosition))
            {
                throw new InvalidOperationException();
            }
        }

        #region IDisposable Support

        void Dispose(bool disposing)
        {
            if (_disposed)
            {
                if (_stream != null)
                {
                    if (disposing)
                    {
                        // FIXME
                        //try
                        //{
                        //    _stream.Close();
                        //}
                        //catch { }
                    }

                    _stream = null;
                }

                _disposed = true;
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            GC.SuppressFinalize(this);
        }

        #endregion

    }
}
