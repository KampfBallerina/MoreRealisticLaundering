using Newtonsoft.Json;

namespace MoreRealisticLaundering.Config
{
	public class ConfigState
	{
		public bool Use_Legit_Version = false;

		public BusinessSettings Businesses { get; set; } = new BusinessSettings();
		public PropertiesSettings Properties { get; set; } = new PropertiesSettings();
		public VehicleSettings Vehicles { get; set; } = new VehicleSettings();
		public SkateboardSettings Skateboards { get; set; } = new SkateboardSettings();
	}

	public class BusinessSettings
	{
		public LaundromatSettings Laundromat { get; set; } = new LaundromatSettings();
		public PostOfficeSettings PostOffice { get; set; } = new PostOfficeSettings();
		public CarWashSettings CarWash { get; set; } = new CarWashSettings();
		public TacoTicklersSettings TacoTicklers { get; set; } = new TacoTicklersSettings();
	}

	public class PropertiesSettings
	{
		public PrivatePropertySettings PrivateProperties { get; set; } = new PrivatePropertySettings();
		public BusinessPropertySettings BusinessProperties { get; set; } = new BusinessPropertySettings();
	}

	public class PrivatePropertySettings
	{
		[JsonConverter(typeof(CleanFloatConverter))]
		public float Storage_Unit_Price = 12000f;
		[JsonConverter(typeof(CleanFloatConverter))]
		public float Motel_Room_Price = 750f;
		[JsonConverter(typeof(CleanFloatConverter))]
		public float Sweatshop_Price = 2500f;
		[JsonConverter(typeof(CleanFloatConverter))]
		public float Bungalow_Price = 10000f;
		[JsonConverter(typeof(CleanFloatConverter))]
		public float Barn_Price = 38000f;
		[JsonConverter(typeof(CleanFloatConverter))]
		public float Docks_Warehouse_Price = 80000f;
		[JsonConverter(typeof(CleanFloatConverter))]
		public float Manor_Price = 250000f;
	}

	public class BusinessPropertySettings
	{
		[JsonConverter(typeof(CleanFloatConverter))]
		public float Laundromat_Price = 10000f;
		[JsonConverter(typeof(CleanFloatConverter))]
		public float Post_Office_Price = 20000f;
		[JsonConverter(typeof(CleanFloatConverter))]
		public float Car_Wash_Price = 30000f;
		[JsonConverter(typeof(CleanFloatConverter))]
		public float Taco_Ticklers_Price = 100000f;
	}

	public class LaundromatSettings
	{
		[JsonConverter(typeof(CleanFloatConverter))]
		public float Laundromat_Cap = 1000f;
		public int Laundromat_Laundering_time_hours = 24;
		[JsonConverter(typeof(CleanFloatConverter))]
		public float Laundromat_Tax_Percentage = 19f;
	}

	public class PostOfficeSettings
	{
		[JsonConverter(typeof(CleanFloatConverter))]
		public float Post_Office_Cap = 2000f;
		public int Post_Office_Laundering_time_hours = 24;
		[JsonConverter(typeof(CleanFloatConverter))]
		public float Post_Office_Tax_Percentage = 19f;
	}

	public class CarWashSettings
	{
		[JsonConverter(typeof(CleanFloatConverter))]
		public float Car_Wash_Cap = 5000f;
		public int Car_Wash_Laundering_time_hours = 24;
		[JsonConverter(typeof(CleanFloatConverter))]
		public float Car_Wash_Tax_Percentage = 19f;
	}

	public class TacoTicklersSettings
	{
		[JsonConverter(typeof(CleanFloatConverter))]
		public float Taco_Ticklers_Cap = 10000f;
		public int Taco_Ticklers_Laundering_time_hours = 24;
		[JsonConverter(typeof(CleanFloatConverter))]
		public float Taco_Ticklers_Tax_Percentage = 19f;
	}

	public class VehicleSettings
	{
		[JsonConverter(typeof(CleanFloatConverter))]
		public float Shitbox_Price = 12800f;
		[JsonConverter(typeof(CleanFloatConverter))]
		public float Veeper_Price = 46999f;
		[JsonConverter(typeof(CleanFloatConverter))]
		public float Bruiser_Price = 25000f;
		[JsonConverter(typeof(CleanFloatConverter))]
		public float Dinkler_Price = 38000f;
		[JsonConverter(typeof(CleanFloatConverter))]
		public float Hounddog_Price = 42000f;
		[JsonConverter(typeof(CleanFloatConverter))]
		public float Cheetah_Price = 120000f;
	}

	public class SkateboardSettings
	{
		[JsonConverter(typeof(CleanFloatConverter))]
		public float Cheap_Skateboard_Price = 800f;
		[JsonConverter(typeof(CleanFloatConverter))]
		public float Skateboard_Price = 1250f;
		[JsonConverter(typeof(CleanFloatConverter))]
		public float Cruiser_Price = 2850f;
		[JsonConverter(typeof(CleanFloatConverter))]
		public float Lightweight_Board_Price = 2850f;
		[JsonConverter(typeof(CleanFloatConverter))]
		public float Golden_Skateboard_Price = 5000f;
	}
}
