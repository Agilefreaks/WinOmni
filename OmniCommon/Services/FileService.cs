namespace OmniCommon.Services
{
    using System;
    using System.IO;
    using OmniCommon.Interfaces;

    public class FileService : IFileService
    {
        public string AppDataDir
        {
            get
            {
                return
                    Path.Combine(
                        Path.Combine(
                            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                            AppInfo.PublisherName),
                        AppInfo.ApplicationName);
            }
        }

        public bool Exists(string filePath)
        {
            return File.Exists(filePath);
        }

        public Stream Open(string path, FileMode mode, FileAccess access)
        {
            return File.Open(path, mode, access);
        }
    }
}
