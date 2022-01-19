using System;

namespace HoPoSim.Framework.Attributes
{
	[AttributeUsage(AttributeTargets.Property)]
	public class ExportDescriptionAttribute : Attribute
	{
		public ExportDescriptionAttribute(string name, int order)
		{
			Name = name;
			Order = order;
		}

		public string Name
		{
			get; private set;
		}

		public int Order
		{
			get; private set;
		}
	}
}
