using Il2CppScheduleOne.UI.MainMenu;
using MelonLoader;
using MelonLoader.Utils;
using Newtonsoft.Json;
using UnityEngine;

namespace MoreRealisticLaundering.Config
{
    public static class ConfigManager
    {
        private static bool isConfigUpdated = false;
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
                    isConfigUpdated = false;

                    // Ergänze alle Felder aus ConfigState
                    //EnsureFieldExists<bool>(jsonObject, "Use_Legit_Version", configState.Use_Legit_Version);
                    EnsureFieldExists((object)jsonObject, "Use_Legit_Version", configState.Use_Legit_Version);
                    // Businesses
                    EnsureFieldExists((object)jsonObject, "Businesses", configState.Businesses);
                    EnsureFieldExists((object)jsonObject.Businesses, "Laundromat", configState.Businesses.Laundromat);
                    EnsureFieldExists((object)jsonObject.Businesses.Laundromat, "Laundromat_Cap", configState.Businesses.Laundromat.Laundromat_Cap);
                    EnsureFieldExists((object)jsonObject.Businesses.Laundromat, "Laundromat_Laundering_time_hours", configState.Businesses.Laundromat.Laundromat_Laundering_time_hours);
                    EnsureFieldExists((object)jsonObject.Businesses.Laundromat, "Laundromat_Tax_Percentage", configState.Businesses.Laundromat.Laundromat_Tax_Percentage);

                    EnsureFieldExists((object)jsonObject.Businesses, "PostOffice", configState.Businesses.PostOffice);
                    EnsureFieldExists((object)jsonObject.Businesses.PostOffice, "Post_Office_Cap", configState.Businesses.PostOffice.Post_Office_Cap);
                    EnsureFieldExists((object)jsonObject.Businesses.PostOffice, "Post_Office_Laundering_time_hours", configState.Businesses.PostOffice.Post_Office_Laundering_time_hours);
                    EnsureFieldExists((object)jsonObject.Businesses.PostOffice, "Post_Office_Tax_Percentage", configState.Businesses.PostOffice.Post_Office_Tax_Percentage);

                    EnsureFieldExists((object)jsonObject.Businesses, "CarWash", configState.Businesses.CarWash);
                    EnsureFieldExists((object)jsonObject.Businesses.CarWash, "Car_Wash_Cap", configState.Businesses.CarWash.Car_Wash_Cap);
                    EnsureFieldExists((object)jsonObject.Businesses.CarWash, "Car_Wash_Laundering_time_hours", configState.Businesses.CarWash.Car_Wash_Laundering_time_hours);
                    EnsureFieldExists((object)jsonObject.Businesses.CarWash, "Car_Wash_Tax_Percentage", configState.Businesses.CarWash.Car_Wash_Tax_Percentage);

                    EnsureFieldExists((object)jsonObject.Businesses, "TacoTicklers", configState.Businesses.TacoTicklers);
                    EnsureFieldExists((object)jsonObject.Businesses.TacoTicklers, "Taco_Ticklers_Cap", configState.Businesses.TacoTicklers.Taco_Ticklers_Cap);
                    EnsureFieldExists((object)jsonObject.Businesses.TacoTicklers, "Taco_Ticklers_Laundering_time_hours", configState.Businesses.TacoTicklers.Taco_Ticklers_Laundering_time_hours);
                    EnsureFieldExists((object)jsonObject.Businesses.TacoTicklers, "Taco_Ticklers_Tax_Percentage", configState.Businesses.TacoTicklers.Taco_Ticklers_Tax_Percentage);

                    // Properties
                    EnsureFieldExists((object)jsonObject, "Properties", configState.Properties);
                    EnsureFieldExists((object)jsonObject.Properties, "PrivateProperties", configState.Properties.PrivateProperties);
                    EnsureFieldExists((object)jsonObject.Properties.PrivateProperties, "Storage_Unit_Price", configState.Properties.PrivateProperties.Storage_Unit_Price);
                    EnsureFieldExists((object)jsonObject.Properties.PrivateProperties, "Motel_Room_Price", configState.Properties.PrivateProperties.Motel_Room_Price);
                    EnsureFieldExists((object)jsonObject.Properties.PrivateProperties, "Sweatshop_Price", configState.Properties.PrivateProperties.Sweatshop_Price);
                    EnsureFieldExists((object)jsonObject.Properties.PrivateProperties, "Bungalow_Price", configState.Properties.PrivateProperties.Bungalow_Price);
                    EnsureFieldExists((object)jsonObject.Properties.PrivateProperties, "Barn_Price", configState.Properties.PrivateProperties.Barn_Price);
                    EnsureFieldExists((object)jsonObject.Properties.PrivateProperties, "Docks_Warehouse_Price", configState.Properties.PrivateProperties.Docks_Warehouse_Price);
                    EnsureFieldExists((object)jsonObject.Properties.PrivateProperties, "Manor_Price", configState.Properties.PrivateProperties.Manor_Price);

