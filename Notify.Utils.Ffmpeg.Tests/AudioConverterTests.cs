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
            fileSystem.GetTempFileName().Returns("temp-output-file.tmp");
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

        [Test]
        public void ConvertTo_InstructsFfmpegToOverrideDestinationFile()
        {
            var converter = new AudioConverter(fileSystem, PlatformID.Win32NT, () => processWrapper);

            converter.ConvertTo(Substitute.For<Stream>(), InputFormat.WAV);

            Assert.That(processStartInfo.Arguments, Does.Contain("-y"));
        }

        [Test]
        public void ConvertTo_PassesInputFormatToFfmpeg()
        {
            var converter = new AudioConverter(fileSystem, PlatformID.Win32NT, () => processWrapper);

            converter.ConvertTo(Substitute.For<Stream>(), InputFormat.WAV);

            Assert.That(processStartInfo.Arguments, Does.Contain("-f wav -i"));
        }

        [Test]
        public void ConvertTo_InstructsFfmpegToReadFromProcessInputStream()
        {
            var converter = new AudioConverter(fileSystem, PlatformID.Win32NT, () => processWrapper);

            converter.ConvertTo(Substitute.For<Stream>(), InputFormat.WAV);

            Assert.That(processStartInfo.Arguments, Does.Contain("-i pipe:0"));
        }

        [Test]
        public void ConvertTo_InstructsFfmpegToConvertFileTo8KSampleRateMonoMuLawWav()
        {
            var converter = new AudioConverter(fileSystem, PlatformID.Win32NT, () => processWrapper);

            converter.ConvertTo(Substitute.For<Stream>(), InputFormat.WAV);

            Assert.That(processStartInfo.Arguments, Does.Contain("-acodec pcm_mulaw"), "Encoding must be MuLaw");
            Assert.That(processStartInfo.Arguments, Does.Contain("-ar 8000"), "Sample rate must be 8000");
            Assert.That(processStartInfo.Arguments, Does.Contain("-ac 1"), "Output must be mono channel");
            Assert.That(processStartInfo.Arguments, Does.Contain("-f wav \""), "Output must be mono channel");
        }

        [Test]
        public void ConvertTo_InstructsToOutputToTempFile()
        {
            var converter = new AudioConverter(fileSystem, PlatformID.Win32NT, () => processWrapper);

            converter.ConvertTo(Substitute.For<Stream>(), InputFormat.WAV);

            Assert.That(processStartInfo.Arguments, Does.Contain("\"temp-output-file.tmp\""));
        }

        [Test]
        public void ConvertTo_AsksToRedirectStandardInpuAndError()
        {
            var converter = new AudioConverter(fileSystem, PlatformID.Win32NT, () => processWrapper);

            converter.ConvertTo(Substitute.For<Stream>(), InputFormat.WAV);

            Assert.That(processStartInfo.RedirectStandardError, Is.True);
            Assert.That(processStartInfo.RedirectStandardInput, Is.True);
        }
    }
}