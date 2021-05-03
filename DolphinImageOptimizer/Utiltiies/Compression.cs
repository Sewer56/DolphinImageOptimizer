using System;
using System.Buffers;
using System.IO;
using K4os.Compression.LZ4;
using Reloaded.Memory.Streams;

namespace DolphinImageOptimizer.Utiltiies
{
    public static class Compression
    {
        private static ArrayPool<byte> _arrayPool = ArrayPool<byte>.Create(400_000_000, 10);

        public static void PickleToFile(string filePath, byte[] data, LZ4Level level)
        {
            var compressedData  = _arrayPool.Rent(data.Length);
            var encoded         = LZ4Codec.Encode(data, compressedData, level);

            using var fileStream = new FileStream(filePath, FileMode.Create);
            
            // Write Compressed and Uncompressed.
            fileStream.Write<int>(encoded);
            fileStream.Write<int>(data.Length);
            fileStream.Write(compressedData.AsSpan().Slice(0, encoded));
            _arrayPool.Return(compressedData);
        }
    }
}
