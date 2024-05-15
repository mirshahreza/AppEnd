using AppEndCommon;
using Newtonsoft.Json.Linq;
using System.IO;
using System.IO.Compression;
using System.Text.Json;
using System.Text.Json.Nodes;


namespace AppEndServer
{
	public static class FileServices
	{
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
			File.Copy($"{AppEndSettings.ClientObjectsPath}/a..templates/VueEmptyComponent.vue", componentFullPath);
			return true;
		}

		public static bool RenameFileItem(string filePath, string newFilePath)
		{
			File.Move($"{filePath}", $"{newFilePath}");
			return true;
		}
		public static bool DuplicateFileItem(string filePath)
		{
			FileInfo fi = new(filePath);
			File.Copy(filePath, filePath.Replace(fi.Extension, StaticMethods.GetRandomName("_Copy") + fi.Extension));
			return true;
		}
		public static bool DeleteFileItem(string filePath)
		{
			File.Delete($"{filePath}");
			return true;
		}

		public static bool FolderHasContent(string pathToCheck)
		{
			string[] directories = Directory.GetDirectories($"{pathToCheck}");
			string[] files = Directory.GetFiles($"{pathToCheck}");
			if (directories.Length > 0 || files.Length > 0) return true;
			return false;
		}
		public static bool CreateNewFolder(string pathToCreate, string newFolderName)
		{
			string p = $"{pathToCreate}/{newFolderName}";
			if (Directory.Exists(p)) throw new AppEndException("NameInUsed")
					.AddParam("PathToCreate", pathToCreate)
					.AddParam("NewFolderName", newFolderName)
					.AddParam("Site", $"{System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType?.Name}, {System.Reflection.MethodBase.GetCurrentMethod()?.Name}")
					;
			Directory.CreateDirectory(p);
			return true;
		}
		public static bool DeleteFolderItem(string pathToDelete, bool recursive)
		{
			Directory.Delete(pathToDelete, recursive);
			return true;
		}
		public static Dictionary<string, List<NameValue>> GetFolderContent(string pathToRead)
		{
			string p = $"{AppEndSettings.RootDeep}/{pathToRead}";

			Dictionary<string, List<NameValue>> keyValuePairs = new()
			{
				{ "folders", new() { } },
				{ "files", new() { } }
			};
			var directories = Directory.GetDirectories(p);
			foreach (var d in directories)
			{
				DirectoryInfo oInfo = new(d);
				string fullName = oInfo.FullName.NormalizePath();
                if (fullName.PathIsManagable()) keyValuePairs["folders"].Add(new() { Name = oInfo.Name.NormalizePath(), Value = fullName });
            }
            var files = Directory.GetFiles(p);
			foreach (string s in files)
			{
                FileInfo oInfo = new(s);
                string fullName = oInfo.FullName.NormalizePath();
                if (fullName.PathIsManagable()) keyValuePairs["files"].Add(new() { Name = oInfo.Name, Value = fullName });
            }
            return keyValuePairs;
		}

		

		public static List<NameValue> GetThemes()
		{
			List<NameValue> apps = [];
			var directories = Directory.GetDirectories(AppEndSettings.ClientObjectsPath);
			foreach (var d in directories)
			{
				DirectoryInfo directoryInfo = new(d);
				if (!AppEndSettings.ReservedFolders.ContainsIgnoreCase(directoryInfo.Name))
				{
					JObject appConf = File.ReadAllText(AppEndSettings.ClientObjectsPath + "/" + directoryInfo.Name + "/app.json").ToJObjectByNewtonsoft();
					appConf["navigation"] = new JObject();
					appConf["translation"] = new JObject();
					apps.Add(new() { Name = directoryInfo.Name, Value = appConf.ToJsonStringByNewtonsoft() });
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
            return File.ReadAllText($"{AppEndSettings.RootDeep}/{pathToRead}");
        }
        public static List<string> GetZipFileContent(string pathToRead)
        {
			List<string> files = [];	
            ZipArchive zipArchive = ZipFile.OpenRead($"{AppEndSettings.RootDeep}/{pathToRead}");
            zipArchive.Entries.ToList().ForEach(e => {
				files.Add("/" + e.FullName);
			});
			zipArchive.Dispose();
			return files;
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
		public static bool ExtractTranslationKeys(string folderName)
		{
			string appConfigAddr = AppEndSettings.ClientObjectsPath + "/" + folderName + "/app.json";
			JObject appConfig = File.ReadAllText(appConfigAddr).ToJObjectByNewtonsoft();

			List<string> Keys = [];
			Keys.AddRange(HostingUtils.GetTranslationKeys(folderName));
			Keys.AddRange(HostingUtils.GetTranslationKeys("a.DbComponents"));
			Keys.AddRange(HostingUtils.GetTranslationKeys("a.PublicComponents"));

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
			JArray installedPackages = File.ReadAllText("installedpackages.json").ToJArrayByNewtonsoft();
			string[] files = Directory.GetFiles(AppEndSettings.AppEndPackagesPath);
			foreach(string file in files)
			{
				if (file.EndsWithIgnoreCase(".aepkg"))
				{
					FileInfo fileInfo = new(file);
					JObject? installationInfo = (JObject?)installedPackages.FirstOrDefault(i => ((JObject)i)["Name"].ToStringEmpty().EqualsIgnoreCase(fileInfo.Name));
					ZipArchive zipArchive = ZipFile.OpenRead(file);
					ZipArchiveEntry? zipArchiveEntry = zipArchive.GetEntry("info.json");

                    if (zipArchiveEntry != null)
					{
						JObject pkgInfo = zipArchiveEntry.Open().ToText().ToJObjectByNewtonsoft();
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
                        zipArchive.Dispose();
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
                    JArray installedPackages = File.ReadAllText("installedpackages.json").ToJArrayByNewtonsoft();
					File.Move($"{AppEndSettings.AppEndPackagesPath}/{packageName}", $"{AppEndSettings.AppEndPackagesPath}/{packageNewName}");
					foreach (var pkg in installedPackages)
					{
						if (((JObject)pkg)["Name"].ToStringEmpty().EqualsIgnoreCase(packageName))
						{
							((JObject)pkg)["Name"] = packageNewName;
							File.WriteAllText("installedpackages.json", installedPackages.ToJsonStringByNewtonsoft());
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
					if(File.Exists(pkgFilePath)) File.Delete(pkgFilePath);
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

        public static object? InstallPackage(string packageName)
        {
			string packageFile = $"{AppEndSettings.AppEndPackagesPath}/{packageName}";
            ZipArchive zipArchive = ZipFile.OpenRead(packageFile);
			zipArchive.ExtractToDirectory(AppEndSettings.RootDeep);
            zipArchive.Dispose();
            return true;
        }

        public static object? UnInstallPackage(string packageName)
        {
            return true;
        }

    }
}
