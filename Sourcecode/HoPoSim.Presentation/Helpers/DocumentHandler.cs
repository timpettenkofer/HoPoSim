using HoPoSim.Framework;
using HoPoSim.Framework.Interfaces;
using Microsoft.Win32;
using Prism.Commands;
using System;
using System.IO;

namespace HoPoSim.Presentation.Helpers
{
    public class DocumentHandler
    {
        private DocumentHandler(IGlobalConfigService config, IInteractionService interaction, Func<string> getPath, Func<string> getFilename, Action<string> setFilename)
        {
            _config = config;
            _interaction = interaction;

            _getPath = getPath;
            SetDocumentCommand = new DelegateCommand(this.SetDocument);
            OpenDocumentCommand = new DelegateCommand(this.OpenDocument);
            AddDocumentCommand = new DelegateCommand(this.AddDocument);
            OpenDirectoryCommand = new DelegateCommand(this.OpenDirectory);

            _getFilename = getFilename;
            _setFilename = setFilename;
        }

        public DocumentHandler(Func<string> getPath, IGlobalConfigService config, IInteractionService interaction): 
            this(config, interaction, getPath, () => null, (s) => { })
        {
        }

        public DocumentHandler(Func<string> getPath, IGlobalConfigService config, IInteractionService interaction, Func<string> getFilename, Action<string> setFilename):
            this(config, interaction, getPath, getFilename, setFilename)
        {
        }


        private Func<string> _getPath;
        private Func<string> _getFilename;
        private Action<string> _setFilename;

        private IGlobalConfigService _config;
        private IInteractionService _interaction;

        public DelegateCommand SetDocumentCommand { get; protected set; }
        public DelegateCommand OpenDocumentCommand { get; protected set; }
        public DelegateCommand AddDocumentCommand { get; protected set; }
        public DelegateCommand OpenDirectoryCommand { get; protected set; }

        public void SetDocument()
        {
            var file = GetFileToBeAddedFromUserSelection();
            if (string.IsNullOrEmpty(file)) return;
            SetDocumentInternal(file);
        }

        public void AddDocument()
        {
            var file = GetFileToBeAddedFromUserSelection();
            if (string.IsNullOrEmpty(file)) return;
            CopyToDirectory(file);
        }

        public void OpenDocument()
        {
            OpenDocumentInternal();
        }

		public static string GetTargetFolderFromUserSelection(string startDir)
		{
			var folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog() { SelectedPath = startDir };
			var result = folderBrowserDialog.ShowDialog();
			return result == System.Windows.Forms.DialogResult.OK?
				folderBrowserDialog.SelectedPath:
				null;
		}

		public static string GetSavedAsFileFromUserSelection(string filter, string initialFilename)
		{
			SaveFileDialog saveFileDialog = new SaveFileDialog() { FileName = initialFilename };
			saveFileDialog.Filter = filter;
			if (saveFileDialog.ShowDialog() == true)
			{
				return saveFileDialog.FileName;
			}
			return null;
		}

		public static string GetFileToBeAddedFromUserSelection(string filter = null)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = filter;
            if (openFileDialog.ShowDialog() == true)
            {
                return openFileDialog.FileName;
            }
            return null;
        }

        private void SetDocumentInternal(string filename)
        {
            var dir = _getPath(); // ApplicationFolders.CombinePath(_config, _getPath());
            var file = ApplicationFolders.GenerateUniqueFileName(dir, filename, false);
            if (ApplicationFolders.CopyFile(filename, dir, file))
                _setFilename(file);
            else
            {
                _interaction.RaiseNotificationAsync(string.Format(Properties.Resources.Notification_DocumentCannotBeCopied, filename));
            }
        }

        private void OpenDocumentInternal()
        {
            try
            {
                var file = Path.Combine(_getPath(), _getFilename()); //  ApplicationFolders.CombinePath(_config, _getPath(), _getFilename());
                if (File.Exists(file))
                    System.Diagnostics.Process.Start(file);
                else
                    _interaction.RaiseNotificationAsync(string.Format(Properties.Resources.Notification_DocumentCannotBeFound, file));
            }
            catch
            { }
        }

        public string CopyToDirectory(string path)
        {
            var dir = _getPath(); //ApplicationFolders.CombinePath(_config, _getPath());
            var file = ApplicationFolders.GenerateUniqueFileName(dir, path, false);
            if (!ApplicationFolders.CopyFile(path, dir, file))
            {
                _interaction.RaiseNotificationAsync(string.Format(Properties.Resources.Notification_DocumentCannotBeCopied, path));
				return null;
            }
			return Path.Combine(dir, file);
        }

        public void OpenDirectory()
        {
            try
            {
                var dir = _getPath();  //ApplicationFolders.CombinePath(_config, _getPath());
                System.Diagnostics.Process.Start("explorer.exe", dir);
            }
            catch
            { }
        }
    }
}
