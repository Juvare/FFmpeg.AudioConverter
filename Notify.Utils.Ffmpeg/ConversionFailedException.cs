namespace Notify.Utils.Ffmpeg
{
    public sealed class ConversionFailedException : Exception
    {
        public ConversionFailedException(string message, Exception innerException) : base(message, innerException) { }

        public ConversionFailedException(string message) : base(message) { }
    }
}
