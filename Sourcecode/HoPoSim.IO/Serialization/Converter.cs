using HoPoSim.Data.Domain;
using System.Data;
using System.Linq;

namespace HoPoSim.IO.Serialization
{
	public static class Converter
	{
		public static DataTable AsDataTable<T,B>(Parameter<T,B> parameter) where T : Range<B>, new()
		{
			var dt = new DataTable();
			dt.Columns.Add(new DataColumn("Klasse", typeof(int)));
			dt.Columns.Add(new DataColumn("von", typeof(B)));
			dt.Columns.Add(new DataColumn("bis", typeof(B)));

			parameter.Values
				.ToList()
				.ForEach(k => dt.Rows.Add(new object[]
				{
					k.RangeId,
					k.MinValue,
					k.MaxValue
				}));
			return dt;
		}

		public static DataTable AsDataTable(Parameter<Durchmesser,int> parameter)
		{
			var dt = new DataTable();
			dt.Columns.Add(new DataColumn("Klasse", typeof(int)));
			dt.Columns.Add(new DataColumn("von (mm)", typeof(int)));
			dt.Columns.Add(new DataColumn("bis (mm)", typeof(int)));
			dt.Columns.Add(new DataColumn("Rindenstärke (mm)", typeof(int)));

			parameter.Values
				.ToList()
				.ForEach(k => dt.Rows.Add(new object[]
				{
					k.RangeId,
					k.MinValue,
					k.MaxValue,
					k.Rindenstärke
				}));
			return dt;
		}

		public static DataTable DurchmesserDistributionsAsDataTable(Distribution distribution)
		{
			var dt = new DataTable("Mittenurchmesser Verteilungen");
			dt.Columns.Add(new DataColumn("Klasse", typeof(int)));
			dt.Columns.Add(new DataColumn("Anteil %", typeof(double)));
			dt.Columns.Add(new DataColumn("Anteil Absolute", typeof(int)));

			distribution
				.Children
				.ToList()
				.ForEach(dm => dt.Rows.Add(new object[]
				{
					dm.RangeId,
					dm.Percent,
					dm.Absolute
				}));
			return dt;
		}

		public static DataTable AbholzigkeitDistributionsAsDataTable(Distribution distribution)
		{
			var dt = new DataTable("Abholzigkeit Verteilungen");
			dt.Columns.Add(new DataColumn("Klasse", typeof(int)));
			dt.Columns.Add(new DataColumn("Mittendurchmesserklasse", typeof(int)));
			dt.Columns.Add(new DataColumn("Anteil %", typeof(double)));
			dt.Columns.Add(new DataColumn("Anteil Absolute", typeof(int)));

			foreach (var dm in distribution.Children)
			{
				foreach (var a in dm.Children)
				{
					dt.Rows.Add(new object[]
					{
						a.RangeId,
						dm.RangeId,
						a.Percent,
						a.Absolute
					});
				}
			}
			return dt;
		}

		public static DataTable KrümmungDistributionsAsDataTable(Distribution distribution)
		{
			var dt = new DataTable("Krümmung Verteilungen");
			dt.Columns.Add(new DataColumn("Klasse", typeof(int)));
			dt.Columns.Add(new DataColumn("Mittendurchmesserklasse", typeof(int)));
			dt.Columns.Add(new DataColumn("Abholzigkeitsklasse", typeof(int)));
			dt.Columns.Add(new DataColumn("Anteil %", typeof(double)));
			dt.Columns.Add(new DataColumn("Anteil Absolute", typeof(int)));

			foreach (var dm in distribution.Children)
			{
				foreach (var a in dm.Children)
				{
					foreach (var k in a.Children)
					{
						dt.Rows.Add(new object[]
						{
							k.RangeId,
							dm.RangeId,
							a.RangeId,
							k.Percent,
							k.Absolute
						});
					}
				}
			}
			return dt;
		}

		public static DataTable OvalitätDistributionsAsDataTable(Distribution distribution)
		{
			var dt = new DataTable("Ovalität Verteilungen");
			dt.Columns.Add(new DataColumn("Klasse", typeof(int)));
			dt.Columns.Add(new DataColumn("Mittendurchmesserklasse", typeof(int)));
			dt.Columns.Add(new DataColumn("Abholzigkeitsklasse", typeof(int)));
			dt.Columns.Add(new DataColumn("Krümmungsklasse", typeof(int)));
			dt.Columns.Add(new DataColumn("Anteil %", typeof(double)));
			dt.Columns.Add(new DataColumn("Anteil Absolute", typeof(int)));

			foreach (var dm in distribution.Children)
			{
				foreach (var a in dm.Children)
				{
					foreach (var k in a.Children)
					{
						foreach (var o in k.Children)
						{
							dt.Rows.Add(new object[]
							{
								o.RangeId,
								dm.RangeId,
								a.RangeId,
								k.RangeId,
								o.Percent,
								o.Absolute
							});
						}
					}
				}
			}
			return dt;
		}

