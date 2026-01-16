using AppEndCommon;
using AppEndDbIO;
using Newtonsoft.Json.Linq;
using System.IO.Compression;
using System.Text.Json;

namespace AppEndServer
{
    public static class FileServices
    {
        private static string installRegistryFile => "installedpackages.json";

        public static byte[] DownloadFile(string fileName)
        {
            return File.ReadAllBytes(fileName);
        }

        public static bool UploadFile(string fileName, string fileBody)
        {
            File.WriteAllBytes(fileName, Convert.FromBase64String(fileBody));
            return true;
        }

        public static List<string> GetUiComponents(string folderName)
        {
            string dir = $"{AppEndSettings.ClientObjectsPath}/{folderName}";
            string[] ts = Directory.GetFiles(dir);
			List<string> result = [];
            foreach (string t in ts)
            {
                result.Add(new FileInfo(t).Name);
            }
            return result;
        }

        public static bool CreateEmptyComponent(string componentFullPath)
        {
            File.Copy($"{AppEndSettings.ClientObjectsPath}/a..templates/BaseEmptyComponent.cshtml", componentFullPath);
            return true;
        }

        public static bool RenameItem(string itemPath, string newItemPath)
        {
            itemPath = itemPath.NormalizeAsHostPath();
            newItemPath = newItemPath.NormalizeAsHostPath();

            if (ExtensionsForFileSystem.IsFile(itemPath))
            {
                File.Move(itemPath, newItemPath);
            }
            if (ExtensionsForFileSystem.IsFolder(itemPath))
            {
                Directory.Move(itemPath, newItemPath);
            }

            return true;
        }

        public static bool DuplicateItem(string pathToDuplicate, string pathType)
        {
            string finalPath = pathToDuplicate.NormalizeAsHostPath();

            if (pathType == "file")
            {
                FileInfo fi = new(finalPath);
                File.Copy(finalPath, finalPath.Replace(fi.Extension, StaticMethods.GetRandomName("_Copy") + fi.Extension));
            }
            else
            {
                DirectoryInfo di = new(finalPath);
                DirectoryInfo target = new DirectoryInfo(finalPath + StaticMethods.GetRandomName("_Copy"));
                di.Copy(target);
            }
            return true;
        }

        public static bool DeleteItem(string itemPath, string pathType)
        {
            string finalPath = itemPath.NormalizeAsHostPath();
            if (pathType == "file")
            {
                File.Delete(finalPath);
            }
            else
            {
                Directory.Delete(finalPath, true);
            }
            return true;
        }

        public static bool DeleteFileItem(string filePath)
        {
            File.Delete(filePath);
            return true;
        }

        public static bool FolderHasContent(string pathToCheck)
        {
            string[] directories = Directory.GetDirectories(pathToCheck);
            string[] files = Directory.GetFiles(pathToCheck);
            return directories.Length > 0 || files.Length > 0;
        }

        public static bool CreateNewFolder(string pathToCreate)
        {
            string finalPath = !pathToCreate.StartsWith("/") ? pathToCreate : pathToCreate[1..];
            Directory.CreateDirectory(finalPath);
            return true;
        }

        public static bool CreateNewFile(string pathToCreate)
        {
            string finalPath = !pathToCreate.StartsWith("/") ? pathToCreate : pathToCreate[1..];
            FileInfo fileInfo = new(finalPath);
            FileInfo emptyTemplate = new($"{AppEndSettings.ClientObjectsPath}/a..templates/{fileInfo.Extension}.{fileInfo.Extension}");
            string finalTemplatePath = emptyTemplate.Exists ? emptyTemplate.FullName : $"{AppEndSettings.ClientObjectsPath}/a..templates/BaseEmptyComponent.cshtml";
            File.Copy(finalTemplatePath, finalPath);
            return true;
        }

        public static bool DeleteFolderItem(string pathToDelete, bool recursive)
        {
            Directory.Delete(pathToDelete, recursive);
            return true;
        }

