﻿using System.Collections;
using Il2CppScheduleOne.Property;
using MelonLoader;
using UnityEngine;
using Il2CppScheduleOne.Money;
using Il2CppScheduleOne.UI;
using MelonLoader.Utils;
using UnityEngine.UI;
using MoreRealisticLaundering.Util;
using Il2CppTMPro;
using Il2CppScheduleOne.Dialogue;
using MoreRealisticLaundering.Config;
using Il2CppScheduleOne.Vehicles;
using Il2CppScheduleOne.Tools;
using Il2CppScheduleOne.Management.Presets.Options;
using UnityEngine.PlayerLoop;

[assembly: MelonInfo(typeof(MoreRealisticLaundering.MRLCore), "MoreRealisticLaundering", "1.2.3", "KampfBallerina", null)]
[assembly: MelonGame("TVGS", "Schedule I")]

namespace MoreRealisticLaundering
{
    public class MRLCore : MelonMod
    {
        public static MRLCore Instance { get; private set; }
        private bool isFirstStart = true;
        private bool isLegitVersion = false;
        public override void OnInitializeMelon()
        {
            LoggerInstance.Msg("Initialized.");
            MRLCore.Instance = this;
            MRLCore.Instance.maxiumumLaunderValues = new System.Collections.Generic.Dictionary<string, float>();
            MRLCore.Instance.aliasMap = new System.Collections.Generic.Dictionary<string, string>
            {
                { "Laundromat", "Laundromat" },
                { "Taco Ticklers", "Taco Ticklers" },
                { "Car Wash", "Car Wash" },
                { "Post Office", "Post Office" },
                { "PostOffice", "Post Office" },
                { "Motel", "Motel"},
                { "MotelRoom", "Motel"},
                { "Motel Room", "Motel"},
                { "Sweatshop", "Sweatshop"},
                { "Bungalow", "Bungalow"},
                { "Barn", "Barn"},
                { "Docks Warehouse", "Docks Warehouse"},
                { "DocksWarehouse", "Docks Warehouse"},
                { "Manor", "Manor"}
            };
            MRLCore.Instance.vehicleAliasMap = new System.Collections.Generic.Dictionary<string, string>
            {
                { "Shitbox", "Shitbox" },
                { "Shitbox_Police", "Shitbox" },

                { "Veeper", "Veeper"},
                { "Van", "Veeper"},

                { "SUV", "Bruiser"},
                { "SUV_Police", "Bruiser"},
                { "Bruiser", "Bruiser" },

                { "Pickup", "Dinkler"},
                { "Pickup_Police", "Dinkler"},
                { "Dinkler", "Dinkler" },


                { "Hounddog", "Hounddog" },
                { "Sedan", "Hounddog" },

                { "Cheetah", "Cheetah" },
                { "Coupe", "Cheetah"}
            };
            MRLCore.Instance.skateboardAliasMap = new System.Collections.Generic.Dictionary<string, string>
            {
                { "Cheap Skateboard", "Cheap Skateboard" },
                { "CheapSkateboard", "Cheap Skateboard" },

                { "Skateboard", "Skateboard" },
                { "SkateBoard", "Skateboard" },

                { "Cruiser", "Cruiser" },
                { "Cruiser_Skateboard", "Cruiser" },

                { "Lightweight Board", "Lightweight Board" },
                { "LightweightBoard", "Lightweight Board" },
                { "Lightweight Skateboard", "Lightweight Board" },
                { "LightweightSkateboard", "Lightweight Board" },

                { "Golden Skateboard", "Golden Skateboard" },
                { "GoldenSkateboard", "Golden Skateboard" }
            };
        }

        public override void OnSceneWasInitialized(int buildIndex, string sceneName)
        {
            if (sceneName == "Main")
            {
                MRLCore.Instance.config = Config.ConfigManager.Load();
                if (MRLCore.Instance.config.Use_Legit_Version)
                {
                    LoggerInstance.Msg("Use_Legit_Version is enabled. Adjusting behavior accordingly.");
                    isLegitVersion = true;
                }
                else
                {
                    LoggerInstance.Msg("Use_Legit_Version is disabled. Proceeding with default behavior.");
                }
                MRLCore.Instance.FillCapDictionary();
                MRLCore.Instance.InitializeListeners();
                MRLCore.Instance.moneyManager = UnityEngine.Object.FindObjectOfType<MoneyManager>();
                MRLCore.Instance.notificationsManager = UnityEngine.Object.FindObjectOfType<NotificationsManager>();
                MRLCore.Instance.vehicleManager = UnityEngine.Object.FindObjectOfType<VehicleManager>();
                MelonCoroutines.Start(StartCoroutinesAfterDelay());
                isFirstStart = false;
            }
            else if (sceneName.Equals("Menu", StringComparison.OrdinalIgnoreCase) && !isFirstStart)
            {
                ResetAllVariables();
                isFirstStart = false;
            }
        }

        private void InitializeListeners()
        {
            try
            {
                Business.onOperationStarted = new Action<LaunderingOperation>(this.OnLaunderingStarted);
                //LoggerInstance.Msg("Registered 'Laundering Started' Listener.");
                Business.onOperationFinished = new Action<LaunderingOperation>(this.OnLaunderingFinished);
                //LoggerInstance.Msg("Registered 'Laundering Finished' Listener.");
            }
            catch (Exception ex)
            {
                MelonLogger.Error("Failed to register laundering listeners: " + ex.Message);
            }
        }

        public IEnumerator StartCoroutinesAfterDelay()
        {
            bool delayedInit = false;
            while (!delayedInit && !isLegitVersion)
            {
                //  MelonLogger.Msg("Waiting 5 Seconds for other mods to load their apps..");
                delayedInit = true;
                yield return new WaitForSeconds(5f);
            }

            MelonCoroutines.Start(MRLCore.Instance.WaitAndApplyCaps());
            if (!isLegitVersion)
            {
                MelonCoroutines.Start(MRLCore.launderingApp.InitializeLaunderApp());
            }
            MelonCoroutines.Start(MRLCore.Instance.ApplyPropertyConfig());

        }

