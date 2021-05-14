﻿//
// Copyright (c) .NET Foundation and Contributors
// See LICENSE file in the project root for full license information.
//

using System;
using System.Text;

namespace Windows.Storage.Streams
{
    /// <summary>
    /// Writes data to an output stream.
    /// </summary>
    public sealed class DataWriter : MarshalByRefObject, IDisposable, IDataWriter
    {
        private IOutputStream _stream;
        private bool _disposed;

        /// <summary>
        /// Creates and initializes a new instance of the data writer.
        /// </summary>
        public DataWriter()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates and initializes a new instance of the data writer to an output stream.
        /// </summary>
        /// <param name="outputStream">The new output stream instance.</param>
        public DataWriter(IOutputStream outputStream)
        {
            _stream = outputStream ?? throw new ArgumentNullException();
            _disposed = false;
        }

        /// <summary>
        /// Gets or sets the byte order of the data in the output stream.
        /// </summary>
        /// <value>One of the enumeration values.</value>
        /// <remarks>
        /// nanoFramework doesn't this feature. ByteOrder is always <see cref="ByteOrder.LittleEndian"/>.
        /// </remarks>
        public ByteOrder ByteOrder => ByteOrder.LittleEndian;

        /// <summary>
        /// Gets or sets the Unicode character encoding for the output stream.
        /// </summary>
        /// <value>One of the enumeration values.</value>
        /// <remarks>
        /// nanoFramework doesn't this feature. UnicodeEncoding is always <see cref="UnicodeEncoding.Utf8"/>.
        /// </remarks>
        public UnicodeEncoding UnicodeEncoding => UnicodeEncoding.Utf8;

        /// <summary>
        /// Gets the size of the buffer that has not been used.
        /// </summary>
        /// <value>The available buffer length, in bytes.</value>
        public uint UnstoredBufferLength
        {
            get
            {
                return _stream.UnstoredBufferLength;
            }
        }

        ///// <summary>
        ///// Closes the current stream and releases system resources.
        ///// </summary>
        ///// <remarks>
        ///// DataWriter takes ownership of the stream that is passed to its constructor. Calling this method also calls on the associated stream. After calling this method, calls to most other DataWriter methods will fail.
        ///// If you do not want the associated stream to be closed when the reader closes, call DataWriter.DetachStream before calling this method.
        ///// </remarks>
        //public void Close()
        //{
        //    // This member is not implemented in C#
        //    throw new NotImplementedException();
        //}


        /// <summary>
        /// Flushes data.
        /// </summary>
        /// <returns>The stream flush operation.</returns>
        /// <remarks>
        /// The Flush method ensures that the data has reached the target storage medium that the stream represents. For example, to improve application responsiveness and throughput, a file stream might respond to a write operation by copying the buffer into another temporary storage medium and returning immediately, while the target device begins writing the data concurrently.
        /// The Flush method doesn't complete until all data specified in previous write calls has reached the target storage medium. If the data can't be written, or an error occurred during a write operation, the method returns false.
        /// The Flush method may produce latencies and does not always guarantee durable and coherent storage of data.It's generally recommended to avoid this method if possible.
        /// This method is specific to nanoFramework. The equivalent method in the UWP API is: FlushAsync.
        /// </remarks>
        //public IAsyncOperation<bool> FlushAsync()
        public bool Flush()
        {
            if (_disposed) throw new ObjectDisposedException();

            Store();

            return true;
        }

        /// <summary>
        /// Gets the size of a string.
        /// </summary>
        /// <param name="value">The string.</param>
        /// <returns>The size of the string, in bytes.</returns>
        public uint MeasureString(String value)
        {
            Encoding encoding = Encoding.UTF8;
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            return (uint)encoding.GetBytes(value).Length;
        }

        /// <summary>
        /// Commits data in the buffer to a backing store.
        /// </summary>
        /// <returns>The store data operation.</returns>
        /// <remarks>
        /// This method is specific to nanoFramework. The equivalent method in the UWP API is: StoreAsync.
        /// </remarks>
        // public DataWriterStoreOperation StoreAsync()
        public uint Store()
        {
            // the underlying stream can implement this or not
            var storeCall = _stream.GetType().GetMethod("Store");
            if (storeCall != null)
            {
                var result = storeCall.Invoke(_stream, null);

                return (uint)result;
            }

            return 0;
        }

        /// <summary>
        /// Writes a Boolean value to the output stream.
        /// </summary>
        /// <param name="value">The value to write.</param>
        public void WriteBoolean(Boolean value)
        {
            WriteBytes(BitConverter.GetBytes(value));
        }

