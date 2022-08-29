using System.Diagnostics;

namespace Notify.Utils.Ffmpeg
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
            await input.CopyToAsync(process.StandardInput.BaseStream, cancellationToken);
            process.StandardInput.Close();
        }

        public async Task<string> GetErrorAsync()
        {
            try
            {
                return await process.StandardError.ReadToEndAsync();
            }
            catch
            {
                return string.Empty;
            }
        }

        public async Task<int> WaitForExitAsync()
        {
            await process.WaitForExitAsync();

            return process.ExitCode;
        }

        public void Dispose() => process?.Dispose();
    }
}