		public static DataTable AsDataTable(SimulationData data)
		{
			var dt = new DataTable();
			//dt.Columns.Add(new DataColumn("Name", typeof(string)));
			dt.Columns.Add(new DataColumn("Polterlänge (m)", typeof(float)));
			dt.Columns.Add(new DataColumn("Poltertiefe (m)", typeof(float)));
			dt.Columns.Add(new DataColumn("Steigungswinkel (°)", typeof(float)));
			dt.Columns.Add(new DataColumn("Seitenspiegelung (%)", typeof(float)));
			dt.Columns.Add(new DataColumn("Zufallsspiegelung", typeof(bool)));
			dt.Columns.Add(new DataColumn("Polterunterlage", typeof(bool)));
			dt.Columns.Add(new DataColumn("Holzdichte (kg/m3)", typeof(float)));
			dt.Columns.Add(new DataColumn("Holzfriktion", typeof(float)));
			dt.Columns.Add(new DataColumn("Rindenbeschädigungen (%)", typeof(float)));
			dt.Columns.Add(new DataColumn("Krümmungsvarianten (%)", typeof(float)));
			dt.Columns.Add(new DataColumn("Baumart", typeof(string)));
			dt.Columns.Add(new DataColumn("Stapelqualitätsstuffe", typeof(int)));
			dt.Columns.Add(new DataColumn("Anteil von Querstämme (%)", typeof(int)));
			dt.Columns.Add(new DataColumn("Querstämme minimaler Winkel (°)", typeof(int)));
			dt.Columns.Add(new DataColumn("Querstämme maximaler Winkel (°)", typeof(int)));

			dt.Rows.Add(new object[]
				{
					//data.Name,
					data.Polterlänge,
					data.Poltertiefe.HasValue? data.Poltertiefe.Value : -1,
					data.Steigungswinkel,
					data.Seitenspiegelung,
					data.Zufallsspiegelung,
					data.Polterunterlage,
					data.WoodDensity,
					data.WoodFriction,
					data.Rindenbeschädigungen,
					data.Krümmungsvarianten,
					data.Baumart?.Name,
					data.Stapelqualität?.Level,
					data.Stapelqualität?.CrossTrunksProportion,
					data.Stapelqualität?.CrossTrunksMinimumAngle,
					data.Stapelqualität?.CrossTrunksMaximumAngle
					//data.Bemerkungen
				});
			return dt;
		}

		public static DataTable AsResultsDataTable(SimulationConfiguration simulation)
		{
			var dt = new DataTable();
			dt.Columns.Add(new DataColumn("Iteration Id", typeof(int)));
			dt.Columns.Add(new DataColumn("Stirnfläche Vorderseite (Qm)", typeof(float)));
			dt.Columns.Add(new DataColumn("Stirnfläche Hinterseite (Qm)", typeof(float)));
			dt.Columns.Add(new DataColumn("Fotooptik Vorderseite (Qm)", typeof(float)));
			dt.Columns.Add(new DataColumn("Fotooptik Hinterseite (Qm)", typeof(float)));
			dt.Columns.Add(new DataColumn("Fotooptik (Rm)", typeof(float)));
			dt.Columns.Add(new DataColumn("Stützpunkte Vorderseite", typeof(int)));
			dt.Columns.Add(new DataColumn("Stützpunkte Hinterseite", typeof(int)));
			dt.Columns.Add(new DataColumn("Polygonzug Vorderseite (Qm)", typeof(float)));
			dt.Columns.Add(new DataColumn("Polygonzug Hinterseite (Qm)", typeof(float)));
			dt.Columns.Add(new DataColumn("Polygonzug (Rm)", typeof(float)));
			dt.Columns.Add(new DataColumn("Sektionsraummaß Vorderseite (Qm)", typeof(float)));
			dt.Columns.Add(new DataColumn("Sektionsraummaß Hinterseite (Qm)", typeof(float)));
			dt.Columns.Add(new DataColumn("Sektionsraummaß (Rm)", typeof(float)));

			dt.Columns.Add(new DataColumn("Poltervolume (FmmR)", typeof(double)));
			dt.Columns.Add(new DataColumn("Poltervolume (FmoR)", typeof(double)));
			dt.Columns.Add(new DataColumn("Polterunterlagevolume (FmmR)", typeof(double)));
			dt.Columns.Add(new DataColumn("Polterunterlagevolume (FmoR)", typeof(double)));
			dt.Columns.Add(new DataColumn("Rindenanteil (%)", typeof(double)));

			dt.Columns.Add(new DataColumn("Umrechnungsfaktor Sektionsraummaß (oR)", typeof(double)));
			dt.Columns.Add(new DataColumn("Umrechnungsfaktor Sektionsraummaß (mR)", typeof(double)));

			dt.Columns.Add(new DataColumn("Umrechnungsfaktor Polygonzug (oR)", typeof(double)));
			dt.Columns.Add(new DataColumn("Umrechnungsfaktor Polygonzug (mR)", typeof(double)));

			dt.Columns.Add(new DataColumn("Umrechnungsfaktor Fotooptik (oR)", typeof(double)));
			dt.Columns.Add(new DataColumn("Umrechnungsfaktor Fotooptik (mR)", typeof(double)));

			dt.Columns.Add(new DataColumn("Höhe", typeof(double)));
			dt.Columns.Add(new DataColumn("Breite", typeof(double)));




			simulation.Results
				.ToList()
				.ForEach(r => dt.Rows.Add(new object[]
				{
					r.IterationId,
					r.StirnflächeV,
					r.StirnflächeH,
					r.FotooptikV,
					r.FotooptikH,
					r.Fotooptik,
					r.FotooptikStützpunkteV,
					r.FotooptikStützpunkteH,
					r.PolygonzugV,
					r.PolygonzugH,
					r.Polygonzug,
					r.SektionV,
					r.SektionH,
					r.Sektion,

					r.PoltervolumeMR,
					r.PoltervolumeOR,
					r.PolterunterlagevolumeMR,
					r.PolterunterlagevolumeOR,
					r.Rindenanteil,

					r.UFSektionOR,
					r.UFSektionMR,

					r.UFPolygonzugOR,
					r.UFPolygonzugMR,

					r.UFFotooptikOR,
					r.UFFotooptikMR,

					r.Höhe,
					r.Breite
				}));
			return dt;
		}
	}
}
