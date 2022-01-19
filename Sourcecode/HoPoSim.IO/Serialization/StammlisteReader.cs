using HoPoSim.Data.Model;
using HoPoSim.IO.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Data;
using System.Linq;
using static HoPoSim.Data.Model.Stammdaten;

namespace HoPoSim.IO.Serialization
{
	[Export(typeof(IStammlisteReader))]
	[PartCreationPolicy(CreationPolicy.Shared)]
	public class StammlisteReader : IStammlisteReader
	{
		[ImportingConstructor]
		public StammlisteReader(IImportService importService)
		{
			ImportService = importService;
		}
		private IImportService ImportService { get; }

		public Stammdaten ReadStammdaten(string filepath)
		{
			var dt = ImportService.ImportExcel(filepath, ApplicationTemplates.Stammdaten.StammdatenSheet, ApplicationTemplates.Stammdaten.StammdatenRegion);
			return ReadStammdaten(dt);
		}

		public Stammdaten ReadStammdaten(DataTable dt)
		{
			CheckColumnNames(dt);
			RenameColumns(dt);
			return new Stammdaten(dt);
		}

		public static string FileExtensionFilter => "Excel Files|*.xlsx;";

		private static Dictionary<string, string> Parameters = new Dictionary<string, string>
		{
			{ STAMM_ID, "Einzelstamm ID" },
			{ LÄNGE, "Länge (m)" },
			{ D_STIRN_mR, "D Stirn mit Rinde (mm)" },
			{ D_MITTE_mR, "D Mitte mit Rinde (mm)" },
			{ D_ZOPF_mR, "D Zopf mit Rinde (mm)" },
			{ D_STIRN_oR, "D Stirn ohne Rinde (mm)" },
			{ D_MITTE_oR, "D Mitte ohne Rinde (mm)" },
			{ D_ZOPF_oR, "D Zopf ohne Rinde (mm)" },
			{ ABHOLZIGKEIT, "Abholzigkeit (mm/lfm)" },
			{ KRÜMMUNG, "Krümmung (mm/lfm)" },
			{ OVALITÄT, "Ovalität (]0,1])" },
			{ RINDENSTÄRKE, "Rindenstärke (mm)" },
			{ STAMMFUßHÖHE, "Stammfußhöhe (cm)" }
		};

		private void CheckColumnNames(DataTable dt)
		{
			foreach(var name in ReverseParameters.Keys)
			{
				if (!dt.Columns.Contains(name))
					throw new ArgumentException($"Input Table does not have any '{name}' column!");
			}
		}

		private void RenameColumns(DataTable dt)
		{
			if (dt == null)
				return;

			foreach (DataColumn column in dt.Columns)
			{
				if (ReverseParameters.TryGetValue(column.ColumnName, out string value))
					column.ColumnName = value.ToString();
			}

		}
		private Dictionary<string, string> ReverseParameters
		{
			get
			{
				if (_reverseParameters == null)
					_reverseParameters = Parameters.ToDictionary(e => e.Value, e => e.Key);
				return _reverseParameters;
			}
		}
		Dictionary<string, string> _reverseParameters;

	}
}
