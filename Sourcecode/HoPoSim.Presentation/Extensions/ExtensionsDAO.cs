using HoPoSim.IPC.DAO;
using System;
using System.Linq;

namespace HoPoSim.Presentation.Extensions
{
	public static class ExtensionsDAO
    {
		public static SimulationData ToDAO(this Data.Domain.SimulationData value)
		{
			var poltermaße = new PolterConfiguration()
			{
				MinimumPolterlänge = value.Polterlänge,
				Polterbreite = Convert.ToSingle(value.HasStammData() ? value.Stammliste.Länge : 0),
				Poltertiefe = value.Poltertiefe,
				Steigungswinkel = value.Steigungswinkel,
				Seitenspiegelung = value.Seitenspiegelung,
				Zufallsspiegelung = value.Zufallsspiegelung,
				Polterunterlage = value.Polterunterlage,
				CrossTrunksProportion = value.Stapelqualität.CrossTrunksProportion,
				CrossTrunksMinimumAngle = value.Stapelqualität.CrossTrunksMinimumAngle,
				CrossTrunksMaximumAngle = value.Stapelqualität.CrossTrunksMaximumAngle,
			};

			var holz = new HolzConfiguration()
			{
				Rindenbeschädigungen = value.Rindenbeschädigungen,
				Krümmungsvarianten = value.Krümmungsvarianten

			};

			var baumart = new BaumartParametrization()
			{
				Name = value.Baumart.Name,
				IncludeRoots = value.Baumart.IncludeRoots,
				MinNoiseSize = value.Baumart.MinNoiseSize,
				MaxNoiseSize = value.Baumart.MaxNoiseSize,
				MinNoiseStrength = value.Baumart.MinNoiseStrength,
				MaxNoiseStrength = value.Baumart.MaxNoiseStrength,
				MinRootRadiusMultiplier = value.Baumart.MinRootRadiusMultiplier,
				MaxRootRadiusMultiplier = value.Baumart.MaxRootRadiusMultiplier,
				MinRootFlareNumber = value.Baumart.MinRootFlareNumber,
				MaxRootFlareNumber = value.Baumart.MaxRootFlareNumber,
				IncludeBranches = value.Baumart.IncludeBranches,
				BranchStubTrunkProportion = value.Baumart.BranchStubTrunkProportion,
				BranchStubMinLength = value.Baumart.BranchStubMinLength,
				BranchStubMaxLength = value.Baumart.BranchStubMaxLength,
				BranchStubMinHeight = value.Baumart.BranchStubMinHeight,
				BranchStubMaxHeight = value.Baumart.BranchStubMaxHeight,
				BranchStubAverageAngle = value.Baumart.BranchStubAverageAngle,
				BranchStubNumberPerMeter = value.Baumart.BranchStubNumberPerMeter,
				BranchStubRadiusMultiplier = value.Baumart.BranchStubRadiusMultiplier
			};

			var data = new SimulationData()
			{
				Id = value.Id,
				Name = value.Name,
				Poltermaße = poltermaße,
				Holz = holz,

				WoodDensity = value.WoodDensity,
				WoodFriction = value.WoodFriction,
				
				Stämme = value.Stammliste.AsEnumerable().Select(s => ToDAO(s)).ToList(),
				Baumart = baumart
			};
			return data;
		}

		public static Stamm ToDAO(this Data.Model.Stamm value)
		{
			var stamm = new Stamm()
			{
				StammId = value.StammId,
				Länge = Convert.ToSingle(value.Länge),
				D_Stirn_mR = value.D_Stirn_mR,
				D_Mitte_mR = value.D_Mitte_mR,
				D_Zopf_mR = value.D_Zopf_mR,
				D_Stirn_oR = value.D_Stirn_oR,
				D_Mitte_oR = value.D_Mitte_oR,
				D_Zopf_oR = value.D_Zopf_oR,
				Abholzigkeit = value.Abholzigkeit,
				Krümmung = value.Krümmung,
				Ovalität = Convert.ToSingle(value.Ovalität),
				Rindenstärke = value.Rindenstärke,
				Stammfußhöhe = value.Stammfußhöhe,
				HasBranchStubs = false
			};
			return stamm;
		}

		public static SimulationConfiguration ToDAO(this Data.Domain.SimulationConfiguration value)
		{
			return new SimulationConfiguration()
			{
				Id = value.Id,
				Name = value.Name,
				Comments = value.Bemerkungen,
				Iterations = value.Iterationanzahl,
				IterationStart = value.IterationStart,
				TimeOutPeriod = value.TimeOutPeriod,
				Seed = value.Seed,
				Quality = value.Quality,
				FotooptikQuality = value.FotooptikQuality
			};
		}

