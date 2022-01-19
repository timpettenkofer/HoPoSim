using System;
using System.Collections.Concurrent;
using UnityEngine;

public class ExecuteOnMainThread : MonoBehaviour
{

	public readonly static ConcurrentQueue<Action> RunOnMainThread = new ConcurrentQueue<Action>();

	private void Awake()
	{
		DontDestroyOnLoad(this.gameObject);
	}

	void Update()
	{
		if (!RunOnMainThread.IsEmpty)
		{
			Action action;
			while (RunOnMainThread.TryDequeue(out action))
			{
				action.Invoke();
			}
		}
	}

}
