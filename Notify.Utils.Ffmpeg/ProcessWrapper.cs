ï»¿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notify.Utils.Ffmpeg
{
    internal interface IProcessWrapper : IDisposable
    {
        void Start(ProcessStartInfo processStartInfo);

        Task WriteInputAsync(Stream input, CancellationToken cancellationToken = default);

        Task<string> GetErrorAsync();

        Task WaitForExitAsync();
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

        public Task WaitForExitAsync() => process.WaitForExitAsync();

        public void Dispose() => process?.Dispose();
    }
}
