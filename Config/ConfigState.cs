using System;
using Newtonsoft.Json;

namespace MoreRealisticLaundering.Config
{
	public class ConfigState
	{
		public bool Use_Legit_Version = false;

		[JsonConverter(typeof(CleanFloatConverter))]
		public float Laundromat_Cap = 1000f;
		public int Laundromat_Laundering_time_hours = 24;
		public float Laundromat_Tax_Percentage = 19f;
		public float Laundromat_Price = 10000f;

		[JsonConverter(typeof(CleanFloatConverter))]
		public float Taco_Ticklers_Cap = 10000f;
		public int Taco_Ticklers_Laundering_time_hours = 24;
		public float Taco_Ticklers_Tax_Percentage = 19f;
		public float Taco_Ticklers_Price = 100000f;

		[JsonConverter(typeof(CleanFloatConverter))]
		public float Car_Wash_Cap = 5000f;
		public int Car_Wash_Laundering_time_hours = 24;
		public float Car_Wash_Tax_Percentage = 19f;
		public float Car_Wash_Price = 30000f;

		[JsonConverter(typeof(CleanFloatConverter))]
		public float Post_Office_Cap = 2000f;
		public int Post_Office_Laundering_time_hours = 24;
		public float Post_Office_Tax_Percentage = 19f;
		public float Post_Office_Price = 20000f;

		[JsonConverter(typeof(CleanFloatConverter))]
		public float Motel_Room_Price = 750f;
		public float Sweatshop_Price = 2500f;
		public float Bungalow_Price = 10000f;
		public float Barn_Price = 38000f;
		public float Docks_Warehouse_Price = 80000f;
		public float Manor_Price = 250000f;

		[JsonConverter(typeof(CleanFloatConverter))]
		public float Shitbox_Price = 12800f;
		public float Veeper_Price = 46999f;
		public float Bruiser_Price = 25000f;
		public float Dinkler_Price = 38000f;
		public float Hounddog_Price = 42000f;
		public float Cheetah_Price = 120000f;
	}
}
