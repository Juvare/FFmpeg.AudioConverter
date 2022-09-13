using System.Diagnostics;

namespace FFmpeg.AudioConverter
{
    public interface IAudioConverter
    {
        /// <summary>
        /// Converts input stream to 8K Sample Rate, Mono channel MuLaw encoded wav file.
        /// Output stream disposal is relegated to consumer
        /// </summary>
        /// <param name="input">Input Audio stream</param>
        /// <param name="inputFormat">Input audio format. Use <see cref="InputFormat.Parse(string)"/> to conveniently get valid format</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<Stream> ConvertToWavAsync(Stream input, InputFormat inputFormat, CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// ffmpeg wrapper for audio conversion
    /// </summary>
    public class AudioConverter : IAudioConverter
    {
        private readonly IFileSystem fileSystem;
        private readonly Func<IProcessWrapper> processWrapperFactory;
        private string ffmpegPath;

        public AudioConverter() : this(new FileSystem(), Environment.OSVersion.Platform, () => new ProcessWrapper())
        {
        }

        internal AudioConverter(IFileSystem fileSystem, PlatformID platform, Func<IProcessWrapper> processWrapperFactory)
        {
            this.fileSystem = fileSystem;
            this.processWrapperFactory = processWrapperFactory;
            var assemblyDirectory = fileSystem.GetAssemblyLocation();
            ffmpegPath = platform switch
            {
                PlatformID.Win32NT => $"{assemblyDirectory}/ffmpeg/ffmpeg.exe",
                PlatformID.Unix => $"{assemblyDirectory}/ffmpeg/ffmpeg",
                _ => throw new PlatformNotSupportedException()
            };
        }

        /// <inheritdoc />
        public async Task<Stream> ConvertToWavAsync(Stream input, InputFormat inputFormat, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(input);
            ArgumentNullException.ThrowIfNull(inputFormat);

            using var destinationFile = new TempFile(fileSystem);

            var ffmpegStartInfo = new ProcessStartInfo
            {
                FileName = ffmpegPath,
                Arguments = @$"-hide_banner -y -f {inputFormat} -i pipe:0 -acodec pcm_mulaw -ar 8000 -ac 1 -f wav ""{destinationFile}""",
                RedirectStandardInput = true,
                CreateNoWindow = false,
                RedirectStandardError = true
            };

            using var processWrapper = processWrapperFactory();

            try
            {
                processWrapper.Start(ffmpegStartInfo);
                await processWrapper.WriteInputAsync(input, cancellationToken);

                var exitCode = 0;
                if ((exitCode = await processWrapper.WaitForExitAsync()) != 0)
                {
                    var error = await processWrapper.GetErrorAsync();
                    var message = $"ffmpeg exited with code {exitCode}\r\n{error}";
                    throw new ConversionFailedException(message);
                }

                var outputStream = new MemoryStream();
                await destinationFile.CopyToAsync(outputStream);
                outputStream.Position = 0;

                return outputStream;
            }
            catch (Exception ex) when (ex is not ConversionFailedException) // something more concrete should be used
            {
                var error = await processWrapper.GetErrorAsync();
                throw new ConversionFailedException(error, ex);
            }
        }
    }
}
