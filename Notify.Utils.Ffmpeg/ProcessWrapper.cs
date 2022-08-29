ï»¿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notify.Utils.Ffmpeg
{
    internal interface IProcessWrapper
    {
        void Start(ProcessStartInfo processStartInfo);

        Task WriteInputAsync(Stream input, CancellationToken cancellationToken = default);
    }

    internal class ProcessWrapper : IProcessWrapper
    {
        private Process? process;

        public void Start(ProcessStartInfo processStartInfo)
        {
            process = Process.Start(processStartInfo);
        }

        public Task WriteInputAsync(Stream input, CancellationToken cancellationToken = default)
        {
            return input.CopyToAsync(process.StandardInput.BaseStream, cancellationToken);
        }
    }
}
