namespace AppEndCommon
{
    public static class ExtensionsForDirectory
    {
        public static void Copy(this DirectoryInfo directoryToCopy, DirectoryInfo directoryTarget)
        {
            string sourcePath = directoryToCopy.FullName;
            string targetPath = directoryTarget.FullName;

            if (!directoryTarget.Exists)
            {
                Directory.CreateDirectory(targetPath);
            }

            foreach (string dirPath in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories))
            {
                Directory.CreateDirectory(dirPath.Replace(sourcePath, targetPath));
            }

            foreach (string newPath in Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories))
            {
                File.Copy(newPath, newPath.Replace(sourcePath, targetPath), true);
            }
        }
    }
}