using MelonLoader;
using MelonLoader.Utils;
using Newtonsoft.Json;

namespace MoreRealisticLaundering.Config
{
    public static class ConfigManager
    {
        public static ConfigState Load()
        {
            ConfigState configState = new ConfigState();

            if (!Directory.Exists(ConfigFolder))
            {
                Directory.CreateDirectory(ConfigFolder);
            }

            bool isConfigFileMissing = !File.Exists(ConfigManager.FilePath);
            ConfigState result;
            if (isConfigFileMissing)
            {
                MelonLogger.Warning("Config file not found. Creating a new one with default values.");
                ConfigManager.Save(configState);
                result = configState;
            }
            else
            {
                try
                {
                    string fileContent = File.ReadAllText(ConfigManager.FilePath);
                    dynamic jsonObject = JsonConvert.DeserializeObject<dynamic>(fileContent) ?? new ConfigState();
                    bool isConfigUpdated = false;

                    void EnsureFieldExists<T>(string fieldName, T defaultValue)
                    {
                        if (jsonObject[fieldName] == null)
                        {
                            MelonLogger.Warning($"Field '{fieldName}' is missing in the config. Adding default value ({defaultValue}).");
                            jsonObject[fieldName] = defaultValue;
                            isConfigUpdated = true;
                        }
                    }

                    // Ergänze alle Felder aus ConfigState
                    EnsureFieldExists("Use_Legit_Version", configState.Use_Legit_Version);

                    EnsureFieldExists("Laundromat_Cap", configState.Laundromat_Cap);
                    EnsureFieldExists("Laundromat_Laundering_time_hours", configState.Laundromat_Laundering_time_hours);
                    EnsureFieldExists("Laundromat_Tax_Percentage", configState.Laundromat_Tax_Percentage);

                    EnsureFieldExists("Post_Office_Cap", configState.Post_Office_Cap);
                    EnsureFieldExists("Post_Office_Laundering_time_hours", configState.Post_Office_Laundering_time_hours);
                    EnsureFieldExists("Post_Office_Tax_Percentage", configState.Post_Office_Tax_Percentage);

                    EnsureFieldExists("Car_Wash_Cap", configState.Car_Wash_Cap);
                    EnsureFieldExists("Car_Wash_Laundering_time_hours", configState.Car_Wash_Laundering_time_hours);
                    EnsureFieldExists("Car_Wash_Tax_Percentage", configState.Car_Wash_Tax_Percentage);

                    EnsureFieldExists("Taco_Ticklers_Cap", configState.Taco_Ticklers_Cap);
                    EnsureFieldExists("Taco_Ticklers_Laundering_time_hours", configState.Taco_Ticklers_Laundering_time_hours);
                    EnsureFieldExists("Taco_Ticklers_Tax_Percentage", configState.Taco_Ticklers_Tax_Percentage);

                    EnsureFieldExists("Laundromat_Price", configState.Laundromat_Price);
                    EnsureFieldExists("Post_Office_Price", configState.Post_Office_Price);
                    EnsureFieldExists("Car_Wash_Price", configState.Car_Wash_Price);
                    EnsureFieldExists("Taco_Ticklers_Price", configState.Taco_Ticklers_Price);

                    EnsureFieldExists("Motel_Room_Price", configState.Motel_Room_Price);
                    EnsureFieldExists("Sweatshop_Price", configState.Sweatshop_Price);
                    EnsureFieldExists("Bungalow_Price", configState.Bungalow_Price);
                    EnsureFieldExists("Barn_Price", configState.Barn_Price);
                    EnsureFieldExists("Docks_Warehouse_Price", configState.Docks_Warehouse_Price);
                    EnsureFieldExists("Manor_Price", configState.Manor_Price);

                    EnsureFieldExists("Shitbox_Price", configState.Shitbox_Price);
                    EnsureFieldExists("Veeper_Price", configState.Veeper_Price);
                    EnsureFieldExists("Bruiser_Price", configState.Bruiser_Price);
                    EnsureFieldExists("Dinkler_Price", configState.Dinkler_Price);
                    EnsureFieldExists("Hounddog_Price", configState.Hounddog_Price);
                    EnsureFieldExists("Cheetah_Price", configState.Cheetah_Price);

                    EnsureFieldExists("Cheap_Skateboard_Price", configState.Cheap_Skateboard_Price);
                    EnsureFieldExists("Skateboard_Price", configState.Skateboard_Price);
                    EnsureFieldExists("Cruiser_Price", configState.Cruiser_Price);
                    EnsureFieldExists("Lightweight_Board_Price", configState.Lightweight_Board_Price);
                    EnsureFieldExists("Golden_Skateboard_Price", configState.Golden_Skateboard_Price);

                    if (isConfigUpdated)
                    {
                        File.WriteAllText(ConfigManager.FilePath, jsonObject.ToString(Formatting.Indented));
                        MelonLogger.Msg("Config file updated with missing fields.");
                    }

                    ConfigState loadedConfigState = jsonObject.ToObject<ConfigState>();

                    // Check if Use Legit Version is a boolean
                    if (loadedConfigState.Use_Legit_Version != true && loadedConfigState.Use_Legit_Version != false)
                    {
                        MelonLogger.Warning("Invalid Use_Legit_Version in config. Adding default value (false).");
                        loadedConfigState.Use_Legit_Version = configState.Use_Legit_Version;
                        isConfigUpdated = true;
                    }

                    // Überprüfe und aktualisiere die Werte für Laundromat
                    if (loadedConfigState.Laundromat_Cap <= 0f)
                    {
                        MelonLogger.Warning("Invalid Laundromat_Cap in config. Reverting to default (1000).");
                        loadedConfigState.Laundromat_Cap = configState.Laundromat_Cap;
                        isConfigUpdated = true;
                    }
                    if (loadedConfigState.Laundromat_Laundering_time_hours < 2 || loadedConfigState.Laundromat_Laundering_time_hours % 2 != 0)
                    {
                        MelonLogger.Warning("Invalid Laundromat_Laundering_time_hours in config. Reverting to default (24).");
                        loadedConfigState.Laundromat_Laundering_time_hours = configState.Laundromat_Laundering_time_hours;
                        isConfigUpdated = true;
                    }
                    if (loadedConfigState.Laundromat_Tax_Percentage < 0 || loadedConfigState.Laundromat_Tax_Percentage > 100)
                    {
                        MelonLogger.Warning("Invalid Laundromat_Tax_Percentage in config. Reverting to default (19%).");
                        loadedConfigState.Laundromat_Tax_Percentage = configState.Laundromat_Tax_Percentage;
                        isConfigUpdated = true;
                    }
                    if (loadedConfigState.Laundromat_Price < 1000f)
                    {
                        MelonLogger.Warning("Invalid Laundromat_Price in config. Reverting to default (10000).");
                        loadedConfigState.Laundromat_Price = configState.Laundromat_Price;
                        isConfigUpdated = true;
                    }

                    // Überprüfe und aktualisiere die Werte für Taco Ticklers
                    if (loadedConfigState.Taco_Ticklers_Cap <= 0f)
                    {
                        MelonLogger.Warning("Invalid Taco_Ticklers_Cap in config. Reverting to default (10000).");
                        loadedConfigState.Taco_Ticklers_Cap = configState.Taco_Ticklers_Cap;
                        isConfigUpdated = true;
                    }
                    if (loadedConfigState.Taco_Ticklers_Laundering_time_hours < 2 || loadedConfigState.Taco_Ticklers_Laundering_time_hours % 2 != 0)
                    {
                        MelonLogger.Warning("Invalid Taco_Ticklers_Laundering_time_hours in config. Reverting to default (24).");
                        loadedConfigState.Taco_Ticklers_Laundering_time_hours = configState.Taco_Ticklers_Laundering_time_hours;
                        isConfigUpdated = true;
                    }
                    if (loadedConfigState.Taco_Ticklers_Tax_Percentage < 0 || loadedConfigState.Taco_Ticklers_Tax_Percentage > 100)
                    {
                        MelonLogger.Warning("Invalid Taco_Ticklers_Tax_Percentage in config. Reverting to default (19%).");
                        loadedConfigState.Taco_Ticklers_Tax_Percentage = configState.Taco_Ticklers_Tax_Percentage;
                        isConfigUpdated = true;
                    }
                    if (loadedConfigState.Taco_Ticklers_Price < 1000f)
                    {
                        MelonLogger.Warning("Invalid Taco_Ticklers_Price in config. Reverting to default (100000).");
                        loadedConfigState.Taco_Ticklers_Price = configState.Taco_Ticklers_Price;
                        isConfigUpdated = true;
                    }

                    // Überprüfe und aktualisiere die Werte für Car Wash
                    if (loadedConfigState.Car_Wash_Cap <= 0f)
                    {
                        loadedConfigState.Car_Wash_Cap = configState.Car_Wash_Cap;
                        isConfigUpdated = true;
                    }
                    if (loadedConfigState.Car_Wash_Laundering_time_hours < 2 || loadedConfigState.Car_Wash_Laundering_time_hours % 2 != 0)
                    {
                        MelonLogger.Warning("Invalid Car_Wash_Laundering_time_hours in config. Reverting to default (24).");
                        loadedConfigState.Car_Wash_Laundering_time_hours = configState.Car_Wash_Laundering_time_hours;
                        isConfigUpdated = true;
                    }
                    if (loadedConfigState.Car_Wash_Tax_Percentage < 0 || loadedConfigState.Car_Wash_Tax_Percentage > 100)
                    {
                        MelonLogger.Warning("Invalid Car_Wash_Tax_Percentage in config. Reverting to default (19%).");
                        loadedConfigState.Car_Wash_Tax_Percentage = configState.Car_Wash_Tax_Percentage;
                        isConfigUpdated = true;
                    }
                    if (loadedConfigState.Car_Wash_Price < 1000f)
                    {
                        MelonLogger.Warning("Invalid Car_Wash_Price in config. Reverting to default (30000).");
                        loadedConfigState.Car_Wash_Price = configState.Car_Wash_Price;
                        isConfigUpdated = true;
                    }

                    // Überprüfe und aktualisiere die Werte für Post Office
                    if (loadedConfigState.Post_Office_Cap <= 0f)
                    {
                        MelonLogger.Warning("Invalid Post_Office_Cap in config. Reverting to default (2000).");
                        loadedConfigState.Post_Office_Cap = configState.Post_Office_Cap;
                        isConfigUpdated = true;
                    }
                    if (loadedConfigState.Post_Office_Laundering_time_hours < 2 || loadedConfigState.Post_Office_Laundering_time_hours % 2 != 0)
                    {
                        MelonLogger.Warning("Invalid Post_Office_Laundering_time_hours in config. Reverting to default (24).");
                        loadedConfigState.Post_Office_Laundering_time_hours = configState.Post_Office_Laundering_time_hours;
                        isConfigUpdated = true;
                    }
                    if (loadedConfigState.Post_Office_Tax_Percentage < 0 || loadedConfigState.Post_Office_Tax_Percentage > 100)
                    {
                        MelonLogger.Warning("Invalid Post_Office_Tax_Percentage in config. Reverting to default (19%).");
                        loadedConfigState.Post_Office_Tax_Percentage = configState.Post_Office_Tax_Percentage;
                        isConfigUpdated = true;
                    }
                    if (loadedConfigState.Post_Office_Price < 1000f)
                    {
                        MelonLogger.Warning("Invalid Post_Office_Price in config. Reverting to default (20000).");
                        loadedConfigState.Post_Office_Price = configState.Post_Office_Price;
                        isConfigUpdated = true;
                    }

                    //Überprüfe und aktualisiere die Werte für Home Properties
                    if (loadedConfigState.Motel_Room_Price <= 100f)
                    {
                        MelonLogger.Warning("Invalid Motel_Room_Price in config. Reverting to default (750).");
                        loadedConfigState.Motel_Room_Price = configState.Motel_Room_Price;
                        isConfigUpdated = true;
                    }
                    if (loadedConfigState.Sweatshop_Price <= 100f)
                    {
                        MelonLogger.Warning("Invalid Sweatshop_Price in config. Reverting to default (2500).");
                        loadedConfigState.Sweatshop_Price = configState.Sweatshop_Price;
                        isConfigUpdated = true;
                    }
                    if (loadedConfigState.Bungalow_Price <= 100f)
                    {
                        MelonLogger.Warning("Invalid Bungalow_Price in config. Reverting to default (10000).");
                        loadedConfigState.Bungalow_Price = configState.Bungalow_Price;
                        isConfigUpdated = true;
                    }
                    if (loadedConfigState.Barn_Price <= 100f)
                    {
                        MelonLogger.Warning("Invalid Barn_Price in config. Reverting to default (38000).");
                        loadedConfigState.Barn_Price = configState.Barn_Price;
                        isConfigUpdated = true;
                    }
                    if (loadedConfigState.Docks_Warehouse_Price <= 100f)
                    {
                        MelonLogger.Warning("Invalid Docks_Warehouse_Price in config. Reverting to default (80000).");
                        loadedConfigState.Docks_Warehouse_Price = configState.Docks_Warehouse_Price;
                        isConfigUpdated = true;
                    }
                    if (loadedConfigState.Manor_Price <= 100f)
                    {
                        MelonLogger.Warning("Invalid Manor_Price in config. Reverting to default (250000).");
                        loadedConfigState.Manor_Price = configState.Manor_Price;
                        isConfigUpdated = true;
                    }

                    // Überprüfe und aktualisiere die Werte für Autos
                    if (loadedConfigState.Shitbox_Price <= 1000f)
                    {
                        MelonLogger.Warning("Invalid Shitbox_Price in config. Reverting to default (12800).");
                        loadedConfigState.Shitbox_Price = configState.Shitbox_Price;
                        isConfigUpdated = true;
                    }
                    if (loadedConfigState.Veeper_Price <= 1000f)
                    {
                        MelonLogger.Warning("Invalid Veeper_Price in config. Reverting to default (46999).");
                        loadedConfigState.Veeper_Price = configState.Veeper_Price;
                        isConfigUpdated = true;
                    }
                    if (loadedConfigState.Bruiser_Price <= 1000f)
                    {
                        MelonLogger.Warning("Invalid Bruiser_Price in config. Reverting to default (25000).");
                        loadedConfigState.Bruiser_Price = configState.Bruiser_Price;
                        isConfigUpdated = true;
                    }
                    if (loadedConfigState.Dinkler_Price <= 1000f)
                    {
                        MelonLogger.Warning("Invalid Dinkler_Price in config. Reverting to default (38000).");
                        loadedConfigState.Dinkler_Price = configState.Dinkler_Price;
                        isConfigUpdated = true;
                    }
                    if (loadedConfigState.Hounddog_Price <= 1000f)
                    {
                        MelonLogger.Warning("Invalid Hounddog_Price in config. Reverting to default (42000).");
                        loadedConfigState.Hounddog_Price = configState.Hounddog_Price;
                        isConfigUpdated = true;
                    }
                    if (loadedConfigState.Cheetah_Price <= 1000f)
                    {
                        MelonLogger.Warning("Invalid Cheetah_Price in config. Reverting to default (120000).");
                        loadedConfigState.Cheetah_Price = configState.Cheetah_Price;
                        isConfigUpdated = true;
                    }

                    // Überprüfe und aktualisiere die Werte für Skateboards
                    if (loadedConfigState.Cheap_Skateboard_Price <= 100f)
                    {
                        MelonLogger.Warning("Invalid Cheap_Skateboard_Price in config. Reverting to default (800).");
                        loadedConfigState.Cheap_Skateboard_Price = configState.Cheap_Skateboard_Price;
                        isConfigUpdated = true;
                    }
                    if (loadedConfigState.Skateboard_Price <= 100f)
                    {
                        MelonLogger.Warning("Invalid Skateboard_Price in config. Reverting to default (1250).");
                        loadedConfigState.Skateboard_Price = configState.Skateboard_Price;
                        isConfigUpdated = true;
                    }
                    if (loadedConfigState.Cruiser_Price <= 100f)
                    {
                        MelonLogger.Warning("Invalid Cruiser_Price in config. Reverting to default (2850).");
                        loadedConfigState.Cruiser_Price = configState.Cruiser_Price;
                        isConfigUpdated = true;
                    }
                    if (loadedConfigState.Lightweight_Board_Price <= 100f)
                    {
                        MelonLogger.Warning("Invalid Lightweight_Skateboard_Price in config. Reverting to default (2850).");
                        loadedConfigState.Lightweight_Board_Price = configState.Lightweight_Board_Price;
                        isConfigUpdated = true;
                    }
                    if (loadedConfigState.Golden_Skateboard_Price <= 100f)
                    {
                        MelonLogger.Warning("Invalid Golden_Skateboard_Price in config. Reverting to default (5000).");
                        loadedConfigState.Golden_Skateboard_Price = configState.Golden_Skateboard_Price;
                        isConfigUpdated = true;
                    }

                    // Speichere die aktualisierte Konfiguration, falls Änderungen vorgenommen wurden
                    if (isConfigUpdated)
                    {
                        ConfigManager.Save(loadedConfigState);
                        isConfigUpdated = false; // Reset after saving
                    }
                    result = loadedConfigState;
                }
                catch
                {
                    MelonLogger.Error("Failed to load MoreRealisticLaundering config. Using defaults.");
                    result = configState;
                }
            }
            return result;
        }