        /// <summary>
        /// Writes a number of bytes from a buffer to the output stream.
        /// </summary>
        /// <param name="buffer">The value to write.</param>
        public void WriteBuffer(IBuffer buffer)
        {
            WriteBuffer(buffer, 0, buffer.Length);
        }

        /// <summary>
        /// Writes a range of bytes from a buffer to the output stream.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="start">The starting byte to be written.</param>
        /// <param name="count">The number of bytes to write.</param>
        public void WriteBuffer(IBuffer buffer, UInt32 start, UInt32 count)
        {
            byte[] copyBuffer = new byte[count];
            Array.Copy(((ByteBuffer)buffer).Data, (int)start, copyBuffer, 0, (int)count);

            WriteBytes(copyBuffer);
        }

        /// <summary>
        /// Writes a byte value to the output stream.
        /// </summary>
        /// <param name="value">The value to write.</param>
        public void WriteByte(Byte value)
        {
            WriteBytes(new byte[] { value });
        }

        /// <summary>
        /// Writes an array of byte values to the output stream.
        /// </summary>
        /// <param name="value">The value to write.</param>
        public void WriteBytes(Byte[] value)
        {
            if (_disposed) throw new ObjectDisposedException();

            _stream.Write(value);
        }

        /// <summary>
        /// Writes a date and time value to the output stream.
        /// </summary>
        /// <param name="value">The value to write.</param>
        public void WriteDateTime(DateTime value)
        {
            WriteInt64(value.Ticks);
        }

        /// <summary>
        /// Writes a floating-point value to the output stream.
        /// </summary>
        /// <param name="value">The value to write.</param>
        public void WriteDouble(Double value)
        {
            WriteBytes(BitConverter.GetBytes(value));
        }

        /// <summary>
        /// Writes a GUID value to the output stream.
        /// </summary>
        /// <param name="value">The value to write.</param>
        public void WriteGuid(Guid value)
        {
            WriteBytes(value.ToByteArray());
        }

        /// <summary>
        /// Writes a 16-bit integer value to the output stream.
        /// </summary>
        /// <param name="value">The value to write.</param>
        public void WriteInt16(Int16 value)
        {
            WriteBytes(BitConverter.GetBytes(value));
        }

        /// <summary>
        /// Writes a 32-bit integer value to the output stream.
        /// </summary>
        /// <param name="value">The value to write.</param>
        public void WriteInt32(Int32 value)
        {
            WriteBytes(BitConverter.GetBytes(value));
        }

        /// <summary>
        /// Writes a 64-bit integer value to the output stream.
        /// </summary>
        /// <param name="value">The value to write.</param>
        public void WriteInt64(Int64 value)
        {
            WriteBytes(BitConverter.GetBytes(value));
        }

        /// <summary>
        /// Write a floating-point value to the output stream.
        /// </summary>
        /// <param name="value">The value to write.</param>
        public void WriteSingle(Single value)
        {
            WriteBytes(BitConverter.GetBytes(value));
        }

        /// <summary>
        /// Writes a string value to the output stream.
        /// </summary>
        /// <param name="value">The value to write.</param>
        /// <returns>The length of the string.</returns>
        public uint WriteString(String value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            byte[] strBytes = Encoding.UTF8.GetBytes(value);
            WriteBytes(strBytes);
            return (uint)strBytes.Length;
        }

        /// <summary>
        /// Writes a time interval value to the output stream.
        /// </summary>
        /// <param name="value">The value to write.</param>
        public void WriteTimeSpan(TimeSpan value)
        {
            WriteInt64(value.Ticks);
        }

        /// <summary>
        /// Writes a 16-bit unsigned integer value to the output stream.
        /// </summary>
        /// <param name="value">The value to write.</param>
        public void WriteUInt16(UInt16 value)
        {
            WriteBytes(BitConverter.GetBytes(value));
        }

        /// <summary>
        /// Writes a 32-bit unsigned integer value to the output stream.
        /// </summary>
        /// <param name="value">The value to write.</param>
        public void WriteUInt32(UInt32 value)
        {
            WriteBytes(BitConverter.GetBytes(value));
        }

        /// <summary>
        /// Writes a 64-bit unsigned integer value to the output stream.
        /// </summary>
        /// <param name="value">The value to write.</param>
        public void WriteUInt64(UInt64 value)
        {
            WriteBytes(BitConverter.GetBytes(value));
        }

        #region IDisposable Support

        void Dispose(bool disposing)
        {
            if (_stream != null)
            {
                if (disposing)
                {
                    try
                    {
                        Flush();
                    }
                    catch { }

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
