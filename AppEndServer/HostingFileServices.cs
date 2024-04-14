using AppEndCommon;
using Newtonsoft.Json.Linq;

namespace AppEndServer
{
	public static class HostingFileServices
	{
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
			File.Copy($"{AppEndSettings.ClientObjectsPath}/..templates/VueEmptyComponent.vue", componentFullPath);
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
			Dictionary<string, List<NameValue>> keyValuePairs = new()
			{
				{ "folders", new() { } },
				{ "files", new() { } }
			};
			var directories = Directory.GetDirectories(pathToRead);
			foreach (var d in directories)
			{
				keyValuePairs["folders"].Add(new()
				{
					Name = (new DirectoryInfo(d)).Name,
					Value = d.Replace("\\", "/")
				});
			}
			var files = Directory.GetFiles(pathToRead);
			foreach (string s in files)
			{
				keyValuePairs["files"].Add(new()
				{
					Name = (new FileInfo(s)).Name,
					Value = s.Replace("\\", "/")
				});
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
			return File.ReadAllText($"{pathToRead}");
		}

	}
}
