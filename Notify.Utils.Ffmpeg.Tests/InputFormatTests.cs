ï»¿namespace Notify.Utils.Ffmpeg.Tests
{
    public class InputFormatTests
    {
        [TestCase("mp3")]
        [TestCase("MP3")]
        public void Parse_WhenProvidedWithMp3File_ReturnsMp3Format(string extension)
        {
            var format = InputFormat.Parse($"thenname.{extension}");

            Assert.That(format.ToString(), Is.EqualTo("mp3"));
        }

        [TestCase("wav")]
        [TestCase("WAV")]
        public void Parse_WhenProvidedWithWavFile_ReturnsWavFormat(string extension)
        {
            var format = InputFormat.Parse($"thenname.{extension}");

            Assert.That(format.ToString(), Is.EqualTo("wav"));
        }

        [Test]
        public void Parse_WhenProvidedWithUnsupportedFormat_ThrowsNotSupportedException()
        {
            var exception = Assert.Throws<NotSupportedException>(() => InputFormat.Parse("thenname.ogg"));

            Assert.That(exception.Message, Is.EqualTo(".OGG is not supported. Only MP3 and WAV is"));
        }
    }
}
