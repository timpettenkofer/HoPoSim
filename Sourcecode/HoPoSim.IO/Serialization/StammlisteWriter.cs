using HoPoSim.Data.Domain;
using HoPoSim.Data.Model;
using HoPoSim.IO.Interfaces;
using System.ComponentModel.Composition;

namespace HoPoSim.IO.Serialization
{
	[Export(typeof(IStammlisteWriter))]
	[PartCreationPolicy(CreationPolicy.Shared)]
	public class StammlisteWriter : IStammlisteWriter
	{
		[ImportingConstructor]
		public StammlisteWriter(IStammlisteReader reader, IExportService exporter)
		{
			Reader = reader;
			Exporter = exporter;
		}
		IStammlisteReader Reader { get; }
		IExportService Exporter { get; }


		public void WriteStammdaten(string filepath, GeneratorData input, Stammdaten data)
		{
			var exports = new[]
			{
				new ExportTarget
				{
					DataTable = data.DataTable,
					SheetName = ApplicationTemplates.Stammdaten.StammdatenSheet,
					RegionName = ApplicationTemplates.Stammdaten.StammdatenRegion
				},
				new ExportTarget
				{
					DataTable = Converter.AsDataTable(input.Durchmesser),
					SheetName = ApplicationTemplates.Stammdaten.EingabeparameterSheet,
					RegionName = ApplicationTemplates.Stammdaten.DurchmesserRegion,
					ExportHeaders = true,
					ShowSheet = true
				},
				new ExportTarget
				{
					DataTable = Converter.AsDataTable(input.Abholzigkeit),
					SheetName = ApplicationTemplates.Stammdaten.EingabeparameterSheet,
					RegionName = ApplicationTemplates.Stammdaten.AbholzigkeitRegion,
					ExportHeaders = true,
					ShowSheet = true
				},
				new ExportTarget
				{
					DataTable = Converter.AsDataTable(input.Krümmung),
					SheetName = ApplicationTemplates.Stammdaten.EingabeparameterSheet,
					RegionName = ApplicationTemplates.Stammdaten.KrümmungRegion,
					ExportHeaders = true,
					ShowSheet = true
				},
				new ExportTarget
				{
					DataTable = Converter.AsDataTable(input.Ovalität),
					SheetName = ApplicationTemplates.Stammdaten.EingabeparameterSheet,
					RegionName = ApplicationTemplates.Stammdaten.OvalitätRegion,
					ExportHeaders = true,
					ShowSheet = true
				},
				new ExportTarget
				{
					DataTable = Converter.DurchmesserDistributionsAsDataTable(input.Distribution),
					SheetName = ApplicationTemplates.Stammdaten.EingabeparameterSheet,
					RegionName = ApplicationTemplates.Stammdaten.DurchmesserverteilungenRegion,
					ExportHeaders = true,
					ShowSheet = true
				},
				new ExportTarget
				{
					DataTable = Converter.AbholzigkeitDistributionsAsDataTable(input.Distribution),
					SheetName = ApplicationTemplates.Stammdaten.EingabeparameterSheet,
					RegionName = ApplicationTemplates.Stammdaten.AbholzigkeitsverteilungenRegion,
					ExportHeaders = true,
					ShowSheet = true
				},
				new ExportTarget
				{
					DataTable = Converter.KrümmungDistributionsAsDataTable(input.Distribution),
					SheetName = ApplicationTemplates.Stammdaten.EingabeparameterSheet,
					RegionName = ApplicationTemplates.Stammdaten.KrümmungsverteilungenRegion,
					ExportHeaders = true,
					ShowSheet = true
				},
				new ExportTarget
				{
					DataTable = Converter.OvalitätDistributionsAsDataTable(input.Distribution),
					SheetName = ApplicationTemplates.Stammdaten.EingabeparameterSheet,
					RegionName = ApplicationTemplates.Stammdaten.OvalitätsverteilungenRegion,
					ExportHeaders = true,
					ShowSheet = true
				}

			};
			Exporter.ExportExcel(filepath, exports);
		}
	}
}