        private void ResetAllVariables()
        {
            LoggerInstance.Msg("Resetting all variables to default values..");
            // Setze die Konfiguration zurück
            MRLCore.Instance.config = null;
            isLegitVersion = false;

            // Leere die Dictionaries
            MRLCore.Instance.maxiumumLaunderValues.Clear();
            MRLCore.Instance.createdAppEntries.Clear();

            // Leere die HashSets
            MRLCore.Instance.processedBusinesses.Clear();
            MRLCore.Instance.createdBusinessesEntries.Clear();
            MRLCore.Instance.boostedOperations.Clear();

            // Reset Money and Notifications Managers
            MRLCore.Instance.moneyManager = null;
            MRLCore.Instance.notificationsManager = null;

            // Setze die Laundering App zurück
            if (launderingApp != null)
            {
                launderingApp._isLaunderingAppLoaded = false;
                launderingApp.launderingAppViewportContentTransform = null;
                launderingApp.dansHardwareTemplate = null;
                launderingApp.gasMartWestTemplate = null;
                launderingApp.viewPortContentSpaceTemplate = null;
            }

            // Setze die CreditCardIcon zurück
            MRLCore.Instance.CreditCardIcon = null;

            // Entferne alle Listener
            Business.onOperationStarted = null;
            Business.onOperationFinished = null;
        }

        public void ChangeAppIconImage(GameObject appIcon, string ImagePath)
        {
            if (ImagePath == null)
            {
                MelonLogger.Msg("ImagePath is null, skipping image change.");
                return;
            }
            Transform transform = appIcon.transform.Find("Mask/Image");
            GameObject gameObject = (transform != null) ? transform.gameObject : null;
            if (gameObject == null)
            {
                return;
            }
            Image component = gameObject.GetComponent<Image>();
            if (!(component == null))
            {
                Texture2D texture2D = Utils.LoadCustomImage(ImagePath);
                if (!(texture2D == null))
                {
                    Sprite sprite = Sprite.Create(texture2D, new Rect(0f, 0f, texture2D.width, texture2D.height), new Vector2(0.5f, 0.5f));
                    component.sprite = sprite;
                }
                else
                {
                    ((MelonBase)this).LoggerInstance.Msg("Custom image failed to load");
                }
            }
        }

        public void RegisterApp(GameObject App, string Title = "Unknown App")
        {
            GameObject appIcons = GameObject.Find("Player_Local/CameraContainer/Camera/OverlayCamera/GameplayMenu/Phone/phone/HomeScreen/AppIcons");
            App.transform.SetParent(appIcons.transform, worldPositionStays: false);
            ((MelonBase)this).LoggerInstance.Msg("Added " + Title + " to Homescreen.");
        }

        public IEnumerator ApplyPropertyConfig()
        {
            MelonLogger.Msg("Found unowned businesses. Processing...");
            if (!isLegitVersion)
            {
                while (!MRLCore.launderingApp._isLaunderingAppLoaded)
                {
                    yield return new WaitForSeconds(1f);
                }
                string imagePath = Path.Combine(ConfigFolder, "Jeff.png");
                MRLCore.launderingApp.AddEntryFromTemplate("Shred Shack", "Shred Shack", "We're all about the grind, bro — street and money.", null, MRLCore.launderingApp.dansHardwareTemplate, ColorUtil.GetColor("Redpurple"), imagePath, null, true);
                imagePath = Path.Combine(ConfigFolder, "HylandAuto.png");
                MRLCore.launderingApp.AddEntryFromTemplate("Hyland Auto", "Hyland Auto", "We make you drive crazy.", null, MRLCore.launderingApp.dansHardwareTemplate, ColorUtil.GetColor("Dark Green"), imagePath, null, true);
                imagePath = Path.Combine(ConfigFolder, "RaysRealEstate.png");
                MRLCore.launderingApp.AddEntryFromTemplate("Rays Real Estate", "Ray's Real Estate", "No credit check. No judgment. Just opportunity.", null, MRLCore.launderingApp.dansHardwareTemplate, ColorUtil.GetColor("Light Purple"), imagePath, null, true);
            }
            ApplyPricesToProperties();
            ApplyPricesToPropertyListings();
            ApplyPricesToVehicles();
            ApplyPricesToSkateboards();
            yield break;
        }

        public void FillCapDictionary()
        {
            MRLCore.Instance.maxiumumLaunderValues.Clear();
            MRLCore.Instance.maxiumumLaunderValues["Laundromat"] = MRLCore.Instance.config.Businesses.Laundromat.Laundromat_Cap;
            MRLCore.Instance.maxiumumLaunderValues["Taco Ticklers"] = MRLCore.Instance.config.Businesses.TacoTicklers.Taco_Ticklers_Cap;
            MRLCore.Instance.maxiumumLaunderValues["Car Wash"] = MRLCore.Instance.config.Businesses.CarWash.Car_Wash_Cap;
            MRLCore.Instance.maxiumumLaunderValues["PostOffice"] = MRLCore.Instance.config.Businesses.PostOffice.Post_Office_Cap;
            MRLCore.Instance.maxiumumLaunderValues["Post Office"] = MRLCore.Instance.config.Businesses.PostOffice.Post_Office_Cap;
        }

        public readonly System.Collections.Generic.HashSet<string> processedBusinesses = new System.Collections.Generic.HashSet<string>();
        public readonly System.Collections.Generic.HashSet<string> createdBusinessesEntries = new System.Collections.Generic.HashSet<string>();

        public IEnumerator WaitAndApplyCaps()
        {
            while (true)
            {
                if (Business.OwnedBusinesses != null)
                {
                    foreach (Business business in Business.OwnedBusinesses)
                    {
                        if (Business.OwnedBusinesses.Count > 0)
                        {
                            if (business == null)
                            {
                                MelonLogger.Warning("Encountered a null business in OwnedBusinesses. Skipping...");
                                continue;
                            }

                            if (!processedBusinesses.Contains(business.name))
                            {
                                if (MRLCore.Instance.config != null)
                                {
                                    MRLCore.Instance.ApplyCap(business);
                                    processedBusinesses.Add(business.name);
                                    if (!createdBusinessesEntries.Contains(business.name))
                                    {
                                        createdBusinessesEntries.Add(business.name);
                                        MRLCore.Instance.CreateAppEntryForBusiness(business);
                                    }
                                }
                                else
                                {
                                    MelonLogger.Warning("Config is not loaded yet. Skipping ApplyCap for business: " + business.name);
                                }
                            }

                            if (processedBusinesses.Count >= 4)
                            {
                                // LoggerInstance.Msg("All 4 Owned Businesses have been processed. Stopping the search.");
                                isWaitAndApplyCapsRunning = false;
                                yield break;
                            }
                        }
                        else
                        {
                            MelonLogger.Warning("Didn't found OwnedBusinesses yet. Retrying...");
                        }
                    }
                }
                else
                {
                    MelonLogger.Warning("OwnedBusinesses is null. Retrying...");
                }

                yield return new WaitForSeconds(3f);
            }
        }

