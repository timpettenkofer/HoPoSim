using HoPoSim.IPC.DAO;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class PolterunterlageBuilder
{
	public static float minSpaceBetweenTrunksInBaseRow = 0.01f;
	public static float maxSpaceBetweenTrunksInBaseRow = 0.03f;

	public static float baseRowDepthRatio = 0.25f; // ratio determining the depth of the row wrt to the trunks' length (0.25=25% is between center and log ends)
	public static float lengthToleranceRatio = 1.015f;

	public static float HeightEpsilon { get { return 0.05f; } }

	public static IEnumerable<GameObject> Build(IEnumerable<GameObject> trunks, SimulationData simulationData)
	{
		if (!simulationData.Poltermaße.Polterunterlage)
			return Enumerable.Empty<GameObject>();

		var length = simulationData.Poltermaße.MinimumPolterlänge;
		var depth = simulationData.Poltermaße.Polterbreite;

		var sortedTrunks = SelectTrunks(trunks, length).ToList();

		var firstRowTrunks = BuildRow(sortedTrunks, 0, length, -depth * baseRowDepthRatio);
		var secondRowTrunks = BuildRow(sortedTrunks.Skip(firstRowTrunks.Count()), 0, length, depth * baseRowDepthRatio);

		CenterRows(firstRowTrunks, secondRowTrunks);
		return firstRowTrunks.Union(secondRowTrunks);
	}

	private static void CenterRows(IEnumerable<GameObject> firstRowTrunks, IEnumerable<GameObject> secondRowTrunks)
	{
		var firstRowBox = BoundingBox.GetAxisAlignedBounds(firstRowTrunks);
		var secondRowBox = BoundingBox.GetAxisAlignedBounds(secondRowTrunks);

		var firstRowMax = firstRowBox.max.x;
		var secondRowMax = secondRowBox.max.x;
		var offset = - Mathf.Abs(firstRowMax - secondRowMax) * 0.5f;

		if (firstRowMax > secondRowMax)
			ShiftRowHorizontally(firstRowTrunks, offset);
		else
			ShiftRowHorizontally(secondRowTrunks, offset);
	}

	private static void ShiftRowHorizontally(IEnumerable<GameObject> trunks, float offset)
	{
		foreach (var t in trunks)
			t.transform.position = t.transform.position + new Vector3(offset, 0, 0);
	}

	private static IEnumerable<GameObject> SelectTrunks(IEnumerable<GameObject> trunks, float length)
	{
		return trunks.Select(t => t.GetComponent<TrunkComponent>()).
			SelectForPolterunterlage(length).
			Select(tc => tc.gameObject);
	}

	private static IEnumerable<GameObject> BuildRow(IEnumerable<GameObject> trunks, float startx, float endx, float depth)
	{
		var baseTrunks = new List<GameObject>();
		float currentx = startx;
		foreach (var t in trunks)
		{
			if ((currentx * lengthToleranceRatio) > endx)
				return baseTrunks;

			var bounds = BoundingBox.GetAxisAlignedBounds(t);
			RotateAndMoveTrunkAt(t, bounds, currentx, HeightEpsilon, depth);
			currentx += bounds.size.z + UnityEngine.Random.Range(minSpaceBetweenTrunksInBaseRow, maxSpaceBetweenTrunksInBaseRow);
			baseTrunks.Add(t);
		}
		return baseTrunks;
	}

	private static void RotateAndMoveTrunkAt(GameObject obj, Bounds bounds, float x, float y, float z)
	{
		var trunk_z = obj.transform.position.z;

		var shift_x = x;
		var shift_y = 0;
		var shift_z = z - trunk_z;

		obj.transform.position += new Vector3(shift_x, shift_y, shift_z);
		var eulers = new Vector3(0, trunk_z < 0 ? 90 : -90, 0);
		obj.transform.Rotate(eulers, Space.World);

		var b = BoundingBox.GetAxisAlignedBounds(obj);
		var pos = obj.transform.position;
		var pos_y = pos.y - b.min.y + y;
		obj.transform.position = new Vector3(pos.x, pos_y, pos.z);
	}

	public static IEnumerable<TrunkComponent> SelectForPolterunterlage(this IEnumerable<TrunkComponent> trunks, float length)
	{
		// Die Polterunterlage wird aus den 10 % gradesten Stämmen zufällig gezogen.
		// Sie sollten überdies in etwa die gleichen Durchmesser haben (10% dicksten Stämmen)
		// Abholzigkeit kann dann vernachlässigt werden
		return SelectForPolterunterlage(trunks, length, 0.1f);
	}

	private static IEnumerable<TrunkComponent> SelectForPolterunterlage(IEnumerable<TrunkComponent> trunks, float length, float ratio)
	{
		var candidates = trunks
			.OrderBy(t => t.trunkParameters.BendingMultiplier)
			.Take(Mathf.FloorToInt(trunks.Count() * ratio));

		if (!HasRequiredLength(candidates, length))
		{
			if (ratio < 1f)
				return SelectForPolterunterlage(trunks, length, ratio * 2);
			return trunks;
		}

		var thickest = candidates
			.OrderByDescending(t => t.trunkParameters.RadiusMultiplier)
			.Take(Mathf.Max(1, Mathf.FloorToInt(candidates.Count() * ratio)));
		var randomIndex = UnityEngine.Random.Range(0, thickest.Count());
		var refRadius = thickest.ElementAt(randomIndex).trunkParameters.RadiusMultiplier;

		return candidates.OrderBy(t => Mathf.Abs(refRadius - t.trunkParameters.RadiusMultiplier));
	}

	private static bool HasRequiredLength(IEnumerable<TrunkComponent> trunks, float length)
	{
		int row = 0;
		float sum = 0;

		// checks that the sum of the trunks' length allows to build two 'Polterunterlage' rows
		foreach (var t in trunks)
		{
			sum += t.trunkParameters.Length;
			if (sum >= length)
			{
				row += 1;
				sum = 0;
			}
			if (row == 2)
				return true;
		}
		return false;
	}
}
