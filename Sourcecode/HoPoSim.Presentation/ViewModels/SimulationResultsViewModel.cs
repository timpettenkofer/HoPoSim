using HoPoSim.Data.Domain;
using HoPoSim.Presentation.Extensions;

namespace HoPoSim.Presentation.ViewModels
{
	public class SimulationResultsViewModel : ValidableEntityViewModel<SimulationResults>
	{
		public SimulationResultsViewModel(SimulationResults data = null) : base(data)
		{
		}

		public string ToJsonResults()
		{
			var dao = This.ToDAO();
			return IPC.DAO.Serializer<IPC.DAO.SimulationResults>.ToJSON(dao, false);
		}

		public string SimulationSnapshot
		{
			get { return This.SimulationSnapshot; }
		}

		public int IterationId
		{
			get { return This.IterationId; }
		}

		public int Quality
		{
			get { return This.Quality; }
		}

		public int FotooptikQuality
		{
			get { return This.FotooptikQuality; }
		}

		public IterationResult IterationStatus
		{
			get { return This.IterationStatus; }
		}

		public double StirnflächeV
		{
			get { return This.StirnflächeV; }
		}

		public double StirnflächeH
		{
			get { return This.StirnflächeH; }
		}

		public double FotooptikV
		{
			get { return This.FotooptikV; }
		}

		public double FotooptikH
		{
			get { return This.FotooptikH; }
		}

		public double Fotooptik
		{
			get { return This.Fotooptik; }
		}

		public double FotooptikStützpunkteV
		{
			get { return This.FotooptikStützpunkteV; }
		}

		public double FotooptikStützpunkteH
		{
			get { return This.FotooptikStützpunkteH; }
		}

		public double PolygonzugV
		{
			get { return This.PolygonzugV; }
		}

		public double PolygonzugH
		{
			get { return This.PolygonzugH; }
		}

		public double Polygonzug
		{
			get { return This.Polygonzug; }
		}

		public double SektionV
		{
			get { return This.SektionV; }
		}

		public double SektionH
		{
			get { return This.SektionH; }
		}

		public double Sektion
		{
			get { return This.Sektion; }
		}

		public double PoltervolumeMR
		{
			get { return This.PoltervolumeMR; }
		}

		public double PoltervolumeOR
		{
			get { return This.PoltervolumeOR; }
		}

		public double PolterunterlagevolumeMR
		{
			get { return This.PolterunterlagevolumeMR; }
		}

		public double PolterunterlagevolumeOR
		{
			get { return This.PolterunterlagevolumeOR; }
		}

		public double Rindenanteil
		{
			get { return This.Rindenanteil; }
		}

		public double UFSektionOR
		{
			get { return This.UFSektionOR; }
		}

		public double UFSektionMR
		{
			get { return This.UFSektionMR; }
		}

		public double UFPolygonzugOR
		{
			get { return This.UFPolygonzugOR; }
		}

		public double UFPolygonzugMR
		{
			get { return This.UFPolygonzugMR; }
		}

		public double UFFotooptikOR
		{
			get { return This.UFFotooptikOR; }
		}

		public double UFFotooptikMR
		{
			get { return This.UFFotooptikMR; }
		}

		public double Höhe
		{
			get { return This.Höhe; }
		}

		public double Breite
		{
			get { return This.Breite; }
		}
	}
}
