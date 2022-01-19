using HoPoSim.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;


namespace HoPoSim.Presentation.Extensions
{
    public static class Extensions
    {
		public static IEnumerable<T> AddFirst<T>(this IEnumerable<T> objects, T instance)
		{
			return new T[] { instance }.Concat(objects);
		}

		public static void ForEach<T>(this IEnumerable<T> value, Action<T> action)
		{
			foreach (T item in value)
			{
				action(item);
			}
		}

		public static string GenerateUniqueName<T>(this IEnumerable<T> entities, string name, string format = "D3") where T : IHaveNameProperty
		{
			return GenerateUniqueName(entities, name, e => e.Name, format);
		}

		public static string GenerateUniqueName<T>(this IEnumerable<T> entities, string name, Func<T,string> getName, string format = "D3")
		{
			try
			{
				string candidate;
				int counter = -1;
				do
				{
					counter = counter + 1;
					if (counter > 9999999) counter = 1;
					candidate = counter == 0? name : $"{name}_{counter.ToString(format)}";
				} while (entities.FirstOrDefault(e => getName(e) == candidate) != null);
				return candidate;
			}
			catch (Exception)
			{
				return name;
			}
		}
	}
}