        public static void Save(ConfigState config)
        {
            try
            {
                string contents = JsonConvert.SerializeObject(config, Newtonsoft.Json.Formatting.Indented);
                File.WriteAllText(ConfigManager.FilePath, contents);
                MelonLogger.Msg("Configuration saved successfully.");
            }
            catch (Exception ex)
            {
                MelonLogger.Error("Failed to save MoreRealisticLaundering config: " + ex.Message);
            }
        }

        public static int GetLaunderingTimeHours(ConfigState config, string businessName)
        {

            switch (businessName.ToLower())
            {
                case "laundromat":
                    return config.Laundromat_Laundering_time_hours;

                case "taco_ticklers":
                case "tacoticklers":
                case "taco ticklers":
                    return config.Taco_Ticklers_Laundering_time_hours;

                case "car_wash":
                case "carwash":
                case "car wash":
                    return config.Car_Wash_Laundering_time_hours;

                case "post_office":
                case "postoffice":
                case "post office":
                    return config.Post_Office_Laundering_time_hours;

                default:
                    MelonLogger.Warning($"Business '{businessName}' not found. Returning default value (24).");
                    return 24; // Standardwert, falls das Unternehmen nicht gefunden wird
            }
        }

        public static float GetTaxationPercentage(ConfigState config, string businessName)
        {

            switch (businessName.ToLower())
            {
                case "laundromat":
                    return config.Laundromat_Tax_Percentage;

                case "taco_ticklers":
                case "tacoticklers":
                case "taco ticklers":
                    return config.Taco_Ticklers_Tax_Percentage;

                case "car_wash":
                case "carwash":
                case "car wash":
                    return config.Car_Wash_Tax_Percentage;

                case "post_office":
                case "postoffice":
                case "post office":
                    return config.Post_Office_Tax_Percentage;

                default:
                    MelonLogger.Warning($"Business '{businessName}' not found. Returning default value (24).");
                    return 19f; // Standardwert, falls das Unternehmen nicht gefunden wird
            }
        }

