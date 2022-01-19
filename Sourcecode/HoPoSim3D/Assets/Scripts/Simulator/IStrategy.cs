using HoPoSim.IPC.DAO;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Interfaces
{
	public class IterationOutcomeArgs : EventArgs
	{
		public int Iteration { get; private set; }
		public IterationResult Status { get; private set; }

		public IterationOutcomeArgs(int iteration, IterationResult status)
		{
			Iteration = iteration;
			Status = status;
		}
	}

	public class BadConfigurationArgs : EventArgs
	{
		public int Iteration { get; private set; }
		public Action<SimulationData> FixConfiguration { get; private set; }

		public BadConfigurationArgs(int iteration, Action<SimulationData> retry)
		{
			Iteration = iteration;
			FixConfiguration = retry;
		}
	}

	public class ConfigurationException : Exception
	{
		public ConfigurationException(string message, Action<SimulationData> fixConfiguration) : base(message)
		{
			FixConfiguration = fixConfiguration;
		}
		public Action<SimulationData> FixConfiguration { get; }
	}

	public interface IStrategy
	{
		void SetTimeOutPeriod(int seconds);
		void Initialize(SimulationData data);
		IEnumerable<GameObject> Configure(int iteration, IEnumerable<GameObject> trunks);
		void Perform();
		event EventHandler<IterationOutcomeArgs> IterationFinished;
		event EventHandler<BadConfigurationArgs> BadConfigurationDetected;
	}
}
