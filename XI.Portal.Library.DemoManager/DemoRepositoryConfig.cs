using System.IO;

namespace XI.Portal.Library.DemoManager
{
    public class DemoRepositoryConfig
    {
        public DemoRepositoryConfig()
        {
            if (!Directory.Exists(FileStoreBasePath))
                Directory.CreateDirectory(FileStoreBasePath);
        }

        public string FileStoreBasePath { get; set; } = @"C:\Temp\DemoManagerFiles";
    }
}