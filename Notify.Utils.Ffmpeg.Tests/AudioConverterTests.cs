using System.Diagnostics;

namespace Notify.Utils.Ffmpeg.Tests
{
    public class AudioConverterTests
    {
        private IFileSystem fileSystem;
        private IProcessWrapper processWrapper;
        private ProcessStartInfo processStartInfo;

        [SetUp]
        public void Setup()
        {
            fileSystem = Substitute.For<IFileSystem>();
            processWrapper = Substitute.For<IProcessWrapper>();
            processStartInfo = null;
            processWrapper.When(pw => pw.Start(Arg.Any<ProcessStartInfo>())).Do(callInfo => processStartInfo = callInfo.Arg<ProcessStartInfo>());
        }

        [Test]
        public void ConvertTo_WhenProvidedWithNullStream_ThrowsArgumentNullException()
        {
            var converter = new AudioConverter(fileSystem, PlatformID.Unix, () => processWrapper);
            var exception = Assert.Throws<ArgumentNullException>(() => converter.ConvertTo(null, InputFormat.MP3));

            Assert.That(exception.Message, Is.EqualTo("Value cannot be null. (Parameter 'input')"));
        }

        [Test]
        public void ConvertTo_WhenProvidedWithNullInputFormat_ThrowsArgumentNullException()
        {
            var converter = new AudioConverter(fileSystem, PlatformID.Unix, () => processWrapper);
            var stream = Substitute.For<Stream>();

            var exception = Assert.Throws<ArgumentNullException>(() => converter.ConvertTo(stream, null));

            Assert.That(exception.Message, Is.EqualTo("Value cannot be null. (Parameter 'format')"));
        }

        [Test]
        public void Constructor_WhenWorkingInLinuxEnvironment_UsesLinuxVersionOfFfmpeg()
        {
            var converter = new AudioConverter(fileSystem, PlatformID.Unix, () => processWrapper);
            var stream = Substitute.For<Stream>();

            converter.ConvertTo(stream, InputFormat.WAV);

            Assert.That(processStartInfo.FileName, Is.EqualTo("./ffmpeg/ffmpeg"));
        }

        [Test]
        public void Constructor_WhenWorkingInWindowsEnvironment_UsesWindowsVersionOfFfmpeg()
        {
            var converter = new AudioConverter(fileSystem, PlatformID.Win32NT, () => processWrapper);
            var stream = Substitute.For<Stream>();

            converter.ConvertTo(stream, InputFormat.WAV);

            Assert.That(processStartInfo.FileName, Is.EqualTo("./ffmpeg/ffmpeg.exe"));
        }

        [Test]
        public void Constructor_WhenWorkingInOtherEnvironment_ThrowsPlatformNotSupportedException()
        {
            Assert.Throws<PlatformNotSupportedException>(() => new AudioConverter(fileSystem, PlatformID.Other, () => processWrapper));
        }
    }
}