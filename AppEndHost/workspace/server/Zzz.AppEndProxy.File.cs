namespace Zzz
{
    public static partial class AppEndProxy
    {
        #region FileServices
        public static object? DownloadFile(string FileName)
        {
            return FileServices.DownloadFile(FileName);
        }

        public static object? UploadFile(string FileName, string FileBody)
        {
            return FileServices.UploadFile(FileName, FileBody);
        }

        public static object? GetFolderContent(string PathToRead)
        {
            return FileServices.GetFolderContent(PathToRead);
        }

        public static object? GetFileContent(string PathToRead)
        {
            return FileServices.GetFileContent(PathToRead);
        }

        public static object? GetZipFileContent(string PathToRead)
        {
            return FileServices.GetZipFileContent(PathToRead);
        }

        public static bool PackItemToZipFile(string ItemToPack, string ZipFile)
        {
            return FileServices.PackItemToZipFile(ItemToPack, ZipFile);
        }

        public static object? SaveFileContent(string PathToWrite, string FileContent)
        {
            return FileServices.SaveFileContent(PathToWrite, FileContent);
        }

        public static object? RemoveTheme(string FolderName)
        {
            return FileServices.RemoveTheme(FolderName);
        }

        public static object? DuplicateTheme(string OrigFolderName, string CopyFolderName)
        {
            return FileServices.DuplicateTheme(OrigFolderName, CopyFolderName);
        }

        public static object? SetThemeProps(string OrigFolderName, string FolderName, string Title, string SubTitle)
        {
            return FileServices.SetThemeProps(OrigFolderName, FolderName, Title, SubTitle);
        }

        public static object? GetSubApps()
        {
            return FileServices.GetSubApps();
        }

        public static object? GetUiComponents(string FolderName)
        {
            return FileServices.GetUiComponents(FolderName);
        }

        public static object? FolderHasContent(string PathToCheck)
        {
            return FileServices.FolderHasContent(PathToCheck);
        }

        public static object? CreateEmptyComponent(string ComponentFullPath)
        {
            return FileServices.CreateEmptyComponent(ComponentFullPath);
        }

        public static object? RenameItem(string ItemPath, string NewItemPath)
        {
            return FileServices.RenameItem(ItemPath, NewItemPath);
        }

        public static object? DuplicateItem(string PathToDuplicate, string PathType)
        {
            return FileServices.DuplicateItem(PathToDuplicate, PathType);
        }

        public static object? DeleteItem(string ItemPath, string PathType)
        {
            return FileServices.DeleteItem(ItemPath, PathType);
        }

        public static object? DeleteFileItem(string FilePath)
        {
            return FileServices.DeleteFileItem(FilePath);
        }

        public static object? DeleteFolderItem(string PathToDelete, bool Recursive)
        {
            return FileServices.DeleteFolderItem(PathToDelete, Recursive);
        }

        public static object? CreateNewFolder(string PathToCreate)
        {
            return FileServices.CreateNewFolder(PathToCreate);
        }

        public static object? CreateNewFile(string PathToCreate)
        {
            return FileServices.CreateNewFile(PathToCreate);
        }

        public static object? GetStoredApiCalls()
        {
            return FileServices.GetStoredApiCalls();
        }

        public static object? GetStoredSqlQueries()
        {
            return FileServices.GetStoredSqlQueries();
        }

        public static object? ExtractTranslationKeys(string FolderName)
        {
            return FileServices.ExtractTranslationKeys(FolderName);
        }

        public static object? ReadPackages()
        {
            return FileServices.ReadPackages();
        }

        public static object? SavePackageInfo(string PackageName, string PackageNewName, JsonElement PackageInfo)
        {
            return FileServices.SavePackageInfo(PackageName, PackageNewName, PackageInfo);
        }

        public static object? DownloadPackage(string PackageName)
        {
            return FileServices.DownloadPackage(PackageName);
        }

        public static object? UploadPackage(string PackageName, string PackageBody)
        {
            return FileServices.UploadPackage(PackageName, PackageBody);
        }

        public static object? RemovePackage(string PackageName)
        {
            return FileServices.RemovePackage(PackageName);
        }

        public static object? InstallPackage(AppEndUser? Actor, string PackageName, string DbConfName)
        {
            return FileServices.InstallPackage(Actor, PackageName, DbConfName);
        }

        public static object? UnInstallPackage(string PackageName)
        {
            return FileServices.UnInstallPackage(PackageName);
        }

        public static object? RepackPackage(string PackageName)
        {
            return FileServices.RepackPackage(PackageName);
        }
        #endregion
    }
}