        public void ApplyCap(Business business)
        {
            if (MRLCore.Instance.aliasMap.TryGetValue(business.name, out string key) && MRLCore.Instance.maxiumumLaunderValues.TryGetValue(key, out float launderCapacity))
            {
                //    MelonLogger.Msg($"Setting Launder Capacity for {business.name} to {launderCapacity}.");
                business.LaunderCapacity = launderCapacity;
            }
        }
        public void CreateAppEntryForBusiness(Business business)
        {
            if (isLegitVersion)
            {
                return;
            }
            // Überprüfe, ob bereits ein AppEntry für dieses Business erstellt wurde
            if (createdAppEntries.Contains(business.name))
            {
                MelonLogger.Warning($"App entry for business '{business.name}' already exists. Skipping creation.");
                return;
            }

            // Überprüfe, ob die Laundering App geladen ist
            if (!MRLCore.launderingApp._isLaunderingAppLoaded)
            {
                MelonCoroutines.Start(WaitForLaunderingAppAndCreateEntry(business));
                return;
            }

            // Überprüfe, ob die Vorlage existiert
            if (MRLCore.launderingApp.dansHardwareTemplate != null)
            {
                // Wähle die richtige Vorlage basierend auf dem Business-Namen
                GameObject template = business.name switch
                {
                    "Laundromat" => MRLCore.launderingApp.dansHardwareTemplate,
                    "Taco Ticklers" => MRLCore.launderingApp.dansHardwareTemplate,
                    "Car Wash" => MRLCore.launderingApp.dansHardwareTemplate,
                    "PostOffice" => MRLCore.launderingApp.dansHardwareTemplate,
                    _ => null
                };

                if (template != null)
                {
                    string displayName = business.name == "PostOffice" ? "Post Office" : business.name;
                    Color color;
                    string imagePath;
                    string subTitle;

                    switch (business.name)
                    {
                        case "Laundromat":
                            color = ColorUtil.GetColor("Yellow");
                            imagePath = Path.Combine(ConfigFolder, "Laundromat.png");
                            subTitle = "Efficient, subtle, and always turning dirty into clean.";
                            break;
                        case "Taco Ticklers":
                            color = ColorUtil.GetColor("Orange");
                            imagePath = Path.Combine(ConfigFolder, "TacoTickler.png");
                            subTitle = "We wrap more than burritos.";
                            break;
                        case "Car Wash":
                            color = ColorUtil.GetColor("Dark Blue");
                            imagePath = Path.Combine(ConfigFolder, "CarWash.png");
                            subTitle = "From powder to polish – all clean.";
                            break;
                        case "PostOffice":
                            color = ColorUtil.GetColor("Light Blue");
                            imagePath = Path.Combine(ConfigFolder, "PostOffice.png");
                            subTitle = "Dead drops, fake names but real profits.";
                            break;
                        default:
                            color = default;
                            imagePath = "";
                            subTitle = "Just a regular business. Nothing to see here.";
                            break;
                    }

                    MRLCore.launderingApp.AddEntryFromTemplate(displayName, displayName, subTitle, business, template, color, imagePath);

                    MelonLogger.Msg($"Created app entry for business: {business.name}");

                    // Füge das Business zur Liste der erstellten AppEntries hinzu
                    createdAppEntries.Add(business.name);
                }
                else
                {
                    MelonLogger.Warning($"No template found for business: {business.name}");
                }
            }
            else
            {
                MelonLogger.Error("Templates are not initialized. Cannot create app entries.");
            }
        }

        private IEnumerator WaitForLaunderingAppAndCreateEntry(Business business)
        {
            while (!MRLCore.launderingApp._isLaunderingAppLoaded)
            {
                yield return new WaitForSeconds(2f); // Warte 2 Sekunde
            }
            CreateAppEntryForBusiness(business);
        }

        public void AdjustLaunderingTime(LaunderingOperation operation)
        {
            if (operation == null || MRLCore.Instance.boostedOperations.Contains(operation))
                return;

            if (MRLCore.Instance.aliasMap.TryGetValue(operation.business.name, out string key))
            {
                int launderingTime = key switch
                {
                    "Laundromat" => MRLCore.Instance.config.Businesses.Laundromat.Laundromat_Laundering_time_hours,
                    "Taco Ticklers" => MRLCore.Instance.config.Businesses.TacoTicklers.Taco_Ticklers_Laundering_time_hours,
                    "Car Wash" => MRLCore.Instance.config.Businesses.CarWash.Car_Wash_Laundering_time_hours,
                    "PostOffice" => MRLCore.Instance.config.Businesses.PostOffice.Post_Office_Laundering_time_hours,
                    "Post Office" => MRLCore.Instance.config.Businesses.PostOffice.Post_Office_Laundering_time_hours,
                    _ => 24 // Standardwert
                };

                operation.completionTime_Minutes = launderingTime * 60;
                string displayName = operation.business.name == "PostOffice" ? "Post Office" : operation.business.name;
                //    MelonLogger.Msg($"Speeding up Laundering Operation: {displayName} Amount: {operation.amount:N0} Completion Time: {operation.completionTime_Minutes}");
                MRLCore.Instance.boostedOperations.Add(operation);
            }
        }

