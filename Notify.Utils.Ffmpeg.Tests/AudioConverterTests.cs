namespace Notify.Utils.Ffmpeg.Tests
{
    public class AudioConverterTests
    {
        private AudioConverter converter;

        [SetUp]
        public void Setup()
        {
            var fileSystem = Substitute.For<IFileSystem>();
            converter = new AudioConverter(fileSystem);
        }

        [Test]
        public void ConvertTo_WhenProvidedWithNullStream_ThrowsArgumentNullException()
        {
            var exception = Assert.Throws<ArgumentNullException>(() => converter.ConvertTo(null, InputFormat.MP3));

            Assert.That(exception.Message, Is.EqualTo("Value cannot be null. (Parameter 'input')"));
        }

        [Test]
        public void ConvertTo_WhenProvidedWithNullInputFormat_ThrowsArgumentNullException()
        {
            var stream = Substitute.For<Stream>();

            var exception = Assert.Throws<ArgumentNullException>(() => converter.ConvertTo(stream, null));

            Assert.That(exception.Message, Is.EqualTo("Value cannot be null. (Parameter 'format')"));
        }
    }
}