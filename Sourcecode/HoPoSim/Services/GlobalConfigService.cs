using HoPoSim.Framework;
using HoPoSim.Framework.Interfaces;
using System;
using System.ComponentModel.Composition;
using System.IO;

namespace HoPoSim.Services
{
    [Export(typeof(IGlobalConfigService))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class GlobalConfigService : IGlobalConfigService
    {
        protected ISettings _Settings;

        [ImportingConstructor]
        public GlobalConfigService(ISettings Settings)
        {
            _Settings = Settings;
        }

        public string DatabaseDirectory
        {
            get
            {
                var userDb = (string)Get(ApplicationSettingNames.DatabaseDirectory);
                if (string.IsNullOrEmpty(userDb))
                    DatabaseDirectory = userDb = GetDefaultDataSourceFolder();
                return userDb;
            }
            set
            {
                Update(ApplicationSettingNames.DatabaseDirectory, value);
            }
        }

		public string DocumentsDirectory
		{
			get
			{
				var userDocuments = (string)Get(ApplicationSettingNames.DocumentsDirectory);
				if (string.IsNullOrEmpty(userDocuments))
				{
					var applicationDataFolder = ApplicationFolders.GetDefaultDataSourceFolder();
					DocumentsDirectory = userDocuments = ApplicationFolders.GetOrCreateSubFolder(applicationDataFolder, "Dokumente");
				}
				return userDocuments;
			}
			set
			{
				Update(ApplicationSettingNames.DocumentsDirectory, value);
			}
		}

		public string Simulator3dExeFile
		{
			get
			{
				var file = (string)Get(ApplicationSettingNames.Simulator3dExeFile);
				return file;
			}
			set
			{
				Update(ApplicationSettingNames.Simulator3dExeFile, value);
			}
		}

		public string Export3dDirectoryPath
		{
			get
			{
				var dir = (string)Get(ApplicationSettingNames.Export3dDirectoryPath);
				if (string.IsNullOrEmpty(dir))
					Export3dDirectoryPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
				return dir;
			}
			set
			{
				Update(ApplicationSettingNames.Export3dDirectoryPath, value);
			}
		}

		public int ExportImgWidth
		{
			get
			{
				return (int)Get(ApplicationSettingNames.ExportImgWidth);
			}
			set
			{
				Update(ApplicationSettingNames.ExportImgWidth, value);
			}
		}

		public int ExportImgHeight
		{
			get
			{
				return (int)Get(ApplicationSettingNames.ExportImgHeight);
			}
			set
			{
				Update(ApplicationSettingNames.ExportImgHeight, value);
			}
		}

		public string ExportImgDirectoryPath
		{
			get
			{
				var dir = (string)Get(ApplicationSettingNames.ExportImgDirectoryPath);
				if (string.IsNullOrEmpty(dir))
					ExportImgDirectoryPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
				return dir;
			}
			set
			{
				Update(ApplicationSettingNames.ExportImgDirectoryPath, value);
			}
		}

		public bool ShowBaumartInformationMessages
		{
			get
			{
				return (bool)Get(ApplicationSettingNames.ShowBaumartInformationMessages);
			}
			set
			{
				Update(ApplicationSettingNames.ShowBaumartInformationMessages, value);
			}
		}

		private string GetDefaultDataSourceFolder()
        {
            string folder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string applicationName = System.Reflection.Assembly.GetEntryAssembly().GetName().Name;
            string specificApplicationFolder = Path.Combine(folder, applicationName);
            if (!Directory.Exists(specificApplicationFolder))
                Directory.CreateDirectory(specificApplicationFolder);
            return specificApplicationFolder;
        }


        public void Update(string SettingName, object value)
        {
            if (String.IsNullOrEmpty(SettingName))
                throw new ArgumentNullException("Setting name must be provided");

            var Setting = _Settings[SettingName];

            if (Setting == null)
            {
                throw new ArgumentException("Setting " + SettingName + " not found.");
            }
            else if (Setting.GetType() != value.GetType())
            {
                throw new InvalidCastException("Unable to cast value to " + Setting.GetType());
            }
            else
            {
                _Settings[SettingName] = value;
                _Settings.Save();
            }

        }

        public object Get(string SettingName)
        {
            if (String.IsNullOrEmpty(SettingName))
                throw new ArgumentNullException("Setting name must be provided");

            return _Settings[SettingName];
        }
    }
}
