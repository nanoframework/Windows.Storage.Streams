//
// Copyright (c) 2017 The nanoFramework project contributors
// See LICENSE file in the project root for full license information.
//

namespace Windows.Storage.Streams
{
    /// <summary>
    /// Represents a sequential stream of bytes to be written.
    /// </summary>
    public interface IOutputStream
    {
        /// <summary>
        /// Flushes data in a sequential stream.
        /// </summary>
        /// <returns>The stream flush operation.</returns>
        /// <remarks>
        /// The Flush method may produce latencies and does not always guarantee durable and coherent storage of data. It's generally recommended to avoid this method if possible.
        /// This method is specific to nanoFramework. The equivalent method in the UWP API is: FlushAsync.
        /// </remarks>
        //IAsyncOperation<bool> FlushAsync()
        bool Flush();

        /// <summary>
        /// Writes data in a sequential stream.
        /// </summary>
        /// <param name="buffer">A byte array buffer that contains the data to be written.</param>
        /// <remarks>
        /// This method is specific to nanoFramework. The equivalent method in the UWP API is: WriteAsync(IBuffer buffer).
        /// </remarks>
        //IAsyncOperationWithProgress<uint, uint> WriteAsync(IBuffer buffer)
        void Write(ref byte[] buffer);
    }
}
