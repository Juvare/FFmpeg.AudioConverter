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
    }

    internal class ProcessWrapper : IProcessWrapper
    {
        private Process? process;

        public void Start(ProcessStartInfo processStartInfo)
        {
            process = Process.Start(processStartInfo);
        }
    }
}
