//
// Copyright (c) .NET Foundation and Contributors
// See LICENSE file in the project root for full license information.
//

using System;

namespace Windows.Storage.Streams
{
    /// <summary>
    /// Represents a sequential stream of bytes to be read.
    /// </summary>
    public interface IInputStream
    {
        /// <summary>
        /// Reads data from the stream.
        /// </summary>
        /// <param name="buffer">A buffer that is used to return the array of bytes that are read. The return value contains the buffer that holds the results.</param>
        /// <param name="count">The number of bytes to read that is less than or equal to the Capacity value.</param>
        /// <param name="options">Specifies the type of the asynchronous read operation.</param>
        /// <returns>The number of bytes that were actually read.</returns>
        /// <remarks>This method is specific to nanoFramework. The equivalent method in the UWP API is: ReadAsync(IBuffer buffer, UInt32 count, InputStreamOptions options).</remarks>
        //IAsyncOperationWithProgress<IBuffer, uint> ReadAsync(IBuffer buffer, UInt32 count, InputStreamOptions options);
        uint Read(IBuffer buffer, UInt32 count, InputStreamOptions options);
    }
}
