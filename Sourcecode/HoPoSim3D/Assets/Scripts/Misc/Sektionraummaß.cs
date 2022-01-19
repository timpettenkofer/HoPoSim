using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Sektionraummaß : MeasureBehavior
{
	private class MeasurePoint
	{
		public enum Polterteil
		{
			A,
			B
		};

		public float x;
		public float width;
		public Vector2 min;
		public Vector2 max;
		public Polterteil Polter; 
		public float Length()
		{
			return Vector2.Distance(min, max);
		}
	}

	private class Segment
	{
		public Segment(Vector2 source, Vector2 target)
		{
			this.source = source;
			this.target = target;
		}

		public Vector2 source;
		public Vector2 target;
	}

	public float ComputeArea(Side side, IList<Vector2> hull, out float heightEstimate)
	{
		var sections = ComputeMeasurePointsForSide(side, hull, true);
		var area = ComputeAreaForSide(sections);
		heightEstimate = EstimatePolterHeight(sections);
		return area;
	}

	private float ComputeAreaForSide(IEnumerable<MeasurePoint> sections)
	{
		var polterA_sections = sections.Where(s => s.Polter == MeasurePoint.Polterteil.A);
		var polterA_height = polterA_sections.Any()? polterA_sections.Average(s => s.Length()) : 0f;
		var polterA_length = polterA_sections.Sum(s => s.width);
		ConfigurationHelper.Callback.Log($"- Polterhöhe (A): {polterA_height}m Polterlänge (A): {polterA_length}m");
		var polterB_sections = sections.Where(s => s.Polter == MeasurePoint.Polterteil.B);
		var polterB_height = polterB_sections.Any()? polterB_sections.Average(s => s.Length()) : 0f;
		var polterB_length = polterB_sections.Sum(s => s.width);
		ConfigurationHelper.Callback.Log($"- Polterhöhe (B): {polterB_height}m Polterlänge (B): {polterB_length}m");
		return polterA_length * polterA_height + polterB_length * polterB_height;
	}

	private float EstimatePolterHeight(IEnumerable<MeasurePoint> sections)
	{
		var count = sections.Count();
		if (count == 0)
			return 0f;
		if (count < 4)
			return sections.Average(s => s.Length());
		var midIndex = sections.Count() / 2;
		return sections.Skip(midIndex - 1).Take(3).Average(section => section.Length());
	}

	IEnumerable<MeasurePoint> ComputeMeasurePointsForSide(Side side, IList<Vector2> hull, bool verbose)
	{
		try
		{
			ConfigurationHelper.Callback.Log($"Computing Sektionraummaß for side {side}...");

			if (!hull.Any())
				return new MeasurePoint[0];

			var minX = hull.Min(pt => pt.x);
			var maxX = hull.Max(pt => pt.x);

			var sektionslänge = DetermineSectionLength(minX, maxX);
			var points = ComputeMeasurePoints(minX, maxX, hull, sektionslänge);
			if (verbose)
			{
				var bounds = BoundingBox.GetPolterBounds(side);
				LogSektionraummaßInfo(side, bounds, sektionslänge, points);
			}
			return points;
		}
		catch(Exception e)
		{
			ConfigurationHelper.Callback.Log($"Error while computing Sektionraummaß: {e.Message}");
			return Enumerable.Empty<MeasurePoint>();
		}
	}

	private IEnumerable<MeasurePoint> ComputeMeasurePoints(float minX, float maxX, IList<Vector2> hull, float sektionslänge)
	{
		var points = ComputeMeasureCoordinates(minX, maxX, sektionslänge);
		foreach(var pt in points)
		{
			if (!GetIntersectionExtrema(pt.x, hull, out pt.min, out pt.max))
				throw new Exception($"Cannot compute heights for coordinate {pt.x}");
		}
		return points;
	}

	private void LogSektionraummaßInfo(Side side, Bounds bounds, float sektionslänge, IEnumerable<MeasurePoint> points)
	{
		ConfigurationHelper.Callback.Log("Sektionraummaß:");
		ConfigurationHelper.Callback.Log($"- Side {side}");
		ConfigurationHelper.Callback.Log($"- Sektionslänge: {sektionslänge} m");
		ConfigurationHelper.Callback.Log($"- Poltergesamtlänge {bounds.size.x} m");
		ConfigurationHelper.Callback.Log($"- Polter Bounding-Box (m): min:({bounds.min.x:0.##},{bounds.min.y:0.##},{bounds.min.z:0.##}) max:({bounds.max.x:0.##},{bounds.max.y:0.##},{bounds.max.z:0.##})");
		ConfigurationHelper.Callback.Log("- Sektionmitten:");
		foreach (var pt in points)
			ConfigurationHelper.Callback.Log($"   x:{pt.x:0.##} Fußpunkt:({pt.min.x:0.##},{pt.min.y:0.##}) Ablesepunkt:({pt.max.x:0.##},{pt.max.y:0.##}) Höhe:{pt.Length():0.##}m Breite:{pt.width:0.##}m Fläche:{pt.width * pt.Length():0.##}m2");
	}

	private bool GetIntersectionExtrema(float x, IList<Vector2> hull, out Vector2 min, out Vector2 max)
	{
		var intersectingSegments= new List<Segment>();
		
		// collects all segments intersectin abscissa x
		for(int i = 0; i < hull.Count - 1; ++i)
		{
			if (SegmentsOverlaps(x, hull[i], hull[i+1]))
				intersectingSegments.Add(new Segment(hull[i], hull[i + 1]));
		}

		// check last segment closing polyline
		var source = hull.Last();
		var target = hull.First();
		
		if (SegmentsOverlaps(x, source, target))
			intersectingSegments.Add(new Segment(source, target));

		// get y extrema
		var ordinates = intersectingSegments.Select(s => GetIntersectionY(x, s.source, s.target));
		if (!ordinates.Any())
		{
			min = max = new Vector2();
			return false;
		}
		min = new Vector2(x, ordinates.Min());
		max = new Vector2(x, ordinates.Max());
		return true;
	}

	private float GetIntersectionY(float x, Vector2 source, Vector2 target)
	{
		var alpha = (x - source.x) / (target.x - source.x);
		return source.y + alpha * (target.y - source.y);
	}

	private bool SegmentsOverlaps(float x, Vector2 source, Vector2 target)
	{
		var min_x = Mathf.Min(source.x, target.x);
		var max_x = Mathf.Max(source.x, target.x);
		return min_x <= x && max_x >= x;
	}

	private IList<MeasurePoint> ComputeMeasureCoordinates(float minX, float maxX, float sektionslänge)
	{
		var remainingLength = maxX - minX;
		var currentX = minX;

		var sektionsmitten = new List<MeasurePoint>();

		// Polterteil A
		while (remainingLength >= sektionslänge)
		{
			var sm = currentX + (sektionslänge * 0.5f);
			sektionsmitten.Add(new MeasurePoint { x = sm, width = sektionslänge, Polter = MeasurePoint.Polterteil.A });
			currentX += sektionslänge;
			remainingLength -= sektionslänge;
		}

		// Polterteil B
		if (remainingLength > 0)
		{
			var sm = currentX + (remainingLength * 0.5f);
			sektionsmitten.Add(new MeasurePoint { x = sm, width = remainingLength, Polter = MeasurePoint.Polterteil.B });
		}
		return sektionsmitten;
	}

	private float DetermineSectionLength(float minX, float maxX)
	{
		var poltergesamtlänge = maxX - minX;

		if (poltergesamtlänge < 1)
			throw new ArgumentException($"WARNING: Poltergesamtlänge ({poltergesamtlänge} m) ist kleiner als die minimale Sektionslänge Ls = 1m!");
		if (poltergesamtlänge <= 10.0f)
			return 1;
		if (poltergesamtlänge <= 20.0f)
			return 2;
		if (poltergesamtlänge <= 40.0f)
			return 4;
		if (poltergesamtlänge <= 60.0f)
			return 6;
		if (poltergesamtlänge <= 80.0f)
			return 8;
		if (poltergesamtlänge <= 100.0f)
			return 10;
		ConfigurationHelper.Callback.Log($"WARNING: Poltergesamtlänge ({poltergesamtlänge} m) ist größer als 100 m. Using maximal Sektionslänge = 10 m");
		return 10;
	}

	public override GameObject Create(Side side, bool verbose)
	{
		try
		{
			var section = new GameObject("Sektion");
			section.transform.SetParent(gameObject.transform);
			int pruningFactor = ConfigurationHelper.SimulationSettings.Quality;
			Vector2[] hull = GetConcaveHullOutline(side, pruningFactor);
			var segments = ComputeMeasurePointsForSide(side, hull, verbose);

			Material materialA = (Material)Resources.Load("Materials/SektionA");
			Material materialB = (Material)Resources.Load("Materials/SektionB");
			Material sektionHöheMaterial = (Material)Resources.Load("Materials/SektionHöhe");

			foreach (var s in segments)
			{
				var p1 = new Vector2(s.min.x - (s.width * 0.5f), s.min.y);
				var p2 = new Vector2(s.max.x - (s.width * 0.5f), s.max.y);
				var p3 = new Vector2(s.max.x + (s.width * 0.5f), s.max.y);
				var p4 = new Vector2(s.min.x + (s.width * 0.5f), s.min.y);

				var points = PolterTransform.Transform3dMaxDepth(new Vector2[] { p1, p2, p3, p4 }, side, offset);
				CreateLineRenderer(points, section, lineWidthMultiplier, s.Polter == MeasurePoint.Polterteil.A? materialA: materialB);

				var sectionPoints = PolterTransform.Transform3dMaxDepth(new Vector2[] { s.min, s.max }, side, offset);
				CreateLineRenderer(sectionPoints, section, lineWidthMultiplier, sektionHöheMaterial);

			}
			return section;
		}
		catch (Exception)
		{
			return null;
		}
	}

	private Vector2[] GetConcaveHullOutline(Side side, int pruningFactor)
	{
		var hc = gameObject.GetComponent<ConcaveHullOutline>();
		var hull = hc.ComputeHullPolyline(side, pruningFactor);
		return hull;
	}
}
