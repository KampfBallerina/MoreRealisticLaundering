using System.Collections;
using Il2CppScheduleOne.Property;
using Il2CppSystem;
using MelonLoader;
using MelonLoader.Utils;
using MoreRealisticLaundering.Config;
using MoreRealisticLaundering.Util;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace MoreRealisticLaundering.PhoneApp
{
    public class LaunderingApp
    {
        public IEnumerator InitializeLaunderApp()
        {
            yield return MelonCoroutines.Start(WaitForHomescreenInstance());
            var MRLInstance = MRLCore.Instance;

            while (!MRLInstance.IsHomescreenLoaded)
            {
                MelonLogger.Msg("Waiting for Homescreen to be loaded..");
                yield return new WaitForSeconds(2f);
            }
            MelonCoroutines.Start(FontLoader.InitializeFonts());
            while (!Util.FontLoader.openSansBoldIsInitialized || !Util.FontLoader.openSansSemiBoldIsInitialized)
            {
                MelonLogger.Msg("Waiting for Fonts to be loaded...");
                yield return new WaitForSeconds(1f);
            }
            yield return MelonCoroutines.Start(CreateApp("TaxNWash", "Tax & Wash", true, FilePath));
            yield break;
        }


        private IEnumerator WaitForHomescreenInstance()
        {
            while (MRLCore.Instance == null)
            {
                MelonLogger.Msg("Waiting for Instance to be initialized...");
                yield return new WaitForSeconds(1f);
            }

            //           MelonLogger.Msg("Instance is now available!");
        }


        public IEnumerator CreateApp(string IDName, string Title, bool IsRotated = true, string IconPath = null)
        {
            GameObject cloningCandidate = null;
            string cloningName = null;
            GameObject icons = null;

            // Warte auf das AppIcons-Objekt
            yield return MelonCoroutines.Start(Utils.WaitForObject(
                "Player_Local/CameraContainer/Camera/OverlayCamera/GameplayMenu/Phone/phone/HomeScreen/Viewport/Content/AppIcons/",
                delegate (GameObject obj) { icons = obj; }
            ));

            // Bestimme das CloningCandidate basierend auf IsRotated
            if (IsRotated)
            {
                yield return MelonCoroutines.Start(Utils.WaitForObject(
                    "Player_Local/CameraContainer/Camera/OverlayCamera/GameplayMenu/Phone/phone/AppsCanvas/DeliveryApp",
                    delegate (GameObject obj)
                    {
                        cloningCandidate = obj;
                        cloningName = "Deliveries";
                    }
                ));
            }
            else
            {
                yield return MelonCoroutines.Start(Utils.WaitForObject(
                    "Player_Local/CameraContainer/Camera/OverlayCamera/GameplayMenu/Phone/phone/AppsCanvas/Messages",
                    delegate (GameObject obj)
                    {
                        cloningCandidate = obj;
                        cloningName = "Messages";
                    }
                ));
            }

            // Klone das App-Canvas
            GameObject parentCanvas = GameObject.Find("Player_Local/CameraContainer/Camera/OverlayCamera/GameplayMenu/Phone/phone/AppsCanvas/");
            GameObject clonedApp = UnityEngine.Object.Instantiate(cloningCandidate, parentCanvas.transform);
            Transform detailEntriesTransform = null;

            // Ändere die Eigenschaften des Containers, ohne Kinder zu löschen
            Transform containerTransform = clonedApp.transform.Find("Container");
            if (containerTransform != null)
            {
                Transform topbarTransform = containerTransform.Find("Topbar");
                if (topbarTransform != null)
                {
                    //Adjust Topbar Color
                    Image topbarImage = topbarTransform.GetComponent<Image>();
                    topbarImage.color = ColorUtil.GetColor("Cyan");

                    //Adjust Topbar Title
                    Transform topbarTitleTransform = topbarTransform.Find("Title");
                    if (topbarTitleTransform != null)
                    {
                        topbarTitleTransform.GetComponent<Text>().text = Title;
                    }
                }

                Transform detailsTransform = containerTransform.Find("Deliveries");
                if (detailsTransform != null)
                {
                    // Ändere den Namen des Details-Objekts
                    detailsTransform.name = "Details";

                    Transform detailsTitleTransform = detailsTransform.Find("Title");
                    if (detailsTitleTransform != null)
                    {
                        detailsTitleTransform.GetComponent<Text>().text = "Details";
                    }

                    detailEntriesTransform = detailsTransform.Find("Entries");
                    if (detailEntriesTransform != null)
                    {
                        // Ändere den Namen des Entries-Objekts
                        detailEntriesTransform.name = "Options";

                        RectTransform detailEntriesRect = detailEntriesTransform.GetComponent<RectTransform>();
                        if (detailEntriesRect != null)
                        {
                            detailEntriesRect.anchoredPosition = new Vector2(10, -25);
                        }

                        Transform noneEntryTransform = detailEntriesTransform.Find("None");
                        if (noneEntryTransform != null)
                        {
                            Text noneEntryText = noneEntryTransform.GetComponent<Text>();
                            if (noneEntryText != null)
                            {
                                noneEntryText.text = "Choose one of your businesses";
                            }
                        }

                        // Erstelle eine Kopie von Entries
                        priceOptionsTransform = UnityEngine.Object.Instantiate(detailEntriesTransform, detailEntriesTransform.parent);
                        if (priceOptionsTransform != null)
                        {
                            priceOptionsTransform.name = "PriceOptions";
                            priceOptionsTransform.gameObject.SetActive(false); // Deaktiviere das Preis-Options-Objekt

                            Transform noneEntryPriceTransform = priceOptionsTransform.Find("None");
                            if (noneEntryPriceTransform != null)
                            {
                                Text noneEntryText = noneEntryPriceTransform.GetComponent<Text>();
                                if (noneEntryText != null)
                                {
                                    noneEntryText.text = "Adjusting the prices of properties";
                                }
                            }
                        }
                    }
                }

                // Setze den Namen des geklonten Objekts
                clonedApp.name = IDName;

                // Aktualisiere das App-Icon
                GameObject appIconByName = Utils.GetAppIconByName(cloningName, 1);
                Transform labelTransform = appIconByName.transform.Find("Label");
                GameObject labelObject = labelTransform != null ? labelTransform.gameObject : null;
                if (labelObject != null)
                {
                    Text labelText = labelObject.GetComponent<Text>();
                    if (labelText != null)
                    {
                        labelText.text = Title;
                    }
                }

                Transform scrollViewTransform = containerTransform.Find("Scroll View");
                if (scrollViewTransform != null)
                {
                    Transform viewportTransform = scrollViewTransform.Find("Viewport");
                    if (viewportTransform != null)
                    {
                        Transform viewportContentTransform = viewportTransform.Find("Content");
                        if (viewportContentTransform != null)
                        {
                            launderingAppViewportContentTransform = viewportContentTransform;
                            // Suche nach den GameObjects "Dan's Hardware" und "Gas-Mart (West)" und "Space" und speichere sie in Variablen
                            GameObject dansHardware = viewportContentTransform.Find("Dan's Hardware").gameObject;
                            GameObject gasMartWest = viewportContentTransform.Find("Gas-Mart (West)").gameObject;
                            GameObject viewPortContentSpace = viewportContentTransform.Find("Space").gameObject;

                            if (dansHardware != null && dansHardwareTemplate == null)
                            {
                                dansHardwareTemplate = UnityEngine.Object.Instantiate(dansHardware);
                                dansHardwareTemplate.name = "Dan's Hardware Template";
                                dansHardwareTemplate.SetActive(false); // Deaktiviere das Template

                                Transform contentsDansTemplateTransform = dansHardwareTemplate.transform.Find("Contents");
                                if (contentsDansTemplateTransform != null)
                                {
                                    UnityEngine.Object.DestroyImmediate(contentsDansTemplateTransform.gameObject); // Entferne den "Contents"
                                                                                                                   //   MelonLogger.Msg("Removed Contents from Dans Hardware Template");
                                }

                                UnityEngine.Object.Destroy(dansHardware); // Entferne das ursprüngliche GameObject
                            }

                            if (gasMartWest != null && gasMartWestTemplate == null)
                            {
                                gasMartWestTemplate = UnityEngine.Object.Instantiate(gasMartWest);
                                gasMartWestTemplate.name = "Gas-Mart (West) Template";
                                gasMartWestTemplate.SetActive(false); // Deaktiviere das Template

                                Transform contentsMarketTemplateTransform = gasMartWestTemplate.transform.Find("Contents");
                                if (contentsMarketTemplateTransform != null)
                                {
                                    UnityEngine.Object.DestroyImmediate(contentsMarketTemplateTransform.gameObject); // Entferne den "Contents"
                                                                                                                     //   MelonLogger.Msg("Removed Contents from Gas Mart West Template");
                                }
                                UnityEngine.Object.Destroy(gasMartWest); // Entferne das ursprüngliche GameObject
                            }

                            if (viewPortContentSpace != null && viewPortContentSpaceTemplate == null)
                            {
                                viewPortContentSpaceTemplate = UnityEngine.Object.Instantiate(viewPortContentSpace);
                                viewPortContentSpaceTemplate.name = "Space Template";
                                viewPortContentSpaceTemplate.SetActive(false); // Deaktiviere das Template
                                UnityEngine.Object.Destroy(viewPortContentSpace); // Entferne das ursprüngliche GameObject
                            }

                            Utils.ClearChildren(viewportContentTransform);

                            //  MelonLogger.Msg("Saved Dan's Hardware, Gas Mart and Space as Templates");

                        }
                        else
                        {
                            MelonLogger.Error("Viewport Content not found!");
                        }

                        optionsTransform = detailEntriesTransform;
                        AddSpaceFromTemplate(viewportContentTransform);
                        AddOptionsForBusiness(detailEntriesTransform);
                        AddPriceOptionsForRealEstate(priceOptionsTransform);
                    }
                }

                // Ändere das App-Icon-Bild
                MRLCore.Instance.ChangeAppIconImage(appIconByName, IconPath);

                // Registriere die App
                MRLCore.Instance.RegisterApp(appIconByName, Title);
                _isLaunderingAppLoaded = true;
            }
        }

        public void AddSpaceFromTemplate(Transform viewportContentTransform = null)
        {
            if (viewportContentTransform == null)
            {
                viewportContentTransform = launderingAppViewportContentTransform;
                if (viewportContentTransform == null)
                {
                    MelonLogger.Error("ViewportContentTransform is null!");
                    return;
                }
            }

            if (viewPortContentSpaceTemplate == null)
            {
                MelonLogger.Error("Can't find Space Template!");
                return;
            }

            // Überprüfe, ob bereits ein "Space"-Eintrag existiert
            Transform existingSpace = viewportContentTransform.Find("Space");
            if (existingSpace != null)
            {
                // Verschiebe den vorhandenen "Space"-Eintrag an die letzte Position
                existingSpace.SetAsLastSibling();
                //    MelonLogger.Msg("Moved existing Space entry to the last position.");
                return;
            }

            // Erstelle ein Duplikat des Templates
            GameObject newSpace = UnityEngine.Object.Instantiate(viewPortContentSpaceTemplate, viewportContentTransform);
            newSpace.name = "Space";
            newSpace.transform.SetAsLastSibling(); // Stelle sicher, dass der Eintrag der letzte ist
            newSpace.SetActive(true);

            //MelonLogger.Msg("Added Custom Spacing for new Entries as the last item.");
        }

        public void AddEntryFromTemplate(string newObjectName, string newTitle, string newSubtitle, Business business, GameObject template = null, Color newBackgroundColor = default,
        string imagePath = null, Transform viewportContentTransform = null, bool isFirstEntry = false)
        {
            if (viewportContentTransform == null)
            {
                if (launderingAppViewportContentTransform == null)
                {
                    MelonLogger.Error("ViewportContentTransform is null!");
                    return;
                }
                else
                {
                    viewportContentTransform = launderingAppViewportContentTransform;
                }
            }

            if (template == null && gasMartWestTemplate != null)
            {
                template = gasMartWestTemplate; // Standard-Template verwenden, wenn kein Template angegeben ist
            }

            // Erstelle ein Duplikat des Templates
            GameObject newEntry = UnityEngine.Object.Instantiate(template, viewportContentTransform);
            newEntry.name = newObjectName;

            //Find the Transforms
            Transform headerTransform = newEntry.transform.Find("Header");
            Transform iconTransform = headerTransform.Find("Icon");
            Transform imageTransform = iconTransform.Find("Image");
            Transform titleTransform = headerTransform.Find("Title");
            Transform subtitleTransform = headerTransform.Find("Description");
            Transform arrowTransform = headerTransform.Find("Arrow");

            // Hide the Arrow
            arrowTransform.gameObject.SetActive(false);

            // Change Title
            Text titleTextComponent = titleTransform.GetComponent<Text>();
            if (titleTextComponent != null)
            {
                titleTextComponent.text = newTitle;
            }

            // Change Subtitle
            Text subtitleTextComponent = subtitleTransform.GetComponent<Text>();
            if (subtitleTextComponent != null)
            {
                subtitleTextComponent.text = newSubtitle;
            }

            // Change Header Color (optional)
            Image headerImageComponent = headerTransform.GetComponent<Image>();
            if (headerImageComponent != null && newBackgroundColor != default)
            {
                headerImageComponent.color = newBackgroundColor;
            }

            // Change Icon Image
            if (!string.IsNullOrEmpty(imagePath) && File.Exists(imagePath))
            {
                byte[] imageData = File.ReadAllBytes(imagePath);
                Texture2D texture = new Texture2D(2, 2);
                if (texture.LoadImage(imageData))
                {
                    Sprite newSprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                    Image iconImageComponent = imageTransform.GetComponent<Image>();
                    if (iconImageComponent != null)
                    {
                        iconImageComponent.sprite = newSprite;
                    }
                }
                else
                {
                    MelonLogger.Error($"Failed to load image from path: {imagePath}");
                }
            }

            if (business)
            {

                Button headerButton = headerTransform.GetComponent<Button>();
                if (headerButton != null)
                {
                    headerButton.name = newObjectName + " Button";
                    headerButton.onClick.RemoveAllListeners(); // Remove existing listeners to avoid old functionality after copying
                    void FuncThatCallsFunc() => BusinessButtonClicked(business);
                    headerButton.onClick.AddListener((UnityAction)FuncThatCallsFunc);
                }
            }

            if (newObjectName == "Rays Real Estate")
            {
                Button headerButton = headerTransform.GetComponent<Button>();
                if (headerButton != null)
                {
                    headerButton.name = newObjectName + " Button";
                    headerButton.onClick.RemoveAllListeners(); // Remove existing listeners to avoid old functionality after copying
                    void FuncThatCallsFunc() => RealEstateButtonClicked();
                    headerButton.onClick.AddListener((UnityAction)FuncThatCallsFunc);
                }
            }

            // Set the new entry as the first child if isFirstEntry is true
            if (isFirstEntry)
            {
                newEntry.transform.SetAsFirstSibling();
            }
            newEntry.SetActive(true);
            //  MelonLogger.Msg($"Added new entry: {newObjectName} with text: {newTitle}");
            AddSpaceFromTemplate(viewportContentTransform); // Add space after the new entry
        }


        void RealEstateButtonClicked()
        {
            if (optionsTransform == null)
            {
                MelonLogger.Error("optionsTransform is null! Ensure it is initialized before calling RealEstateButtonClicked.");
                return;
            }

            // Überprüfe, ob priceOptionsTransform bereits aktiv ist
            if (priceOptionsTransform == null)
            {
                MelonLogger.Error("priceOptionsTransform is null! Ensure it is initialized before calling RealEstateButtonClicked.");
                return;
            }

            if (!priceOptionsTransform.gameObject.activeSelf)
            {
                priceOptionsTransform.gameObject.SetActive(true);
            }

            // Deaktiviere optionsTransform
            if (optionsTransform.gameObject.activeSelf)
            {
                optionsTransform.gameObject.SetActive(false);
            }

            // Aktiviere priceOptionsTransform und alle Kinder
            if (priceOptionsTransform != null)
            {
                priceOptionsTransform.gameObject.SetActive(true);

                foreach (var child in priceOptionsTransform)
                {
                    if (child is Transform transformChild)
                    {
                        if (transformChild.gameObject && !transformChild.gameObject.activeSelf)
                        {
                            transformChild.gameObject.SetActive(true);
                        }
                    }
                }
            }
            else
            {
                MelonLogger.Error("priceOptionsTransform is null! Ensure it is initialized before calling RealEstateButtonClicked.");
            }

            // Lade die Preise für OwnedBusinesses und UnownedBusinesses
            if (Business.OwnedBusinesses == null && Business.UnownedBusinesses == null)
            {
                MelonLogger.Error("OwnedBusinesses or UnownedBusinesses is null! Cannot load price values.");
                return;
            }
            else
            {
                // OwnedBusinesses
                foreach (Business business in Business.OwnedBusinesses)
                {
                    if (business == null) continue;

                    string displayName = business.name;
                    if (displayName == "PostOffice")
                    {
                        displayName = "Post Office";
                    }
                    float price = business.Price;
                    SetInputFieldValue(priceOptionsTransform, displayName, price);
                }

                // UnownedBusinesses
                foreach (Business business in Business.UnownedBusinesses)
                {
                    if (business == null) continue;

                    string displayName = business.name;
                    if (displayName == "PostOffice")
                    {
                        displayName = "Post Office";
                    }
                    float price = business.Price;
                    SetInputFieldValue(priceOptionsTransform, displayName, price);
                }
            }
        }

        private void SetInputFieldValue(Transform parentTransform, string displayName, float value)
        {
            Transform inputFieldTransform = parentTransform.Find($"{displayName} Horizontal Container/{displayName} Input");
            if (inputFieldTransform != null)
            {
                InputField inputField = inputFieldTransform.GetComponent<InputField>();
                if (inputField != null)
                {
                    inputField.text = value.ToString();
                }
                else
                {
                    MelonLogger.Error($"InputField component not found for {displayName}.");
                }
            }
            else
            {
                MelonLogger.Error($"Transform not found for {displayName}.");
            }
        }

        void BusinessButtonClicked(Business business)
        {
            if (optionsTransform == null)
            {
                MelonLogger.Error("optionsTransform is null! Ensure it is initialized before calling BusinessButtonClicked.");
                return;
            }

            if (priceOptionsTransform != null)
            {
                if (priceOptionsTransform.gameObject.activeSelf)
                {
                    priceOptionsTransform.gameObject.SetActive(false);
                }
            }
            else
            {
                MelonLogger.Error("priceOptionsTransform is null! Ensure it is initialized before calling BusinessButtonClicked.");
            }

            _selectedBusiness = business;
            _selectedBusinessName = business.name;

            // Aktiviere die drei Container in optionsTransform, falls sie deaktiviert sind
            Transform maxLaunderContainer = optionsTransform.Find("Maximum Launder Horizontal Container");
            Transform launderTimeContainer = optionsTransform.Find("Launder Time Horizontal Container");
            Transform taxationContainer = optionsTransform.Find("Taxation Horizontal Container");

            if (!optionsTransform.gameObject.activeSelf)
            {
                optionsTransform.gameObject.SetActive(true);
                //   MelonLogger.Msg("Activated Options Transform.");
            }
            if (maxLaunderContainer != null && !maxLaunderContainer.gameObject.activeSelf)
            {
                maxLaunderContainer.gameObject.SetActive(true);
                //   MelonLogger.Msg("Activated Maximum Launder Horizontal Container.");
            }

            if (launderTimeContainer != null && !launderTimeContainer.gameObject.activeSelf)
            {
                launderTimeContainer.gameObject.SetActive(true);
                // MelonLogger.Msg("Activated Launder Time Horizontal Container.");
            }

            if (taxationContainer != null && !taxationContainer.gameObject.activeSelf)
            {
                taxationContainer.gameObject.SetActive(true);
                //   MelonLogger.Msg("Activated Taxation Horizontal Container.");
            }

            string businessName = business.name;
            if (business.name == "PostOffice")
            {
                businessName = "Post Office";
            }
            string subTitleText = "Adjusting the settings for " + businessName;
            Transform noneTransform = optionsTransform.Find("None");
            if (noneTransform != null)
            {
                Text noneText = noneTransform.gameObject.GetComponentInChildren<Text>();
                if (noneText != null)
                {
                    noneText.text = subTitleText;
                }
            }

            Transform saveButtonTransform = optionsTransform.Find("Save Button");
            if (saveButtonTransform != null)
            {
                saveButtonTransform.gameObject.SetActive(true);
            }

            // Update Maximum Launder Input
            Transform maxLaunderInputTransform = optionsTransform.Find("Maximum Launder Horizontal Container/Maximum Launder Input");
            if (maxLaunderInputTransform != null)
            {
                InputField maxLaunderInputField = maxLaunderInputTransform.GetComponent<InputField>();
                if (maxLaunderInputField != null)
                {
                    maxLaunderInputField.text = business.LaunderCapacity.ToString();
                    //   MelonLogger.Msg($"Set Maximum Launder Input Field to {business.LaunderCapacity} for business {businessName}");
                }
                else
                {
                    MelonLogger.Error("Maximum Launder Input Field component not found!");
                }
            }
            else
            {
                MelonLogger.Error("Maximum Launder Input Transform not found!");
            }

            // Update Launder Time Input
            Transform launderTimeInputTransform = optionsTransform.Find("Launder Time Horizontal Container/Launder Time Input");
            if (launderTimeInputTransform != null)
            {
                InputField launderTimeInputField = launderTimeInputTransform.GetComponent<InputField>();
                if (launderTimeInputField != null)
                {
                    if (MRLCore.Instance.config != null)
                    {
                        int launderTimeHours = ConfigManager.GetLaunderingTimeHours(MRLCore.Instance.config, business.name);
                        launderTimeInputField.text = launderTimeHours.ToString();
                        // MelonLogger.Msg($"Set Launder Time Input Field to {launderTimeHours} for business {businessName}");
                    }
                    else
                    {
                        MelonLogger.Error($"LaunderTimeHours not found in config for business {businessName}");
                    }
                }
                else
                {
                    MelonLogger.Error("Launder Time Input Field component not found!");
                }
            }
            else
            {
                MelonLogger.Error("Launder Time Input Transform not found!");
            }

            // Update Launder Taxation
            Transform taxationInputTransform = optionsTransform.Find("Taxation Horizontal Container/Taxation Input");
            if (taxationInputTransform != null)
            {
                InputField taxationInputField = taxationInputTransform.GetComponent<InputField>();
                if (taxationInputField != null)
                {
                    if (MRLCore.Instance.config != null)
                    {
                        float taxationPercentage = ConfigManager.GetTaxationPercentage(MRLCore.Instance.config, business.name);
                        taxationInputField.text = taxationPercentage.ToString();
                        //    MelonLogger.Msg($"Set Taxation Input Field to {taxationPercentage} for business {businessName}");
                    }
                    else
                    {
                        MelonLogger.Error($"Taxation Percentage not found in config for business {businessName}");
                    }
                }
                else
                {
                    MelonLogger.Error("Taxation Input Field component not found!");
                }
            }
            else
            {
                MelonLogger.Error("Taxation Input Transform not found!");
            }
        }


        public void AddOptionsForBusiness(Transform detailEntriesTransform)
        {
            if (detailEntriesTransform == null)
            {
                MelonLogger.Error("detailEntriesTransform is null! Cannot add options.");
                return;
            }

            VerticalLayoutGroup optionVerticalLayout = detailEntriesTransform.GetComponent<VerticalLayoutGroup>();
            if (optionVerticalLayout == null)
            {
                optionVerticalLayout = detailEntriesTransform.gameObject.AddComponent<VerticalLayoutGroup>();
            }

            optionVerticalLayout.childControlWidth = true;
            optionVerticalLayout.childForceExpandWidth = false;
            optionVerticalLayout.childAlignment = TextAnchor.UpperLeft;
            optionVerticalLayout.spacing = -20f; // Abstand zwischen den Optionen

            // Füge die Optionen hinzu
            AddLabelInputPair("Maximum Launder", detailEntriesTransform, "$");
            AddLabelInputPair("Launder Time", detailEntriesTransform, "hr");
            AddLabelInputPair("Taxation", detailEntriesTransform, "%");
            AddSaveButton(detailEntriesTransform, "BusinessDetails");

            //   MelonLogger.Msg("Added options for business to detailEntriesTransform.");
        }

        void AddLabelInputPair(string labelText, Transform parentTransform, string prefixText)
        {
            if (parentTransform == null)
            {
                MelonLogger.Error($"Parent transform is null for label: {labelText}");
                return;
            }

            // Erstelle einen Container für das Label und das Inputfeld
            GameObject container = new GameObject($"{labelText} Horizontal Container");
            container.transform.SetParent(parentTransform, false);
            container.SetActive(false);

            // Füge eine HorizontalLayoutGroup hinzu, falls sie nicht existiert
            HorizontalLayoutGroup horizontalContainerGroup = container.GetComponent<HorizontalLayoutGroup>();
            if (horizontalContainerGroup == null)
            {
                horizontalContainerGroup = container.AddComponent<HorizontalLayoutGroup>();
                horizontalContainerGroup.spacing = 10f; // Abstand zwischen den Elementen
                horizontalContainerGroup.childAlignment = TextAnchor.MiddleLeft; // Elemente linksbündig ausrichten
                horizontalContainerGroup.childForceExpandWidth = false; // Breite der Kinder nicht erzwingen
                horizontalContainerGroup.childForceExpandHeight = false; // Höhe der Kinder nicht erzwingen
            }

            // Erstelle ein neues GameObject für das Label
            GameObject labelObject = new GameObject($"{labelText} Label");
            labelObject.transform.SetParent(container.transform, false);

            // Füge eine Text-Komponente hinzu, falls sie nicht existiert
            Text labelTextComponent = labelObject.GetComponent<Text>();
            if (labelTextComponent == null)
            {
                labelTextComponent = labelObject.AddComponent<Text>();
                labelTextComponent.text = labelText;
                labelTextComponent.font = FontLoader.openSansSemiBold ?? Resources.GetBuiltinResource<Font>("Arial.ttf");
                labelTextComponent.fontSize = 18;
                labelTextComponent.color = Color.white;
                labelTextComponent.alignment = TextAnchor.MiddleLeft;
            }

            // Füge eine RectTransform-Komponente hinzu, falls sie nicht existiert
            RectTransform labelRect = labelObject.GetComponent<RectTransform>();
            if (labelRect == null)
            {
                labelRect = labelObject.AddComponent<RectTransform>();
                labelRect.sizeDelta = new Vector2(250, 30); // Breite und Höhe des Labels
            }

            // Füge ein LayoutElement hinzu, falls es nicht existiert
            LayoutElement labelLayoutElement = labelObject.GetComponent<LayoutElement>();
            if (labelLayoutElement == null)
            {
                labelLayoutElement = labelObject.AddComponent<LayoutElement>();
                labelLayoutElement.minWidth = 250; // Mindestbreite des Labels
                labelLayoutElement.preferredWidth = 250; // Bevorzugte Breite des Labels
            }

            // Erstelle ein neues GameObject für das Inputfeld
            GameObject inputObject = new GameObject($"{labelText} Input");
            inputObject.transform.SetParent(container.transform, false);

            // Füge ein Hintergrundbild hinzu, falls es nicht existiert
            Image backgroundImage = inputObject.GetComponent<Image>();
            if (backgroundImage == null)
            {
                backgroundImage = inputObject.AddComponent<Image>();

                if (inputBackgroundSprite == null)
                {
                    string imagePath = Path.Combine(ConfigFolder, "InputBackground.png");
                    if (!string.IsNullOrEmpty(imagePath) && File.Exists(imagePath))
                    {
                        byte[] imageData = File.ReadAllBytes(imagePath);
                        Texture2D texture = new Texture2D(2, 2);
                        if (texture.LoadImage(imageData))
                        {
                            Sprite newSprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                            inputBackgroundSprite = newSprite;
                            backgroundImage.sprite = newSprite;
                        }
                        else
                        {
                            MelonLogger.Error($"Failed to load image from path: {imagePath}");
                            backgroundImage.color = ColorUtil.GetColor("Grey");
                        }
                    }
                }
                else
                {
                    backgroundImage.sprite = inputBackgroundSprite;
                }
            }

            // Füge eine InputField-Komponente hinzu, falls sie nicht existiert
            InputField inputFieldComponent = inputObject.GetComponent<InputField>();
            if (inputFieldComponent == null)
            {
                inputFieldComponent = inputObject.AddComponent<InputField>();
            }

            // Füge ein LayoutElement hinzu, falls es nicht existiert
            LayoutElement layoutElement = inputObject.GetComponent<LayoutElement>();
            if (layoutElement == null)
            {
                layoutElement = inputObject.AddComponent<LayoutElement>();
                layoutElement.minWidth = 100; // Mindestbreite des Input-Felds
                layoutElement.preferredWidth = 100; // Bevorzugte Breite des Input-Felds
            }

            // Erstelle ein neues GameObject für den Text des Inputfelds
            GameObject inputTextObject = new GameObject($"{labelText} Input Text");
            inputTextObject.transform.SetParent(inputObject.transform, false);

            // Füge eine Text-Komponente hinzu, falls sie nicht existiert
            Text inputTextComponent = inputTextObject.GetComponent<Text>();
            if (inputTextComponent == null)
            {
                inputTextComponent = inputTextObject.AddComponent<Text>();
                inputTextComponent.font = FontLoader.openSansSemiBold ?? Resources.GetBuiltinResource<Font>("Arial.ttf");
                inputTextComponent.fontSize = 18;
                inputTextComponent.color = ColorUtil.GetColor("Bright Green");
                inputTextComponent.alignment = TextAnchor.MiddleLeft;
            }

            // Weise den Text dem InputField zu
            inputFieldComponent.textComponent = inputTextComponent;

            inputFieldComponent.characterLimit = 6; // Maximal 6 Zeichen eingeben
            inputFieldComponent.text = "1000";

            void FuncThatCallsFunc(string value) => onEndEditCheck(inputFieldComponent, value, labelText);
            inputFieldComponent.onEndEdit.AddListener((UnityAction<string>)FuncThatCallsFunc);

            inputFieldComponent.lineType = InputField.LineType.SingleLine; // Einzeiliges Eingabefeld
            inputFieldComponent.contentType = InputField.ContentType.IntegerNumber; // Nur Zahlen erlauben

            // Erstelle ein neues GameObject für das "$"-Symbol
            GameObject prefixObject = new GameObject($"{labelText} Prefix");
            prefixObject.transform.SetParent(inputObject.transform, false);

            // Füge eine Text-Komponente für das "$"-Symbol hinzu
            Text prefixTextComponent = prefixObject.AddComponent<Text>();
            prefixTextComponent.text = prefixText;
            prefixTextComponent.font = FontLoader.openSansSemiBold ?? Resources.GetBuiltinResource<Font>("Arial.ttf");
            prefixTextComponent.fontSize = 18;
            prefixTextComponent.color = ColorUtil.GetColor("Bright Green");
            prefixTextComponent.alignment = TextAnchor.MiddleLeft;

            // Positioniere das "$"-Symbol innerhalb des Inputfelds
            RectTransform prefixRect = prefixObject.GetComponent<RectTransform>();
            if (prefixRect == null)
            {
                prefixRect = prefixObject.AddComponent<RectTransform>();
            }
            prefixRect.anchorMin = new Vector2(0, 0.5f);
            prefixRect.anchorMax = new Vector2(0, 0.5f);
            prefixRect.pivot = new Vector2(0, 0.5f);
            prefixRect.anchoredPosition = new Vector2(5, 0); // Leicht nach rechts verschoben

            // Verschiebe den Text des Inputfelds nach rechts, um Platz für das "$"-Symbol zu schaffen
            RectTransform inputTextRect = inputTextObject.GetComponent<RectTransform>();
            if (inputTextRect != null)
            {
                inputTextRect.offsetMin = new Vector2(-20, -50); // Verschiebe den linken Rand nach rechts
            }

            //  MelonLogger.Msg($"Added Label, Prefix '$', and InputField for '{labelText}' to Horizontal Container.");
        }

        void onEndEditCheck(InputField inputFieldComponent, string value, string labelText)
        {
            string input = value.Trim(); // Entferne Leerzeichen am Anfang und Ende der Eingabe
            if (labelText == "Maximum Launder")
            {
                // Überprüfe, ob die Eingabe leer ist oder kleiner als 1000
                if (string.IsNullOrEmpty(input) || int.TryParse(input, out int parsedValue) && parsedValue < 1000)
                {
                    inputFieldComponent.text = "1000";
                    MelonLogger.Warning($"Maximum Launder value was < 1000. Applying '$1000' as value.");
                }
            }
            else if (labelText == "Launder Time")
            {
                // Überprüfe, ob die Eingabe leer ist oder kleiner als 4
                if (string.IsNullOrEmpty(input) || int.TryParse(input, out int parsedValue) && parsedValue < 4)
                {
                    inputFieldComponent.text = "24";
                    MelonLogger.Warning($"Launder Time was < 4. Applying '24 hrs' as value.");
                }
            }
            else if (labelText == "Taxation")
            {
                if (string.IsNullOrEmpty(input) || float.TryParse(input, out float parsedValue) && parsedValue < 1)
                {
                    inputFieldComponent.text = "19";
                    MelonLogger.Warning($"Taxation < 1. Applying '19 %' as value.");
                }
            }
            else if (labelText.Contains("Post Office") || labelText.Contains("Taco Tickler") || labelText.Contains("Laundromat") || labelText.Contains("Car Wash"))
            {
                // Überprüfe, ob die Eingabe leer ist oder kleiner als 1000
                if (string.IsNullOrEmpty(input) || float.TryParse(input, out float parsedValue) && parsedValue < 1000)
                {
                    inputFieldComponent.text = "1000";
                    MelonLogger.Warning($"{labelText} was < 1000. Applying '$1000' as value.");
                }
            }


        }

        void AddSaveButton(Transform parentTransform, string saveString = null)
        {
            if (parentTransform == null)
            {
                MelonLogger.Error("Parent transform is null! Cannot add Save Button.");
                return;
            }

            GameObject saveSpaceObject = new GameObject("SaveSpace");
            saveSpaceObject.transform.SetParent(parentTransform, false);
            RectTransform saveSpaceRect = saveSpaceObject.AddComponent<RectTransform>();
            saveSpaceRect.sizeDelta = new Vector2(100, 35); // Abstand zwischen dem letzten Element und dem Button

            LayoutElement spaceLayoutElement = saveSpaceObject.GetComponent<LayoutElement>();
            if (spaceLayoutElement == null)
            {
                spaceLayoutElement = saveSpaceObject.AddComponent<LayoutElement>();
                spaceLayoutElement.minHeight = 35; // Mindesthöhe des Space
                spaceLayoutElement.preferredHeight = 35; // Bevorzugte Höhe des Space
                spaceLayoutElement.minWidth = 100; // Mindestbreite des Space
                spaceLayoutElement.preferredWidth = 100; // Bevorzugte Breite des Space
            }

            // Erstelle ein neues GameObject für den Button
            GameObject saveButtonObject = new GameObject("Save Button");
            saveButtonObject.transform.SetParent(parentTransform, false);
            saveButtonObject.SetActive(false); // Deaktiviere den Button vorübergehend

            // Füge ein RectTransform hinzu, falls es nicht existiert
            RectTransform buttonRect = saveButtonObject.GetComponent<RectTransform>();
            if (buttonRect == null)
            {
                buttonRect = saveButtonObject.AddComponent<RectTransform>();
                buttonRect.sizeDelta = new Vector2(360, 35); // Standardgröße des Buttons
            }

            // Füge ein Button-Objekt hinzu
            Button saveButton = saveButtonObject.GetComponent<Button>();
            if (saveButton == null)
            {
                saveButton = saveButtonObject.AddComponent<Button>();
            }

            // Füge ein Hintergrundbild hinzu
            Image buttonImage = saveButtonObject.GetComponent<Image>();
            if (buttonImage == null)
            {
                buttonImage = saveButtonObject.AddComponent<Image>();
                if (saveButtonSprite == null)
                {
                    string imagePath = Path.Combine(ConfigFolder, "SaveButton.png");
                    if (!string.IsNullOrEmpty(imagePath) && File.Exists(imagePath))
                    {
                        byte[] imageData = File.ReadAllBytes(imagePath);
                        Texture2D texture = new Texture2D(2, 2);
                        if (texture.LoadImage(imageData))
                        {
                            Sprite newSprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                            saveButtonSprite = newSprite;
                            buttonImage.sprite = newSprite;
                        }
                        else
                        {
                            MelonLogger.Error($"Failed to load image from path: {imagePath}");
                            buttonImage.color = new Color(0.2f, 0.6f, 0.2f); // Grüne Farbe für den Button
                        }
                    }
                }
                else
                {
                    buttonImage.sprite = saveButtonSprite;
                }
            }

            // Erstelle ein neues GameObject für den Text des Buttons
            GameObject buttonTextObject = new GameObject("Save Button Text");
            buttonTextObject.transform.SetParent(saveButtonObject.transform, false);

            // Füge eine Text-Komponente hinzu
            Text buttonText = buttonTextObject.GetComponent<Text>();
            if (buttonText == null)
            {
                buttonText = buttonTextObject.AddComponent<Text>();
                buttonText.text = "Save & Apply";
                buttonText.font = FontLoader.openSansSemiBold ?? Resources.GetBuiltinResource<Font>("Arial.ttf");
                buttonText.fontSize = 18;
                buttonText.color = Color.white;
                buttonText.alignment = TextAnchor.MiddleCenter;
            }

            // Positioniere den Text innerhalb des Buttons
            RectTransform textRect = buttonTextObject.GetComponent<RectTransform>();
            if (textRect == null)
            {
                textRect = buttonTextObject.AddComponent<RectTransform>();
            }
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;

            // Füge ein LayoutElement hinzu, um die Integration in die VerticalLayoutGroup zu gewährleisten
            LayoutElement layoutElement = saveButtonObject.GetComponent<LayoutElement>();
            if (layoutElement == null)
            {
                layoutElement = saveButtonObject.AddComponent<LayoutElement>();
                layoutElement.minHeight = 35; // Mindesthöhe des Buttons
                layoutElement.preferredHeight = 35; // Bevorzugte Höhe des Buttons
                layoutElement.minWidth = 200; // Mindestbreite des Buttons
                layoutElement.preferredWidth = 360; // Bevorzugte Breite des Buttons
            }


            if (saveString == "BusinessDetails")
            {
                void FuncThatCallsFunc() => SaveBusinessDetails(buttonText);
                saveButton.onClick.AddListener((UnityAction)FuncThatCallsFunc);
            }

            if (saveString == "RealEstate")
            {
                void FuncThatCallsFunc() => SaveRealEstate(buttonText);
                saveButton.onClick.AddListener((UnityAction)FuncThatCallsFunc);
            }

        }

        void SaveBusinessDetails(Text buttonText)
        {
            isSaveStillRunning = true;
            buttonText.text = "Saving..";
            if (_selectedBusiness == null)
            {
                MelonLogger.Error("No business is currently selected! Cannot save details.");
                buttonText.text = "Save & Apply";
                isSaveStillRunning = false;
                return;
            }

            // Greife auf die Eingabefelder zu und speichere die Werte
            Transform maxLaunderInputTransform = optionsTransform.Find("Maximum Launder Horizontal Container/Maximum Launder Input");
            Transform launderTimeInputTransform = optionsTransform.Find("Launder Time Horizontal Container/Launder Time Input");
            Transform taxationInputTransform = optionsTransform.Find("Taxation Horizontal Container/Taxation Input");

            float maxLaunderValue = _selectedBusiness.LaunderCapacity;
            int launderTimeValue = MRLCore.Instance.config.Laundromat_Laundering_time_hours; // Standardwert
            float taxationValue = MRLCore.Instance.config.Laundromat_Tax_Percentage; // Standardwert

            if (maxLaunderInputTransform != null)
            {
                InputField maxLaunderInputField = maxLaunderInputTransform.GetComponent<InputField>();
                if (maxLaunderInputField != null && float.TryParse(maxLaunderInputField.text, out float parsedMaxLaunderValue))
                {
                    maxLaunderValue = parsedMaxLaunderValue;
                    //    MelonLogger.Msg($"Updated Launder Capacity for {_selectedBusiness.name}: {maxLaunderValue}");
                }
            }

            if (launderTimeInputTransform != null)
            {
                InputField launderTimeInputField = launderTimeInputTransform.GetComponent<InputField>();
                if (launderTimeInputField != null && int.TryParse(launderTimeInputField.text, out int parsedLaunderTimeValue))
                {
                    launderTimeValue = parsedLaunderTimeValue;
                    //    MelonLogger.Msg($"Updated Launder Time for {_selectedBusiness.name}: {launderTimeValue} hours");
                }
            }

            if (taxationInputTransform != null)
            {
                InputField taxationInputField = taxationInputTransform.GetComponent<InputField>();
                if (taxationInputField != null && float.TryParse(taxationInputField.text, out float parsedTaxationValue))
                {
                    taxationValue = parsedTaxationValue;
                    //    MelonLogger.Msg($"Updated Taxation for {_selectedBusiness.name}: {taxationValue}%");
                }
            }

            // Speichere die Werte in der Config
            switch (_selectedBusiness.name.ToLower())
            {
                case "laundromat":
                    MRLCore.Instance.config.Laundromat_Cap = maxLaunderValue;
                    MRLCore.Instance.config.Laundromat_Laundering_time_hours = launderTimeValue;
                    MRLCore.Instance.config.Laundromat_Tax_Percentage = taxationValue;
                    break;

                case "taco ticklers":
                case "tacoticklers":
                    MRLCore.Instance.config.Taco_Ticklers_Cap = maxLaunderValue;
                    MRLCore.Instance.config.Taco_Ticklers_Laundering_time_hours = launderTimeValue;
                    MRLCore.Instance.config.Taco_Ticklers_Tax_Percentage = taxationValue;
                    break;

                case "car wash":
                case "carwash":
                    MRLCore.Instance.config.Car_Wash_Cap = maxLaunderValue;
                    MRLCore.Instance.config.Car_Wash_Laundering_time_hours = launderTimeValue;
                    MRLCore.Instance.config.Car_Wash_Tax_Percentage = taxationValue;
                    break;

                case "post office":
                case "postoffice":
                    MRLCore.Instance.config.Post_Office_Cap = maxLaunderValue;
                    MRLCore.Instance.config.Post_Office_Laundering_time_hours = launderTimeValue;
                    MRLCore.Instance.config.Post_Office_Tax_Percentage = taxationValue;
                    break;

                default:
                    MelonLogger.Warning($"Unknown business: {_selectedBusiness.name}. Config not updated.");
                    isSaveStillRunning = false;
                    buttonText.text = "Save & Apply";
                    return;
            }

            // Speichere die aktualisierte Config
            ConfigManager.Save(MRLCore.Instance.config);
            //MelonLogger.Msg($"Config updated and saved for {_selectedBusiness.name}.");

            // Wende die Änderungen auf das Unternehmen an
            MRLCore.Instance.FillCapDictionary();
            if (MRLCore.Instance.processedBusinesses.Contains(_selectedBusiness.name))
            {
                MRLCore.Instance.processedBusinesses.Remove(_selectedBusiness.name);
                //    MelonLogger.Msg($"Removed {_selectedBusiness.name} from processedBusinesses.");
            }
            if (!MRLCore.Instance.isWaitAndApplyCapsRunning)
            {
                MelonCoroutines.Start(MRLCore.Instance.WaitAndApplyCaps());
            }
            isSaveStillRunning = false;
            buttonText.text = "Save & Apply";


            string displayName = _selectedBusiness.name;
            if (displayName == "PostOffice")
            {
                displayName = "Post Office";
            }

            if (appIconSprite == null)
            {
                string imagePath = Path.Combine(ConfigFolder, "LaunderingIcon.png");
                if (!string.IsNullOrEmpty(imagePath) && File.Exists(imagePath))
                {
                    byte[] imageData = File.ReadAllBytes(imagePath);
                    Texture2D texture = new Texture2D(2, 2);
                    if (texture.LoadImage(imageData))
                    {
                        Sprite newSprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                        appIconSprite = newSprite;
                    }
                    else
                    {
                        MelonLogger.Error($"Failed to load image from path: {imagePath}");
                    }
                }
            }


            if (MRLCore.Instance.notificationsManager != null)
            {
                string subTitleString = $"for <color=#329AC5>{displayName}</color>";
                MRLCore.Instance.notificationsManager.SendNotification("Saved Changes", subTitleString, appIconSprite, 5, true);
            }
            MelonLogger.Msg($"Details for {displayName} have been saved and applied successfully.");
        }

        void SaveRealEstate(Text buttonText)
        {
            isSaveStillRunning = true;
            buttonText.text = "Saving...";

            if (priceOptionsTransform == null)
            {
                MelonLogger.Error("priceOptionsTransform is null! Cannot save real estate prices.");
                buttonText.text = "Save & Apply";
                isSaveStillRunning = false;
                return;
            }

            // Greife auf die Eingabefelder zu und speichere die Werte
            Transform laundromatPriceInputTransform = priceOptionsTransform.Find("Laundromat Horizontal Container/Laundromat Input");
            Transform tacoTicklersPriceInputTransform = priceOptionsTransform.Find("Taco Ticklers Horizontal Container/Taco Ticklers Input");
            Transform carWashPriceInputTransform = priceOptionsTransform.Find("Car Wash Horizontal Container/Car Wash Input");
            Transform postOfficePriceInputTransform = priceOptionsTransform.Find("Post Office Horizontal Container/Post Office Input");

            float laundromatPrice = MRLCore.Instance.config.Laundromat_Price;
            float tacoTicklersPrice = MRLCore.Instance.config.Taco_Ticklers_Price;
            float carWashPrice = MRLCore.Instance.config.Car_Wash_Price;
            float postOfficePrice = MRLCore.Instance.config.Post_Office_Price;

            //   MelonLogger.Msg($"Current Prices: {laundromatPrice}, {tacoTicklersPrice}, {carWashPrice}, {postOfficePrice}");

            // Aktualisiere den Preis für Laundromat
            if (laundromatPriceInputTransform != null)
            {
                InputField laundromatPriceInputField = laundromatPriceInputTransform.GetComponent<InputField>();
                if (laundromatPriceInputField != null && float.TryParse(laundromatPriceInputField.text, out float parsedLaundromatPrice))
                {
                    laundromatPrice = parsedLaundromatPrice;
                }
            }

            // Aktualisiere den Preis für Taco Ticklers
            if (tacoTicklersPriceInputTransform != null)
            {
                InputField tacoTicklersPriceInputField = tacoTicklersPriceInputTransform.GetComponent<InputField>();
                if (tacoTicklersPriceInputField != null && float.TryParse(tacoTicklersPriceInputField.text, out float parsedTacoTicklersPrice))
                {
                    tacoTicklersPrice = parsedTacoTicklersPrice;
                }
            }

            // Aktualisiere den Preis für Car Wash
            if (carWashPriceInputTransform != null)
            {
                InputField carWashPriceInputField = carWashPriceInputTransform.GetComponent<InputField>();
                if (carWashPriceInputField != null && float.TryParse(carWashPriceInputField.text, out float parsedCarWashPrice))
                {
                    carWashPrice = parsedCarWashPrice;
                }
            }

            // Aktualisiere den Preis für Post Office
            if (postOfficePriceInputTransform != null)
            {
                InputField postOfficePriceInputField = postOfficePriceInputTransform.GetComponent<InputField>();
                if (postOfficePriceInputField != null && float.TryParse(postOfficePriceInputField.text, out float parsedPostOfficePrice))
                {
                    postOfficePrice = parsedPostOfficePrice;
                }
            }

            //   MelonLogger.Msg($"Updated Prices: {laundromatPrice}, {tacoTicklersPrice}, {carWashPrice}, {postOfficePrice}");

            // Speichere die aktualisierten Preise in der Konfiguration
            MRLCore.Instance.config.Laundromat_Price = laundromatPrice;
            MRLCore.Instance.config.Taco_Ticklers_Price = tacoTicklersPrice;
            MRLCore.Instance.config.Car_Wash_Price = carWashPrice;
            MRLCore.Instance.config.Post_Office_Price = postOfficePrice;

            // Speichere die aktualisierte Konfiguration
            ConfigManager.Save(MRLCore.Instance.config);

            // Aktualisiere die Preise in den PropertyListings
            MRLCore.Instance.ApplyPricesToPropertyListings();
            MRLCore.Instance.ApplyPricesToUnownedBusinesses();

            // Zeige eine Benachrichtigung an
            if (MRLCore.Instance.notificationsManager != null)
            {
                string subTitleString = "Prices updated.";

                if (appIconSprite == null)
                {
                    string imagePath = Path.Combine(ConfigFolder, "LaunderingIcon.png");
                    if (!string.IsNullOrEmpty(imagePath) && File.Exists(imagePath))
                    {
                        byte[] imageData = File.ReadAllBytes(imagePath);
                        Texture2D texture = new Texture2D(2, 2);
                        if (texture.LoadImage(imageData))
                        {
                            Sprite newSprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                            appIconSprite = newSprite;
                        }
                        else
                        {
                            MelonLogger.Error($"Failed to load image from path: {imagePath}");
                        }
                    }
                }
                MRLCore.Instance.notificationsManager.SendNotification("Prices Saved", subTitleString, appIconSprite, 5, true);
            }

            buttonText.text = "Save & Apply";
            isSaveStillRunning = false;

            MelonLogger.Msg("Real estate prices have been saved and applied successfully.");
        }

        public void AddPriceOptionsForRealEstate(Transform parentTransform)
        {
            priceOptionsTransform = parentTransform;
            if (parentTransform == null)
            {
                MelonLogger.Error("detailEntriesTransform is null! Cannot add price options.");
                return;
            }

            // Füge ein VerticalLayoutGroup hinzu, falls es nicht existiert
            VerticalLayoutGroup optionVerticalLayout = priceOptionsTransform.GetComponent<VerticalLayoutGroup>();
            if (optionVerticalLayout == null)
            {
                optionVerticalLayout = priceOptionsTransform.gameObject.AddComponent<VerticalLayoutGroup>();
            }

            // Konfiguriere das Layout
            optionVerticalLayout.childControlWidth = true;
            optionVerticalLayout.childForceExpandWidth = false;
            optionVerticalLayout.childAlignment = TextAnchor.UpperLeft;
            optionVerticalLayout.spacing = -20f; // Abstand zwischen den Optionen

            // Füge die Label-Input-Paare für die vier Unternehmen hinzu
            AddLabelInputPair("Laundromat", priceOptionsTransform, "$");
            AddLabelInputPair("Post Office", priceOptionsTransform, "$");
            AddLabelInputPair("Car Wash", priceOptionsTransform, "$");
            AddLabelInputPair("Taco Ticklers", priceOptionsTransform, "$");
            /* AddLabelInputPair("Motel", priceOptionsTransform, "$");
             AddLabelInputPair("Bungalow", priceOptionsTransform, "$");
             AddLabelInputPair("Barn", priceOptionsTransform, "$");
             AddLabelInputPair("Docks Warehouse", priceOptionsTransform, "$"); */

            // Füge einen Save-Button hinzu
            AddSaveButton(priceOptionsTransform, "RealEstate");

            MelonLogger.Msg("Added price options for real estate to priceOptionsTransform.");
        }

        public GameObject dansHardwareTemplate;
        public GameObject gasMartWestTemplate;
        public GameObject viewPortContentSpaceTemplate;
        public Transform launderingAppViewportContentTransform;
        private static readonly string ConfigFolder = Path.Combine(MelonEnvironment.UserDataDirectory, "MoreRealisticLaundering");
        private static readonly string FilePath = Path.Combine(ConfigFolder, "LaunderingIcon.png");
        public static string _appName;
        public bool _isLaunderingAppLoaded = false;
        public Transform optionsTransform = null;
        public Transform priceOptionsTransform = null;
        Sprite inputBackgroundSprite = null;
        Sprite saveButtonSprite = null;
        Sprite appIconSprite = null;
        private Business _selectedBusiness = null;
        private string _selectedBusinessName = null;
        public bool isSaveStillRunning = false;
    }
}