//
// Copyright (c) .NET Foundation and Contributors
// See LICENSE file in the project root for full license information.
//

using System;

namespace Windows.Storage.Streams
{
    /// <summary>
    /// Supports random access of data in input and output streams.
    /// </summary>
    public interface IRandomAccessStream : IDisposable, IInputStream, IOutputStream
    {
        /// <summary>
        /// Gets a value that indicates whether the stream can be read from.
        /// </summary>
        /// <value>
        /// True if the stream can be read from. Otherwise, false.
        /// </value>
        bool CanRead { get; }

        /// <summary>
        /// Gets a value that indicates whether the stream can be written to.
        /// </summary>
        /// <value>
        /// True if the stream can be written to. Otherwise, false.
        /// </value>
        bool CanWrite { get; }

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
        ulong Position { get; }

        /// <summary>
        /// Gets or sets the size of the random access stream.
        /// </summary>
        /// <value>
        /// The size of the stream.
        /// </value>
        ulong Size { get; set; }

        /// <summary>
        /// Sets the position of the stream to the specified value.
        /// </summary>
        /// <param name="position">The new position of the stream.</param>
        /// <remarks>
        /// Warning! This method does not check the position to make sure the value is valid for the stream. If the position is invalid for the stream, the ReadAsync and WriteAsync methods will return an error if you call them.
        ///</remarks>
        void Seek(UInt64 position);
    }
}
