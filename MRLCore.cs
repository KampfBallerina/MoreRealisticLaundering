﻿using System.Collections;
using Il2CppScheduleOne.Property;
using MelonLoader;
using UnityEngine;
using Il2CppScheduleOne.Money;
using Il2CppScheduleOne.UI;
using MelonLoader.Utils;
using Il2CppInterop.Runtime;

using UnityEngine.UI;
using MoreRealisticLaundering.Util;
using MoreRealisticLaundering.PhoneApp;
using Il2CppTMPro;

[assembly: MelonInfo(typeof(MoreRealisticLaundering.MRLCore), "MoreRealisticLaundering", "1.1.2", "KampfBallerina", null)]
[assembly: MelonGame("TVGS", "Schedule I")]

namespace MoreRealisticLaundering
{
    public class MRLCore : MelonMod
    {
        public static MRLCore Instance { get; private set; }
        private bool isFirstStart = true;

        private bool isLegitVersion = false;

        private ScrollRect HomeScreenScrollRect;

        private GameObject IconsPageTemplate;

        private GameObject CompatabilityIconsPage;

        private GameObject HomeScreenScrollRectContent;

        private HorizontalLayoutGroup HomeScreenHorizontalLayoutGroup;

        public List<GameObject> UIAppPages = new List<GameObject>();

        public List<List<GameObject>> AllApps = new List<List<GameObject>>();

        private int currentAppPage;

        private float snapOffset = -30f;

        private float snapDuration = 0.15f;

        private float snapVelocity;

        private float snapVelocityThreshold = 200f;

        private bool isInitialSnapDone = false;

        private RectTransform contentRect;

        private bool isDraggingBlocked;

        private float mouseDownTime;

        private Vector2 mouseDownPosition;

        private float minHoldDuration = 0.15f;

        private float dragThreshold = 15f;

