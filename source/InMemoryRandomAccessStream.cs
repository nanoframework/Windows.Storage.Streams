//
// Copyright (c) 2017 The nanoFramework project contributors
// See LICENSE file in the project root for full license information.
//

using System;

namespace Windows.Storage.Streams
{
    /// <summary>
    /// Provides random access of data in input and output streams that are stored in memory instead of on disk.
    /// </summary>
    public sealed class InMemoryRandomAccessStream : MarshalByRefObject, IDisposable, IInputStream, IOutputStream, IRandomAccessStream
    {
        private byte[] _buffer;
        private uint _capacity;     // length of usable portion of buffer for stream
        private uint _position;     // read/write head.
        private uint _length;       // Number of bytes within the memory stream

        private bool _disposed = false; // To detect redundant calls
        private bool _isOpen;      // Is this stream open or closed?

        private const uint MemStreamMaxLength = 0xFFFF;

        /// <summary>
        /// Gets a value that indicates whether the stream can be read from.
        /// </summary>
        /// <value>
        /// True if the stream can be read from. Otherwise, false.
        /// </value>
        public bool CanRead => _isOpen;

        /// <summary>
        /// Gets a value that indicates whether the stream can be written to.
        /// </summary>
        /// <value>
        /// True if the stream can be written to. Otherwise, false.
        /// </value>
        public bool CanWrite => _isOpen;

        /// <summary>
        /// Gets the byte offset of the stream.
        /// </summary>
        /// <value>
        /// The number of bytes from the start of the stream.
        /// </value>
        /// <remarks>
        /// The initial offset of a IRandomAccessStream is 0.
        /// This offset is affected by both <see cref="IInputStream"/> and <see cref="IOutputStream"/> operations.
        /// </remarks>
        public ulong Position
        {
            get
            {
                if (!_isOpen) throw new ObjectDisposedException();

                return (ulong)(_position);
            }
        }

        /// <summary>
        /// Gets the number of bytes currently in use in the buffer.
        /// </summary>
        /// <value>
        /// he number of bytes currently in use in the buffer, which is less than or equal to the capacity of the buffer.
        /// </value>
        public ulong Length
        {
            get
            {
                if (!_isOpen) throw new ObjectDisposedException();

                return _length;
            }

        }

        /// <summary>
        /// Gets or sets the size of the random access stream.
        /// </summary>
        /// <value>
        /// The size of the stream.
        /// </value>
        public ulong Size
        {
            get
            {
                if (!_isOpen) throw new ObjectDisposedException();

                return _length;
            }

            set
            {
                if (!_isOpen)
                {
                    throw new ObjectDisposedException();
                }

                if (value > MemStreamMaxLength || value < 0)
                {
                    throw new ArgumentOutOfRangeException();
                }

                uint newLength = (uint)value;

                bool allocatedNewArray = EnsureCapacity(newLength);

                if (!allocatedNewArray && newLength > _length)
                {
                    Array.Clear(_buffer, (int)_length, (int)(newLength - _length));
                }

                _length = newLength;

                if (_position > newLength)
                {
                    _position = newLength;
                }
            }
        }

        /// <summary>
        /// Creates a new instance of the InMemoryRandomAccessStream class.
        /// </summary>
        public InMemoryRandomAccessStream()
        {
            _buffer = new byte[256];
            _capacity = 256;
            _isOpen = true;
        }

        /// <summary>
        /// Reads data from the stream.
        /// </summary>
        /// <param name="buffer">A buffer that is used to return the array of bytes that are read. The return value contains the buffer that holds the results.</param>
        /// <param name="count">The number of bytes to read that is less than or equal to the Capacity value.</param>
        /// <param name="options">Specifies the type of the asynchronous read operation.</param>
        /// <returns>The number of bytes that were actually read.</returns>
        /// <remarks>This method is specific to nanoFramework. The equivalent method in the UWP API is: ReadAsync(IBuffer buffer, UInt32 count, InputStreamOptions options).</remarks>
        public uint Read(IBuffer buffer, uint count, InputStreamOptions options)
        {
            var size = count;

            // how many bytes can we actually read?
            if(count < (_length - _position))
            {
                // we have enough bytes to read
            }
            else
            {
                // requested more bytes than what we have on the buffer
                size = _length - _position;
            }
            buffer = new ByteBuffer(count);

            // copy to destination array
            Array.Copy(_buffer, (int)_position, ((ByteBuffer)buffer).Data, 0, (int)size);
            ((ByteBuffer)buffer).Length += size;

            // update pointers
            _position += buffer.Length;

            return (uint) buffer.Length;
        }


        /// <summary>
        /// Sets the position of the stream to the specified value.
        /// </summary>
        /// <param name="position">The new position of the stream.</param>
        /// <remarks>
        /// Warning! This method does not check the position to make sure the value is valid for the stream. If the position is invalid for the stream, the ReadAsync and WriteAsync methods will return an error if you call them.
        ///</remarks>
        public void Seek(ulong position)
        {
            if (!_isOpen)
            {
                throw new ObjectDisposedException();
            }

            if (position > MemStreamMaxLength)
            {
                throw new ArgumentOutOfRangeException();
            }

            _position = (uint)position;
        }

        /// <summary>
        /// Flushes data in a sequential stream.
        /// </summary>
        /// <returns>The stream flush operation.</returns>
        /// <remarks>
        /// The Flush method may produce latencies and does not always guarantee durable and coherent storage of data. It's generally recommended to avoid this method if possible.
        /// This method is specific to nanoFramework. The equivalent method in the UWP API is: FlushAsync.
        /// </remarks>
        public bool Flush()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Writes data in a sequential stream.
        /// </summary>
        /// <param name="buffer">A buffer that contains the data to be written.</param>
        /// <returns>The byte writer operation.</returns>
        /// <remarks>This method is specific to nanoFramework. The equivalent method in the UWP API is: WriteAsync.</remarks>
        public void Write(byte[] buffer)
        {
            if (!_isOpen)
            {
                throw new ObjectDisposedException();
            }

            if (buffer == null)
            {
                throw new ArgumentNullException();
            }

            // the cast bellow is safe because there is no way that it will overflow on the expectable memory limits of a nanoFramework platform
            uint i = _position + (uint)buffer.Length;

            // Check for overflow
            if (i > _length)
            {
                if (i > _capacity) EnsureCapacity(i);
                _length = i;
            }

            Array.Copy(buffer, 0, _buffer, (int)_position, buffer.Length);

            _position = (uint)i;

            return;
        }

        private bool EnsureCapacity(uint value)
        {
            if (value > _capacity)
            {
                var newCapacity = value;
                if (newCapacity < 256)
                    newCapacity = 256;
                if (newCapacity < _capacity * 2)
                    newCapacity = (uint)_capacity * 2;

                if (newCapacity > _capacity) throw new NotSupportedException();

                if (newCapacity > 0)
                {
                    byte[] newBuffer = new byte[newCapacity];
                    if (_length > 0) Array.Copy(_buffer, 0, newBuffer, 0, (int)_length);
                    _buffer = newBuffer;
                }
                else
                {
                    _buffer = null;
                }

                _capacity = newCapacity;

                return true;
            }

            return false;
        }


        #region IDisposable Support

        void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _isOpen = false;
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                _disposed = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
        }

        #endregion
    }
}
