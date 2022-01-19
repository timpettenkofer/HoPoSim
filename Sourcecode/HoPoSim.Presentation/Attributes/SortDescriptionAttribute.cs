using System;
using System.ComponentModel;

namespace HoPoSim.Presentation.Attributes
{
	[AttributeUsage(AttributeTargets.Property)]
	public class SortCriteriumAttribute : Attribute
	{
		public SortCriteriumAttribute(string name, int order = int.MaxValue, ListSortDirection direction = ListSortDirection.Ascending)
		{
			Name = name;
			Order = order;
			Direction = direction;
		}

		public string Name
		{
			get; private set;
		}

		public int Order
		{
			get; private set;
		}

		public ListSortDirection Direction
		{
			get; private set;
		}
	}
}
