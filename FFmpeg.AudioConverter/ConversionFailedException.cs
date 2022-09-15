namespace FFmpeg.AudioConverter
{
    /// <summary>
    /// The exception that is thrown when conversion fails.
    /// </summary>
    public sealed class ConversionFailedException : Exception
    {
        /// <summary>
        /// Initialize a new ConversionFailedException instance.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="innerException">The exception that is the cause of the current exception. If the <paramref name="innerException" /> parameter is not a null reference, the current exception is raised in a catch block that handles the inner exception.</param>
        public ConversionFailedException(string? message, Exception innerException) : base(message, innerException) { }

        /// <summary>
        /// Initialize a new ConversionFailedException instance.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public ConversionFailedException(string message) : base(message) { }
    }
}
