using System.Diagnostics;

namespace Notify.Utils.Ffmpeg
{
    public interface IAudioConverter
    {
        /// <summary>
        /// Converts input stream to 8K Sample Rate, Mono channel MuLaw encoded wav file.
        /// Output stream disposal is relegated to consumer
        /// </summary>
        /// <param name="input">Input Audio stream</param>
        /// <param name="format">Input audio format. Use <see cref="InputFormat.Parse(string)"/> to conveniently get valid format</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<Stream> ConvertToAsync(Stream input, InputFormat format, CancellationToken cancellationToken = default);
    }

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
        public async Task<Stream> ConvertToAsync(Stream input, InputFormat format, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(input);
            ArgumentNullException.ThrowIfNull(format);

            using var destinationFile = new TempFile(fileSystem);

            var ffmpegStartInfo = new ProcessStartInfo
            {
                FileName = ffmpegPath,
                Arguments = @$"-hide_banner -y -f {format} -i pipe:0 -acodec pcm_mulaw -ar 8000 -ac 1 -f wav ""{destinationFile}""",
                RedirectStandardInput = true,
                CreateNoWindow = false,
                RedirectStandardError = true
            };

            using var processWrapper = processWrapperFactory();

            try
            {
                processWrapper.Start(ffmpegStartInfo);
                await processWrapper.WriteInputAsync(input, cancellationToken);
                await processWrapper.WaitForExitAsync();

                var outputStream = new MemoryStream();
                await destinationFile.CopyToAsync(outputStream);

                return outputStream;
            }
            catch (Exception ex) // something more concrete should be used
            {
                var error = await processWrapper.GetErrorAsync();
                throw new ConversionFailedException(error, ex);
            }
        }
    }
}
