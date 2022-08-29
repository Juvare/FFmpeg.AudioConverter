ï»¿namespace Notify.Utils.Ffmpeg.Tests
{
    public class TempFileTests
    {
        private IFileSystem fileSystem;

        [SetUp]
        public void SetUp()
        {
            fileSystem = Substitute.For<IFileSystem>();
            fileSystem.GetTempFileName().Returns("unit-testing.tmp");
            fileSystem.ReadAllBytesAsync(Arg.Any<string>()).Returns(Task.FromResult(new byte[] { 0x00, 0x01, 0x10 }));
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

        [Test]
        public async Task CopyToAsync_CopiesTempFileToDestinationStream()
        {
            using var stream = new MemoryStream();
            using var tempFile = new TempFile(fileSystem);

            await tempFile.CopyToAsync(stream);

            Assert.That(stream.ToArray(), Is.EqualTo(new byte[] { 0x00, 0x01, 0x10 }));
        }
    }
}