        public static Dictionary<string, List<NameValue>> GetFolderContent(string pathToRead)
        {
            string p = $"{AppEndSettings.ProjectRoot}/{pathToRead}";

            Dictionary<string, List<NameValue>> keyValuePairs = new()
            {
                { "folders", new List<NameValue>() },
                { "files", new List<NameValue>() }
            };
            var directories = Directory.GetDirectories(p);
            foreach (var d in directories)
            {
                DirectoryInfo oInfo = new(d);
                string fullName = oInfo.FullName.NormalizeAsHostPath();
                if (fullName.PathIsManagable()) keyValuePairs["folders"].Add(new() { Name = oInfo.Name.NormalizeAsHostPath(), Value = fullName });
            }
            var files = Directory.GetFiles(p);
            foreach (string s in files)
            {
                FileInfo oInfo = new(s);
                string fullName = oInfo.FullName.NormalizeAsHostPath();
                if (fullName.PathIsManagable()) keyValuePairs["files"].Add(new() { Name = oInfo.Name, Value = fullName });
            }
            return keyValuePairs;
        }

        public static List<NameValue> GetSubApps()
        {
			List<NameValue> apps = [];
            var directories = Directory.GetDirectories(AppEndSettings.ClientObjectsPath);
            foreach (var d in directories)
            {
                DirectoryInfo directoryInfo = new(d);
                if (!AppEndSettings.ReservedFolders.ContainsIgnoreCase(directoryInfo.Name))
                {
                    string appConfigPath = AppEndSettings.ClientObjectsPath + "/" + directoryInfo.Name + "/app.json";
                    if (File.Exists(appConfigPath))
                    {
                        JObject appConf = File.ReadAllText(appConfigPath).ToJObjectByNewtonsoft();
                        appConf["navigation"] = new JObject();
                        appConf["translation"] = new JObject();
                        apps.Add(new() { Name = directoryInfo.Name, Value = appConf.ToJsonStringByNewtonsoft() });
                    }
                }
            }
            return apps;
        }

        public static bool SetThemeProps(string origFolderName, string folderName, string title, string subTitle)
        {
            string appPath = AppEndSettings.ClientObjectsPath + "/" + origFolderName;
            string appConfPath = appPath + "/app.json";
            JObject joAppConf = File.ReadAllText(appConfPath).ToJObjectByNewtonsoft();
            joAppConf["title"] = title;
            joAppConf["sub-title"] = subTitle;
            File.WriteAllText(appConfPath, joAppConf.ToJsonStringByNewtonsoft());

            if (!AppEndSettings.ReservedFolders.ContainsIgnoreCase(folderName) && !origFolderName.EqualsIgnoreCase(folderName))
            {
                Directory.Move(appPath, AppEndSettings.ClientObjectsPath + "/" + folderName);
            }

            return true;
        }

        public static bool DuplicateTheme(string origFolderName, string copyFolderName)
        {
            if (!AppEndSettings.ReservedFolders.ContainsIgnoreCase(copyFolderName) && !origFolderName.EqualsIgnoreCase(copyFolderName))
            {
                DirectoryInfo dOrig = new(AppEndSettings.ClientObjectsPath + "/" + origFolderName);
                DirectoryInfo dCopy = new(AppEndSettings.ClientObjectsPath + "/" + copyFolderName);
                dOrig.CopyFilesRecursively(dCopy);
            }
            return true;
        }

        public static bool RemoveTheme(string folderName)
        {
            if (!AppEndSettings.ReservedFolders.ContainsIgnoreCase(folderName))
            {
                Directory.Delete(AppEndSettings.ClientObjectsPath + "/" + folderName, true);
            }
            return true;
        }
		public static bool SaveFileContent(string pathToWrite, string fileContent)
        {
            File.WriteAllText(pathToWrite, fileContent);
            SV.SharedMemoryCache.TryRemove(new FileInfo(pathToWrite).GetCacheKeyForFiles());
            return true;
        }

        public static string GetFileContent(string pathToRead)
        {
            return File.ReadAllText($"{AppEndSettings.ProjectRoot}/{pathToRead}");
        }

        public static List<string> GetZipFileContent(string pathToRead)
        {
            List<string> files = [];
            using var zipArchive = ZipFile.OpenRead($"{AppEndSettings.ProjectRoot}/{pathToRead}");
            foreach (var e in zipArchive.Entries)
            {
                files.Add("/" + e.FullName);
            }
            return files;
        }

