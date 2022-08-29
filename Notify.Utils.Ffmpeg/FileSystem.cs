ï»¿namespace Notify.Utils.Ffmpeg
{
    internal interface IFileSystem
    {
        string GetTempFileName();
        void DeleteFile(string path);
        Task<byte[]> ReadAllBytesAsync(string path);

        string GetAssemblyLocation();
    }

    internal class FileSystem : IFileSystem
    {
        public void DeleteFile(string path) => File.Delete(path);

        public string GetTempFileName() => Path.GetTempFileName();

        public Task<byte[]> ReadAllBytesAsync(string path) => File.ReadAllBytesAsync(path);

        public string GetAssemblyLocation() => Path.GetDirectoryName(typeof(FileSystem).Assembly.Location);
    }
}
