namespace OmniCommon.Interfaces
{
    using System.IO;

    public interface IFileService
    {
        string AppDataDir { get; }

        bool Exists(string filePath);

        Stream Open(string path, FileMode mode, FileAccess access);
    }
}