        public static bool PackItemToZipFile(string itemToPack, string zipFile)
        {
            itemToPack = itemToPack.NormalizeAsHostPath();
            zipFile = zipFile.NormalizeAsHostPath();

            string tempFolder = "temp_" + Guid.NewGuid().ToString().Replace("-", "");
            if (Directory.Exists(tempFolder)) { Directory.Delete(tempFolder, true); }
            Directory.CreateDirectory(tempFolder);
            ZipFile.ExtractToDirectory(zipFile, tempFolder);

            if (ExtensionsForFileSystem.IsFile(itemToPack))
            {
                File.Copy(itemToPack, $"{tempFolder}/{itemToPack}");
            }
            if (ExtensionsForFileSystem.IsFolder(itemToPack))
            {
                string destination = $"{tempFolder}/{itemToPack}";
                (new DirectoryInfo(itemToPack)).Copy(new DirectoryInfo(destination));
            }

            File.Delete(zipFile);
            ZipFile.CreateFromDirectory(tempFolder, zipFile, CompressionLevel.Optimal, false);

            Directory.Delete(tempFolder, true);
            return true;
        }

        public static string[] GetStoredApiCalls()
        {
            List<string> res = [];
            string[] files = Directory.GetFiles($"{AppEndSettings.ApiCallsPath}");
            foreach (string f in files)
            {
                res.Add(new FileInfo(f).Name.Replace(".json", ""));
            }
            return [.. res];
        }
        public static string[] GetStoredSqlQueries()
        {
            List<string> res = [];
            string[] files = Directory.GetFiles($"{AppEndSettings.SqlQueriesPath}");
            foreach (string f in files)
            {
                res.Add(new FileInfo(f).Name.Replace(".json", ""));
            }
            return [.. res];
        }

        public static bool ExtractTranslationKeys(string folderName)
        {
            string appConfigAddr = AppEndSettings.ClientObjectsPath + "/" + folderName + "/app.json";
            JObject appConfig = File.ReadAllText(appConfigAddr).ToJObjectByNewtonsoft();

			List<string> Keys = [];
            Keys.AddRange(HostingUtils.GetTranslationKeys(folderName));
            Keys.AddRange(HostingUtils.GetTranslationKeys("a.Components"));
            Keys.AddRange(HostingUtils.GetTranslationKeys("a.SharedComponents"));

            if (appConfig["translation"] == null) appConfig["translation"] = new JObject();

            foreach (string k in Keys)
            {
                if (appConfig["translation"]?[k] == null) appConfig["translation"][k] = k;
            }

            File.WriteAllText(appConfigAddr, appConfig.ToJsonStringByNewtonsoft(true));

            return true;
        }

        public static List<AppEndPackage> ReadPackages()
        {
			List<AppEndPackage> packages = [];
            JArray installedPackages = File.ReadAllText(installRegistryFile).ToJArrayByNewtonsoft();
            string[] files = Directory.GetFiles(AppEndSettings.AppEndPackagesPath);
            foreach (string file in files)
            {
                if (file.EndsWithIgnoreCase(".aepkg"))
                {
                    FileInfo fileInfo = new(file);
                    JObject? installationInfo = (JObject?)installedPackages.FirstOrDefault(i => ((JObject)i)["Name"].ToStringEmpty().EqualsIgnoreCase(fileInfo.Name));
                    using var zipArchive = ZipFile.OpenRead(file);
                    ZipArchiveEntry? zipArchiveEntry = zipArchive.GetEntry("info.json");

                    if (zipArchiveEntry != null)
                    {
                        using var s = zipArchiveEntry.Open();
                        JObject pkgInfo = s.ToText().ToJObjectByNewtonsoft();
                        AppEndPackage pkg = new()
                        {
                            Name = fileInfo.Name,
                            Title = pkgInfo["Title"].ToStringEmpty(),
                            Note = pkgInfo["Note"].ToStringEmpty(),
                            Version = pkgInfo["Version"].ToStringEmpty(),
                            Url = pkgInfo["Url"].ToStringEmpty(),
                            CreatedBy = pkgInfo["CreatedBy"].ToStringEmpty(),
                            CreatedOn = pkgInfo["CreatedOn"].ToDateTimeSafe(DateTime.Now),
                            UpdatedBy = pkgInfo["UpdatedBy"].ToStringEmpty(),
                            UpdatedOn = pkgInfo["UpdatedOn"].ToDateTimeSafe(DateTime.Now),
                            InstallSql = pkgInfo["InstallSql"].ToStringEmpty(),
                            UnInstallSql = pkgInfo["UnInstallSql"].ToStringEmpty(),
                            Installed = false
                        };
                        if (installationInfo != null)
                        {
                            pkg.Installed = true;
                            pkg.InstalledBy = installationInfo["InstalledBy"].ToStringEmpty();
                            pkg.InstalledOn = installationInfo["InstalledOn"].ToDateTimeSafe(null);
                        }
                        packages.Add(pkg);
                    }
                }
            }
            return packages.OrderBy(i => i.Installed).Reverse().ToList();
        }