        public bool IsHomescreenLoaded = false;

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
                { "PostOffice", "Post Office" }
            };
        }

        public override void OnUpdate()
        {
            if (isLegitVersion)
            {
                return;
            }
            if (!IsHomescreenLoaded)
            {
                return;
            }
            if (contentRect == null)
            {
                if (HomeScreenScrollRectContent == null)
                {
                    return;
                }
                else
                {
                    contentRect = HomeScreenScrollRectContent.GetComponent<RectTransform>();
                }
                if (contentRect == null)
                {
                    return;
                }
            }
            if (UIAppPages == null || UIAppPages.Count <= 0)
            {
                return;
            }
            HomeScreenScrollRect.enabled = UIAppPages.Count > 1;
            if (!HomeScreenScrollRect.enabled)
            {
                if (!isInitialSnapDone)
                {
                    SnapImmediately(0);
                    isInitialSnapDone = true;
                }
                return;
            }
            if (Input.GetMouseButtonDown(0))
            {
                mouseDownTime = Time.time;
                mouseDownPosition = Input.mousePosition;
                isDraggingBlocked = true;
            }
            if (Input.GetMouseButton(0) && isDraggingBlocked)
            {
                float num = Vector2.Distance(Input.mousePosition, mouseDownPosition);
                if (Time.time - mouseDownTime >= minHoldDuration || num >= dragThreshold)
                {
                    isDraggingBlocked = false;
                }
            }
            if (isDraggingBlocked && HomeScreenScrollRect.m_Dragging)
            {
                HomeScreenScrollRect.velocity = Vector2.zero;
                SnapImmediately(currentAppPage);
                HomeScreenScrollRect.StopMovement();
            }
            if (!isInitialSnapDone)
            {
                SnapImmediately(0);
                isInitialSnapDone = true;
                return;
            }
            int count = UIAppPages.Count;
            if (count == 0)
            {
                return;
            }
            float num2 = UIAppPages[0].GetComponent<RectTransform>().rect.width + HomeScreenHorizontalLayoutGroup.spacing;
            float x = contentRect.anchoredPosition.x;
            currentAppPage = Mathf.Clamp(Mathf.RoundToInt((0f - x - snapOffset) / num2), 0, count - 1);
            float num3 = (float)(-currentAppPage) * num2 - snapOffset;
            float num4 = 0f - ((float)(count - 1) * num2 + snapOffset);
            float num5 = 0f - snapOffset;
            if (x < num4 || x > num5 || (!HomeScreenScrollRect.m_Dragging && Mathf.Abs(x - num3) < 1f))
            {
                SnapImmediately(currentAppPage);
                return;
            }
            if (!HomeScreenScrollRect.m_Dragging && !isDraggingBlocked)
            {
                contentRect.anchoredPosition = new Vector2(Mathf.SmoothDamp(x, num3, ref snapVelocity, snapDuration), contentRect.anchoredPosition.y);
            }
            if ((HomeScreenScrollRect.m_Dragging || HomeScreenScrollRect.velocity.magnitude > snapVelocityThreshold) && !isDraggingBlocked)
            {
                HomeScreenScrollRect.velocity = Vector2.zero;
                snapVelocity = 0f;
            }
        }

        private void SnapImmediately(int page)
        {
            if (UIAppPages.Count != 0)
            {
                float num = UIAppPages[0].GetComponent<RectTransform>().rect.width + HomeScreenHorizontalLayoutGroup.spacing;
                float x = Mathf.Clamp((float)(-page) * num - snapOffset, 0f - ((float)(UIAppPages.Count - 1) * num + snapOffset), 0f - snapOffset);
                contentRect.anchoredPosition = new Vector2(x, contentRect.anchoredPosition.y);
                snapVelocity = 0f;
            }
        }

        public override void OnLateUpdate()
        {
            if (isLegitVersion)
            {
                return;
            }
            bool toggle = false;
            if (IsHomescreenLoaded && CompatabilityIconsPage != null && CompatabilityIconsPage.transform.childCount > 0)
            {
                if (CompatabilityIconsPage.GetComponentsInChildren<Transform>().Length > 1)
                {
                    ((MelonBase)this).LoggerInstance.Msg("Missing app found, attempting to patch. Count: " + (CompatabilityIconsPage.GetComponentsInChildren<Transform>().Length - 1));
                    GameObject[] array = (from t in (IEnumerable<Transform>)CompatabilityIconsPage.GetComponentsInChildren<Transform>(includeInactive: true)
                                          select t.gameObject into obj
                                          where obj.transform != CompatabilityIconsPage.transform
                                          select obj).ToArray();
                    foreach (GameObject child2 in array)
                    {
                        MelonCoroutines.Start(Patcher(child2));
                    }
                }
            }
            IEnumerator Patcher(GameObject child)
            {
                while (!toggle)
                {
                    toggle = true;
                    yield return new WaitForSeconds(5f);
                }
                RegisterApp(child);
                toggle = false;
            }
        }

        private IEnumerator InitHomescreen()
        {
            GameObject Icons = null;
            yield return MelonCoroutines.Start(Utils.WaitForObjectByFrame("Player_Local/CameraContainer/Camera/OverlayCamera/GameplayMenu/Phone/phone/HomeScreen/AppIcons/", delegate (GameObject obj)
            {
                Icons = obj;
            }));
            ((MelonBase)this).LoggerInstance.Msg("Editing Homescreen");
            GameObject gameObject = GameObject.Find("Player_Local/CameraContainer/Camera/OverlayCamera/GameplayMenu/Phone/phone/HomeScreen");
            ScrollRect scrollRect = gameObject.AddComponent<ScrollRect>();
            scrollRect.horizontal = true;
            scrollRect.vertical = false;
            HomeScreenScrollRect = scrollRect;
            GameObject gameObject2 = new GameObject("Viewport", Il2CppType.Of<RectTransform>(), Il2CppType.Of<Image>(), Il2CppType.Of<Mask>());
            RectTransform component = gameObject2.GetComponent<RectTransform>();
            gameObject2.transform.SetParent(gameObject.transform, worldPositionStays: false);
            component.anchorMin = Vector2.zero;
            component.anchorMax = Vector2.one;
            component.offsetMin = Vector2.zero;
            component.offsetMax = Vector2.zero;
            gameObject2.GetComponent<Mask>().showMaskGraphic = false;
            GameObject gameObject3 = (HomeScreenScrollRectContent = new GameObject("Content", Il2CppType.Of<RectTransform>()));
            RectTransform component2 = gameObject3.GetComponent<RectTransform>();
            gameObject3.transform.SetParent(gameObject2.transform, worldPositionStays: false);
            component2.anchorMin = new Vector2(0f, 0.5f);
            component2.anchorMax = new Vector2(0f, 0.5f);
            component2.pivot = new Vector2(0f, 0.5f);
            component2.anchoredPosition = Vector2.zero;
            component2.sizeDelta = new Vector2(1000f, 200f);
            component2.localPosition = new Vector3(component2.localPosition.x, 300f, component2.localPosition.z);
            gameObject3.AddComponent<ContentSizeFitter>().horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            scrollRect.viewport = component;
            scrollRect.content = component2;
            scrollRect.m_Dragging = true;
            scrollRect.m_ScrollSensitivity = 0f;
            HorizontalLayoutGroup horizontalLayoutGroup = gameObject3.AddComponent<HorizontalLayoutGroup>();
            horizontalLayoutGroup.childForceExpandHeight = false;
            horizontalLayoutGroup.childForceExpandWidth = false;
            horizontalLayoutGroup.spacing = 100f;
            HomeScreenHorizontalLayoutGroup = horizontalLayoutGroup;
            Icons.transform.SetParent(gameObject3.transform, worldPositionStays: true);
            UIAppPages.Add(Icons);
            AllApps.Add(new List<GameObject>());
            GameObject[] array = (from t in (IEnumerable<Transform>)Icons.GetComponentsInChildren<Transform>(includeInactive: true)
                                  select t.gameObject into g
                                  where g.name.StartsWith("AppIcon(Clone)")
                                  select g).ToArray();
            for (int i = 0; i < array.Length; i++)
            {
                AllApps[0].Add(array[i]);
            }
        ((MelonBase)this).LoggerInstance.Msg("App & Page List Established");
            IconsPageTemplate = UnityEngine.Object.Instantiate(Icons, null);
            Utils.ClearChildren(IconsPageTemplate.transform);
            CompatabilityIconsPage = new GameObject("AppIcons", Il2CppType.Of<RectTransform>());
            CompatabilityIconsPage.transform.SetParent(gameObject.transform, worldPositionStays: true);
            ((MelonBase)this).LoggerInstance.Msg("[INIT COMPLETE] Cloned & Cleaned AppUI");
            IsHomescreenLoaded = true;
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
            if (AllApps[AllApps.Count - 1].Count >= 12)
            {
                ((MelonBase)this).LoggerInstance.Msg("Exceeded Space For Page: " + (AllApps.Count - 1) + " Creating New Page");
                AllApps.Add(new List<GameObject>());
                UIAppPages.Add(UnityEngine.Object.Instantiate(IconsPageTemplate, HomeScreenScrollRectContent.transform));
                UIAppPages[UIAppPages.Count - 1].GetComponent<GridLayoutGroup>().enabled = true;
                UIAppPages[UIAppPages.Count - 1].name = "AppIcons" + UIAppPages.Count;
            }
            AllApps[AllApps.Count - 1].Add(App);
            App.transform.SetParent(UIAppPages[UIAppPages.Count - 1].transform, worldPositionStays: false);
            ((MelonBase)this).LoggerInstance.Msg("Added " + Title + " to Page: " + AllApps.Count);
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
                MelonCoroutines.Start(StartCoroutinesAfterDelay());
                isFirstStart = false;
            }
            else if (sceneName.Equals("Menu", StringComparison.OrdinalIgnoreCase) && !isFirstStart)
            {
                ResetAllVariables();
                isFirstStart = false;
            }
        }
        public IEnumerator StartCoroutinesAfterDelay()
        {
            // Wait for other Mods that create Apps to load
            bool delayedInit = false;
            while (!delayedInit && !isLegitVersion)
            {
                MelonLogger.Msg("Waiting 8 Seconds for other mods to load their apps..");
                delayedInit = true;
                yield return new WaitForSeconds(8f);
            }

            MelonCoroutines.Start(MRLCore.Instance.WaitAndApplyCaps());
            if (!isLegitVersion)
            {
                MelonCoroutines.Start(InitHomescreen());
                MelonCoroutines.Start(MRLCore.launderingApp.InitializeLaunderApp());
            }
            MelonCoroutines.Start(MRLCore.Instance.CheckForUnownedBusinesses());

        }

        private void ResetAllVariables()
        {
            LoggerInstance.Msg("Resetting all variables to default values...");
            HomeScreenScrollRect = null;
            IconsPageTemplate = null;
            HomeScreenScrollRectContent = null;
            HomeScreenHorizontalLayoutGroup = null;
            CompatabilityIconsPage = null;
            UIAppPages.Clear();
            AllApps.Clear();
            currentAppPage = 0;
            snapOffset = -30f;
            snapDuration = 0.15f;
            snapVelocity = 0f;
            isInitialSnapDone = false;
            IsHomescreenLoaded = false;

            // Setze die Konfiguration zurück
            MRLCore.Instance.config = null;
            isLegitVersion = false;

            // Leere die Dictionaries
            MRLCore.Instance.maxiumumLaunderValues.Clear();
            MRLCore.Instance.createdAppEntries.Clear();

            // Leere die HashSets
            MRLCore.Instance.processedBusinesses.Clear();
            MRLCore.Instance.createdBusinessesEntries.Clear();
            MRLCore.Instance.boostedOps.Clear();

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


            LoggerInstance.Msg("All variables have been reset.");
        }

        private void InitializeListeners()
        {
            try
            {
                Business.onOperationStarted = new Action<LaunderingOperation>(this.OnLaunderingStarted);
                LoggerInstance.Msg("Registered 'Laundering Started' Listener.");
                Business.onOperationFinished = new Action<LaunderingOperation>(this.OnLaunderingFinished);
                LoggerInstance.Msg("Registered 'Laundering Finished' Listener.");
            }
            catch (Exception ex)
            {
                MelonLogger.Error("Failed to register laundering listeners: " + ex.Message);
            }
        }
        public IEnumerator CheckForUnownedBusinesses()
        {
            MelonLogger.Msg("Checking for unowned businesses...");
            if (Business.UnownedBusinesses != null && Business.UnownedBusinesses.Count > 0)
            {
                MelonLogger.Msg("Found unowned businesses. Processing...");
                if (!isLegitVersion)
                {
                    while (!MRLCore.launderingApp._isLaunderingAppLoaded)
                    {
                        yield return new WaitForSeconds(2f); // Warte 2 Sekunde
                    }
                    string imagePath = Path.Combine(ConfigFolder, "RaysRealEstate.png");
                    MRLCore.launderingApp.AddEntryFromTemplate("Rays Real Estate", "Ray's Real Estate", "No credit check. No judgment. Just opportunity.", null, MRLCore.launderingApp.dansHardwareTemplate, ColorUtil.GetColor("Light Purple"), imagePath, null, true);
                }
                ApplyPricesToUnownedBusinesses();
                ApplyPricesToPropertyListings();
                yield break;
            }
            else
            {
                MelonLogger.Msg("No unowned businesses found.");
                yield break;
            }
        }

        public void FillCapDictionary()
        {
            MRLCore.Instance.maxiumumLaunderValues.Clear();
            MRLCore.Instance.maxiumumLaunderValues["Laundromat"] = MRLCore.Instance.config.Laundromat_Cap;
            MRLCore.Instance.maxiumumLaunderValues["Taco Ticklers"] = MRLCore.Instance.config.Taco_Ticklers_Cap;
            MRLCore.Instance.maxiumumLaunderValues["Car Wash"] = MRLCore.Instance.config.Car_Wash_Cap;
            MRLCore.Instance.maxiumumLaunderValues["PostOffice"] = MRLCore.Instance.config.Post_Office_Cap;
            MRLCore.Instance.maxiumumLaunderValues["Post Office"] = MRLCore.Instance.config.Post_Office_Cap;
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

        public void MaybeBoost(LaunderingOperation operation)
        {
            if (operation == null || MRLCore.Instance.boostedOps.Contains(operation))
                return;

            if (MRLCore.Instance.aliasMap.TryGetValue(operation.business.name, out string key))
            {
                int launderingTime = key switch
                {
                    "Laundromat" => MRLCore.Instance.config.Laundromat_Laundering_time_hours,
                    "Taco Ticklers" => MRLCore.Instance.config.Taco_Ticklers_Laundering_time_hours,
                    "Car Wash" => MRLCore.Instance.config.Car_Wash_Laundering_time_hours,
                    "PostOffice" => MRLCore.Instance.config.Post_Office_Laundering_time_hours,
                    "Post Office" => MRLCore.Instance.config.Post_Office_Laundering_time_hours,
                    _ => 24 // Default fallback
                };

                operation.completionTime_Minutes = launderingTime * 60;
                string displayName = operation.business.name == "PostOffice" ? "Post Office" : operation.business.name;
                //    MelonLogger.Msg($"Speeding up Laundering Operation: {displayName} Amount: {operation.amount:N0} Completion Time: {operation.completionTime_Minutes}");
                MRLCore.Instance.boostedOps.Add(operation);
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

                    float taxAmount = CalculateTaxedPayout(operation);
                    if (taxAmount > 0)
                    {
                        DeductBankMoney(taxAmount, operation.business.name);
                    }
                }
            }
            catch (Exception ex)
            {
                LoggerInstance.Error($"Error in OnLaunderingFinished: {ex.Message}");
            }
        }

        private void DeductBankMoney(float amount, string businessName)
        {
            try
            {
                if (moneyManager != null)
                {
                    string transactionText = $"Tax for {businessName}";
                    //moneyManager.ReceiveOnlineTransaction(transactionText, -amount, 1, transactionText);
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

        public float CalculateTaxedPayout(LaunderingOperation operation)
        {
            if (MRLCore.Instance.aliasMap.TryGetValue(operation.business.name, out string key))
            {
                float taxRate = key switch
                {
                    "Laundromat" => MRLCore.Instance.config.Laundromat_Tax_Percentage / 100,
                    "Taco Ticklers" => MRLCore.Instance.config.Taco_Ticklers_Tax_Percentage / 100,
                    "Car Wash" => MRLCore.Instance.config.Car_Wash_Tax_Percentage / 100,
                    "PostOffice" => MRLCore.Instance.config.Post_Office_Tax_Percentage / 100,
                    "Post Office" => MRLCore.Instance.config.Post_Office_Tax_Percentage / 100,
                    _ => 0.19f // Default fallback
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

        public void ApplyPricesToUnownedBusinesses()
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
                            "Laundromat" => MRLCore.Instance.config.Laundromat_Price,
                            "Taco Ticklers" => MRLCore.Instance.config.Taco_Ticklers_Price,
                            "Car Wash" => MRLCore.Instance.config.Car_Wash_Price,
                            "PostOffice" => MRLCore.Instance.config.Post_Office_Price,
                            "Post Office" => MRLCore.Instance.config.Post_Office_Price,
                            _ => 999999f // Standardwert, falls das Business nicht gefunden wird
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
                        "Laundromat" => MRLCore.Instance.config.Laundromat_Price,
                        "Taco Ticklers" => MRLCore.Instance.config.Taco_Ticklers_Price,
                        "Car Wash" => MRLCore.Instance.config.Car_Wash_Price,
                        "Post Office" => MRLCore.Instance.config.Post_Office_Price,
                        "PostOffice" => MRLCore.Instance.config.Post_Office_Price,
                        _ => 999999f // Standardwert, falls das Business nicht gefunden wird
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
            UpdateSellSignPrices();
        }

        private void UpdateSellSignPrices()
        {
            // Suche nach den Verkaufsschildern
            GameObject sellSignPostOffice = GameObject.Find("@Businesses/PostOffice/ForSaleSign_Blue (1)");
            GameObject sellSignCarWash = GameObject.Find("@Businesses/Car Wash/ForSaleSign_Blue");
            GameObject sellSignLaundromat = GameObject.Find("@Businesses/Laundromat/ForSaleSign_Blue (1)");
            GameObject sellSignTacoTicklers = GameObject.Find("@Businesses/Taco Ticklers/ForSaleSign_Blue (1)");

            // Aktualisiere die Preise für jedes Schild
            UpdateSignPrice(sellSignPostOffice, "Post Office");
            UpdateSignPrice(sellSignCarWash, "Car Wash");
            UpdateSignPrice(sellSignLaundromat, "Laundromat");
            UpdateSignPrice(sellSignTacoTicklers, "Taco Ticklers");
        }

        private void UpdateSignPrice(GameObject sellSign, string businessName)
        {
            if (sellSign == null)
            {
                MelonLogger.Warning($"Sell sign for {businessName} not found. Skipping...");
                return;
            }

            // Hole den Preis aus der Konfiguration
            if (MRLCore.Instance.aliasMap.TryGetValue(businessName, out string key))
            {
                float price = key switch
                {
                    "Laundromat" => MRLCore.Instance.config.Laundromat_Price,
                    "Taco Ticklers" => MRLCore.Instance.config.Taco_Ticklers_Price,
                    "Car Wash" => MRLCore.Instance.config.Car_Wash_Price,
                    "Post Office" => MRLCore.Instance.config.Post_Office_Price,
                    "PostOffice" => MRLCore.Instance.config.Post_Office_Price,
                    _ => 999999f // Standardwert, falls das Business nicht gefunden wird
                };

                // Suche nach dem "Price"-Objekt und aktualisiere den Text
                Transform priceTransform = sellSign.transform.Find("Price");
                if (priceTransform != null)
                {
                    TextMeshPro priceText = priceTransform.GetComponent<TextMeshPro>();
                    if (priceText != null)
                    {
                        priceText.text = $"${price}";
                        //    MelonLogger.Msg($"Updated sell sign price for {businessName} to ${price}.");
                    }
                    else
                    {
                        MelonLogger.Warning($"Price TextMeshPro component not found for sell sign of {businessName}.");
                    }
                }
                else
                {
                    MelonLogger.Warning($"Price object not found for sell sign of {businessName}.");
                }
            }
            else
            {
                MelonLogger.Warning($"Business '{businessName}' not found in aliasMap. Skipping sell sign price assignment.");
            }
        }

        public Sprite CreditCardIcon;
        public Config.ConfigState config;
        public static PhoneApp.LaunderingApp launderingApp = new PhoneApp.LaunderingApp();
        public System.Collections.Generic.Dictionary<string, float> maxiumumLaunderValues;
        public System.Collections.Generic.Dictionary<string, string> aliasMap;
        private readonly System.Collections.Generic.HashSet<string> createdAppEntries = new System.Collections.Generic.HashSet<string>();
        public bool isWaitAndApplyCapsRunning = false;
        private static readonly string ConfigFolder = Path.Combine(MelonEnvironment.UserDataDirectory, "MoreRealisticLaundering");
        private readonly System.Collections.Generic.HashSet<LaunderingOperation> boostedOps = new System.Collections.Generic.HashSet<LaunderingOperation>();

        public MoneyManager moneyManager;
        public NotificationsManager notificationsManager;
    }
}