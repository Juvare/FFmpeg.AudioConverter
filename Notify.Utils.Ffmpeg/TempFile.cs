ï»¿namespace Notify.Utils.Ffmpeg
{
    internal class TempFile : IDisposable
    {
        private readonly string tempFile;
        private readonly IFileSystem fileSystem;

        public TempFile(IFileSystem fileSystem)
        {
            this.fileSystem = fileSystem;

            tempFile = fileSystem.GetTempFileName();
        }

        public async Task CopyToAsync(Stream destination)
        {
            var bytes = await fileSystem.ReadAllBytesAsync(tempFile);

            await destination.WriteAsync(bytes);
        }

        public void Dispose() => fileSystem.DeleteFile(tempFile);

        public static implicit operator string(TempFile tempFile) => tempFile.tempFile;
    }
}