		public static SimulationResults ToDAO(this Data.Domain.SimulationResults value)
		{
			var results = new SimulationResults()
			{
				SimulationConfigurationId = value.SimulationConfigurationId,
				SimulationSnapshot = value.SimulationSnapshot,
				SimulationSeed = value.SimulationConfiguration.Seed,
				SimulationQuality = value.Quality,
				FotooptikQuality = value.FotooptikQuality,
				IterationId = value.IterationId,
				IterationStatus = ConvertStatus(value.IterationStatus),
				StirnflächeV = value.StirnflächeV,
				StirnflächeH = value.StirnflächeH,
				FotooptikV = value.FotooptikV,
				FotooptikH = value.FotooptikH,
				Fotooptik = value.Fotooptik,
				FotooptikStützpunkteV = value.FotooptikStützpunkteV,
				FotooptikStützpunkteH = value.FotooptikStützpunkteH,
				PolygonzugV = value.PolygonzugV,
				PolygonzugH = value.PolygonzugH,
				Polygonzug = value.Polygonzug,
				SektionV = value.SektionV,
				SektionH = value.SektionH,
				Sektion = value.Sektion,
				PoltervolumeMR = value.PoltervolumeMR,
				PoltervolumeOR = value.PoltervolumeOR,
				PolterunterlagevolumeMR = value.PolterunterlagevolumeMR,
				PolterunterlagevolumeOR = value.PolterunterlagevolumeOR,
				Rindenanteil = value.Rindenanteil,
				UFSektionOR = value.UFSektionOR,
				UFSektionMR = value.UFSektionMR,
				UFPolygonzugOR = value.UFPolygonzugOR,
				UFPolygonzugMR = value.UFPolygonzugMR,
				UFFotooptikOR = value.UFFotooptikOR,
				UFFotooptikMR = value.UFFotooptikMR,
				Höhe = value.Höhe,
				Breite = value.Breite,
				Modeler = value.Modeler,
				Strategy = value.Strategy,
				Processor = value.Processor
			};
			return results;
		}

		public static Data.Domain.SimulationResults FromDAO(this SimulationResults value)
		{
			var results = new Data.Domain.SimulationResults()
			{
				SimulationConfigurationId = value.SimulationConfigurationId,
				SimulationSnapshot = value.SimulationSnapshot,
				IterationId = value.IterationId,
				Quality = value.SimulationQuality,
				FotooptikQuality= value.FotooptikQuality,
				IterationStatus = ConvertStatus(value.IterationStatus),
				StirnflächeV = value.StirnflächeV,
				StirnflächeH = value.StirnflächeH,
				FotooptikV = value.FotooptikV,
				FotooptikH = value.FotooptikH,
				Fotooptik = value.Fotooptik,
				FotooptikStützpunkteV = value.FotooptikStützpunkteV,
				FotooptikStützpunkteH = value.FotooptikStützpunkteH,
				PolygonzugV = value.PolygonzugV,
				PolygonzugH = value.PolygonzugH,
				Polygonzug = value.Polygonzug,
				SektionV = value.SektionV,
				SektionH = value.SektionH,
				Sektion = value.Sektion,
				PoltervolumeMR = value.PoltervolumeMR,
				PoltervolumeOR = value.PoltervolumeOR,
				PolterunterlagevolumeMR = value.PolterunterlagevolumeMR,
				PolterunterlagevolumeOR = value.PolterunterlagevolumeOR,
				Rindenanteil = value.Rindenanteil,
				UFSektionOR = value.UFSektionOR,
				UFSektionMR = value.UFSektionMR,
				UFPolygonzugOR = value.UFPolygonzugOR,
				UFPolygonzugMR = value.UFPolygonzugMR,
				UFFotooptikOR = value.UFFotooptikOR,
				UFFotooptikMR = value.UFFotooptikMR,
				Höhe = value.Höhe,
				Breite = value.Breite,
				Modeler = value.Modeler,
				Strategy = value.Strategy,
				Processor = value.Processor
			};
			return results;
		}

		private static IterationResult ConvertStatus(Data.Domain.IterationResult status)
		{
			switch (status)
			{
				case Data.Domain.IterationResult.Success:
					return IterationResult.Success;
				case Data.Domain.IterationResult.Timeout:
					return IterationResult.Timeout;
				case Data.Domain.IterationResult.Error:
				default:
					return IterationResult.Error;
			}
		}

		private static Data.Domain.IterationResult ConvertStatus(IterationResult status)
		{
			switch (status)
			{
				case IterationResult.Success:
					return Data.Domain.IterationResult.Success;
				case IterationResult.Timeout:
					return Data.Domain.IterationResult.Timeout;
				case IterationResult.Error:
				default:
					return Data.Domain.IterationResult.Error;
			}
		}
	}
}