                    EnsureFieldExists((object)jsonObject.Properties, "BusinessProperties", configState.Properties.BusinessProperties);
                    EnsureFieldExists((object)jsonObject.Properties.BusinessProperties, "Laundromat_Price", configState.Properties.BusinessProperties.Laundromat_Price);
                    EnsureFieldExists((object)jsonObject.Properties.BusinessProperties, "Post_Office_Price", configState.Properties.BusinessProperties.Post_Office_Price);
                    EnsureFieldExists((object)jsonObject.Properties.BusinessProperties, "Car_Wash_Price", configState.Properties.BusinessProperties.Car_Wash_Price);
                    EnsureFieldExists((object)jsonObject.Properties.BusinessProperties, "Taco_Ticklers_Price", configState.Properties.BusinessProperties.Taco_Ticklers_Price);

                    // Vehicles
                    EnsureFieldExists((object)jsonObject, "Vehicles", configState.Vehicles);
                    EnsureFieldExists((object)jsonObject.Vehicles, "Shitbox_Price", configState.Vehicles.Shitbox_Price);
                    EnsureFieldExists((object)jsonObject.Vehicles, "Veeper_Price", configState.Vehicles.Veeper_Price);
                    EnsureFieldExists((object)jsonObject.Vehicles, "Bruiser_Price", configState.Vehicles.Bruiser_Price);
                    EnsureFieldExists((object)jsonObject.Vehicles, "Dinkler_Price", configState.Vehicles.Dinkler_Price);
                    EnsureFieldExists((object)jsonObject.Vehicles, "Hounddog_Price", configState.Vehicles.Hounddog_Price);
                    EnsureFieldExists((object)jsonObject.Vehicles, "Cheetah_Price", configState.Vehicles.Cheetah_Price);

                    // Skateboards
                    EnsureFieldExists((object)jsonObject, "Skateboards", configState.Skateboards);
                    EnsureFieldExists((object)jsonObject.Skateboards, "Cheap_Skateboard_Price", configState.Skateboards.Cheap_Skateboard_Price);
                    EnsureFieldExists((object)jsonObject.Skateboards, "Skateboard_Price", configState.Skateboards.Skateboard_Price);
                    EnsureFieldExists((object)jsonObject.Skateboards, "Cruiser_Price", configState.Skateboards.Cruiser_Price);
                    EnsureFieldExists((object)jsonObject.Skateboards, "Lightweight_Board_Price", configState.Skateboards.Lightweight_Board_Price);
                    EnsureFieldExists((object)jsonObject.Skateboards, "Golden_Skateboard_Price", configState.Skateboards.Golden_Skateboard_Price);


                    if (isConfigUpdated)
                    {
                        File.WriteAllText(ConfigManager.FilePath, jsonObject.ToString(Formatting.Indented));
                        MelonLogger.Msg("Config file updated with missing fields.");
                    }

                    ConfigState loadedConfigState = jsonObject.ToObject<ConfigState>();