        private void OnLaunderingStarted(LaunderingOperation operation)
        {
            try
            {
                if (operation != null && operation.business != null)
                {
                    string displayName = operation.business.name == "PostOffice" ? "Post Office" : operation.business.name;
                    //    MelonLogger.Msg($"Laundering started: {displayName} {operation.amount:N0} {operation.minutesSinceStarted:N0} minutes since started. {operation.completionTime_Minutes:N0} minutes to complete.");
                }
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"Error in OnLaunderingStarted: {ex.Message}");
            }
        }

        private void OnLaunderingFinished(LaunderingOperation operation)
        {
            try
            {
                if (operation != null && operation.business != null)
                {
                    string displayName = operation.business.name == "PostOffice" ? "Post Office" : operation.business.name;
                    //   MelonLogger.Msg($"Laundering finished: {displayName} {operation.amount:N0} {operation.minutesSinceStarted:N0} minutes since started. {operation.completionTime_Minutes:N0} minutes to complete.");

                    float taxAmount = CalculateTaxes(operation);
                    if (taxAmount > 0)
                    {
                        RemoveTaxesFromBankBalance(taxAmount, operation.business.name);
                    }
                }
            }
            catch (Exception ex)
            {
                LoggerInstance.Error($"Error in OnLaunderingFinished: {ex.Message}");
            }
        }

        private void RemoveTaxesFromBankBalance(float amount, string businessName)
        {
            try
            {
                if (moneyManager != null)
                {
                    string transactionText = $"Tax for {businessName}";
                    moneyManager.CreateOnlineTransaction(transactionText, -amount, 1, transactionText);
                    //    MelonLogger.Msg($"Removed {amount:N0} from Bank Account.");

                    if (notificationsManager != null)
                    {
                        string negativeAmountString = $"<color=#E60000>${amount:N0}</color> Lost in Taxes";
                        string titleString = $"{businessName}";
                        Sprite icon = GetCreditCardIcon();
                        notificationsManager.SendNotification(titleString, negativeAmountString, icon, 5, true);
                    }
                }
            }
            catch (Exception ex)
            {
                LoggerInstance.Error($"An error occurred while deducting bank money: {ex.Message}");
            }
        }

        public float CalculateTaxes(LaunderingOperation operation)
        {
            if (MRLCore.Instance.aliasMap.TryGetValue(operation.business.name, out string key))
            {
                float taxRate = key switch
                {
                    "Laundromat" => MRLCore.Instance.config.Businesses.Laundromat.Laundromat_Tax_Percentage / 100,
                    "Taco Ticklers" => MRLCore.Instance.config.Businesses.TacoTicklers.Taco_Ticklers_Tax_Percentage / 100,
                    "Car Wash" => MRLCore.Instance.config.Businesses.CarWash.Car_Wash_Tax_Percentage / 100,
                    "PostOffice" => MRLCore.Instance.config.Businesses.PostOffice.Post_Office_Tax_Percentage / 100,
                    "Post Office" => MRLCore.Instance.config.Businesses.PostOffice.Post_Office_Tax_Percentage / 100,
                    _ => 0.19f // Standardwert
                };

                float launderedAmount = operation.amount;
                float taxAmount = Mathf.RoundToInt(launderedAmount * taxRate);
                float taxedPayout = launderedAmount - taxAmount;

                string displayName = operation.business.name == "PostOffice" ? "Post Office" : operation.business.name;
                //    MelonLogger.Msg($"Calculating Taxes for {displayName}: Amount: {launderedAmount:N0}, Tax: {taxAmount:N0}, Payout: {taxedPayout:N0}");
                operation.amount = taxedPayout;

                return taxAmount;
            }
            return 0f;
        }

        private Sprite GetCreditCardIcon()
        {
            if (MRLCore.Instance.CreditCardIcon != null)
                return MRLCore.Instance.CreditCardIcon;

            GameObject iconObject = GameObject.Find("UI/HUD/HotbarContainer/Slots/CashSlotContainer/OnlineSlot/ItemUI_Cash/Icon");
            if (iconObject != null)
            {
                UnityEngine.UI.Image image = iconObject.GetComponent<UnityEngine.UI.Image>();
                if (image != null && image.sprite != null && image.sprite.name == "CreditCard")
                {
                    MRLCore.Instance.CreditCardIcon = image.sprite;
                    return image.sprite;
                }
            }
            LoggerInstance.Error("Failed to find the CreditCard icon.");
            return null;
        }

        public void ApplyPricesToProperties()
        {
            if (Business.UnownedBusinesses != null)
            {
                foreach (Business business in Business.UnownedBusinesses)
                {
                    if (business == null)
                    {
                        MelonLogger.Warning("Encountered a null business in UnownedBusinesses. Skipping...");
                        continue;
                    }

                    if (MRLCore.Instance.aliasMap.TryGetValue(business.name, out string key))
                    {
                        float price = key switch
                        {
                            "Laundromat" => MRLCore.Instance.config.Properties.BusinessProperties.Laundromat_Price,
                            "Taco Ticklers" => MRLCore.Instance.config.Properties.BusinessProperties.Taco_Ticklers_Price,
                            "Car Wash" => MRLCore.Instance.config.Properties.BusinessProperties.Car_Wash_Price,
                            "PostOffice" => MRLCore.Instance.config.Properties.BusinessProperties.Post_Office_Price,
                            "Post Office" => MRLCore.Instance.config.Properties.BusinessProperties.Post_Office_Price,
                            _ => 1000f // Standardwert
                        };

                        business.Price = price;
                        //   MelonLogger.Msg($"Set price for unowned business '{business.name}' to {price:C}.");
                    }
                    else
                    {
                        MelonLogger.Warning($"Business '{business.name}' not found in aliasMap. Skipping price assignment.");
                    }
                }
            }
            else
            {
                MelonLogger.Warning("UnownedBusinesses is null. Cannot apply prices.");
            }
            ApplyPricesForHomeProperties();
        }

