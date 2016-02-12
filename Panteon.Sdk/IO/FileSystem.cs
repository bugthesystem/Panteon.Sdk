using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Panteon.Sdk.IO
{
    public class FileSystem : IFileSystem
    {
        public bool DoesDirectoryExist(string path)
        {
            if (!string.IsNullOrWhiteSpace(path))
            {
                return Directory.Exists(path);
            }

            return false;
        }

        public IEnumerable<string> GetDirectories(string path)
        {
            if (!string.IsNullOrWhiteSpace(path))
            {
                return Directory.GetDirectories(path);
            }

            return Enumerable.Empty<string>();
        }


        public bool DoesFileExist(string path)
        {
            if (!string.IsNullOrWhiteSpace(path))
            {
                return File.Exists(path);
            }

            return false;
        }

        public string GetFileNameWithoutExtension(string path)
        {
            return Path.GetFileNameWithoutExtension(path);
        }

        public string ReadAllText(string path)
        {
            return File.ReadAllText(path);
        }

        public string GetParentDirectory(string path)
        {
            return Directory.GetParent(path).ToString();
        }

        public string CombinePaths(string path1, string path2)
        {
            return Path.Combine(path1, path2);
        }

        public void TryCreateDirectory(string path)
        {
            if (!DoesDirectoryExist(path))
            {
                Directory.CreateDirectory(path);
            }
        }
    }
}