                    // Check Sleep Settings
                    ValidateConfigSettings(loadedConfigState, configState, ref isConfigUpdated);

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
                    if (isFirstFailure)
                    {
                        MelonLogger.Warning("Failed to read MoreRealisticLaundering config. Recreating config file to fix structure. Please restart the game.");
                        isFirstFailure = false; // Set to false after the first failure

                        // Reset the configuration file
                        ResetConfig();
                        return result = MRLCore.Instance.config = Load(); // Try loading again after resetting
                    }
                    else
                    {
                        MelonLogger.Error("Failed to read MoreRealisticLaundering config. Please check the file structure.");
                        result = configState; // Return the default config state in case of failure
                    }
                }
            }
            return result;
        }

        private static void EnsureFieldExists<T>(dynamic parent, string fieldName, T defaultValue)
        {
            if (parent[fieldName] == null)
            {
                MelonLogger.Warning($"Field '{fieldName}' is missing in the config. Adding default value ({defaultValue}).");
                parent[fieldName] = defaultValue;

                // Setze isConfigUpdated auf true
                isConfigUpdated = true;
            }
            else if (defaultValue is not null && defaultValue.GetType().IsClass && !(defaultValue is string))
            {
                // Rekursive Überprüfung für verschachtelte Objekte
                foreach (var property in defaultValue.GetType().GetProperties())
                {
                    string subFieldName = property.Name;
                    var subDefaultValue = property.GetValue(defaultValue);

                    if (parent[fieldName][subFieldName] == null)
                    {
                        MelonLogger.Warning($"Field '{fieldName}.{subFieldName}' is missing in the config. Adding default value ({subDefaultValue}).");
                        parent[fieldName][subFieldName] = subDefaultValue;

                        // Setze isConfigUpdated auf true
                        isConfigUpdated = true;
                    }
                    else if (subDefaultValue is not null && subDefaultValue.GetType().IsClass && !(subDefaultValue is string))
                    {
                        // Rekursion für tiefere Ebenen
                        EnsureFieldExists(parent[fieldName], subFieldName, subDefaultValue);
                    }
                }
            }
        }

        public static void ResetConfig()
        {
            try
            {
                // Überprüfen, ob die Datei existiert
                if (File.Exists(FilePath))
                {
                    File.Delete(FilePath);
                    MelonLogger.Msg("Existing MoreRealisticLaundering.json file deleted.");
                }

                // Erstellen einer neuen Datei mit Standardwerten
                ConfigState defaultConfig = new ConfigState();
                Save(defaultConfig);

                // Sicherstellen, dass die Datei vollständig geschrieben wurde
                if (!File.Exists(FilePath))
                {
                    throw new Exception("Failed to create the new configuration file.");
                }

                MelonLogger.Msg("New MoreRealisticLaundering.json file created with default values.");
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"Failed to reset MoreRealisticLaundering.json: {ex.Message}");
            }
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
                    return config.Businesses.Laundromat.Laundromat_Laundering_time_hours;

                case "taco_ticklers":
                case "tacoticklers":
                case "taco ticklers":
                    return config.Businesses.TacoTicklers.Taco_Ticklers_Laundering_time_hours;

                case "car_wash":
                case "carwash":
                case "car wash":
                    return config.Businesses.CarWash.Car_Wash_Laundering_time_hours;

                case "post_office":
                case "postoffice":
                case "post office":
                    return config.Businesses.PostOffice.Post_Office_Laundering_time_hours;

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
                    return config.Businesses.Laundromat.Laundromat_Tax_Percentage;

                case "taco_ticklers":
                case "tacoticklers":
                case "taco ticklers":
                    return config.Businesses.TacoTicklers.Taco_Ticklers_Tax_Percentage;

                case "car_wash":
                case "carwash":
                case "car wash":
                    return config.Businesses.CarWash.Car_Wash_Tax_Percentage;

                case "post_office":
                case "postoffice":
                case "post office":
                    return config.Businesses.PostOffice.Post_Office_Tax_Percentage;

                default:
                    MelonLogger.Warning($"Business '{businessName}' not found. Returning default value (19).");
                    return 19f; // Standardwert, falls das Unternehmen nicht gefunden wird
            }
        }

        public static float GetPropertyPrice(ConfigState config, string propertyName)
        {
            switch (propertyName.ToLower())
            {
                case "laundromat":
                    return config.Properties.BusinessProperties.Laundromat_Price;

                case "taco_ticklers":
                case "tacoticklers":
                case "taco ticklers":
                    return config.Properties.BusinessProperties.Taco_Ticklers_Price;

                case "car_wash":
                case "carwash":
                case "car wash":
                    return config.Properties.BusinessProperties.Car_Wash_Price;

                case "post_office":
                case "postoffice":
                case "post office":
                    return config.Properties.BusinessProperties.Post_Office_Price;

                case "donna":
                case "motel_room":
                case "motelroom":
                case "motel room":
                    return config.Properties.PrivateProperties.Motel_Room_Price;

                case "ming":
                case "sweatshop":
                    return config.Properties.PrivateProperties.Sweatshop_Price;

                case "bungalow":
                    return config.Properties.PrivateProperties.Bungalow_Price;

                case "barn":
                    return config.Properties.PrivateProperties.Barn_Price;

                case "docks_warehouse":
                case "dockswarehouse":
                case "docks warehouse":
                    return config.Properties.PrivateProperties.Docks_Warehouse_Price;

                case "manor":
                    return config.Properties.PrivateProperties.Manor_Price;
                case "storage_unit":
                case "storageunit":
                case "storage unit":
                    return config.Properties.PrivateProperties.Storage_Unit_Price;
                default:
                    MelonLogger.Warning($"Property '{propertyName}' not found. Returning default value (1000).");
                    return 1000f; // Default value if the property is not found
            }
        }

        public static float GetVehiclePrice(ConfigState config, string vehicleName)
        {
            switch (vehicleName.ToLower())
            {
                case "shitbox":
                    return config.Vehicles.Shitbox_Price;

                case "veeper":
                    return config.Vehicles.Veeper_Price;

                case "bruiser":
                    return config.Vehicles.Bruiser_Price;

                case "dinkler":
                    return config.Vehicles.Dinkler_Price;

                case "hounddog":
                    return config.Vehicles.Hounddog_Price;

                case "cheetah":
                    return config.Vehicles.Cheetah_Price;

                default:
                    MelonLogger.Warning($"Vehicle '{vehicleName}' not found. Returning default value (1000).");
                    return 1000f; // Default value if the vehicle is not found
            }
        }

        public static float GetSkateboardPrice(ConfigState config, string skateboardName)
        {
            switch (skateboardName.ToLower())
            {
                case "cheap_skateboard":
                case "cheap skateboard":
                    return config.Skateboards.Cheap_Skateboard_Price;

                case "skateboard":
                    return config.Skateboards.Skateboard_Price;

                case "cruiser":
                    return config.Skateboards.Cruiser_Price;

                case "lightweight_skateboard":
                case "lightweight skateboard":
                case "lightweight board":
                case "lightweightboard":
                case "lightweight_board":
                    return config.Skateboards.Lightweight_Board_Price;

                case "golden_skateboard":
                case "golden skateboard":
                    return config.Skateboards.Golden_Skateboard_Price;

                default:
                    MelonLogger.Warning($"Skateboard '{skateboardName}' not found. Returning default value (1000).");
                    return 1000f; // Default value if the skateboard is not found
            }
        }

        private static void ValidateConfigSettings(ConfigState loadedConfigState, ConfigState configState, ref bool isConfigUpdated)
        {
            // Check if Use Legit Version is a boolean
            if (loadedConfigState.Use_Legit_Version != true && loadedConfigState.Use_Legit_Version != false)
            {
                MelonLogger.Warning("Invalid Use_Legit_Version in config. Adding default value (false).");
                loadedConfigState.Use_Legit_Version = configState.Use_Legit_Version;
                isConfigUpdated = true;
            }

            // Überprüfe und aktualisiere die Werte für Laundromat
            if (loadedConfigState.Businesses.Laundromat.Laundromat_Cap <= 0f)
            {
                MelonLogger.Warning("Invalid Laundromat_Cap in config. Reverting to default (1000).");
                loadedConfigState.Businesses.Laundromat.Laundromat_Cap = configState.Businesses.Laundromat.Laundromat_Cap;
                isConfigUpdated = true;
            }
            if (loadedConfigState.Businesses.Laundromat.Laundromat_Laundering_time_hours < 2 || loadedConfigState.Businesses.Laundromat.Laundromat_Laundering_time_hours % 2 != 0)
            {
                MelonLogger.Warning("Invalid Laundromat_Laundering_time_hours in config. Reverting to default (24).");
                loadedConfigState.Businesses.Laundromat.Laundromat_Laundering_time_hours = configState.Businesses.Laundromat.Laundromat_Laundering_time_hours;
                isConfigUpdated = true;
            }
            if (loadedConfigState.Businesses.Laundromat.Laundromat_Tax_Percentage < 0 || loadedConfigState.Businesses.Laundromat.Laundromat_Tax_Percentage > 100)
            {
                MelonLogger.Warning("Invalid Laundromat_Tax_Percentage in config. Reverting to default (19%).");
                loadedConfigState.Businesses.Laundromat.Laundromat_Tax_Percentage = configState.Businesses.Laundromat.Laundromat_Tax_Percentage;
                isConfigUpdated = true;
            }
            if (loadedConfigState.Properties.BusinessProperties.Laundromat_Price < 1000f)
            {
                MelonLogger.Warning("Invalid Laundromat_Price in config. Reverting to default (10000).");
                loadedConfigState.Properties.BusinessProperties.Laundromat_Price = configState.Properties.BusinessProperties.Laundromat_Price;
                isConfigUpdated = true;
            }

            // Überprüfe und aktualisiere die Werte für Taco Ticklers
            if (loadedConfigState.Businesses.TacoTicklers.Taco_Ticklers_Cap <= 0f)
            {
                MelonLogger.Warning("Invalid Taco_Ticklers_Cap in config. Reverting to default (10000).");
                loadedConfigState.Businesses.TacoTicklers.Taco_Ticklers_Cap = configState.Businesses.TacoTicklers.Taco_Ticklers_Cap;
                isConfigUpdated = true;
            }
            if (loadedConfigState.Businesses.TacoTicklers.Taco_Ticklers_Laundering_time_hours < 2 || loadedConfigState.Businesses.TacoTicklers.Taco_Ticklers_Laundering_time_hours % 2 != 0)
            {
                MelonLogger.Warning("Invalid Taco_Ticklers_Laundering_time_hours in config. Reverting to default (24).");
                loadedConfigState.Businesses.TacoTicklers.Taco_Ticklers_Laundering_time_hours = configState.Businesses.TacoTicklers.Taco_Ticklers_Laundering_time_hours;
                isConfigUpdated = true;
            }
            if (loadedConfigState.Businesses.TacoTicklers.Taco_Ticklers_Tax_Percentage < 0 || loadedConfigState.Businesses.TacoTicklers.Taco_Ticklers_Tax_Percentage > 100)
            {
                MelonLogger.Warning("Invalid Taco_Ticklers_Tax_Percentage in config. Reverting to default (19%).");
                loadedConfigState.Businesses.TacoTicklers.Taco_Ticklers_Tax_Percentage = configState.Businesses.TacoTicklers.Taco_Ticklers_Tax_Percentage;
                isConfigUpdated = true;
            }
            if (loadedConfigState.Properties.BusinessProperties.Taco_Ticklers_Price < 1000f)
            {
                MelonLogger.Warning("Invalid Taco_Ticklers_Price in config. Reverting to default (100000).");
                loadedConfigState.Properties.BusinessProperties.Taco_Ticklers_Price = configState.Properties.BusinessProperties.Taco_Ticklers_Price;
                isConfigUpdated = true;
            }

            // Überprüfe und aktualisiere die Werte für Car Wash
            if (loadedConfigState.Businesses.CarWash.Car_Wash_Cap <= 0f)
            {
                loadedConfigState.Businesses.CarWash.Car_Wash_Cap = configState.Businesses.CarWash.Car_Wash_Cap;
                isConfigUpdated = true;
            }
            if (loadedConfigState.Businesses.CarWash.Car_Wash_Laundering_time_hours < 2 || loadedConfigState.Businesses.CarWash.Car_Wash_Laundering_time_hours % 2 != 0)
            {
                MelonLogger.Warning("Invalid Car_Wash_Laundering_time_hours in config. Reverting to default (24).");
                loadedConfigState.Businesses.CarWash.Car_Wash_Laundering_time_hours = configState.Businesses.CarWash.Car_Wash_Laundering_time_hours;
                isConfigUpdated = true;
            }
            if (loadedConfigState.Businesses.CarWash.Car_Wash_Tax_Percentage < 0 || loadedConfigState.Businesses.CarWash.Car_Wash_Tax_Percentage > 100)
            {
                MelonLogger.Warning("Invalid Car_Wash_Tax_Percentage in config. Reverting to default (19%).");
                loadedConfigState.Businesses.CarWash.Car_Wash_Tax_Percentage = configState.Businesses.CarWash.Car_Wash_Tax_Percentage;
                isConfigUpdated = true;
            }
            if (loadedConfigState.Properties.BusinessProperties.Car_Wash_Price < 1000f)
            {
                MelonLogger.Warning("Invalid Car_Wash_Price in config. Reverting to default (30000).");
                loadedConfigState.Properties.BusinessProperties.Car_Wash_Price = configState.Properties.BusinessProperties.Car_Wash_Price;
                isConfigUpdated = true;
            }

            // Überprüfe und aktualisiere die Werte für Post Office
            if (loadedConfigState.Businesses.PostOffice.Post_Office_Cap <= 0f)
            {
                MelonLogger.Warning("Invalid Post_Office_Cap in config. Reverting to default (2000).");
                loadedConfigState.Businesses.PostOffice.Post_Office_Cap = configState.Businesses.PostOffice.Post_Office_Cap;
                isConfigUpdated = true;
            }
            if (loadedConfigState.Businesses.PostOffice.Post_Office_Laundering_time_hours < 2 || loadedConfigState.Businesses.PostOffice.Post_Office_Laundering_time_hours % 2 != 0)
            {
                MelonLogger.Warning("Invalid Post_Office_Laundering_time_hours in config. Reverting to default (24).");
                loadedConfigState.Businesses.PostOffice.Post_Office_Laundering_time_hours = configState.Businesses.PostOffice.Post_Office_Laundering_time_hours;
                isConfigUpdated = true;
            }
            if (loadedConfigState.Businesses.PostOffice.Post_Office_Tax_Percentage < 0 || loadedConfigState.Businesses.PostOffice.Post_Office_Tax_Percentage > 100)
            {
                MelonLogger.Warning("Invalid Post_Office_Tax_Percentage in config. Reverting to default (19%).");
                loadedConfigState.Businesses.PostOffice.Post_Office_Tax_Percentage = configState.Businesses.PostOffice.Post_Office_Tax_Percentage;
                isConfigUpdated = true;
            }
            if (loadedConfigState.Properties.BusinessProperties.Post_Office_Price < 1000f)
            {
                MelonLogger.Warning("Invalid Post_Office_Price in config. Reverting to default (20000).");
                loadedConfigState.Properties.BusinessProperties.Post_Office_Price = configState.Properties.BusinessProperties.Post_Office_Price;
                isConfigUpdated = true;
            }

            // Überprüfe und aktualisiere die Werte für Home Properties
            if (loadedConfigState.Properties.PrivateProperties.Storage_Unit_Price <= 100f)
            {
                MelonLogger.Warning("Invalid Storage_Unit_Price in config. Reverting to default (12000).");
                loadedConfigState.Properties.PrivateProperties.Storage_Unit_Price = configState.Properties.PrivateProperties.Storage_Unit_Price;
                isConfigUpdated = true;
            }
            if (loadedConfigState.Properties.PrivateProperties.Motel_Room_Price < 75f)
            {
                MelonLogger.Warning("Invalid Motel_Room_Price in config. Reverting to default (250).");
                loadedConfigState.Properties.PrivateProperties.Motel_Room_Price = configState.Properties.PrivateProperties.Motel_Room_Price;
                isConfigUpdated = true;
            }
            if (loadedConfigState.Properties.PrivateProperties.Sweatshop_Price <= 100f)
            {
                MelonLogger.Warning("Invalid Sweatshop_Price in config. Reverting to default (2500).");
                loadedConfigState.Properties.PrivateProperties.Sweatshop_Price = configState.Properties.PrivateProperties.Sweatshop_Price;
                isConfigUpdated = true;
            }
            if (loadedConfigState.Properties.PrivateProperties.Bungalow_Price <= 100f)
            {
                MelonLogger.Warning("Invalid Bungalow_Price in config. Reverting to default (10000).");
                loadedConfigState.Properties.PrivateProperties.Bungalow_Price = configState.Properties.PrivateProperties.Bungalow_Price;
                isConfigUpdated = true;
            }
            if (loadedConfigState.Properties.PrivateProperties.Barn_Price <= 100f)
            {
                MelonLogger.Warning("Invalid Barn_Price in config. Reverting to default (38000).");
                loadedConfigState.Properties.PrivateProperties.Barn_Price = configState.Properties.PrivateProperties.Barn_Price;
                isConfigUpdated = true;
            }
            if (loadedConfigState.Properties.PrivateProperties.Docks_Warehouse_Price <= 100f)
            {
                MelonLogger.Warning("Invalid Docks_Warehouse_Price in config. Reverting to default (80000).");
                loadedConfigState.Properties.PrivateProperties.Docks_Warehouse_Price = configState.Properties.PrivateProperties.Docks_Warehouse_Price;
                isConfigUpdated = true;
            }
            if (loadedConfigState.Properties.PrivateProperties.Manor_Price <= 100f)
            {
                MelonLogger.Warning("Invalid Manor_Price in config. Reverting to default (250000).");
                loadedConfigState.Properties.PrivateProperties.Manor_Price = configState.Properties.PrivateProperties.Manor_Price;
                isConfigUpdated = true;
            }

            // Überprüfe und aktualisiere die Werte für Autos
            if (loadedConfigState.Vehicles.Shitbox_Price <= 1000f)
            {
                MelonLogger.Warning("Invalid Shitbox_Price in config. Reverting to default (12800).");
                loadedConfigState.Vehicles.Shitbox_Price = configState.Vehicles.Shitbox_Price;
                isConfigUpdated = true;
            }
            if (loadedConfigState.Vehicles.Veeper_Price <= 1000f)
            {
                MelonLogger.Warning("Invalid Veeper_Price in config. Reverting to default (46999).");
                loadedConfigState.Vehicles.Veeper_Price = configState.Vehicles.Veeper_Price;
                isConfigUpdated = true;
            }
            if (loadedConfigState.Vehicles.Bruiser_Price <= 1000f)
            {
                MelonLogger.Warning("Invalid Bruiser_Price in config. Reverting to default (25000).");
                loadedConfigState.Vehicles.Bruiser_Price = configState.Vehicles.Bruiser_Price;
                isConfigUpdated = true;
            }
            if (loadedConfigState.Vehicles.Dinkler_Price <= 1000f)
            {
                MelonLogger.Warning("Invalid Dinkler_Price in config. Reverting to default (38000).");
                loadedConfigState.Vehicles.Dinkler_Price = configState.Vehicles.Dinkler_Price;
                isConfigUpdated = true;
            }
            if (loadedConfigState.Vehicles.Hounddog_Price <= 1000f)
            {
                MelonLogger.Warning("Invalid Hounddog_Price in config. Reverting to default (42000).");
                loadedConfigState.Vehicles.Hounddog_Price = configState.Vehicles.Hounddog_Price;
                isConfigUpdated = true;
            }
            if (loadedConfigState.Vehicles.Cheetah_Price <= 1000f)
            {
                MelonLogger.Warning("Invalid Cheetah_Price in config. Reverting to default (120000).");
                loadedConfigState.Vehicles.Cheetah_Price = configState.Vehicles.Cheetah_Price;
                isConfigUpdated = true;
            }

            // Überprüfe und aktualisiere die Werte für Skateboards
            if (loadedConfigState.Skateboards.Cheap_Skateboard_Price <= 100f)
            {
                MelonLogger.Warning("Invalid Cheap_Skateboard_Price in config. Reverting to default (800).");
                loadedConfigState.Skateboards.Cheap_Skateboard_Price = configState.Skateboards.Cheap_Skateboard_Price;
                isConfigUpdated = true;
            }
            if (loadedConfigState.Skateboards.Skateboard_Price <= 100f)
            {
                MelonLogger.Warning("Invalid Skateboard_Price in config. Reverting to default (1250).");
                loadedConfigState.Skateboards.Skateboard_Price = configState.Skateboards.Skateboard_Price;
                isConfigUpdated = true;
            }
            if (loadedConfigState.Skateboards.Cruiser_Price <= 100f)
            {
                MelonLogger.Warning("Invalid Cruiser_Price in config. Reverting to default (2850).");
                loadedConfigState.Skateboards.Cruiser_Price = configState.Skateboards.Cruiser_Price;
                isConfigUpdated = true;
            }
            if (loadedConfigState.Skateboards.Lightweight_Board_Price <= 100f)
            {
                MelonLogger.Warning("Invalid Lightweight_Skateboard_Price in config. Reverting to default (2850).");
                loadedConfigState.Skateboards.Lightweight_Board_Price = configState.Skateboards.Lightweight_Board_Price;
                isConfigUpdated = true;
            }
            if (loadedConfigState.Skateboards.Golden_Skateboard_Price <= 100f)
            {
                MelonLogger.Warning("Invalid Golden_Skateboard_Price in config. Reverting to default (5000).");
                loadedConfigState.Skateboards.Golden_Skateboard_Price = configState.Skateboards.Golden_Skateboard_Price;
                isConfigUpdated = true;
            }
        }

        private static bool isFirstFailure = true;
        private static readonly string ConfigFolder = Path.Combine(MelonEnvironment.UserDataDirectory, "MoreRealisticLaundering");
        private static readonly string FilePath = Path.Combine(ConfigFolder, "MoreRealisticLaundering.json");
    }
}