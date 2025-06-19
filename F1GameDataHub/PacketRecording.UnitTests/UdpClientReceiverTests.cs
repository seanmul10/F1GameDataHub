using System;
using System.IO;
using System.IO.Compression;
using PacketRecording;

namespace PacketRecording.UnitTests
{
    [TestClass]
    public class FileStreamFactoryTests
    {
        private string _testDirectory;

        [TestInitialize]
        public void TestInitialize()
        {
            _testDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(_testDirectory);
        }

        [TestMethod]
        public void CreateStream_CompressionDisabled_CreatesBinFile()
        {
            var factory = new FileStreamFactory(compressionEnabled: false);
            var filePath = Path.Combine(_testDirectory, "testfile");

            using var stream = factory.CreateStream(filePath);
            Assert.IsInstanceOfType<FileStream>(stream);

            stream.Flush();
            Assert.IsTrue(File.Exists(filePath + ".bin"));
            Assert.IsFalse(File.Exists(filePath + ".gz"));
        }

        [TestMethod]
        public void CreateStream_CompressionEnabled_CreatesGzFile()
        {
            var factory = new FileStreamFactory(compressionEnabled: true);
            var filePath = Path.Combine(_testDirectory, "testfile");

            using var stream = factory.CreateStream(filePath);
            Assert.IsInstanceOfType<GZipStream>(stream);

            stream.Flush();
            Assert.IsTrue(File.Exists(filePath + ".gz"));
            Assert.IsFalse(File.Exists(filePath + ".bin"));
        }

        [TestCleanup]
        public void Cleanup()
        {
            if (Directory.Exists(_testDirectory))
            {
                Directory.Delete(_testDirectory, true);
            }
        }
    }
}
