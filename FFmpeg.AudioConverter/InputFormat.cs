namespace FFmpeg.AudioConverter
{
    /// <summary>
    /// Classifier for input formats. Currently only MP3 and WAV input files are supported.
    /// </summary>
    public class InputFormat
    {
        private readonly string format;

        /// <summary>
        /// MP3 File format 
        /// </summary>
        public static readonly InputFormat MP3 = new InputFormat("mp3");
        
        /// <summary>
        /// WAV File format
        /// </summary>
        public static readonly InputFormat WAV = new InputFormat("wav");

        private InputFormat(string format)
        {
            this.format = format;
        }

        /// <summary>
        /// Checks if format is supported by parsing file name.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>Parsed format instance.</returns>
        /// <exception cref="NotSupportedException">Will throw if file was unsupported extension.</exception>
        public static InputFormat Parse(string fileName)
        {
            var extension = Path.GetExtension(fileName).ToUpperInvariant();

            return extension switch
            {
                ".MP3" => MP3,
                ".WAV" => WAV,
                _ => throw new NotSupportedException($"{extension} is not supported. Only MP3 and WAV is")
            };
        }

        /// <summary>
        /// Implicitly converts <see cref="InputFormat"/> to string.
        /// </summary>
        /// <param name="format">Format to convert.</param>
        /// <returns>format as string.</returns>
        public static implicit operator string(InputFormat format) => format.format;

        /// <summary>
        /// Converts strongly typed data to an equivalent string representation.
        /// </summary>
        /// <returns>string representation.</returns>
        public override string ToString() => this;
    }
}
