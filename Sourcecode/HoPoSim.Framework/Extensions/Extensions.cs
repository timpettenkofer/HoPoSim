using System;
using System.ComponentModel;
using System.Data;
using System.Reflection;

namespace HoPoSim.Framework.Extensions
{
	public static class Extensions
	{
		public static string GetEnumDescription(this Enum enumObj)
		{
			FieldInfo fieldInfo = enumObj.GetType().GetField(enumObj.ToString());

			object[] attribArray = fieldInfo.GetCustomAttributes(false);

			if (attribArray.Length == 0)
			{
				return enumObj.ToString();
			}
			else
			{
				DescriptionAttribute attrib = attribArray[0] as DescriptionAttribute;
				return attrib.Description;
			}
		}

		public static DataTable DeepCopy(this DataTable dt)
		{
			var other = dt.Clone();
			foreach (DataRow dr in dt.Rows)
			{
				other.Rows.Add(dr.ItemArray);
			}
			return other;
		}
	}
}
