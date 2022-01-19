using Assets;
using Assets.Interfaces;
using Assets.IPC;
using HoPoSim.IPC.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class Analyzer : MonoBehaviour, IResultProcessor
{
	public void Process(IterationOutcomeArgs outcome, SimulationSettings settings, IpcCallback callback)
	{
		SimulationResults results = CreateResults(settings, outcome);
		switch (outcome.Status)
		{
			case IterationResult.Success:
				{
					ExtractResults(ref results, settings);
					ExtractSnapshot(ref results);
					break;
				}
			case IterationResult.Error:
			case IterationResult.Timeout:
				{
					ExtractSnapshot(ref results);
					break;
				}
		}
		Reporter.SendSimulationResults(results, callback);
	}

	private SimulationResults CreateResults(SimulationSettings settings, IterationOutcomeArgs outcome)
	{
		return new SimulationResults
		{
			SimulationConfigurationId = settings.SimulationConfigurationId,
			IterationId = outcome.Iteration,
			IterationStatus = outcome.Status,
			Modeler = settings.Modeler,
			Strategy = settings.Strategy,
			Processor = settings.Processor,
			SimulationSeed = settings.Seed,
			SimulationQuality = settings.Quality,
			FotooptikQuality = settings.FotooptikQuality,
			SimulationSnapshot = string.Empty
		};
	}

	private void ExtractResults(ref SimulationResults results, SimulationSettings settings)
	{
		ConfigurationHelper.Callback.Log($"Extracting results from simulation iteration {results.IterationId}");
		ConfigurationHelper.Callback.Log($"Computing volumes...");
		var FmmR = GetPoltervolumeFmmR();
		var FmoR = GetPoltervolumeFmoR();
		var polterunterlageFmR = GetPolterunterlagevolumeFmmR();
		var polterunterlageFoR = GetPolterunterlagevolumeFmoR();
		double rindenanteil = GetRindenanteil(FmmR, FmoR);
		ConfigurationHelper.Callback.Log($"Computing trunks outline area...");
		double stirnflächeV = GetStirnflächeFor(Side.FRONT);
		double stirnflächeH = GetStirnflächeFor(Side.BACK);

		ConfigurationHelper.Callback.Log($"Computing Polygonzug values...");
		double polygonzugV = ComputePolygonzugAreaFor(Side.FRONT, out Vector2[] hull_front);
		double polygonzugH = ComputePolygonzugAreaFor(Side.BACK, out Vector2[] hull_back);
		double polygonVolume = ComputeVolumeFromSides(polygonzugV, polygonzugH);

		ConfigurationHelper.Callback.Log($"Computing Fotooptik  values...");
		double fotooptikV = ComputeFotooptikAreaFor(Side.FRONT, hull_front, out int stützpunkteV);
		double fotooptikH = ComputeFotooptikAreaFor(Side.BACK, hull_back, out int stützpunkteH);
		double fotooptikVolume = ComputeVolumeFromSides(fotooptikV, fotooptikH);
		
		ConfigurationHelper.Callback.Log($"Computing Sektionraummaß values...");
		float heightV, heightH;
		double sektionV = ComputeSektionraummaßAreaFor(Side.FRONT, hull_front, out heightV);
		double sektionH = ComputeSektionraummaßAreaFor(Side.BACK, hull_back, out heightH);
		double sektionVolume = ComputeVolumeFromSides(sektionV, sektionH);

		ConfigurationHelper.Callback.Log($"Computing Umrechnungfaktoren...");
		double ufPolygonMR = GetUmrechnungfaktor(FmmR, polygonVolume);
		double ufPolygonOR = GetUmrechnungfaktor(FmoR, polygonVolume);

		double ufSektionMR = GetUmrechnungfaktor(FmmR, sektionVolume);
		double ufSektionOR = GetUmrechnungfaktor(FmoR, sektionVolume);

		double ufFotooptikMR = GetUmrechnungfaktor(FmmR, fotooptikVolume);
		double ufFotooptikOR = GetUmrechnungfaktor(FmoR, fotooptikVolume);

		ConfigurationHelper.Callback.Log($"Results extracted from simulation iteration {results.IterationId}.");

		results.StirnflächeV = Round(stirnflächeV, 3);
		results.StirnflächeH = Round(stirnflächeH, 3);

		results.FotooptikV = Round(fotooptikV, 3);
		results.FotooptikH = Round(fotooptikH, 3);
		results.Fotooptik = Round(fotooptikVolume, 3);
		results.FotooptikStützpunkteV = stützpunkteV;
		results.FotooptikStützpunkteH = stützpunkteH;

		results.PolygonzugV = Round(polygonzugV, 3);
		results.PolygonzugH = Round(polygonzugH, 3);
		results.Polygonzug = Round(polygonVolume, 3);

		results.SektionV = Round(sektionV, 2);
		results.SektionH = Round(sektionH, 2);
		results.Sektion = Round(sektionVolume, 2);

		results.PoltervolumeMR = Round(FmmR, 2);
		results.PoltervolumeOR = Round(FmoR, 2);
		results.PolterunterlagevolumeMR = Round(polterunterlageFmR, 2);
		results.PolterunterlagevolumeOR = Round(polterunterlageFoR, 2);
		results.Rindenanteil = Round(rindenanteil, 2);

		results.UFPolygonzugMR = Round(ufPolygonMR, 3);
		results.UFPolygonzugOR = Round(ufPolygonOR, 3);
		results.UFSektionMR = Round(ufSektionMR, 3);
		results.UFSektionOR = Round(ufSektionOR, 3);
		results.UFFotooptikMR = Round(ufFotooptikMR, 3);
		results.UFFotooptikOR = Round(ufFotooptikOR, 3);

		var höhe = (heightV + heightH) * 0.5;
		var breite = GetPolterLength();
		results.Höhe = Round(höhe, 2);
		results.Breite = Round(breite, 2);
	}

	private void ExtractSnapshot(ref SimulationResults results)
	{
		results.SimulationSnapshot =  SnapshotController.CreateSnapshot();
	}

	private double GetPolterLength()
	{
		var trunks = PolterManager.GetPolterTrunks(PolterManager.PolterTag);
		var bounds = BoundingBox.GetRendererBounds(trunks);
		return bounds.size.x;
	}

	private double GetUmrechnungfaktor(double fm, double rm)
	{
		return fm / rm;
	}

	private double GetStirnflächeFor(Side side)
	{
		return GetTrunkOutlines().Sum(s => s.ComputeArea(side));
	}

	private static TrunkOutline[] GetTrunkOutlines()
	{
		return PolterManager.GetPolterTrunkComponents<TrunkOutline>(PolterManager.PolterTag);
	}

	private double ComputePolygonzugAreaFor(Side side, out Vector2[] vertices)
	{
		var hull = GameObject.FindObjectOfType<ConcaveHullOutline>();
		return hull.ComputeArea(side, out vertices);
	}

	private double ComputeFotooptikAreaFor(Side side, Vector2[] vertices, out int numVertices)
	{
		var hull = GameObject.FindObjectOfType<Fotooptik>();
		return hull.ComputeArea(side, vertices, out numVertices);
	}

	private double ComputeSektionraummaßAreaFor(Side side, IList<Vector2> vertices, out float height)
	{
		var sektion = GameObject.FindObjectOfType<Sektionraummaß>();
		return sektion.ComputeArea(side, vertices, out height);
	}

	private double ComputeVolumeFromSides(double vorderseiteSurface, double hinterseiteSurface)
	{
		var trunks = GetInspectors(PolterManager.PolterTag);
		var length = PolterManager.GetPolterCustomOrAverageDepth();
		var averageSurface = (vorderseiteSurface + hinterseiteSurface) * 0.5;
		return  averageSurface * length;
	}

	private double GetPoltervolumeFmmR()
	{
		return GetSum(PolterManager.PolterTag,i => i.FmmR) +
			GetPolterunterlagevolumeFmmR();
	}

	private double GetPoltervolumeFmoR()
	{
		return GetSum(PolterManager.PolterTag,i => i.FmoR) + 
			GetPolterunterlagevolumeFmoR();
	}

	private double GetPolterunterlagevolumeFmmR()
	{
		return GetSum(PolterManager.PolterunterlageTag, i => i.FmmR);
	}

	private double GetPolterunterlagevolumeFmoR()
	{
		return GetSum(PolterManager.PolterunterlageTag, i => i.FmoR);
	}

	private double GetSum(string trunkTag, Func<Inspector, double> func)
	{
		return GetInspectors(trunkTag).Sum(func);
	}

	private static Inspector[] GetInspectors(string tag)
	{
		return PolterManager.GetPolterTrunkComponents<Inspector>(tag);
	}

	private static double GetRindenanteil(double FmmR, double FmoR)
	{
		return FmmR != 0.0 ? ((FmmR - FmoR) / FmmR) * 100.0 : -1;
	}

	private static double Round(double value, int digits = 2)
	{
		return Math.Round(value, digits);
	}
}