        public void ApplyPricesForHomeProperties()
        {
            if (Property.UnownedProperties != null)
            {
                foreach (Property property in Property.UnownedProperties)
                {

                    if (property == null)
                    {
                        MelonLogger.Warning("Encountered a null property in UnownedProperties. Skipping...");
                        continue;
                    }

                    if (MRLCore.Instance.aliasMap.TryGetValue(property.name, out string key))
                    {
                        float price = key switch
                        {
                            "Motel Room" => MRLCore.Instance.config.Properties.PrivateProperties.Motel_Room_Price,
                            "Motel" => MRLCore.Instance.config.Properties.PrivateProperties.Motel_Room_Price,
                            "MotelRoom" => MRLCore.Instance.config.Properties.PrivateProperties.Motel_Room_Price,
                            "Sweatshop" => MRLCore.Instance.config.Properties.PrivateProperties.Sweatshop_Price,
                            "Bungalow" => MRLCore.Instance.config.Properties.PrivateProperties.Bungalow_Price,
                            "Barn" => MRLCore.Instance.config.Properties.PrivateProperties.Barn_Price,
                            "Docks Warehouse" => MRLCore.Instance.config.Properties.PrivateProperties.Docks_Warehouse_Price,
                            "Manor" => MRLCore.Instance.config.Properties.PrivateProperties.Manor_Price,
                            _ => 1000f // Standardwert, falls das Business nicht gefunden wird
                        };

                        property.Price = price;
                    }
                    else
                    {
                        MelonLogger.Warning($"Property '{property.name}' / '{property.propertyName}' not found in aliasMap. Skipping price assignment.");
                    }
                }

                if (Property.OwnedProperties != null)
                {
                    foreach (Property property in Property.OwnedProperties)
                    {

                        if (property == null)
                        {
                            MelonLogger.Warning("Encountered a null property in OwnedProperties. Skipping...");
                            continue;
                        }

                        if (MRLCore.Instance.aliasMap.TryGetValue(property.name, out string key))
                        {
                            float price = key switch
                            {
                                "Motel Room" => MRLCore.Instance.config.Properties.PrivateProperties.Motel_Room_Price,
                                "Motel" => MRLCore.Instance.config.Properties.PrivateProperties.Motel_Room_Price,
                                "MotelRoom" => MRLCore.Instance.config.Properties.PrivateProperties.Motel_Room_Price,
                                "Sweatshop" => MRLCore.Instance.config.Properties.PrivateProperties.Sweatshop_Price,
                                "Bungalow" => MRLCore.Instance.config.Properties.PrivateProperties.Bungalow_Price,
                                "Barn" => MRLCore.Instance.config.Properties.PrivateProperties.Barn_Price,
                                "Docks Warehouse" => MRLCore.Instance.config.Properties.PrivateProperties.Docks_Warehouse_Price,
                                "Manor" => MRLCore.Instance.config.Properties.PrivateProperties.Manor_Price,
                                _ => 1000f // Standardwert, falls die Property nicht gefunden wird
                            };

                            property.Price = price;
                        }
                        else
                        {
                            if (property.name == "RV")
                            {
                                // Don't do shit with the RV for other mods
                            }
                            else
                            {
                                MelonLogger.Warning($"Property '{property.name}' not found in aliasMap. Skipping price assignment.");
                            }
                        }
                    }
                }

                //Find Dialogs for Donna and Ming
                DialogueController_Ming[] dialogueControllers = UnityEngine.Object.FindObjectsOfType<DialogueController_Ming>();
                foreach (DialogueController_Ming item in dialogueControllers)
                {
                    string sNPCName = item.gameObject.transform.parent?.name;
                    if (sNPCName != null)
                    {
                        item.Price = ConfigManager.GetPropertyPrice(MRLCore.Instance.config, sNPCName);
                        //  MelonLogger.Msg($"Adjusting {sNPCName}'s Dialogue to {item.Price}.");
                    }
                }
            }
            else
            {
                MelonLogger.Warning("UnownedProperties is null. Cannot apply prices.");
            }
        }


