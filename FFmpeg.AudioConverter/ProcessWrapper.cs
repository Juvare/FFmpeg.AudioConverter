using System.Diagnostics;

namespace FFmpeg.AudioConverter
{
    internal interface IProcessWrapper : IDisposable
    {
        void Start(ProcessStartInfo processStartInfo);

        Task WriteInputAsync(Stream input, CancellationToken cancellationToken = default);

        Task<string> GetErrorAsync();

        Task<int> WaitForExitAsync();
    }

    internal class ProcessWrapper : IProcessWrapper
    {
        private Process? process;

        public void Start(ProcessStartInfo processStartInfo)
        {
            process = Process.Start(processStartInfo);
        }

        public async Task WriteInputAsync(Stream input, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(process);
            
            await input.CopyToAsync(process.StandardInput.BaseStream, cancellationToken);
            process.StandardInput.Close();
        }

        public async Task<string> GetErrorAsync()
        {
            try
            {
                if (process == null)
                {
                    return "";
                }
                
                return await process.StandardError.ReadToEndAsync();
            }
            catch
            {
                return string.Empty;
            }
        }

        public async Task<int> WaitForExitAsync()
        {
            if (process == null)
            {
                return -1;
            }
            
            await process.WaitForExitAsync();

            return process.ExitCode;
        }

        public void Dispose() => process?.Dispose();
    }
}