        public static object? SavePackageInfo(string packageName, string packageNewName, JsonElement packageInfo)
        {
            JObject joPkg = packageInfo.ToJsonStringByBuiltIn().ToJObjectByNewtonsoft();
            joPkg["CreatedOn"] = joPkg["CreatedOn"].ToDateTimeSafe(DateTime.Now);
            joPkg["UpdatedOn"] = joPkg["UpdatedOn"].ToDateTimeSafe(DateTime.Now);
            if (packageName.IsNullOrEmpty()) // it is a new package, must create a new package
            {
                string tempFolderPath = $"{AppEndSettings.AppEndPackagesPath}/{packageNewName.Replace(".aepkg", "")}";
                string packagePath = $"{AppEndSettings.AppEndPackagesPath}/{packageNewName}";
                Directory.CreateDirectory(tempFolderPath);
                Thread.Sleep(100);
                File.WriteAllText($"{tempFolderPath}/info.json", joPkg.ToJsonStringByNewtonsoft());
                Thread.Sleep(100);
                ZipFile.CreateFromDirectory(tempFolderPath, packagePath, CompressionLevel.Optimal, false);
                Thread.Sleep(100);
                Directory.Delete(tempFolderPath, true);
                Thread.Sleep(100);
                return true;
            }
            else
            {
                if (!packageName.EqualsIgnoreCase(packageNewName)) // it is renamming, must rename at first
                {
                    JArray installedPackages = File.ReadAllText(installRegistryFile).ToJArrayByNewtonsoft();
                    File.Move($"{AppEndSettings.AppEndPackagesPath}/{packageName}", $"{AppEndSettings.AppEndPackagesPath}/{packageNewName}");
                    foreach (var pkg in installedPackages)
                    {
                        if (((JObject)pkg)["Name"].ToStringEmpty().EqualsIgnoreCase(packageName))
                        {
                            ((JObject)pkg)["Name"] = packageNewName;
                            File.WriteAllText(installRegistryFile, installedPackages.ToJsonStringByNewtonsoft());
                            Thread.Sleep(100);
                            break;
                        }
                    }
                }

                string? pkgFilePath = !packageName.IsNullOrEmpty() ? Directory.GetFiles(AppEndSettings.AppEndPackagesPath).FirstOrDefault(i => (new FileInfo(i)).Name.EqualsIgnoreCase(packageNewName)) : "";

                if (pkgFilePath != null)
                {
                    string tempFolderPath = $"{AppEndSettings.AppEndPackagesPath}/{(new FileInfo(pkgFilePath)).Name.Replace(".aepkg", "")}";
                    ZipFile.ExtractToDirectory(pkgFilePath, tempFolderPath);
                    Thread.Sleep(100);
                    File.WriteAllText($"{tempFolderPath}/info.json", joPkg.ToJsonStringByNewtonsoft());
                    Thread.Sleep(100);
                    if (File.Exists(pkgFilePath)) File.Delete(pkgFilePath);
                    ZipFile.CreateFromDirectory(tempFolderPath, pkgFilePath, CompressionLevel.Optimal, false);
                    Thread.Sleep(100);
                    Directory.Delete(tempFolderPath, true);
                    Thread.Sleep(100);
                    return true;
                }
                return true;
            }
        }

        public static byte[] DownloadPackage(string packageName)
        {
            return File.ReadAllBytes($"{AppEndSettings.AppEndPackagesPath}/{packageName}");
        }

        public static bool UploadPackage(string packageName, string packageBody)
        {
            File.WriteAllBytes($"{AppEndSettings.AppEndPackagesPath}/{packageName}", Convert.FromBase64String(packageBody));
            return true;
        }

        public static bool RemovePackage(string packageName)
        {
            File.Delete($"{AppEndSettings.AppEndPackagesPath}/{packageName}");
            return true;
        }

        public static object? InstallPackage(AppEndUser? actor, string packageName, string dbConfName)
        {
            string infoFile = "info.json";
            string packageFile = $"{AppEndSettings.AppEndPackagesPath}/{packageName}".NormalizeAsHostPath();
            JArray installedPackages = File.ReadAllText(installRegistryFile).ToJArrayByNewtonsoft();