        public static float GetPropertyPrice(ConfigState config, string propertyName)
        {
            switch (propertyName.ToLower())
            {
                case "laundromat":
                    return config.Laundromat_Price;

                case "taco_ticklers":
                case "tacoticklers":
                case "taco ticklers":
                    return config.Taco_Ticklers_Price;

                case "car_wash":
                case "carwash":
                case "car wash":
                    return config.Car_Wash_Price;

                case "post_office":
                case "postoffice":
                case "post office":
                    return config.Post_Office_Price;

                case "donna":
                case "motel_room":
                case "motelroom":
                case "motel room":
                    return config.Motel_Room_Price;

                case "ming":
                case "sweatshop":
                    return config.Sweatshop_Price;

                case "bungalow":
                    return config.Bungalow_Price;

                case "barn":
                    return config.Barn_Price;

                case "docks_warehouse":
                case "dockswarehouse":
                case "docks warehouse":
                    return config.Docks_Warehouse_Price;

                case "manor":
                    return config.Manor_Price;

                default:
                    MelonLogger.Warning($"Property/Business '{propertyName}' not found. Returning default value (1000).");
                    return 1000f; // Standardwert, falls das Property/Business nicht gefunden wird
            }
        }

        public static float GetVehiclePrice(ConfigState config, string vehicleName)
        {
            switch (vehicleName.ToLower())
            {
                case "shitbox":
                    return config.Shitbox_Price;

                case "bruiser":
                    return config.Bruiser_Price;

                case "dinkler":
                    return config.Dinkler_Price;

                case "hounddog":
                    return config.Hounddog_Price;

                case "cheetah":
                    return config.Cheetah_Price;

                default:
                    MelonLogger.Warning($"Vehicle '{vehicleName}' not found. Returning default value (1000).");
                    return 1000f; // Standardwert, falls das Fahrzeug nicht gefunden wird
            }
        }

        public static float GetSkateboardPrice(ConfigState config, string skateboardName)
        {
            switch (skateboardName.ToLower())
            {
                case "cheap_skateboard":
                case "cheap skateboard":
                    return config.Cheap_Skateboard_Price;

                case "skateboard":
                    return config.Skateboard_Price;

                case "cruiser":
                    return config.Cruiser_Price;

                case "lightweight_skateboard":
                case "lightweight skateboard":
                case "lightweight board":
                case "lightweightboard":
                case "lightweight_board":
                    return config.Lightweight_Board_Price;

                case "golden_skateboard":
                case "golden skateboard":
                    return config.Golden_Skateboard_Price;

                default:
                    MelonLogger.Warning($"Skateboard '{skateboardName}' not found. Returning default value (1000).");
                    return 1000f; // Standardwert, falls das Skateboard nicht gefunden wird
            }
        }

        private static readonly string ConfigFolder = Path.Combine(MelonEnvironment.UserDataDirectory, "MoreRealisticLaundering");
        private static readonly string FilePath = Path.Combine(ConfigFolder, "MoreRealisticLaundering.json");
    }
}