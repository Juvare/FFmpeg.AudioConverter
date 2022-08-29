ï»¿namespace Notify.Utils.Ffmpeg
{
    public class ConversionFailedException : Exception
    {
        public ConversionFailedException(Exception innerException) : base("Conversion failed", innerException)
        {

        }
    }
}
