using Assets;
using Assets.IPC;
using HoPoSim.IPC.DAO;
using UnityEngine;

public class Configuration : MonoBehaviour
{
	private void Awake()
	{
		DontDestroyOnLoad(gameObject);
	}

	public SimulationData SimulationData { get; set; }
	public SimulationSettings SimulationSettings { get; set; }
	public IpcCallback Callback { get; set; }
}

public class ConfigurationHelper
{
	public static SimulationData SimulationData
	{
		get
		{
			return Configuration.SimulationData;

		}
		set
		{
			Configuration.SimulationData = value;
		}
	}

	public static SimulationSettings SimulationSettings
	{
		get
		{
			return Configuration.SimulationSettings;

		}
		set
		{
			Configuration.SimulationSettings = value;
		}
	}

	public static IpcCallback Callback
	{
		get
		{
			return Configuration.Callback;
		}
		set
		{
			Configuration.Callback = value;
		}
	}

	private static Configuration Configuration
	{
		get
		{
			if (_configuration == null)
			{
				var obj = GameObject.Find("Configuration");
				_configuration = obj.GetComponent<Configuration>();
			}
			return _configuration;
		}
	}
	private static Configuration _configuration;
}
