using HoPoSim.Framework.Interfaces;
using System;
using System.IO;

namespace HoPoSim.Framework
{
	public static class ApplicationFolders
	{
		public static string CombinePath(IGlobalConfigService config, string dirName, string filename = null)
		{
			try
			{
				var path = GetOrCreateSubFolder(config.DocumentsDirectory, dirName);
				return filename != null ? Path.Combine(path, filename) : path;
			}
			catch
			{
				return null;
			}
		}

		private static string CombinePath(IGlobalConfigService config, int id, string dirname, string filename = null)
		{
			try
			{
				var path = GetOrCreateSubFolder(config.DocumentsDirectory, dirname);
				path = GetOrCreateSubFolder(path, id.ToString());
				return filename != null ? Path.Combine(path, filename) : path;
			}
			catch
			{
				return null;
			}
		}

		public static string GetDefaultDataSourceFolder()
		{
			string folder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
			string applicationName = System.Reflection.Assembly.GetEntryAssembly().GetName().Name;
			string specificApplicationFolder = Path.Combine(folder, applicationName);
			if (!Directory.Exists(specificApplicationFolder))
				Directory.CreateDirectory(specificApplicationFolder);
			return specificApplicationFolder;
		}

		public static string GetOrCreateSubFolder(string path, string subfolder)
		{
			var dir = Path.Combine(path, subfolder);
			if (!Directory.Exists(dir))
				Directory.CreateDirectory(dir);
			return dir;
		}

		public static string GenerateUniqueFileName(string dir, string startFilename, bool appendDirectoryToResult)
		{
			var name = Path.GetFileNameWithoutExtension(startFilename);
			var ext = Path.GetExtension(startFilename);

			string newFile = Path.Combine(dir, $"{name}{ext}");
			if (!File.Exists(newFile))
				return appendDirectoryToResult ? newFile : $"{name}{ext}";

			// use counter
			int i = 0;
			do
			{
				i++;
				newFile = Path.Combine(dir, $"{name}_{i}{ext}");
			} while (File.Exists(newFile));
			return appendDirectoryToResult ? newFile : $"{name}_{i}{ext}";
		}

		public static bool CopyFile(string sourceFile, string targetDir, string targetFile)
		{
			try
			{
				var destFile = Path.Combine(targetDir, targetFile);
				File.Copy(sourceFile, destFile, true);
				return true;
			}
			catch
			{
				return false;
			}
		}

		public static bool CopyTemplate(string templateFileName, string targetFile)
		{
			try
			{
				var sourceFile = Path.Combine(GetExecutingFolder(), templateFileName);
				File.Copy(sourceFile, targetFile, true);
				return true;
			}
			catch (Exception e)
			{
				throw new ArgumentException($"Das Dokument {targetFile} ist bereits geöffnet. Bitte schließen Sie es und führen Sie die Operation erneut aus.\n{e.GetBaseException().Message}");
			}
		}

		public static string GetExecutingFolder()
		{
			return Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
		}

	}
}
