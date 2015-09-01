using System.Collections.Generic;

namespace Panteon.Sdk.IO
{
    public interface IFileSystem
    {
        bool DoesDirectoryExist(string path);
        bool DoesFileExist(string path);
        IEnumerable<string> GetDirectories(string path);

        string GetFileNameWithoutExtension(string path);
        string ReadAllText(string path);
        string GetParentDirectory(string path);
        string CombinePaths(string path1, string path2);
    }
}