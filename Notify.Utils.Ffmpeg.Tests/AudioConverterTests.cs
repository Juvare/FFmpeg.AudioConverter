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
        public void ConvertToAsync_WhenProvidedWithNullStream_ThrowsArgumentNullException()
        {
            var converter = new AudioConverter(fileSystem, PlatformID.Unix, () => processWrapper);
            var exception = Assert.ThrowsAsync<ArgumentNullException>(() => converter.ConvertToAsync(null, InputFormat.MP3));

            Assert.That(exception.Message, Is.EqualTo("Value cannot be null. (Parameter 'input')"));
        }

        [Test]
        public void ConvertToAsync_WhenProvidedWithNullInputFormat_ThrowsArgumentNullException()
        {
            var converter = new AudioConverter(fileSystem, PlatformID.Unix, () => processWrapper);
            var stream = Substitute.For<Stream>();

            var exception = Assert.ThrowsAsync<ArgumentNullException>(() => converter.ConvertToAsync(stream, null));

            Assert.That(exception.Message, Is.EqualTo("Value cannot be null. (Parameter 'format')"));
        }

        [Test]
        public async Task Constructor_WhenWorkingInLinuxEnvironment_UsesLinuxVersionOfFfmpeg()
        {
            var converter = new AudioConverter(fileSystem, PlatformID.Unix, () => processWrapper);
            var stream = Substitute.For<Stream>();

            await converter.ConvertToAsync(stream, InputFormat.WAV);

            Assert.That(processStartInfo.FileName, Is.EqualTo("./ffmpeg/ffmpeg"));
        }

        [Test]
        public async Task Constructor_WhenWorkingInWindowsEnvironment_UsesWindowsVersionOfFfmpeg()
        {
            var converter = new AudioConverter(fileSystem, PlatformID.Win32NT, () => processWrapper);
            var stream = Substitute.For<Stream>();

            await converter.ConvertToAsync(stream, InputFormat.WAV);

            Assert.That(processStartInfo.FileName, Is.EqualTo("./ffmpeg/ffmpeg.exe"));
        }

        [Test]
        public void Constructor_WhenWorkingInOtherEnvironment_ThrowsPlatformNotSupportedException()
        {
            Assert.Throws<PlatformNotSupportedException>(() => new AudioConverter(fileSystem, PlatformID.Other, () => processWrapper));
        }

        [Test]
        public async Task ConvertToAsync_InstructsFfmpegToOverrideDestinationFile()
        {
            var converter = new AudioConverter(fileSystem, PlatformID.Win32NT, () => processWrapper);

            await converter.ConvertToAsync(Substitute.For<Stream>(), InputFormat.WAV);

            Assert.That(processStartInfo.Arguments, Does.Contain("-y"));
        }

        [Test]
        public async Task ConvertToAsync_PassesInputFormatToFfmpeg()
        {
            var converter = new AudioConverter(fileSystem, PlatformID.Win32NT, () => processWrapper);

            await converter.ConvertToAsync(Substitute.For<Stream>(), InputFormat.WAV);

            Assert.That(processStartInfo.Arguments, Does.Contain("-f wav -i"));
        }

        [Test]
        public async Task ConvertToAsync_InstructsFfmpegToReadFromProcessInputStream()
        {
            var converter = new AudioConverter(fileSystem, PlatformID.Win32NT, () => processWrapper);

            await converter.ConvertToAsync(Substitute.For<Stream>(), InputFormat.WAV);

            Assert.That(processStartInfo.Arguments, Does.Contain("-i pipe:0"));
        }

        [Test]
        public async Task ConvertToAsync_InstructsFfmpegToConvertFileTo8KSampleRateMonoMuLawWav()
        {
            var converter = new AudioConverter(fileSystem, PlatformID.Win32NT, () => processWrapper);

            await converter.ConvertToAsync(Substitute.For<Stream>(), InputFormat.WAV);

            Assert.That(processStartInfo.Arguments, Does.Contain("-acodec pcm_mulaw"), "Encoding must be MuLaw");
            Assert.That(processStartInfo.Arguments, Does.Contain("-ar 8000"), "Sample rate must be 8000");
            Assert.That(processStartInfo.Arguments, Does.Contain("-ac 1"), "Output must be mono channel");
            Assert.That(processStartInfo.Arguments, Does.Contain("-f wav \""), "Output format must be wav");
        }

        [Test]
        public async Task ConvertToAsync_InstructsToOutputToTempFile()
        {
            var converter = new AudioConverter(fileSystem, PlatformID.Win32NT, () => processWrapper);

            await converter.ConvertToAsync(Substitute.For<Stream>(), InputFormat.WAV);

            Assert.That(processStartInfo.Arguments, Does.Contain("\"temp-output-file.tmp\""));
        }

        [Test]
        public async Task ConvertToAsync_AsksToRedirectStandardInpuAndError()
        {
            var converter = new AudioConverter(fileSystem, PlatformID.Win32NT, () => processWrapper);

            await converter.ConvertToAsync(Substitute.For<Stream>(), InputFormat.WAV);

            Assert.That(processStartInfo.RedirectStandardError, Is.True);
            Assert.That(processStartInfo.RedirectStandardInput, Is.True);
        }

        [Test]
        public async Task ConvertToAsync_WritesInputStreamToFfmpegProcess()
        {
            var converter = new AudioConverter(fileSystem, PlatformID.Win32NT, () => processWrapper);
            var stream = Substitute.For<Stream>();

            await converter.ConvertToAsync(stream, InputFormat.WAV);

            await processWrapper.Received().WriteInputAsync(stream, Arg.Any<CancellationToken>());
        }

        [Test]
        public void ConvertToAsync_WhenStartingProcessFails_ThrowsConversionFailedException()
        {
            processWrapper.When(p => p.Start(Arg.Any<ProcessStartInfo>())).Throw(new Exception("test"));
            var converter = new AudioConverter(fileSystem, PlatformID.Win32NT, () => processWrapper);

            Assert.ThrowsAsync<ConversionFailedException>(() => converter.ConvertToAsync(Substitute.For<Stream>(), InputFormat.WAV));
        }

        [Test]
        public void ConvertToAsync_WhenWrittingToProcessFails_ThrowsConversionFailedException()
        {
            processWrapper.WriteInputAsync(Arg.Any<Stream>(), Arg.Any<CancellationToken>()).Returns(Task.FromException(new Exception("test")));
            var converter = new AudioConverter(fileSystem, PlatformID.Win32NT, () => processWrapper);

            Assert.ThrowsAsync<ConversionFailedException>(() => converter.ConvertToAsync(Substitute.For<Stream>(), InputFormat.WAV));
        }

        [Test]
        public void ConvertToAsync_WhenFfmpegProcessFails_RetrievesErrorOuput()
        {
            processWrapper.GetErrorAsync().Returns(Task.FromResult("test error"));
            processWrapper.WriteInputAsync(Arg.Any<Stream>(), Arg.Any<CancellationToken>()).Returns(Task.FromException(new Exception("test")));
            var converter = new AudioConverter(fileSystem, PlatformID.Win32NT, () => processWrapper);

            var exception = Assert.ThrowsAsync<ConversionFailedException>(() => converter.ConvertToAsync(Substitute.For<Stream>(), InputFormat.WAV));

            Assert.That(exception.Message, Is.EqualTo("test error"));
        }

        [Test]
        public async Task ConvertToAsync_WaitsForProcessToFinishBeforeCopyintTempFileContentToOutputStream()
        {
            var converter = new AudioConverter(fileSystem, PlatformID.Win32NT, () => processWrapper);

            await converter.ConvertToAsync(new MemoryStream(), InputFormat.WAV);

            Received.InOrder(() => {
                processWrapper.WaitForExitAsync();
                fileSystem.ReadAllBytesAsync(Arg.Any<string>()); // used indirectly
            });
        }

        [Test]
        public async Task ConvertToAsync_WhenAllGoesToPlan_ReturnsConvertedStream()
        {
            var converter = new AudioConverter(fileSystem, PlatformID.Win32NT, () => processWrapper);

            var output = await converter.ConvertToAsync(new MemoryStream(), InputFormat.WAV);

            Assert.That(output, Is.Not.Null);
        }
    }
}