        public void ApplyPricesToPropertyListings()
        {
            // Suche nach dem "RE Office"-Objekt im Spiel
            GameObject reOfficeWhiteboard = GameObject.Find("Map/Container/RE Office/Interior/Whiteboard (1)");
            if (reOfficeWhiteboard == null)
            {
                MelonLogger.Warning("RE Office not found. Cannot apply prices to PropertyListings.");
                return;
            }

            // Suche nach allen PropertyListing-Objekten unter "RE Office"
            Transform[] propertyListings = new Transform[]
            {
                reOfficeWhiteboard.transform.Find("PropertyListing Laundromat"),
                reOfficeWhiteboard.transform.Find("PropertyListing Taco Ticklers"),
                reOfficeWhiteboard.transform.Find("PropertyListing Car Wash"),
                reOfficeWhiteboard.transform.Find("PropertyListing Post Office")
            };

            foreach (Transform propertyListing in propertyListings)
            {
                if (propertyListing == null)
                {
                    MelonLogger.Warning("A PropertyListing was not found. Skipping...");
                    continue;
                }

                // Hole den Namen des PropertyListings
                string listingName = propertyListing.name.Replace("PropertyListing ", "").Trim();

                // Hole den Preis aus der Konfiguration
                if (MRLCore.Instance.aliasMap.TryGetValue(listingName, out string key))
                {
                    float price = key switch
                    {
                        "Laundromat" => MRLCore.Instance.config.Properties.BusinessProperties.Laundromat_Price,
                        "Taco Ticklers" => MRLCore.Instance.config.Properties.BusinessProperties.Taco_Ticklers_Price,
                        "Car Wash" => MRLCore.Instance.config.Properties.BusinessProperties.Car_Wash_Price,
                        "Post Office" => MRLCore.Instance.config.Properties.BusinessProperties.Post_Office_Price,
                        "PostOffice" => MRLCore.Instance.config.Properties.BusinessProperties.Post_Office_Price,
                        _ => 1000f // Standardwert, falls das Business nicht gefunden wird
                    };

                    // Suche nach dem "Price"-Objekt und aktualisiere den Text
                    Transform priceTransform = propertyListing.Find("Price");
                    if (priceTransform != null)
                    {
                        TextMeshPro priceText = priceTransform.GetComponent<TextMeshPro>();
                        if (priceText != null)
                        {
                            priceText.text = $"${price}";
                            //   MelonLogger.Msg($"Set price for {listingName} to {price:C}.");
                        }
                        else
                        {
                            MelonLogger.Warning($"Price TextMeshPro component not found for {listingName}.");
                        }
                    }
                    else
                        MelonLogger.Warning($"Price object not found for {listingName}.");
                }
                else
                {
                    MelonLogger.Warning($"Business '{listingName}' not found in aliasMap. Skipping price assignment.");
                }
            }

            GameObject reOfficeWhiteboardHomes = GameObject.Find("Map/Container/RE Office/Interior/Whiteboard");
            if (reOfficeWhiteboardHomes == null)
            {
                MelonLogger.Warning("RE Office not found. Cannot apply prices to PropertyListings.");
                return;
            }

            // Suche nach allen PropertyListing-Objekten unter "RE Office"
            Transform[] homePropertyListings = new Transform[]
            {
                reOfficeWhiteboardHomes.transform.Find("PropertyListing Motel Room"),
                reOfficeWhiteboardHomes.transform.Find("PropertyListing Sweatshop"),
                reOfficeWhiteboardHomes.transform.Find("PropertyListing Bungalow"),
                reOfficeWhiteboardHomes.transform.Find("PropertyListing Barn"),
                reOfficeWhiteboardHomes.transform.Find("PropertyListing Docks Warehouse"),
                reOfficeWhiteboardHomes.transform.Find("PropertyListing Manor")
            };

            foreach (Transform homePropertyListing in homePropertyListings)
            {
                if (homePropertyListing == null)
                {
                    //MelonLogger.Warning("A PropertyListing was not found. Skipping...");
                    continue;
                }

                // Hole den Namen des PropertyListings
                string listingName = homePropertyListing.name.Replace("PropertyListing ", "").Trim();

                // Hole den Preis aus der Konfiguration
                if (MRLCore.Instance.aliasMap.TryGetValue(listingName, out string key))
                {
                    float price = key switch
                    {
                        "Motel Room" => MRLCore.Instance.config.Properties.PrivateProperties.Motel_Room_Price,
                        "Sweatshop" => MRLCore.Instance.config.Properties.PrivateProperties.Sweatshop_Price,
                        "Bungalow" => MRLCore.Instance.config.Properties.PrivateProperties.Bungalow_Price,
                        "Barn" => MRLCore.Instance.config.Properties.PrivateProperties.Barn_Price,
                        "Docks Warehouse" => MRLCore.Instance.config.Properties.PrivateProperties.Docks_Warehouse_Price,
                        "Manor" => MRLCore.Instance.config.Properties.PrivateProperties.Manor_Price,
                        _ => 1000f // Standardwert, falls das Business nicht gefunden wird
                    };

                    // Suche nach dem "Price"-Objekt und aktualisiere den Text
                    Transform priceTransform = homePropertyListing.Find("Price");
                    if (priceTransform != null)
                    {
                        TextMeshPro priceText = priceTransform.GetComponent<TextMeshPro>();
                        if (priceText != null)
                        {
                            priceText.text = $"${price}";
                            //   MelonLogger.Msg($"Set price for {listingName} to {price:C}.");
                        }
                        else
                        {
                            MelonLogger.Warning($"Price TextMeshPro component not found for {listingName}.");
                        }
                    }
                    else
                        MelonLogger.Warning($"Price object not found for {listingName}.");
                }
                else
                {
                    MelonLogger.Warning($"Property '{listingName}' not found in aliasMap. Skipping price assignment.");
                }
            }
            UpdateSellSignPrices();
        }

        private void UpdateSellSignPrices()
        {
            // Suche nach den Verkaufsschildern
            GameObject sellSignPostOffice = GameObject.Find("@Businesses/PostOffice/ForSaleSign_Blue (1)");
            GameObject sellSignCarWash = GameObject.Find("@Businesses/Car Wash/ForSaleSign_Blue");
            GameObject sellSignLaundromat = GameObject.Find("@Businesses/Laundromat/ForSaleSign_Blue (1)");
            GameObject sellSignTacoTicklers = GameObject.Find("@Businesses/Taco Ticklers/ForSaleSign_Blue (1)");

            GameObject sellSignMotelRoom = GameObject.Find("@Properties/MotelRoom/ForSaleSign");
            GameObject sellSignSweatshop = GameObject.Find("@Properties/Sweatshop/ForSaleSign");
            GameObject sellSignBungalow = GameObject.Find("@Properties/Bungalow/ForSaleSign");
            GameObject sellSignBarn = GameObject.Find("@Properties/Barn/ForSaleSign");
            GameObject sellSignDocksWarehouse = GameObject.Find("@Properties/DocksWarehouse/ForSaleSign");
            GameObject sellSignManor = GameObject.Find("@Properties/Manor/ForSaleSign (1)");

            // Aktualisiere die Preise für jedes Schild
            UpdateSignPrice(sellSignPostOffice, "Post Office");
            UpdateSignPrice(sellSignCarWash, "Car Wash");
            UpdateSignPrice(sellSignLaundromat, "Laundromat");
            UpdateSignPrice(sellSignTacoTicklers, "Taco Ticklers");

            UpdateSignPrice(sellSignMotelRoom, "Motel Room");
            UpdateSignPrice(sellSignSweatshop, "Sweatshop");
            UpdateSignPrice(sellSignBungalow, "Bungalow");
            UpdateSignPrice(sellSignBarn, "Barn");
            UpdateSignPrice(sellSignDocksWarehouse, "Docks Warehouse");
            UpdateSignPrice(sellSignManor, "Manor");
            // MelonLogger.Msg("Updated all sell sign prices.");
        }

