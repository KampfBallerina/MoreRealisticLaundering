using System;
using System.Collections.Generic;
using HarmonyLib;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppScheduleOne.Property;
using Il2CppScheduleOne.UI;
using Il2CppTMPro;
using MelonLoader;

namespace MoreRealisticLaundering.Patches
{
    [HarmonyPatch(typeof(LaunderingInterface), "Open")]
    public class LaunderingInterface_Open_Patch
    {
        // Dictionary zur Speicherung der geänderten Werte für jedes Unternehmen
        private static readonly Dictionary<string, Dictionary<string, string>> patchedValues = new Dictionary<string, Dictionary<string, string>>();

        public static void Postfix(LaunderingInterface __instance)
        {
            if (__instance == null || __instance.business == null)
                return;

            // Hole den Namen des Geschäfts
            string businessName = __instance.business.name;

            // Initialisiere das Dictionary für das Unternehmen, falls es noch nicht existiert
            if (!patchedValues.ContainsKey(businessName))
            {
                patchedValues[businessName] = new Dictionary<string, string>();
            }

            // Aktualisiere die Launder Capacity
            if (MRLCore.Instance.maxiumumLaunderValues.TryGetValue(businessName, out float capacity))
            {
                __instance.launderCapacityLabel.text = capacity.ToString("N0");
            }

            // Aktualisiere die Laundering Time
            Il2CppArrayBase<TextMeshProUGUI> componentsInChildren = __instance.GetComponentsInChildren<TextMeshProUGUI>(true);
            foreach (TextMeshProUGUI textMeshProUGUI in componentsInChildren)
            {
                if (textMeshProUGUI.text == "12 Hrs" || textMeshProUGUI.text == "24 Hrs")
                {
                    int launderingTime = businessName switch
                    {
                        "Laundromat" => MRLCore.Instance.config.Laundromat_Laundering_time_hours,
                        "Taco Ticklers" => MRLCore.Instance.config.Taco_Ticklers_Laundering_time_hours,
                        "Car Wash" => MRLCore.Instance.config.Car_Wash_Laundering_time_hours,
                        "PostOffice" => MRLCore.Instance.config.Post_Office_Laundering_time_hours,
                        _ => 24 // Standardwert
                    };

                    // Speichere die Werte für "12 Hrs" und "24 Hrs"
                    if (textMeshProUGUI.text == "12 Hrs")
                    {
                        int halfTime = launderingTime / 2; // Nutze die Hälfte der Zeit
                        if (textMeshProUGUI.text != $"{halfTime} Hrs")
                        {
                            textMeshProUGUI.text = $"{halfTime} Hrs";
                            patchedValues[businessName]["12Hrs"] = $"{halfTime} Hrs";
                        }
                    }
                    else if (textMeshProUGUI.text == "24 Hrs")
                    {
                        if (textMeshProUGUI.text != $"{launderingTime} Hrs")
                        {
                            textMeshProUGUI.text = $"{launderingTime} Hrs";
                            patchedValues[businessName]["24Hrs"] = $"{launderingTime} Hrs";
                        }
                    }
                }
                else
                {
                    TryPatchedValues(__instance);
                }
            }
        }

        // Methode zum erneuten Patchen der gespeicherten Werte
        public static void TryPatchedValues(LaunderingInterface launderingInterface)
        {
            if (launderingInterface == null || launderingInterface.business == null)
                return;

            string businessName = launderingInterface.business.name;

            if (!patchedValues.ContainsKey(businessName))
                return;

            if (patchedValues[businessName].TryGetValue("12Hrs", out string twelveHrs) &&
                patchedValues[businessName].TryGetValue("24Hrs", out string twentyFourHrs))
            {
                Il2CppArrayBase<TextMeshProUGUI> componentsInChildren = launderingInterface.GetComponentsInChildren<TextMeshProUGUI>(true);
                foreach (TextMeshProUGUI textMeshProUGUI in componentsInChildren)
                    if (textMeshProUGUI.text.Equals(twelveHrs, StringComparison.OrdinalIgnoreCase))
                    {
                        int launderingTime = businessName switch
                        {
                            "Laundromat" => MRLCore.Instance.config.Laundromat_Laundering_time_hours,
                            "Taco Ticklers" => MRLCore.Instance.config.Taco_Ticklers_Laundering_time_hours,
                            "Car Wash" => MRLCore.Instance.config.Car_Wash_Laundering_time_hours,
                            "PostOffice" => MRLCore.Instance.config.Post_Office_Laundering_time_hours,
                            _ => 24 // Standardwert
                        };
                        if (launderingTime.ToString() == twelveHrs)
                        {
                            return;
                        }

                        if (textMeshProUGUI.text == twelveHrs)
                        {
                            int halfTime = launderingTime / 2; // Nutze die Hälfte der Zeit
                            if (textMeshProUGUI.text != $"{halfTime} Hrs")
                            {
                                textMeshProUGUI.text = $"{halfTime} Hrs";
                                patchedValues[businessName]["12Hrs"] = $"{halfTime} Hrs";
                            }
                        }
                    }
                    else if (textMeshProUGUI.text.Equals(twentyFourHrs, StringComparison.OrdinalIgnoreCase))
                    {
                        int launderingTime = businessName switch
                        {
                            "Laundromat" => MRLCore.Instance.config.Laundromat_Laundering_time_hours,
                            "Taco Ticklers" => MRLCore.Instance.config.Taco_Ticklers_Laundering_time_hours,
                            "Car Wash" => MRLCore.Instance.config.Car_Wash_Laundering_time_hours,
                            "PostOffice" => MRLCore.Instance.config.Post_Office_Laundering_time_hours,
                            _ => 24 // Standardwert
                        };
                        if (launderingTime.ToString() == twentyFourHrs)
                        {
                            return;
                        }

                        if (textMeshProUGUI.text == twelveHrs)
                        {
                            int halfTime = launderingTime / 2; // Nutze die Hälfte der Zeit
                            if (textMeshProUGUI.text != $"{halfTime} Hrs")
                            {
                                textMeshProUGUI.text = $"{halfTime} Hrs";
                                patchedValues[businessName]["12Hrs"] = $"{halfTime} Hrs";
                            }
                        }
                        else if (textMeshProUGUI.text == twentyFourHrs)
                        {
                            if (textMeshProUGUI.text != $"{launderingTime} Hrs")
                            {
                                textMeshProUGUI.text = $"{launderingTime} Hrs";
                                patchedValues[businessName]["24Hrs"] = $"{launderingTime} Hrs";
                            }
                        }
                    }
            }
        }
    }
}
