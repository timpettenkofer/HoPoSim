using NUnit.Framework;
using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace HoPoSim.IPC.Tests
{
	public class TestBase
	{
		protected static string GetTestDataFile(string filename)
		{
			string startupPath = System.AppDomain.CurrentDomain.BaseDirectory;
			var pathItems = startupPath.Split(Path.DirectorySeparatorChar);
			string projectPath = String.Join(Path.DirectorySeparatorChar.ToString(), pathItems.Take(pathItems.Length - 3));
			return Path.Combine(projectPath, "Data", filename);
		}

		protected static string DumpDataTableToString(DataTable dt)
		{
			using (var writer = new StringWriter())
			{
				dt.WriteXml(writer);
				return writer.ToString();
			}
		}

		protected static DataTable LoadFromFile(string path)
		{
			var fullpath = GetTestDataFile(path);
			var value = File.ReadAllText(fullpath);
			return LoadFromString(value);
		}

		protected static DataTable LoadFromString(string value)
		{
			var dt = new DataTable();

			using (var reader = XmlReader.Create(new StringReader(value)))
			{
				dt.ReadXml(reader);
				return dt;
			}
		}

		protected static void AssertEqualReferenceFile(DataTable dt, string refFile)
		{
			var fullpath = GetTestDataFile(refFile);
			if (!File.Exists(fullpath))
			{
				File.WriteAllText(fullpath, DumpDataTableToString(dt));
				return;
			}
			string refValue = File.ReadAllText(fullpath);
			Assert.AreEqual(refValue, DumpDataTableToString(dt), "Computed value and reference value differ");
		}

		protected static string GetReferenceDataFileAsText(string refFile)
		{
			var fullpath = GetTestDataFile(refFile);
			return File.ReadAllText(fullpath, Encoding.GetEncoding("iso-8859-1"));
		}
	}
}
