using System;
using System.IO;
using Il2CppInterop.Common;
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
                    ConfigState loadedConfigState = JsonConvert.DeserializeObject<ConfigState>(fileContent) ?? new ConfigState();
                    bool isConfigUpdated = false;

                    // Überprüfe und aktualisiere die Werte für Laundromat
                    if (loadedConfigState.Laundromat_Cap <= 0f)
                    {
                        loadedConfigState.Laundromat_Cap = configState.Laundromat_Cap;
                        isConfigUpdated = true;
                    }
                    if (loadedConfigState.Laundromat_Laundering_time_hours < 2 || loadedConfigState.Laundromat_Laundering_time_hours % 2 != 0)
                    {
                        MelonLogger.Warning("Invalid Laundromat_Laundering_time_hours in config. Reverting to default (24).");
                        loadedConfigState.Laundromat_Laundering_time_hours = configState.Laundromat_Laundering_time_hours;
                        isConfigUpdated = true;
                    }
                    if (loadedConfigState.Laundromat_Tax_Percentage <= 0 || loadedConfigState.Laundromat_Tax_Percentage > 100)
                    {
                        MelonLogger.Warning("Invalid Laundromat_Tax_Percentage in config. Reverting to default (19%).");
                        loadedConfigState.Laundromat_Tax_Percentage = configState.Laundromat_Tax_Percentage;
                        isConfigUpdated = true;
                    }

                    // Überprüfe und aktualisiere die Werte für Taco Ticklers
                    if (loadedConfigState.Taco_Ticklers_Cap <= 0f)
                    {
                        loadedConfigState.Taco_Ticklers_Cap = configState.Taco_Ticklers_Cap;
                        isConfigUpdated = true;
                    }
                    if (loadedConfigState.Taco_Ticklers_Laundering_time_hours < 2 || loadedConfigState.Taco_Ticklers_Laundering_time_hours % 2 != 0)
                    {
                        MelonLogger.Warning("Invalid Taco_Ticklers_Laundering_time_hours in config. Reverting to default (24).");
                        loadedConfigState.Taco_Ticklers_Laundering_time_hours = configState.Taco_Ticklers_Laundering_time_hours;
                        isConfigUpdated = true;
                    }
                    if (loadedConfigState.Taco_Ticklers_Tax_Percentage <= 0 || loadedConfigState.Taco_Ticklers_Tax_Percentage > 100)
                    {
                        MelonLogger.Warning("Invalid Taco_Ticklers_Tax_Percentage in config. Reverting to default (19%).");
                        loadedConfigState.Taco_Ticklers_Tax_Percentage = configState.Taco_Ticklers_Tax_Percentage;
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
                    if (loadedConfigState.Car_Wash_Tax_Percentage <= 0 || loadedConfigState.Car_Wash_Tax_Percentage > 100)
                    {
                        MelonLogger.Warning("Invalid Car_Wash_Tax_Percentage in config. Reverting to default (19%).");
                        loadedConfigState.Car_Wash_Tax_Percentage = configState.Car_Wash_Tax_Percentage;
                        isConfigUpdated = true;
                    }

                    // Überprüfe und aktualisiere die Werte für Post Office
                    if (loadedConfigState.Post_Office_Cap <= 0f)
                    {
                        loadedConfigState.Post_Office_Cap = configState.Post_Office_Cap;
                        isConfigUpdated = true;
                    }
                    if (loadedConfigState.Post_Office_Laundering_time_hours < 2 || loadedConfigState.Post_Office_Laundering_time_hours % 2 != 0)
                    {
                        MelonLogger.Warning("Invalid Post_Office_Laundering_time_hours in config. Reverting to default (24).");
                        loadedConfigState.Post_Office_Laundering_time_hours = configState.Post_Office_Laundering_time_hours;
                        isConfigUpdated = true;
                    }
                    if (loadedConfigState.Post_Office_Tax_Percentage <= 0 || loadedConfigState.Post_Office_Tax_Percentage > 100)
                    {
                        MelonLogger.Warning("Invalid Post_Office_Tax_Percentage in config. Reverting to default (19%).");
                        loadedConfigState.Post_Office_Tax_Percentage = configState.Post_Office_Tax_Percentage;
                        isConfigUpdated = true;
                    }

                    // Speichere die aktualisierte Konfiguration, falls Änderungen vorgenommen wurden
                    if (isConfigUpdated)
                    {
                        ConfigManager.Save(loadedConfigState);
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
              //  MelonLogger.Msg("Configuration saved successfully.");
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

        private static readonly string ConfigFolder = Path.Combine(MelonEnvironment.UserDataDirectory, "MoreRealisticLaundering");
        private static readonly string FilePath = Path.Combine(ConfigFolder, "MoreRealisticLaundering.json");
    }
}