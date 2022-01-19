using UnityEngine;

namespace Assets.Scripts.Framework
{
	public class Timer
	{
		public Timer()
		{
			_startTime = Time.realtimeSinceStartup;
		}

		public void Reset()
		{
			_startTime = Time.realtimeSinceStartup;
		}

		public float ElapsedSeconds()
		{
			return Time.realtimeSinceStartup - _startTime;
			
		}

		private float _startTime = -1;
	}
}
