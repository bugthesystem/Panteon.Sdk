using System;

namespace Panteon.Sdk.IO
{
    public class FileReader : IFileReader
    {
        private readonly IFileSystem _fileSystem;

        public FileReader(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        public FileContentResult ReadFileContent(string path)
        {
            FileContentResult result = new FileContentResult { IsContentRead = true };

            try
            {
                result.Content = _fileSystem.ReadAllText(path);
                result.Name = _fileSystem.GetFileNameWithoutExtension(path);
            }
            catch (Exception)
            {
                result.IsContentRead = false;
            }

            return result;
        }
    }
}