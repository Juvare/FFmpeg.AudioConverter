namespace Notify.Utils.Ffmpeg.Tests
{
    public class AudioConverterTests
    {
        private AudioConverter converter;

        [SetUp]
        public void Setup()
        {
            converter = new AudioConverter();
        }

        [Test]
        public void ConvertTo_ThrowsNotImplementedException()
        {
            Assert.Throws<NotImplementedException>(() => converter.ConvertTo(null));
        }
    }
}