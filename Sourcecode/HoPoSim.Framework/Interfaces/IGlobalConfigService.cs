namespace HoPoSim.Framework.Interfaces
{
	public interface IGlobalConfigService
	{
		string DatabaseDirectory { get; set; }
		string DocumentsDirectory { get; set; }
		string Simulator3dExeFile { get; set; }

		string Export3dDirectoryPath { get; set; }

		string ExportImgDirectoryPath { get; set; }
		int ExportImgWidth { get; set; }
		int ExportImgHeight { get; set; }

		bool ShowBaumartInformationMessages { get; set; }

		object Get(string SettingName);
		void Update(string SettingName, object value);
	}
}
