using HoPoSim.Data.Interfaces;
using HoPoSim.Data.Domain;
using HoPoSim.Data.Model;
using System.Data;
using HoPoSim.Presentation.Extensions;
using System.Collections.Generic;
using System.Linq;

namespace HoPoSim.Presentation.ViewModels
{
	public class SimulationDataDetailsViewModel : ValidableEntityViewModel<SimulationData>, IHaveNameProperty
	{
		public SimulationDataDetailsViewModel(IEnumerable<Stapelqualität> stapelqualitäten, SimulationData data = null) : base(data)
		{
			Stapelqualitäten = stapelqualitäten;
		}

		private IEnumerable<Stapelqualität> Stapelqualitäten { get; }
		public string Name
		{
			get { return This.Name; }
			set { SetProperty(This.Name, value, () => This.Name = value); }
		}

		public int EntityId
		{
			get { return This.Id; }
		}

		public string Bemerkungen
		{
			get { return This.Bemerkungen; }
			set { SetProperty(This.Bemerkungen, value, () => This.Bemerkungen = value); }
		}

		public float Polterlänge
		{
			get { return This.Polterlänge; }
			set { SetProperty(This.Polterlänge, value, () => This.Polterlänge = value); }
		}

		public float? Tiefe
		{
			get { return This.Poltertiefe.HasValue ? This.Poltertiefe.Value : System.Convert.ToSingle(Stammdaten != null ? Stammdaten.Länge : 0f); }
			set { SetProperty(This.Poltertiefe, value, () => This.Poltertiefe = value); }
		}

		public bool Polterunterlage
		{
			get { return This.Polterunterlage; }
			set { SetProperty(This.Polterunterlage, value, () => This.Polterunterlage = value); }
		}

		public float Steigungswinkel
		{
			get { return This.Steigungswinkel; }
			set { SetProperty(This.Steigungswinkel, value, () => This.Steigungswinkel = value); }
		}

		public float WoodDensity
		{
			get { return This.WoodDensity; }
			set { SetProperty(This.WoodDensity, value, () => This.WoodDensity = value); }
		}

		public float WoodFriction
		{
			get { return This.WoodFriction; }
			set { SetProperty(This.WoodFriction, value, () => This.WoodFriction = value); }
		}

		public float Seitenspiegelung
		{
			get { return This.Seitenspiegelung; }
			set { SetProperty(This.Seitenspiegelung, value, () => This.Seitenspiegelung = value); }
		}

		public bool Zufallsspiegelung
		{
			get { return This.Zufallsspiegelung; }
			set { SetProperty(This.Zufallsspiegelung, value, () => This.Zufallsspiegelung = value); }
		}

		public string SourceFile
		{
			get { return This.SourceFile; }
			set { SetProperty(This.SourceFile, value, () => This.SourceFile = value); }
		}

		public int Rindenbeschädigungen
		{
			get { return This.Rindenbeschädigungen; }
			set { SetProperty(This.Rindenbeschädigungen, value, () => This.Rindenbeschädigungen = value); }
		}

		public int Krümmungsvarianten
		{
			get { return This.Krümmungsvarianten; }
			set { SetProperty(This.Krümmungsvarianten, value, () => This.Krümmungsvarianten = value); }
		}

		public BaumartParametrization Baumart
		{
			get { return This.Baumart; }
			set { SetProperty(This.Baumart, value, () => This.Baumart = value); }
		}

		public int StapelqualitätStufe
		{
			get { return Stapelqualität.Level; }
			set
			{
				var qualität = Stapelqualitäten.FirstOrDefault(g => g.Level == value);
				Stapelqualität = qualität;
				OnPropertyChanged(nameof(StapelqualitätStufe));
				OnPropertyChanged(nameof(StapelqualitätDescription));
			}
		}

		public string StapelqualitätDescription
		{
			get
			{
				var desc = $"{Stapelqualität.CrossTrunksProportion}% der Stämme schräg" +
					(Stapelqualität.CrossTrunksProportion > 0 ? $" (zwischen {Stapelqualität.CrossTrunksMinimumAngle} und {Stapelqualität.CrossTrunksMaximumAngle} Grad)" : string.Empty);
				return string.IsNullOrEmpty(Stapelqualität.Bemerkungen) ? desc : $"{Stapelqualität.Bemerkungen}\n{desc}";
			}
		}

		public Stapelqualität Stapelqualität
		{
			get { return This.Stapelqualität; }
			set { SetProperty(This.Stapelqualität, value, () => This.Stapelqualität = value); }
		}


		public Stammdaten Stammdaten
		{
			get { return This.Stammliste; }
			set
			{
				if (SetProperty(This.Stammliste, value, () => This.Stammliste = value))
				{
					OnPropertyChanged(nameof(StammdatenView));
					OnPropertyChanged(nameof(Stammanzahl));
					OnPropertyChanged(nameof(Tiefe));
				}
			}
		}

		public DataView StammdatenView
		{
			get { return Stammdaten?.DataTable?.AsDataView(); }
		}


		public int Stammanzahl
		{
			get { return Stammdaten != null? Stammdaten.Stammanzahl : 0; }
		}

		public bool IsValid()
		{
			return This.IsValidInput();
		}

		public string ToJSON(bool indented, bool convertToIdeal = false)
		{
			var data = This.ToDAO();
			if (convertToIdeal)
				data = ConvertToIdealTrunks(data);
			return IPC.DAO.Serializer<IPC.DAO.SimulationData>.ToJSON(data, indented);
		}

		private IPC.DAO.SimulationData ConvertToIdealTrunks(IPC.DAO.SimulationData data)
		{
			data.Baumart.MinNoiseSize = 0;
			data.Baumart.MaxNoiseSize = 0;
			data.Baumart.MinNoiseStrength = 0;
			data.Baumart.MaxNoiseStrength = 0;
			data.Baumart.IncludeRoots = false;
			data.Baumart.IncludeBranches = false;

			foreach (var t in data.Stämme)
			{
				t.Abholzigkeit = 0;
				t.Krümmung = 0;
				t.Ovalität = 1;
				t.D_Stirn_mR = t.D_Zopf_mR = t.D_Mitte_mR;
				t.D_Stirn_oR = t.D_Zopf_oR = t.D_Mitte_oR;
			}
			return data;
		}
	}
}

