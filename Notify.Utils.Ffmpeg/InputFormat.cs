namespace Notify.Utils.Ffmpeg
{
    public class InputFormat
    {
        private readonly string format;

        public static InputFormat MP3 = new InputFormat("mp3");
        public static InputFormat WAV = new InputFormat("wav");

        private InputFormat(string format)
        {
            this.format = format;
        }

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

        public static implicit operator string(InputFormat format) => format.format;

        public override string ToString() => this;
    }
}
