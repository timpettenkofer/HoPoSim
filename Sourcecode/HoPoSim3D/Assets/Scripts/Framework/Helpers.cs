using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts.Framework
{
	public static class Helpers
	{
		public static List<T> ShuffleTrunks<T>(IEnumerable<T> input)
		{
			return input
				.Select(t => new { obj = t, value = UnityEngine.Random.Range(0f, 1f) })
				.OrderBy(x => x.value)
				.Select(x => x.obj)
				.ToList();
		}
	}
}
