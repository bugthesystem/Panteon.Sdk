namespace Panteon.Sdk.IO
{
    public interface IFileReader
    {
        FileContentResult ReadFileContent(string path);
    }
}