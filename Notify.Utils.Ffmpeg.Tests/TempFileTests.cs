ï»¿using NSubstitute;

namespace Notify.Utils.Ffmpeg.Tests
{
    public class TempFileTests
    {
        private IFileSystem fileSystem;

        [SetUp]
        public void SetUp()
        {
            fileSystem = Substitute.For<IFileSystem>();
            fileSystem.GetTempFileName().Returns("unit-testing.tmp");
        }

        [Test]
        public void Constructor_CreatesTempFile()
        {
            using var _ = new TempFile(fileSystem);

            fileSystem.Received().GetTempFileName();
        }

        [Test]
        public void Dispose_RemovesTempFile()
        {
            using (var _ = new TempFile(fileSystem)) { }

            fileSystem.Received().DeleteFile("unit-testing.tmp");
        }
    }
}
