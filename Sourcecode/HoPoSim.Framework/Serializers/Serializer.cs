using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using Newtonsoft.Json.Schema.Generation;
using System;
using System.Collections.Generic;

namespace HoPoSim.Framework.Serializers
{
	public class Serializer<T>
	{
		public static string ToJSON(T data, bool indented = true)
		{
			return JsonConvert.SerializeObject(data, indented ? Formatting.Indented : Formatting.None);
		}


		public static T FromJSON(string json)
		{
			try
			{
				var instance = JsonConvert.DeserializeObject<T>(json);
				return instance;
			}
			catch
			{
				return default(T);
			}
		}

		public static bool Validate(string input)
		{
			JObject eingabe = JObject.Parse(input);
			return eingabe.IsValid(Schema);
		}

		public static bool Validate(string input, out IList<string> errors)
		{
			try
			{
				JObject eingabe = JObject.Parse(input);
				return eingabe.IsValid(Schema, out errors);
			}
			catch (Exception e)
			{
				errors = new List<string> { e.Message };
				return false;
			}
		}

		internal static JSchema GenerateSchemaForClass()
		{
			JSchemaGenerator jsonSchemaGenerator = new JSchemaGenerator();
			JSchema schema = jsonSchemaGenerator.Generate(typeof(T));
			schema.Title = typeof(T).Name;

			return schema;
		}

		private static JSchema Schema
		{
			get
			{
				if (_schema == null)
					_schema = GenerateSchemaForClass();
				return _schema;
			}
		}
		private static JSchema _schema = null;
	}
}
