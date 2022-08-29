ï»¿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Notify.Utils.Ffmpeg
{
    public interface IAudioConverter
    {
        Task<Stream> ConvertTo(Stream input, InputFormat format, CancellationToken cancellationToken = default);
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
            ffmpegPath = platform switch
            {
                PlatformID.Win32NT => "./ffmpeg/ffmpeg.exe",
                PlatformID.Unix => "./ffmpeg/ffmpeg",
                _ => throw new PlatformNotSupportedException()
            };
        }

        public async Task<Stream> ConvertTo(Stream input, InputFormat format, CancellationToken cancellationToken = default)
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
            }
            catch (Exception ex) // something more concrete should be used
            {
                var error = await processWrapper.GetErrorAsync();
                throw new ConversionFailedException(error, ex);
            }
            

            return null;
        }
    }
}
