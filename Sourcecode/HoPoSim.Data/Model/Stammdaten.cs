using HoPoSim.Data.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace HoPoSim.Data.Model
{
	public class Stamm
	{
		public Stamm(string id)
		{
			StammId = id;
		}

		public string StammId { get; set; }
		public double Länge { get; set; }
		public int D_Stirn_mR { get; set; }
		public int D_Mitte_mR { get; set; }
		public int D_Zopf_mR { get; set; }
		public int D_Stirn_oR { get; set; }
		public int D_Mitte_oR { get; set; }
		public int D_Zopf_oR { get; set; }
		public int Abholzigkeit { get; set; }
		public int Krümmung { get; set; }
		public int Rindenstärke { get; set; }
		public double Ovalität { get; set; }
		public int Stammfußhöhe { get; set; }
	}

	public class StammIdComparer : IEqualityComparer<Stamm>
	{
		public bool Equals(Stamm x, Stamm y)
		{
			return x.StammId == y.StammId;
		}

		public int GetHashCode(Stamm obj)
		{
			int hash = 23;
			foreach (char c in obj.StammId)
			{
				hash = hash * 31 + c;
			}
			return hash;
		}
	}

	public class Stammdaten
	{
		public const string STAMM_ID = "STAMM_ID";
		public const string LÄNGE = "LÄNGE";
		public const string D_STIRN_mR = "D_STIRN_mR";
		public const string D_MITTE_mR = "D_MITTE_mR";
		public const string D_ZOPF_mR = "D_ZOPF_mR";
		public const string D_STIRN_oR = "D_STIRN_oR";
		public const string D_MITTE_oR = "D_MITTE_oR";
		public const string D_ZOPF_oR = "D_ZOPF_oR";
		public const string ABHOLZIGKEIT = "ABHOLZIGKEIT";
		public const string KRÜMMUNG = "KRÜMMUNG";
		public const string RINDENSTÄRKE = "RINDENSTÄRKE";
		public const string OVALITÄT = "OVALITÄT";
		public const string STAMMFUßHÖHE = "STAMMFUßHÖHE";


		public Stammdaten()
		{
			DataTable = CreateDataTable();
		}

		public Stammdaten(DataTable dt)
		{
			DataTable = dt;
		}

		public Stammdaten(string value)
		{
			DataTable = Extensions.DataTableExtensions.LoadFromString(value);
		}

		public void Add(Stamm s)
		{
			var values = new object[]
			{
				s.StammId,
				s.Länge,
				s.D_Stirn_mR,
				s.D_Mitte_mR,
				s.D_Zopf_mR,
				s.D_Stirn_oR,
				s.D_Mitte_oR,
				s.D_Zopf_oR,
				s.Abholzigkeit,
				s.Krümmung,
				s.Ovalität,
				s.Rindenstärke,
				s.Stammfußhöhe
			};
			DataTable.Rows.Add(values);
		}

		public void Clear()
		{
			DataTable.Clear();
		}

		public IEnumerable<Stamm> AsEnumerable()
		{
			var hasStammfußhöheColumn = HasColumn(STAMMFUßHÖHE);

			return DataTable.AsEnumerable()
				.Select(r => new Stamm(Convert.ToString(r[STAMM_ID]))
				{
					Länge = ConvertToDouble(r[LÄNGE]),
					D_Stirn_mR = ConvertToInt(r[D_STIRN_mR]),
					D_Mitte_mR = ConvertToInt(r[D_MITTE_mR]),
					D_Zopf_mR = ConvertToInt(r[D_ZOPF_mR]),
					D_Stirn_oR = ConvertToInt(r[D_STIRN_oR]),
					D_Mitte_oR = ConvertToInt(r[D_MITTE_oR]),
					D_Zopf_oR = ConvertToInt(r[D_ZOPF_oR]),
					Abholzigkeit = ConvertToInt(r[ABHOLZIGKEIT]),
					Krümmung = ConvertToInt(r[KRÜMMUNG]),
					Ovalität = ConvertToDouble(r[OVALITÄT]),
					Rindenstärke = ConvertToInt(r[RINDENSTÄRKE]),
					Stammfußhöhe = hasStammfußhöheColumn? ConvertToInt(r[STAMMFUßHÖHE]) : 0
				});
		}

		private int ConvertToInt(object value)
		{
			return Convert.IsDBNull(value) ? 0 : Convert.ToInt32(value);
		}

		private double ConvertToDouble(object value)
		{
			return Convert.IsDBNull(value) ? 0.0 : Convert.ToDouble(value);
		}

		public bool HasColumn(string name)
		{
			return DataTable.Columns.Contains(name);
		}

		public string AsString()
		{
			return DataTable.AsString();
		}

		public int Stammanzahl
		{
			get { return DataTable != null ? DataTable.GetRowCount() : 0; }
		}


		public double Länge
		{
			get { return DataTable != null ? DataTable.GetColumnAverage(LÄNGE) : 0; }
		}

		private DataTable CreateDataTable()
		{
			var dt = new DataTable("Stammdaten");
			dt.Columns.Add("STAMM_ID", typeof(string));
			dt.Columns.Add("LÄNGE", typeof(double));
			dt.Columns.Add("D_STIRN_mR", typeof(int));
			dt.Columns.Add("D_MITTE_mR", typeof(int));
			dt.Columns.Add("D_ZOPF_mR", typeof(int));
			dt.Columns.Add("D_STIRN_oR", typeof(int));
			dt.Columns.Add("D_MITTE_oR", typeof(int));
			dt.Columns.Add("D_ZOPF_oR", typeof(int));
			dt.Columns.Add("ABHOLZIGKEIT", typeof(int));
			dt.Columns.Add("KRÜMMUNG", typeof(int));
			dt.Columns.Add("OVALITÄT", typeof(double));
			dt.Columns.Add("RINDENSTÄRKE", typeof(int));
			dt.Columns.Add("STAMMFUßHÖHE", typeof(int));
			return dt;
		}

		public DataTable DataTable { get; }
	}
}
