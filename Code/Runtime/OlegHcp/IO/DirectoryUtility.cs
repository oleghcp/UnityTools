using System.IO;

namespace OlegHcp.IO
{
    public static class DirectoryUtility
    {
        public static void CopyDirectory(string sourceDir, string destinationDir, bool recursive, bool overwrite)
        {
            DirectoryInfo dir = new DirectoryInfo(sourceDir);

            if (!dir.Exists)
                throw new DirectoryNotFoundException($"Source directory not found: {dir.FullName}");

            Directory.CreateDirectory(destinationDir);

            foreach (FileInfo file in dir.EnumerateFiles())
            {
                string targetFilePath = Path.Combine(destinationDir, file.Name);
                file.CopyTo(targetFilePath, overwrite);
            }

            if (recursive)
            {
                foreach (DirectoryInfo subDir in dir.EnumerateDirectories())
                {
                    string newDestinationDir = Path.Combine(destinationDir, subDir.Name);
                    CopyDirectory(subDir.FullName, newDestinationDir, true, overwrite);
                }
            }
        }
    }
}
