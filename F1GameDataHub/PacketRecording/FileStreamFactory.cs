using System.IO;
using System.IO.Compression;

namespace PacketRecording
{
    public class FileStreamFactory(bool compressionEnabled) : IStreamFactory
    {
        private readonly bool compressionEnabled = compressionEnabled;

        public Stream CreateStream(string filePath)
        {
            var path = filePath + (compressionEnabled ? ".gz" : ".bin");
            Console.WriteLine($"Creating stream for file: {path}");

            if (File.Exists(path))
            {
                throw new InvalidOperationException("Dump file already exists. Cannot run until dump.gz is deleted.");
            }

            var fileStream = new FileStream(path, FileMode.CreateNew);
            if (compressionEnabled)
            {
                return new GZipStream(fileStream, CompressionMode.Compress);
            }
            return fileStream;
        }
    }
}
