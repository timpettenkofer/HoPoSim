using Assets.Interfaces;
using HoPoSim.IPC.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TierStrategy : BaseStrategy
{
	public class GameObjectWithBounds
	{
		public GameObject go;
		public Bounds bounds;
	}

	public class GameObjectPositioning : GameObjectWithBounds
	{
		public Vector2 position;
	}

	protected float InitialSteigungswinkel { get; private set; }

	public override void BuildInitialConfiguration(IEnumerable<GameObject> polterTrunks, IEnumerable<GameObject> polterunterlageTrunks)
	{
		InitialSteigungswinkel = CurrentSimulationData.Poltermaße.Steigungswinkel; // * 0.9f;
		InitializePolterTrunks(polterTrunks);
		SetInitialTrapez();
	}

	private void SetInitialTrapez()
	{
		var maxDepth = PolterManager.GetPolterMaxDepth();
		var length = CurrentSimulationData.Poltermaße.MinimumPolterlänge;
		TrapezBuilder.SetTrapezInitialBounds(length, InitialSteigungswinkel, CurrentHorizontalPolterOffset, maxDepth);
	}

	public override void OnIterationStarting()
	{
		IterationStarted = false;
	}

	bool rightToLeft = true;

	public override void OnFixedUpdate()
	{
		if (!IsPhysicsEngineRunning())
		{
			if (GetAllTrunksToBeProcessed().Any())
			{
				var height = GetCurrentPolterHeight();
				//PositionTrunkLayer(CurrentHorizontalPolterOffset, height, GetAllTrunksAlreadyProcessed().Any()? 1 : 2, rightToLeft);
				PositionTrunkLayer(CurrentHorizontalPolterOffset, height, 1, rightToLeft);
				rightToLeft = !rightToLeft;
				ForcePhysicsEngineRun();
			}
			else
			{
				NotifyIterationSuccess();

				//if (!trapezModificationPerformed)
				//{
				//	trapezModificationPerformed = true;
				//	TrapezBuilder.ResetSideAngle(CurrentSimulationData, CurrentHorizontalPolterOffset, CurrentSimulationData.Poltermaße.Steigungswinkel);
				//	ForcePhysicsEngineRun();
				//}
				//else
				//{
				//	NotifyIterationSuccess();
				//	SetInitialTrapez();
				//}
			}
		}
	}

	public override void OnIterationTimedOut()
	{
		var trunks = GetAllTrunksToBeProcessed().Select(t => t.go);
		SetProcessed(trunks);
	}

	private void PositionTrunkLayer(float horizontalOffset, float verticalOffset, int numLayers, bool rightToLeft)
	{
		if (numLayers < 1)
			return;

		var positions = new List<GameObjectPositioning>();

		float maxHeight = TrapezBuilder.MaximumHeight(CurrentSimulationData);
		Vector2 pos = Vector2.zero;
		float x = 0f;
		float y = verticalOffset;

		var halfPolterLength = CurrentSimulationData.Poltermaße.MinimumPolterlänge * 0.5f;

		//var processedTrunkBounds = GetAllTrunksAlreadyProcessed().Select(t => BoundingBox.GetAxisAlignedBounds(t.go));

		foreach (var trunkWithBound in GetAllTrunksToBeProcessed())
		{
			var trunk = trunkWithBound.go;
			var bounds = trunkWithBound.bounds;

			var width = bounds.size.x;
			var height = bounds.size.y;

			if (TryAddToLayer(width, height, x, y, ref pos))
			{
				x = pos.x + width + EpsilonPolter;

				if (rightToLeft)
					pos = new Vector2(halfPolterLength + halfPolterLength - (pos.x + width), pos.y);

				//var corrected_y = pos.y; // correctY? GetMinimalHeightForTrunkAt(pos.x, pos.y, width, height, processedTrunkBounds) : pos.y;
				var position = new GameObjectPositioning { go = trunk, bounds = bounds, position = new Vector2(pos.x, pos.y) };
				positions.Add(position);
			}
			else
			{
				if (x == 0f) // cannot position a single trunk. Trapez is filled
				{
					var factor = 10f;
					var length = CurrentSimulationData.Poltermaße.MinimumPolterlänge * (1 + (factor / 100f));
					var msg = $"[WARNING] Minimum configured Polterlänge is too small for simulation. Trying again after increasing Polterlänge by {factor}% ({length:0.##} m).";
					Action<SimulationData> func = (data) => { CurrentSimulationData.Poltermaße.MinimumPolterlänge = length; };
					throw new ConfigurationException(msg, func);
				}
				SetActiveTrunkPositions(horizontalOffset, positions);

				var polterY = GetCurrentPolterHeight();
				PositionTrunkLayer(horizontalOffset, polterY, numLayers - 1, rightToLeft);
				return;
			}
		}
		// last layer is incomplete, distribute evenly and position
		EvenlyDistributeTrunkLayer(ref positions, horizontalOffset, verticalOffset, rightToLeft);
		SetSleepThreshold(0.001f);
		SetActiveTrunkPositions(horizontalOffset, positions);
	}

	private void SetActiveTrunkPositions(float horizontalOffset, List<GameObjectPositioning> positions)
	{
		var angle = InitialSteigungswinkel * Mathf.Deg2Rad;
		var polterLength = CurrentSimulationData.Poltermaße.MinimumPolterlänge;
		

		foreach (var pos in positions)
		{
			MoveTrunkAt(pos.go, pos.bounds, pos.position.x + horizontalOffset, pos.position.y);
			SetProcessed(pos.go);
			ApplyLateralForce(pos, polterLength * 0.5f, angle, 25f);
		}
	}

	//private float GetMinimalHeightForTrunkAt(float x, float y, float width, float height, IEnumerable<Bounds> bounds)
	//{
	//	var boxesAtHorizontalOffset = bounds.Where(b => Overlaps(b, x, x + width));
	//	return boxesAtHorizontalOffset.Any() ? boxesAtHorizontalOffset.Max(b => b.max.y) : GetPolterunterlageVerticalOffset();
	//}

	//private bool Overlaps(Bounds bounds, float xmin, float xmax)
	//{
	//	return !(xmin > bounds.max.x || xmax < bounds.min.x);
	//}

	private void ApplyLateralForce(GameObjectPositioning pos, float polterHalfLength, float polterAngle, float forceMultiplier)
	{
		var pyramidHeight = Mathf.Tan(polterAngle) * polterHalfLength;
		var y = pyramidHeight - pos.position.y;
		var length = y / Mathf.Tan(polterAngle);
		var distanceToCenter = polterHalfLength - pos.position.x;

		var factorToCenter = Mathf.Abs(distanceToCenter) / length;
		var forceValue = Mathf.Lerp(0, distanceToCenter > 0 ? -1 : 1, factorToCenter);
		//var multiplier = Mathf.Lerp(1, 0.1f, y / (pyramidHeight * 2));
		var force = new Vector3(forceValue * forceMultiplier, 0f);
		
		var rb = pos.go.GetComponent<Rigidbody>();
		rb.AddForce(force);
	}

	private void EvenlyDistributeTrunkLayer(ref List<GameObjectPositioning> positions, float horizontalOffset, float verticalOffset, bool rightToLeft)
	{
		if (!positions.Any())
			return;

		var x_space = GetExtraHorizontalSpaceToDistribute(positions, verticalOffset);
		var interval_count = positions.Count() - 1;
		var interval_length = x_space / interval_count;

		for (int i = 1; i < positions.Count(); ++i)
		{
			positions[i].position.x = positions[i].position.x + (rightToLeft? - 1 : +1) * (i * interval_length);
			positions[i].position.y = verticalOffset;
		}
	}

	private float GetExtraHorizontalSpaceToDistribute(List<GameObjectPositioning> positions, float verticalOffset)
	{
		var max_y = positions.Max(p => verticalOffset + p.bounds.size.y);
		
		var polterLength = CurrentSimulationData.Poltermaße.MinimumPolterlänge;
		var angle = InitialSteigungswinkel * Mathf.Deg2Rad;
		var maxTrapezY = (polterLength  * 0.5f) * Mathf.Tan(angle);

		var totalAvailableHorizontalSpace = 2 * ((maxTrapezY - max_y) / Mathf.Tan(angle));

		var max_X = positions.Max(p => p.position.x + p.bounds.size.x);
		var min_X = positions.Min(p => p.position.x);

		var deltaX = max_X - min_X;
		var extraHorizontalSpace = totalAvailableHorizontalSpace - deltaX;
		return extraHorizontalSpace > 0? extraHorizontalSpace : 0f;
	}

	private float GetCurrentPolterHeight()
	{
		var trunks = GetAllTrunksAlreadyProcessed();
		return trunks.Any()? BoundingBox.GetAxisAlignedBounds(trunks.Select(t => t.go)).max.y : GetPolterunterlageVerticalOffset();
	}

	private bool TryAddToLayer(float width, float height, float horizontalOffset, float verticalOffset, ref Vector2 pos)
	{
		var left_xfit = xFit(horizontalOffset, verticalOffset + height);
		if (left_xfit >= 0)
		{
			var right_xfit = xFit(left_xfit + width, verticalOffset + height);
			if (right_xfit >= 0)
			{
				pos = new Vector2(left_xfit, verticalOffset);
				return true;
			}
		}
		return false;
	}


	//private bool TryAddToLayer(float width, float height, float horizontalOffset, float verticalOffset, IEnumerable<Bounds> bounds, ref Vector2 pos)
	//{
	//	var left_xfit = xFit(horizontalOffset, verticalOffset + height);
	//	if (left_xfit >= 0)
	//	{
	//		var right_xfit = xFit(left_xfit + width, verticalOffset + height);
	//		if (right_xfit >= 0)
	//		{
	//			//var y = GetMinimalHeightForTrunkAt(left_xfit, verticalOffset, width, height, bounds);
	//			//pos = rightToLeft ? new Vector2(left_xfit, y) : new Vector2(midX - (left_xfit - midX) - width, y);
	//			//pos = new Vector2(left_xfit, y); //
	//			pos = new Vector2(left_xfit, verticalOffset);
	//			return true;
	//		}
	//	}
	//	return false;
	//}


	private float xFit(float x, float y)
	{
		var angle = InitialSteigungswinkel * Mathf.Deg2Rad;
		var polterLength = CurrentSimulationData.Poltermaße.MinimumPolterlänge;
		var half_length = 0.5f * polterLength;
		var max_y = x <= half_length ?
			Mathf.Tan(angle) * x :
			Mathf.Tan(angle) * (polterLength - x);

		if (y <= max_y) // valid candidate
			return x;
		if (x > half_length) // slope is decreasing after half length, no solution
			return -1;
		if (y > Mathf.Tan(angle) * half_length) // candidate is higher than maximal height / than top of pyramid
			return -1;
		var fit_x = y / Mathf.Tan(angle); // compute next fit
		return fit_x;
	}

	private void Awake()
	{
		TrapezBuilder.CreateTrapez();
	}

	private TrapezBuilder TrapezBuilder { get; } = new TrapezBuilder();

	IList<GameObjectWithBounds> PolterTrunksWithBounds;

	private void InitializePolterTrunks(IEnumerable<GameObject> polterTrunks)
	{
		PolterTrunksWithBounds = polterTrunks
			.Select(t => new GameObjectWithBounds { go = t, bounds = BoundingBox.GetAxisAlignedBounds(t) })
			.ToList();
		SetUnprocessed(polterTrunks);
	}

	#region Trunk Activation
	private IEnumerable<GameObjectWithBounds> GetAllTrunksWithState(bool active)
	{
		return PolterTrunksWithBounds.Where(t => t.go.activeInHierarchy == active);
	}

	private IEnumerable<GameObjectWithBounds> GetAllTrunksToBeProcessed()
	{
		return GetAllTrunksWithState(false);
	}

	private IEnumerable<GameObjectWithBounds> GetAllTrunksAlreadyProcessed()
	{
		return GetAllTrunksWithState(true);
	}

	private void SetProcessed(GameObject trunk)
	{
		trunk.SetActive(true);
	}

	private void SetProcessed(IEnumerable<GameObject> trunks)
	{
		foreach (var trunk in trunks)
			trunk.SetActive(true);
	}

	private void SetUnprocessed(IEnumerable<GameObject> trunks)
	{
		foreach (var trunk in trunks)
			trunk.SetActive(false);
	}
	#endregion
}
