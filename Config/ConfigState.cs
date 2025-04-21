using System;
using Newtonsoft.Json;

namespace MoreRealisticLaundering.Config
{
	public class ConfigState
	{
		[JsonConverter(typeof(CleanFloatConverter))]
		public float Laundromat_Cap = 4000f;
		public int Laundromat_Laundering_time_hours = 24;
		public float Laundromat_Tax_Percentage = 19f;

		[JsonConverter(typeof(CleanFloatConverter))]
		public float Taco_Ticklers_Cap = 16000f;
		public int Taco_Ticklers_Laundering_time_hours = 24;
		public float Taco_Ticklers_Tax_Percentage = 19f;

		[JsonConverter(typeof(CleanFloatConverter))]
		public float Car_Wash_Cap = 12000f;
		public int Car_Wash_Laundering_time_hours = 24;
		public float Car_Wash_Tax_Percentage = 19f;

		[JsonConverter(typeof(CleanFloatConverter))]
		public float Post_Office_Cap = 8000f;
		public int Post_Office_Laundering_time_hours = 24;
		public float Post_Office_Tax_Percentage = 19f;
	}
}