        public void ApplyPricesToVehicles()
        {

            if (vehicleManager == null)
            {
                MelonLogger.Warning("VehicleManager not found. Cannot apply prices to vehicles.");
                return;
            }

            // Setze die Preise für die Fahrzeuge
            foreach (LandVehicle vehicle in vehicleManager.VehiclePrefabs)
            {
                if (vehicle == null)
                {
                    MelonLogger.Warning("Encountered a null vehicle. Skipping...");
                    continue;
                }
                // MelonLogger.Msg($"{vehicle.name}: {vehicle.vehiclePrice}, {vehicle.vehicleCode}");
                if (MRLCore.Instance.vehicleAliasMap.TryGetValue(vehicle.name, out string key))
                {
                    float price = key switch
                    {
                        "Shitbox" => MRLCore.Instance.config.Vehicles.Shitbox_Price,
                        "Veeper" => MRLCore.Instance.config.Vehicles.Veeper_Price,
                        "Bruiser" => MRLCore.Instance.config.Vehicles.Bruiser_Price,
                        "Dinkler" => MRLCore.Instance.config.Vehicles.Dinkler_Price,
                        "Hounddog" => MRLCore.Instance.config.Vehicles.Hounddog_Price,
                        "Cheetah" => MRLCore.Instance.config.Vehicles.Cheetah_Price,
                        _ => 1000f // Standardwert, falls das Fahrzeug nicht gefunden wird
                    };
                    vehicle.vehiclePrice = price;
                    // MelonLogger.Msg($"Changed to - {vehicle.name}: {vehicle.vehiclePrice}, {vehicle.vehicleCode}");
                }
                else
                {
                    MelonLogger.Warning($"Vehicle '{vehicle.name}' not found in vehicleAliasMap. Skipping price assignment.");

                }
            }
            UpdateVehicleSignPrices();
        }

        private void UpdateVehicleSignPrices()
        {
            VehicleSaleSign vehicleSaleSign = GameObject.FindObjectOfType<VehicleSaleSign>();
            if (vehicleSaleSign != null)
            {
                VehicleSaleSign[] allVehicleSaleSigns = GameObject.FindObjectsOfType<VehicleSaleSign>();
                foreach (VehicleSaleSign sign in allVehicleSaleSigns)
                {
                    TextMeshPro nameLabel = sign.NameLabel;
                    TextMeshPro priceLabel = sign.PriceLabel;

                    if (nameLabel != null && priceLabel != null)
                    {
                        if (MRLCore.Instance.vehicleAliasMap.TryGetValue(nameLabel.text, out string key))
                        {
                            float price = key switch
                            {
                                "Shitbox" => MRLCore.Instance.config.Vehicles.Shitbox_Price,
                                "Veeper" => MRLCore.Instance.config.Vehicles.Veeper_Price,
                                "Bruiser" => MRLCore.Instance.config.Vehicles.Bruiser_Price,
                                "Dinkler" => MRLCore.Instance.config.Vehicles.Dinkler_Price,
                                "Hounddog" => MRLCore.Instance.config.Vehicles.Hounddog_Price,
                                "Cheetah" => MRLCore.Instance.config.Vehicles.Cheetah_Price,
                                _ => 1000f // Default fallback price
                            };
                            priceLabel.text = $"${price}";
                        }
                        else
                        {
                            MelonLogger.Warning($"Vehicle '{nameLabel.text}' not found in vehicleAliasMap. Skipping price update.");
                        }
                    }
                    else
                    {
                        MelonLogger.Warning("NameLabel or PriceLabel not found for a VehicleSaleSign.");
                    }
                }
            }
            else
            {
                MelonLogger.Warning("No VehicleSaleSign objects found in the scene.");
            }
        }

        public void ApplyPricesToSkateboards()
        {
            DialogueController_SkateboardSeller[] skateboardDialogueControllers = UnityEngine.Object.FindObjectsOfType<DialogueController_SkateboardSeller>();
            foreach (DialogueController_SkateboardSeller dialogController in skateboardDialogueControllers)
            {
                string sNPCName = dialogController.gameObject.transform.parent?.name;
                if (sNPCName != null)
                {
                    // MelonLogger.Msg($"Adjusting {sNPCName}'s Dialogue.");
                    if (sNPCName.Contains("Jeff"))
                    {
                        shackShopDialogueController = dialogController;
                        foreach (DialogueController_SkateboardSeller.Option option in dialogController.Options)
                        {
                            // MelonLogger.Msg($"Option: {option.Name} - Price: {option.Price}");
                            if (MRLCore.Instance.skateboardAliasMap.TryGetValue(option.Name, out string key))
                            {
                                float price = key switch
                                {
                                    "Cheap Skateboard" => MRLCore.Instance.config.Skateboards.Cheap_Skateboard_Price,
                                    "Skateboard" => MRLCore.Instance.config.Skateboards.Skateboard_Price,
                                    "Cruiser" => MRLCore.Instance.config.Skateboards.Cruiser_Price,
                                    "Lightweight Board" => MRLCore.Instance.config.Skateboards.Lightweight_Board_Price,
                                    "Golden Skateboard" => MRLCore.Instance.config.Skateboards.Golden_Skateboard_Price,
                                    _ => 1000f // Standardwert, falls das Skateboard nicht gefunden wird
                                };
                                option.Price = price;
                            }
                            else
                            {
                                MelonLogger.Warning($"Skateboard '{option.Name}' not found in skateBoardAliasMap. Skipping price assignment.");
                            }
                        }
                        UpdateSkateboardSign();
                    }
                }
            }
        }

        private void UpdateSkateboardSign()
        {
            GameObject sellSignCheapSkateboard = GameObject.Find("Map/Container/North town/Jeff's Shred Shack/Shred Shack/Catalog/Paper (1)/Text (TMP)");
            GameObject sellSignSkateboard = GameObject.Find("Map/Container/North town/Jeff's Shred Shack/Shred Shack/Catalog/Paper (2)/Text (TMP)");
            GameObject sellSignCruiser = GameObject.Find("Map/Container/North town/Jeff's Shred Shack/Shred Shack/Catalog/Paper (3)/Text (TMP)");
            GameObject sellSignLightweightBoard = GameObject.Find("Map/Container/North town/Jeff's Shred Shack/Shred Shack/Catalog/Paper (4)/Text (TMP)");
            GameObject sellSignGoldenSkateboard = GameObject.Find("Map/Container/North town/Jeff's Shred Shack/Shred Shack/Catalog/Paper (5)/Text (TMP)");

            // Aktualisiere die Preise für jedes Schild
            UpdateSignPrice(sellSignCheapSkateboard, "Cheap Skateboard");
            UpdateSignPrice(sellSignSkateboard, "Skateboard");
            UpdateSignPrice(sellSignCruiser, "Cruiser_Skateboard");
            UpdateSignPrice(sellSignLightweightBoard, "Lightweight Board");
            UpdateSignPrice(sellSignGoldenSkateboard, "Golden Skateboard");
            // MelonLogger.Msg("Updated all skateboard sign prices.");
        }

