namespace FFmpeg.AudioConverter
{
    public sealed class ConversionFailedException : Exception
    {
        public ConversionFailedException(string message, Exception innerException) : base(message, innerException) { }

        public ConversionFailedException(string message) : base(message) { }
    }
}
