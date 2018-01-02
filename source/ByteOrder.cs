//
// Copyright (c) 2017 The nanoFramework project contributors
// See LICENSE file in the project root for full license information.
//

namespace Windows.Storage.Streams
{
    /// <summary>
    /// Specifies the byte order of a stream.
    /// </summary>
    /// <remarks>nanoFramework API only supports LittleEndian order.</remarks>
    public enum ByteOrder
    {
        /// <summary>
        /// The least significant byte (lowest address) is stored first.
        /// </summary>
        LittleEndian
    }
}