        private void UpdateSignPrice(GameObject sellSign, string objectName)
        {
            if (sellSign == null)
            {
                MelonLogger.Warning($"Sell sign for {objectName} not found. Skipping...");
                return;
            }

            // Hole den Preis aus der Konfiguration
            float price;
            if (MRLCore.Instance.aliasMap.TryGetValue(objectName, out string key))
            {
                price = key switch
                {
                    "Laundromat" => MRLCore.Instance.config.Properties.BusinessProperties.Laundromat_Price,
                    "Taco Ticklers" => MRLCore.Instance.config.Properties.BusinessProperties.Taco_Ticklers_Price,
                    "Car Wash" => MRLCore.Instance.config.Properties.BusinessProperties.Car_Wash_Price,
                    "Post Office" => MRLCore.Instance.config.Properties.BusinessProperties.Post_Office_Price,
                    "PostOffice" => MRLCore.Instance.config.Properties.BusinessProperties.Post_Office_Price,
                    "Motel" => MRLCore.Instance.config.Properties.PrivateProperties.Motel_Room_Price,
                    "MotelRoom" => MRLCore.Instance.config.Properties.PrivateProperties.Motel_Room_Price,
                    "Motel Room" => MRLCore.Instance.config.Properties.PrivateProperties.Motel_Room_Price,
                    "Sweatshop" => MRLCore.Instance.config.Properties.PrivateProperties.Sweatshop_Price,
                    "Bungalow" => MRLCore.Instance.config.Properties.PrivateProperties.Bungalow_Price,
                    "Barn" => MRLCore.Instance.config.Properties.PrivateProperties.Barn_Price,
                    "Docks Warehouse" => MRLCore.Instance.config.Properties.PrivateProperties.Docks_Warehouse_Price,
                    "Manor" => MRLCore.Instance.config.Properties.PrivateProperties.Manor_Price,
                    _ => 1000f
                };
            }
            else if (MRLCore.Instance.vehicleAliasMap.TryGetValue(objectName, out string vehicleKey))
            {
                price = vehicleKey switch
                {
                    "Shitbox" => MRLCore.Instance.config.Vehicles.Shitbox_Price,
                    "Veeper" => MRLCore.Instance.config.Vehicles.Veeper_Price,
                    "Bruiser" => MRLCore.Instance.config.Vehicles.Bruiser_Price,
                    "Dinkler" => MRLCore.Instance.config.Vehicles.Dinkler_Price,
                    "Hounddog" => MRLCore.Instance.config.Vehicles.Hounddog_Price,
                    "Cheetah" => MRLCore.Instance.config.Vehicles.Cheetah_Price,
                    _ => 1000f
                };
            }
            else if (MRLCore.Instance.skateboardAliasMap.TryGetValue(objectName, out string skateboardKey))
            {
                price = skateboardKey switch
                {
                    "Cheap Skateboard" => MRLCore.Instance.config.Skateboards.Cheap_Skateboard_Price,
                    "Skateboard" => MRLCore.Instance.config.Skateboards.Skateboard_Price,
                    "Cruiser" => MRLCore.Instance.config.Skateboards.Cruiser_Price,
                    "Lightweight Board" => MRLCore.Instance.config.Skateboards.Lightweight_Board_Price,
                    "Golden Skateboard" => MRLCore.Instance.config.Skateboards.Golden_Skateboard_Price,
                    _ => 1000f
                };
            }
            else
            {
                MelonLogger.Warning($"Property/Vehicle '{objectName}' not found in aliasMap, vehicleAliasMap or skateboardAliasMap. Skipping sell sign price assignment.");
                return;
            }

            // Is it a Skateboard?
            if (objectName.ToLower().Contains("board"))
            {
                TextMeshPro priceText = sellSign.GetComponent<TextMeshPro>();
                if (priceText != null)
                {
                    // Keeping the first line of the text and adding the price to the second line with green color
                    string[] lines = priceText.text.Split('\n');
                    priceText.text = lines[0];
                    priceText.text += $"\n<color=#55C61E>${price}</color>";
                }
                else
                {
                    MelonLogger.Warning($"Price TextMeshPro component not found for sell sign of {objectName}.");
                }
            }
            else
            {
                // Suche nach dem "Price"-Objekt und aktualisiere den Text
                Transform priceTransform = sellSign.transform.Find("Price");
                if (priceTransform != null)
                {
                    TextMeshPro priceText = priceTransform.GetComponent<TextMeshPro>();
                    if (priceText != null)
                    {
                        priceText.text = $"${price}";
                        // MelonLogger.Msg($"Updated sell sign price for {objectName} to ${price}.");
                    }
                    else
                    {
                        MelonLogger.Warning($"Price TextMeshPro component not found for sell sign of {objectName}.");
                    }
                }
                else
                {
                    MelonLogger.Warning($"Price object not found for sell sign of {objectName}.");
                }
            }
        }

        public Sprite CreditCardIcon;
        public Config.ConfigState config;
        public static PhoneApp.LaunderingApp launderingApp = new PhoneApp.LaunderingApp();
        public System.Collections.Generic.Dictionary<string, float> maxiumumLaunderValues;
        public System.Collections.Generic.Dictionary<string, string> aliasMap;
        public System.Collections.Generic.Dictionary<string, string> vehicleAliasMap;
        public System.Collections.Generic.Dictionary<string, string> skateboardAliasMap;
        private readonly System.Collections.Generic.HashSet<string> createdAppEntries = new System.Collections.Generic.HashSet<string>();
        public bool isWaitAndApplyCapsRunning = false;
        private static readonly string ConfigFolder = Path.Combine(MelonEnvironment.UserDataDirectory, "MoreRealisticLaundering");
        private readonly System.Collections.Generic.HashSet<LaunderingOperation> boostedOperations = new System.Collections.Generic.HashSet<LaunderingOperation>();
        public DialogueController_SkateboardSeller shackShopDialogueController;
        public MoneyManager moneyManager;
        public NotificationsManager notificationsManager;
        public VehicleManager vehicleManager;
    }
}