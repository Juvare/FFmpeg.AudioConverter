ï»¿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notify.Utils.Ffmpeg
{
    public interface IAudioConverter
    {
        Stream ConvertTo(Stream input, InputFormat format);
    }

    public class AudioConverter : IAudioConverter
    {
        public Stream ConvertTo(Stream input, InputFormat format)
        {
            ArgumentNullException.ThrowIfNull(input);
            ArgumentNullException.ThrowIfNull(format);

            throw new NotImplementedException();
        }
    }
}
