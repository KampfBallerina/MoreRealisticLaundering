using System;
using System.Globalization;
using Newtonsoft.Json;

namespace MoreRealisticLaundering.Config
{

	public class CleanFloatConverter : JsonConverter
	{
		public override bool CanConvert(Type objectType)
		{
			return objectType == typeof(float);
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			return Convert.ToSingle(reader.Value);
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			writer.WriteRawValue(((float)value).ToString("0.##", CultureInfo.InvariantCulture));
		}
	}
}