            // copy files
            ZipFile.ExtractToDirectory(packageFile, AppEndSettings.ProjectRoot.FullName.NormalizeAsHostPath(false), true);

            // run install sql
            if (File.Exists(infoFile))
            {
                JObject joInfo = File.ReadAllText(infoFile).ToJObjectByNewtonsoft();
                string installSql = joInfo["InstallSql"].ToStringEmpty();
                if (!installSql.IsNullOrEmpty())
                {
                    DbIO dbIO = DbIO.Instance(DbConf.FromSettings(dbConfName));
                    dbIO.ToNoneQuery(installSql);
                }
            }

            // register as installed
            JObject? joIns = (JObject?)installedPackages.FirstOrDefault(i => i["Name"].ToStringEmpty() == packageName);

            if (joIns == null)
            {
                joIns = new JObject();
                joIns["Name"] = packageName;
                installedPackages.Add(joIns);
            }

            joIns["InstalledBy"] = actor?.UserName;
            joIns["InstalledOn"] = DateTime.Now;
            joIns["DbConfName"] = dbConfName;

            if (File.Exists(infoFile)) File.Delete(infoFile);

            File.WriteAllText(installRegistryFile, installedPackages.ToJsonStringByNewtonsoft());
            return true;
        }

        public static object? UnInstallPackage(string packageName)
        {
            string packageFullName = (AppEndSettings.AppEndPackagesPath + "/" + packageName).NormalizeAsHostPath();
            JArray installedPackages = File.ReadAllText(installRegistryFile).ToJArrayByNewtonsoft();
            List<string> files = [];
            using (var zipArchive = ZipFile.OpenRead(packageFullName))
            {
                foreach (var e in zipArchive.Entries)
                {
                    if (!e.FullName.EndsWith("/")) files.Add(e.FullName.NormalizeAsHostPath());
                }

                // run UnInstallSql
                ZipArchiveEntry? zipArchiveEntry = zipArchive.GetEntry("info.json");
                if (zipArchiveEntry != null)
                {
                    using var s = zipArchiveEntry.Open();
                    JObject pkgInfo = s.ToText().ToJObjectByNewtonsoft();
                    string unInstallSql = pkgInfo["UnInstallSql"].ToStringEmpty();
                    string? dbConfName = installedPackages.FirstOrDefault(i => i["Name"].ToStringEmpty() == packageName)?["DbConfName"]?.ToStringEmpty();
                    if (!unInstallSql.IsNullOrEmpty() && !dbConfName.IsNullOrEmpty())
                    {
                        DbIO dbIO = DbIO.Instance(DbConf.FromSettings(dbConfName.ToStringEmpty()));
                        dbIO.ToNoneQuery(unInstallSql);
                    }
                }
            }

            // remove files
            foreach (string file in files)
            {
                if (File.Exists(file)) File.Delete(file);
            }

            // unregister the package
            JArray finalItems = [];
            foreach (var pkg in installedPackages) if (pkg["Name"].ToStringEmpty() != packageName) finalItems.Add(pkg);
            File.WriteAllText(installRegistryFile, finalItems.ToJsonStringByNewtonsoft());

            return true;
        }

        public static object? RepackPackage(string packageName)
        {
            string packageFullName = (AppEndSettings.AppEndPackagesPath + "/" + packageName).NormalizeAsHostPath();
            string tempFolder = "temp_" + Guid.NewGuid().ToString().Replace("-", "");
            if (Directory.Exists(tempFolder)) { Directory.Delete(tempFolder, true); }
            Directory.CreateDirectory(tempFolder);
            ZipFile.ExtractToDirectory(packageFullName, tempFolder);

            List<string> packageFiles = ExtensionsForFileSystem.GetFilesRecursive(new DirectoryInfo(tempFolder)).ToList();
            foreach (string file in packageFiles)
            {
                string sourcePath = file.Replace(tempFolder, "").NormalizeAsHostPath();
                string targetPath = file.NormalizeAsHostPath();
                if (File.Exists(sourcePath))
                {
                    File.Copy(sourcePath, targetPath, true);
                }
            }

            File.Delete(packageFullName);
            ZipFile.CreateFromDirectory(tempFolder, packageFullName, CompressionLevel.Optimal, false);
            Directory.Delete(tempFolder, true);
            return true;
        }
    }
}
