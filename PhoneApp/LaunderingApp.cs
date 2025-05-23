using System.Collections;
using Il2CppFluffyUnderware.DevTools.Extensions;
using Il2CppScheduleOne.Dialogue;
using Il2CppScheduleOne.Property;
using Il2CppScheduleOne.Vehicles;
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
            while (MRLCore.Instance == null)
            {
                MelonLogger.Msg("Waiting for Instance to be initialized...");
                yield return new WaitForSeconds(1f);
            }

            MelonCoroutines.Start(FontLoader.InitializeFonts());
            while (!Util.FontLoader.openSansBoldIsInitialized || !Util.FontLoader.openSansSemiBoldIsInitialized)
            {
                MelonLogger.Msg("Waiting for Fonts to be loaded...");
                yield return new WaitForSeconds(2f);
            }
            yield return MelonCoroutines.Start(CreateApp("TaxNWash", "Tax & Wash", true, FilePath));
            yield break;
        }

        public IEnumerator CreateApp(string IDName, string Title, bool IsRotated = true, string IconPath = null)
        {
            GameObject cloningCandidateProducts = null;
            string cloningNameProducts = null;
            GameObject cloningCandidateDeliveries = null;
            string cloningNameDeliveries = null;
            GameObject cloningCandidateProductsCopy = null;
            string cloningNameProductsCopy = null;
            GameObject icons = null;

            // Warte auf das AppIcons-Objekt
            yield return MelonCoroutines.Start(Utils.WaitForObject(
                "Player_Local/CameraContainer/Camera/OverlayCamera/GameplayMenu/Phone/phone/HomeScreen/AppIcons/",
                delegate (GameObject obj)
                {
                    icons = obj;
                }
            ));

            // Bestimme das CloningCandidate basierend auf IsRotated
            if (IsRotated)
            {
                yield return MelonCoroutines.Start(Utils.WaitForObject(
                   "Player_Local/CameraContainer/Camera/OverlayCamera/GameplayMenu/Phone/phone/AppsCanvas/DeliveryApp",
                   delegate (GameObject objD)
                   {
                       cloningCandidateDeliveries = objD;
                       cloningNameDeliveries = "Deliveries";
                   }
               ));

                yield return MelonCoroutines.Start(Utils.WaitForObject(
                     "Player_Local/CameraContainer/Camera/OverlayCamera/GameplayMenu/Phone/phone/AppsCanvas/ProductManagerApp",
                     delegate (GameObject objP)
                     {
                         cloningCandidateProductsCopy = objP;
                         cloningNameProductsCopy = "Products";
                     }
                 ));

                yield return MelonCoroutines.Start(Utils.WaitForObject(
                    "Player_Local/CameraContainer/Camera/OverlayCamera/GameplayMenu/Phone/phone/AppsCanvas/ProductManagerApp",
                    delegate (GameObject obj)
                    {
                        cloningCandidateProducts = obj;
                        cloningNameProducts = "Products";
                    }
                ));
            }

            // Klone das App-Canvas
            GameObject parentCanvas = GameObject.Find("Player_Local/CameraContainer/Camera/OverlayCamera/GameplayMenu/Phone/phone/AppsCanvas/");

            GameObject deliveriesApp = UnityEngine.Object.Instantiate(cloningCandidateDeliveries, parentCanvas.transform);
            deliveriesApp.name = "DeliveriesTemp";

            GameObject productsApp = UnityEngine.Object.Instantiate(cloningCandidateProductsCopy, parentCanvas.transform);
            deliveriesApp.name = "ProductsTemp";

            GameObject mrlApp = UnityEngine.Object.Instantiate(cloningCandidateProducts, parentCanvas.transform);
            mrlApp.name = IDName;

            // Aktualisiere den Namen vom App-Icon für das Deliveries Copy Object
            GameObject appIconByNameDeliveries = Utils.ChangeLabelFromAppIcon(cloningNameDeliveries, "DeliveriesCopy");

            // Aktualisiere den Namen vom App-Icon für das Products Copy Object
            GameObject appIconByNameProducts = Utils.ChangeLabelFromAppIcon(cloningNameProductsCopy, "ProductsCopy");

            Transform container = mrlApp.transform.Find("Container");

            //Adjust Topbar for Sleep App
            Transform topbarTransform = container.Find("Topbar");
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

                Transform topbarSubtitleTransform = topbarTransform.Find("Subtitle");
                if (topbarSubtitleTransform != null)
                {
                    topbarSubtitleTransform.GetComponent<Text>().text = "by KampfBallerina";
                    topbarSubtitleTransform.GetComponent<Text>().alignment = TextAnchor.MiddleRight;
                    topbarSubtitleTransform.GetComponent<Text>().color = ColorUtil.GetColor("White");
                    topbarSubtitleTransform.GetComponent<RectTransform>().anchorMax = new Vector2(0.98f, 1);
                }
            }

            // Scroll View from the Sleep App
            Transform scrollViewProductsApp = container.Find("Scroll View");
            if (scrollViewProductsApp != null)
            {
                // This needs to stay to avoid the Meth Oven to brick
                scrollViewProductsApp.gameObject.SetActive(false);
                scrollViewProductsApp.gameObject.name = "DeactivatedScrollView";
                /* This needs to stay since new products try to find the SleepingApp ScrollView*/
            }

            Transform containerTransformClonedDeliveriesApp = deliveriesApp.transform.Find("Container");
            if (containerTransformClonedDeliveriesApp != null)
            {
                Transform scrollViewTransformDeliveriesClone = containerTransformClonedDeliveriesApp.Find("Scroll View");
                if (scrollViewTransformDeliveriesClone != null)
                {

                    Transform scrollViewViewport = scrollViewTransformDeliveriesClone.transform.Find("Viewport");
                    if (scrollViewViewport != null)
                    {
                        Transform scrollViewContent = scrollViewViewport.transform.Find("Content");
                        /* Utils.ClearChildren(scrollViewContent, child =>
                             child.name == "Space" ||
                             child == scrollViewContent.GetChild(0).gameObject ||
                             child.transform.IsChildOf(scrollViewContent.GetChild(0))
                         );*/
                    }

                    Transform orderSubmitted = scrollViewTransformDeliveriesClone.transform.Find("OrderSubmitted");
                    if (orderSubmitted != null)
                    {
                        UnityEngine.Object.DestroyImmediate(orderSubmitted.gameObject);
                    }
                    scrollViewTransformDeliveriesClone.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(-100, -60); // Set the sizeDelta to match the original
                    scrollViewTransformDeliveriesClone.transform.GetComponent<RectTransform>().anchoredPosition = new Vector2(-50, -30); // Set the anchoredPosition to match the original
                    scrollViewTransformDeliveriesClone.SetParent(container, false);
                }
            }

            Transform settingsTransform = container.Find("Details");
            if (settingsTransform != null)
            {
                // Ändere den Namen des Details-Objekts
                settingsTransform.name = "Details";
                settingsTransform.GetComponent<RectTransform>().sizeDelta = new Vector2(95f, -60f); // Setze die Größe des Details-Objekts
                settingsTransform.GetComponent<RectTransform>().anchoredPosition = new Vector2(-47.5f, -30f); // Setze die Position des Details-Objekts

                Transform detailsTitleTransform = settingsTransform.Find("Scroll View/Viewport/Content/Title");
                if (detailsTitleTransform != null)
                {
                    detailsTitleObject = detailsTitleTransform.gameObject;
                    detailsTitleObject.SetActive(false);
                    Text detailsTitleText = detailsTitleTransform.GetComponent<Text>();
                    detailsTitleText.text = "DetailsTitleText";
                }

                Transform detailsSubtitleTransform = settingsTransform.Find("Scroll View/Viewport/Content/Description");
                if (detailsSubtitleTransform != null)
                {
                    detailsSubtitleTransform.GetComponent<Text>().text = "This is going to be the description of the app.";
                    detailsSubtitleTransform.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0); // Set the anchoredPosition to match the original
                    detailsSubtitleTransform.GetComponent<RectTransform>().sizeDelta = new Vector2(400, 50); // Set the sizeDelta to match the original
                    detailsSubtitleTransform.GetComponent<Text>().fontSize = 18;
                    detailsSubtitleTransform.gameObject.SetActive(false);
                    detailsSubtitleObject = detailsSubtitleTransform.gameObject;
                }

                //  MelonLogger.Msg("Deleting unneeded objects");
                settingsContentTransform = settingsTransform.Find("Scroll View/Viewport/Content");
                Transform toDeleteTransform = settingsContentTransform.Find("Value");
                toDeleteTransform.gameObject.Destroy(); // Zerstöre das Value-Objekt
                toDeleteTransform = settingsContentTransform.Find("Effects");
                toDeleteTransform.gameObject.Destroy(); // Zerstöre das Effects-Objekt
                toDeleteTransform = settingsContentTransform.Find("Addiction");
                toDeleteTransform.gameObject.Destroy(); // Zerstöre das Addiction-Objekt
                toDeleteTransform = settingsContentTransform.Find("Recipes");
                toDeleteTransform.gameObject.Destroy(); // Zerstöre das Recipes-Objekt
                toDeleteTransform = settingsContentTransform.Find("RecipesContainer");
                toDeleteTransform.gameObject.Destroy(); // Zerstöre das RecipesContainer-Objekt
                toDeleteTransform = settingsContentTransform.Find("Listed");
                toDeleteTransform.gameObject.Destroy(); // Zerstöre das Listed-Objekt
                toDeleteTransform = settingsContentTransform.Find("Delisted");
                toDeleteTransform.gameObject.Destroy(); // Zerstöre das Delisted-Objekt
                toDeleteTransform = settingsContentTransform.Find("NotDiscovered");
                toDeleteTransform.gameObject.Destroy(); // Zerstöre das NotDiscovered-Objekt
                toDeleteTransform = settingsContentTransform.Find("Properties");
                toDeleteTransform.gameObject.Destroy(); // Zerstöre das Properties-Objekt

                Transform instructionTransform = settingsTransform.Find("Instruction");
                if (instructionTransform != null)
                {
                    InstructionsTextObject = instructionTransform.gameObject;
                    Text instructionsText = instructionTransform.GetComponent<Text>();
                    instructionsText.text = "Select a section to adjust settings";
                    instructionsText.fontSize = 20;

                }
                
                // Adjust the Spacing for the right Scroll View, so that the Title is on top
                Transform settingsSpaceTransform = settingsContentTransform.Find("Space");
                if (settingsSpaceTransform != null)
                {
                    settingsSpaceTransform.gameObject.SetActive(true); // Hide
                    RectTransform settingsSpaceRect = settingsSpaceTransform.GetComponent<RectTransform>();
                    settingsSpaceRect.sizeDelta = new Vector2(settingsSpaceRect.sizeDelta.x, 200f);
                }            
            }

            CreateTemplates(container, settingsContentTransform);

            // Destroy the Temp Apps
            UnityEngine.Object.DestroyImmediate(appIconByNameDeliveries.gameObject);
            deliveriesApp.Destroy();
            UnityEngine.Object.DestroyImmediate(appIconByNameProducts.gameObject);
            productsApp.Destroy();

            // Aktualisiere den Namen vom App-Icon für das Sleeping App Object
            GameObject appIconByName = Utils.ChangeLabelFromAppIcon(cloningNameProducts, Title);

            // Ändere das App-Icon-Bild
            MRLCore.Instance.ChangeAppIconImage(appIconByName, IconPath);

            // Registriere die App
            MRLCore.Instance.RegisterApp(appIconByName, Title);
            settingsContentTransform.gameObject.SetActive(true);


            //AddSpaceFromTemplate(viewportContentTransform);
            AddOptionsForBusiness(settingsContentTransform);
            AddPriceOptionsWithToggleButton(settingsContentTransform);
            AddVehicleOptions(settingsContentTransform);
            AddSkateboardOptions(settingsContentTransform);
            _isLaunderingAppLoaded = true;


        }

        void CreateTemplates(Transform deliveriesContainer, Transform productsContainer = null)
        {
            if (deliveriesContainer == null)
            {
                MelonLogger.Error("Container not found!");
                return;
            }

            //Products Templates
            if (productsContainer != null)
            {
                Transform toggleTransform = productsContainer.Find("Toggle");
                if (toggleTransform != null)
                {
                    RectTransform toggleRectTransform = toggleTransform.GetComponent<RectTransform>();
                    if (toggleRectTransform != null)
                    {
                        // toggleRectTransform.anchoredPosition = new Vector2(0, 0); // Set the anchoredPosition to match the original
                    }

                    HorizontalLayoutGroup horizontalContainerGroup = toggleTransform.GetComponent<HorizontalLayoutGroup>();
                    if (horizontalContainerGroup == null)
                    {
                        horizontalContainerGroup = toggleTransform.gameObject.AddComponent<HorizontalLayoutGroup>();
                        horizontalContainerGroup.spacing = 100f; // Abstand zwischen den Elementen
                        horizontalContainerGroup.childForceExpandWidth = false; // Breite der Kinder nicht erzwingen
                        horizontalContainerGroup.childForceExpandHeight = false; // Höhe der Kinder nicht erzwingen
                        horizontalContainerGroup.childControlWidth = true; // Breite der Kinder steuern
                        horizontalContainerGroup.childControlHeight = true; // Höhe der Kinder steuern
                        horizontalContainerGroup.childAlignment = TextAnchor.MiddleLeft; // Elemente linksbündig ausrichten
                    }

                    // Füge einen ContentSizeFitter hinzu, um die Breite automatisch anzupassen
                    ContentSizeFitter contentSizeFitter = toggleTransform.GetComponent<ContentSizeFitter>();
                    if (contentSizeFitter == null)
                    {
                        contentSizeFitter = toggleTransform.gameObject.AddComponent<ContentSizeFitter>();
                        contentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize; // Passe die Breite an den Inhalt an
                        contentSizeFitter.verticalFit = ContentSizeFitter.FitMode.Unconstrained; // Höhe nicht anpassen
                    }


                    Transform toggleTextTransform = toggleTransform.Find("Text");
                    if (toggleTextTransform != null)
                    {
                        Text textComponent = toggleTextTransform.GetComponent<Text>();
                        textComponent.font = FontLoader.openSansSemiBold ?? Resources.GetBuiltinResource<Font>("Arial.ttf");
                        textComponent.text = "toggleTransformText";
                        textComponent.color = ColorUtil.GetColor("White");
                        textComponent.fontSize = 18;
                        toggleTextTransform.SetAsFirstSibling();
                        toggleTextTransform.GetComponent<RectTransform>().sizeDelta = new Vector2(325, 30);

                        LayoutElement labelLayoutElement = toggleTextTransform.GetComponent<LayoutElement>();
                        if (labelLayoutElement == null)
                        {
                            labelLayoutElement = toggleTextTransform.gameObject.AddComponent<LayoutElement>();
                            labelLayoutElement.minWidth = 325; // Mindestbreite des Labels
                        }
                    }

                    Toggle toggleToggle = toggleTransform.GetComponent<Toggle>();
                    if (toggleToggle != null)
                    {
                        toggleToggle.isOn = false; // Set the toggle to off
                        toggleToggle.onValueChanged.RemoveAllListeners(); // Remove existing listeners to avoid old functionality after copying
                    }
                    Transform backgroundTransform = toggleTransform.Find("Background");
                    if (backgroundTransform != null)
                    {
                        backgroundTransform.name = "CheckboxBackground";
                        LayoutElement checkboxLayoutElement = backgroundTransform.GetComponent<LayoutElement>();
                        if (checkboxLayoutElement == null)
                        {
                            checkboxLayoutElement = backgroundTransform.gameObject.AddComponent<LayoutElement>();
                            checkboxLayoutElement.minWidth = 30; // Mindestbreite des Labels
                            checkboxLayoutElement.preferredWidth = 30; // Bevorzugte Breite des Labels
                            checkboxLayoutElement.minHeight = 30; // Mindesthöhe des Labels
                            checkboxLayoutElement.preferredHeight = 30; // Bevorzugte Höhe des Labels

                        }
                    }
                    Transform checkmarkTransform = backgroundTransform.Find("Checkmark");
                    if (checkmarkTransform != null)
                    {
                        checkmarkTransform.GetComponent<Image>().color = ColorUtil.GetColor("Cyan");
                    }

                    RectTransform checkboxRect = toggleTransform.gameObject.GetComponent<RectTransform>();
                    if (checkboxRect != null)
                    {
                        checkboxRect.sizeDelta = new Vector2(30, 30);
                    }

                    // Save as Template
                    checkboxTemplate = UnityEngine.Object.Instantiate(toggleTransform.gameObject);
                    checkboxTemplate.name = "Checkbox Template";
                    checkboxTemplate.SetActive(true);

                    toggleTransform.gameObject.Destroy(); // Zerstöre das ursprüngliche GameObject
                }
            }


            // Deliveries Templates and Cleaning
            Transform scrollViewTransform = deliveriesContainer.Find("Scroll View");
            if (scrollViewTransform != null)
            {
                Transform orderSubmittedTransform = scrollViewTransform.Find("OrderSubmitted");
                if (orderSubmittedTransform != null)
                {
                    orderSubmittedTransform.gameObject.Destroy(); // Zerstöre das ursprüngliche GameObject
                }

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

                        if (dansHardware != null && DansHardwareTemplate == null)
                        {
                            DansHardwareTemplate = UnityEngine.Object.Instantiate(dansHardware);
                            DansHardwareTemplate.name = "Dan's Hardware Template";
                            DansHardwareTemplate.SetActive(false); // Deaktiviere das Template

                            Transform contentsDansTemplateTransform = DansHardwareTemplate.transform.Find("Contents");
                            if (contentsDansTemplateTransform != null)
                            {
                                UnityEngine.Object.DestroyImmediate(contentsDansTemplateTransform.gameObject); // Entferne den "Contents"
                                                                                                               //   MelonLogger.Msg("Removed Contents from Dans Hardware Template");
                            }

                            UnityEngine.Object.Destroy(dansHardware); // Entferne das ursprüngliche GameObject
                        }

                        if (gasMartWest != null && GasMartWestTemplate == null)
                        {
                            GasMartWestTemplate = UnityEngine.Object.Instantiate(gasMartWest);
                            GasMartWestTemplate.name = "Gas-Mart (West) Template";
                            GasMartWestTemplate.SetActive(false); // Deaktiviere das Template

                            Transform contentsMarketTemplateTransform = GasMartWestTemplate.transform.Find("Contents");
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
                        AddSpaceFromTemplate(viewportContentTransform);
                        //  MelonLogger.Msg("Saved Dan's Hardware, Gas Mart and Space as Templates");
                    }
                    else
                    {
                        MelonLogger.Error("Viewport Content not found!");
                    }
                }
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

            if (template == null && GasMartWestTemplate != null)
            {
                template = GasMartWestTemplate; // Standard-Template verwenden, wenn kein Template angegeben ist
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
                        if (imagePath.Contains("Laundromat.png"))
                        {
                            laundromatSprite = newSprite;
                        }
                        else if (imagePath.Contains("TacoTickler.png"))
                        {
                            tacoTicklersSprite = newSprite;
                        }
                        else if (imagePath.Contains("CarWash.png"))
                        {
                            carWashSprite = newSprite;
                        }
                        else if (imagePath.Contains("PostOffice.png"))
                        {
                            postOfficeSprite = newSprite;
                        }
                        else if (imagePath.Contains("HylandAuto.png"))
                        {
                            hylandAutoSprite = newSprite;
                        }
                        else if (imagePath.Contains("Jeff.png"))
                        {
                            shredShackSprite = newSprite;
                        }
                        else if (imagePath.Contains("RaysRealEstate.png"))
                        {
                            raysRealEstateSprite = newSprite;
                        }
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

            if (newObjectName == "Hyland Auto")
            {
                Button headerButton = headerTransform.GetComponent<Button>();
                if (headerButton != null)
                {
                    headerButton.name = newObjectName + " Button";
                    headerButton.onClick.RemoveAllListeners(); // Remove existing listeners to avoid old functionality after copying
                    void FuncThatCallsFunc() => HylandAutoButtonClicked();
                    headerButton.onClick.AddListener((UnityAction)FuncThatCallsFunc);
                }
            }

            if (newObjectName == "Shred Shack")
            {
                Button headerButton = headerTransform.GetComponent<Button>();
                if (headerButton != null)
                {
                    headerButton.name = newObjectName + " Button";
                    headerButton.onClick.RemoveAllListeners(); // Remove existing listeners to avoid old functionality after copying
                    void FuncThatCallsFunc() => ShredShackButtonClicked();
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

        void ShredShackButtonClicked()
        {
            if (optionsTransform == null)
            {
                MelonLogger.Error("optionsTransform is null! Ensure it is initialized before calling RealEstateButtonClicked.");
                return;
            }

            if (priceOptionsTransform == null)
            {
                MelonLogger.Error("priceOptionsTransform is null! Ensure it is initialized before calling RealEstateButtonClicked.");
                return;
            }

            if (vehicleOptionsTransform == null)
            {
                MelonLogger.Error("vehicleOptionsTransform is null! Ensure it is initialized before calling RealEstateButtonClicked.");
                return;
            }

            if (skateboardOptionsTransform == null)
            {
                MelonLogger.Error("skateboardOptionsTransform is null! Ensure it is initialized before calling RealEstateButtonClicked.");
                return;
            }

            if (InstructionsTextObject.activeSelf)
            {
                InstructionsTextObject.SetActive(false);
            }

            if (!detailsTitleObject.activeSelf)
            {
                detailsTitleObject.SetActive(true);
            }
            detailsTitleObject.GetComponent<Text>().text = "Shred Shack";

            if (!detailsSubtitleObject.activeSelf)
            {
                detailsSubtitleObject.SetActive(true);
            }
            detailsSubtitleObject.GetComponent<Text>().text = "Adjusting the prices of skateboards.";

            // Deaktiviere priceOptionsTransform
            if (priceOptionsTransform.gameObject.activeSelf)
            {
                priceOptionsTransform.gameObject.SetActive(false);
            }

            // Deaktiviere optionsTransform
            if (optionsTransform.gameObject.activeSelf)
            {
                optionsTransform.gameObject.SetActive(false);
            }
            // Deaktiviere vehicleOptionsTransform
            if (vehicleOptionsTransform.gameObject.activeSelf)
            {
                vehicleOptionsTransform.gameObject.SetActive(false);
            }

            // Aktiviere vehicleOptionsTransform
            if (!skateboardOptionsTransform.gameObject.activeSelf)
            {
                skateboardOptionsTransform.gameObject.SetActive(true);
            }

            Transform saveButtonTransform = skateboardOptionsTransform.Find("Save Button");
            if (saveButtonTransform != null && !saveButtonTransform.gameObject.activeSelf)
            {
                saveButtonTransform.gameObject.SetActive(true);
            }

            Transform cheapSkateboardContainer = skateboardOptionsTransform.Find("Cheap Skateboard Horizontal Container");
            Transform skateboardContainer = skateboardOptionsTransform.Find("Skateboard Horizontal Container");
            Transform lightweightBoardContainer = skateboardOptionsTransform.Find("Lightweight Board Horizontal Container");
            Transform cruiserContainer = skateboardOptionsTransform.Find("Cruiser Horizontal Container");
            Transform goldenSkateboardContainer = skateboardOptionsTransform.Find("Golden Skateboard Horizontal Container");

            if (cheapSkateboardContainer != null && !cheapSkateboardContainer.gameObject.activeSelf)
            {
                cheapSkateboardContainer.gameObject.SetActive(true);
            }
            if (skateboardContainer != null && !skateboardContainer.gameObject.activeSelf)
            {
                skateboardContainer.gameObject.SetActive(true);
            }
            if (lightweightBoardContainer != null && !lightweightBoardContainer.gameObject.activeSelf)
            {
                lightweightBoardContainer.gameObject.SetActive(true);
            }
            if (cruiserContainer != null && !cruiserContainer.gameObject.activeSelf)
            {
                cruiserContainer.gameObject.SetActive(true);
            }
            if (goldenSkateboardContainer != null && !goldenSkateboardContainer.gameObject.activeSelf)
            {
                goldenSkateboardContainer.gameObject.SetActive(true);
            }

            foreach (DialogueController_SkateboardSeller.Option option in MRLCore.Instance.shackShopDialogueController.Options)
            {
                if (option == null) continue;
                string displayName = MRLCore.Instance.skateboardAliasMap.ContainsKey(option.Name) ? MRLCore.Instance.skateboardAliasMap[option.Name] : option.Name;
                float price = option.Price;
                SetInputFieldValue(skateboardOptionsTransform, displayName, price);
            }
        }

        void HylandAutoButtonClicked()
        {
            if (optionsTransform == null)
            {
                MelonLogger.Error("optionsTransform is null! Ensure it is initialized before calling RealEstateButtonClicked.");
                return;
            }

            if (priceOptionsTransform == null)
            {
                MelonLogger.Error("priceOptionsTransform is null! Ensure it is initialized before calling RealEstateButtonClicked.");
                return;
            }

            if (vehicleOptionsTransform == null)
            {
                MelonLogger.Error("vehicleOptionsTransform is null! Ensure it is initialized before calling RealEstateButtonClicked.");
                return;
            }

            if (skateboardOptionsTransform == null)
            {
                MelonLogger.Error("skateboardOptionsTransform is null! Ensure it is initialized before calling RealEstateButtonClicked.");
                return;
            }

            if (InstructionsTextObject.activeSelf)
            {
                InstructionsTextObject.SetActive(false);
            }

            if (!detailsTitleObject.activeSelf)
            {
                detailsTitleObject.SetActive(true);
            }
            detailsTitleObject.GetComponent<Text>().text = "Hyland Auto";

            if (!detailsSubtitleObject.activeSelf)
            {
                detailsSubtitleObject.SetActive(true);
            }
            detailsSubtitleObject.GetComponent<Text>().text = "Adjusting the prices of vehicles.";

            // Deaktiviere priceOptionsTransform
            if (priceOptionsTransform.gameObject.activeSelf)
            {
                priceOptionsTransform.gameObject.SetActive(false);
            }

            // Deaktiviere optionsTransform
            if (optionsTransform.gameObject.activeSelf)
            {
                optionsTransform.gameObject.SetActive(false);
            }

            // Deaktiviere skateboardOptionsTransform
            if (skateboardOptionsTransform.gameObject.activeSelf)
            {
                skateboardOptionsTransform.gameObject.SetActive(false);
            }

            // Aktiviere vehicleOptionsTransform
            if (!vehicleOptionsTransform.gameObject.activeSelf)
            {
                vehicleOptionsTransform.gameObject.SetActive(true);
            }

            Transform saveButtonTransform = vehicleOptionsTransform.Find("Save Button");
            if (saveButtonTransform != null && !saveButtonTransform.gameObject.activeSelf)
            {
                saveButtonTransform.gameObject.SetActive(true);
            }

            Transform shitboxContainer = vehicleOptionsTransform.Find("Shitbox Horizontal Container");
            Transform veeperContainer = vehicleOptionsTransform.Find("Veeper Horizontal Container");
            Transform bruiserContainer = vehicleOptionsTransform.Find("Bruiser Horizontal Container");
            Transform dinklerContainer = vehicleOptionsTransform.Find("Dinkler Horizontal Container");
            Transform hounddogContainer = vehicleOptionsTransform.Find("Hounddog Horizontal Container");
            Transform cheetahContainer = vehicleOptionsTransform.Find("Cheetah Horizontal Container");

            if (shitboxContainer != null && !shitboxContainer.gameObject.activeSelf)
            {
                shitboxContainer.gameObject.SetActive(true);
            }
            if (veeperContainer != null && !veeperContainer.gameObject.activeSelf)
            {
                veeperContainer.gameObject.SetActive(true);
            }
            if (bruiserContainer != null && !bruiserContainer.gameObject.activeSelf)
            {
                bruiserContainer.gameObject.SetActive(true);
            }
            if (dinklerContainer != null && !dinklerContainer.gameObject.activeSelf)
            {
                dinklerContainer.gameObject.SetActive(true);
            }
            if (hounddogContainer != null && !hounddogContainer.gameObject.activeSelf)
            {
                hounddogContainer.gameObject.SetActive(true);
            }
            if (cheetahContainer != null && !cheetahContainer.gameObject.activeSelf)
            {
                cheetahContainer.gameObject.SetActive(true);
            }

            foreach (LandVehicle vehicle in MRLCore.Instance.vehicleManager.VehiclePrefabs)
            {
                if (vehicle == null) continue;
                string displayName = MRLCore.Instance.vehicleAliasMap.ContainsKey(vehicle.name) ? MRLCore.Instance.vehicleAliasMap[vehicle.name] : vehicle.name;
                float price = vehicle.vehiclePrice;
                SetInputFieldValue(vehicleOptionsTransform, displayName, price);
            }


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

            // Deaktiviere optionsTransform
            if (vehicleOptionsTransform.gameObject.activeSelf)
            {
                vehicleOptionsTransform.gameObject.SetActive(false);
            }

            // Deaktiviere skateboardOptionsTransform
            if (skateboardOptionsTransform.gameObject.activeSelf)
            {
                skateboardOptionsTransform.gameObject.SetActive(false);
            }

            if (InstructionsTextObject.activeSelf)
            {
                InstructionsTextObject.SetActive(false);
            }

            if (!detailsTitleObject.activeSelf)
            {
                detailsTitleObject.SetActive(true);
            }
            detailsTitleObject.GetComponent<Text>().text = "Ray's Real Estate";

            if (!detailsSubtitleObject.activeSelf)
            {
                detailsSubtitleObject.SetActive(true);
            }
            detailsSubtitleObject.GetComponent<Text>().text = "Adjusting the prices of properties.";

            // Aktiviere die vier Container in priceOptionsTransform, falls sie deaktiviert sind
            Transform laundromatContainer = priceOptionsTransform.Find("Laundromat Horizontal Container");
            Transform postOfficeContainer = priceOptionsTransform.Find("Post Office Horizontal Container");
            Transform carWashContainer = priceOptionsTransform.Find("Car Wash Horizontal Container");
            Transform tacoTicklersContainer = priceOptionsTransform.Find("Taco Ticklers Horizontal Container");

            if (laundromatContainer != null && !laundromatContainer.gameObject.activeSelf)
            {
                laundromatContainer.gameObject.SetActive(true);
            }
            if (postOfficeContainer != null && !postOfficeContainer.gameObject.activeSelf)
            {
                postOfficeContainer.gameObject.SetActive(true);
            }
            if (carWashContainer != null && !carWashContainer.gameObject.activeSelf)
            {
                carWashContainer.gameObject.SetActive(true);
            }
            if (tacoTicklersContainer != null && !tacoTicklersContainer.gameObject.activeSelf)
            {
                tacoTicklersContainer.gameObject.SetActive(true);
            }

            Transform saveButton = priceOptionsTransform.Find("Save Button");
            if (saveButton != null && saveButton.gameObject != null && !saveButton.gameObject.activeSelf)
            {
                saveButton.gameObject.SetActive(true);
            }

            // Deaktiviere die vier Container in priceOptionsTransform, falls sie deaktiviert sind
            Transform storageUnitContainer = priceOptionsTransform.Find("Storage Unit Horizontal Container");
            Transform motelContainer = priceOptionsTransform.Find("Motel Horizontal Container");
            Transform SweatshopContainer = priceOptionsTransform.Find("Sweatshop Horizontal Container");
            Transform bungalowContainer = priceOptionsTransform.Find("Bungalow Horizontal Container");
            Transform barnContainer = priceOptionsTransform.Find("Barn Horizontal Container");
            Transform docksContainer = priceOptionsTransform.Find("Docks Warehouse Horizontal Container");
            Transform manorContainer = priceOptionsTransform.Find("Manor Horizontal Container");

            if (storageUnitContainer != null && storageUnitContainer.gameObject.activeSelf)
            {
                storageUnitContainer.gameObject.SetActive(false);
            }
            if (motelContainer != null && motelContainer.gameObject.activeSelf)
            {
                motelContainer.gameObject.SetActive(false);
            }
            if (SweatshopContainer != null && SweatshopContainer.gameObject.activeSelf)
            {
                SweatshopContainer.gameObject.SetActive(false);
            }
            if (bungalowContainer != null && bungalowContainer.gameObject.activeSelf)
            {
                bungalowContainer.gameObject.SetActive(false);
            }
            if (barnContainer != null && barnContainer.gameObject.activeSelf)
            {
                barnContainer.gameObject.SetActive(false);
            }
            if (docksContainer != null && docksContainer.gameObject.activeSelf)
            {
                docksContainer.gameObject.SetActive(false);
            }
            if (manorContainer != null && manorContainer.gameObject.activeSelf)
            {
                manorContainer.gameObject.SetActive(false);
            }

            // Setze den Business Toggle Button auf Pressed

            Transform businessToggleButtonTransform = priceOptionsTransform.Find("ToggleContainer/ToggleButton");
            GameObject businessToggleButtonObject = businessToggleButtonTransform?.gameObject;
            if (businessToggleButtonObject != null)
            {

                Image buttonImage = businessToggleButtonObject.GetComponent<Image>();
                if (buttonImage != null)
                {
                    if (toggleButtonPressedSprite == null)
                    {
                        string imagePath = Path.Combine(ConfigFolder, "ToggleButtonPressed.png");
                        if (!string.IsNullOrEmpty(imagePath) && File.Exists(imagePath))
                        {
                            byte[] imageData = File.ReadAllBytes(imagePath);
                            Texture2D texture = new Texture2D(2, 2);
                            if (texture.LoadImage(imageData))
                            {
                                Sprite newSprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                                toggleButtonPressedSprite = newSprite;
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
                        buttonImage.sprite = toggleButtonPressedSprite;
                    }
                }
                else
                {
                    MelonLogger.Warning("Button Image component not found!");
                }
            }

            // Setze den Home Toggle Button auf nicht gedrückt
            Transform homeToggleButtonTransform = priceOptionsTransform.Find("ToggleContainer/SecondToggleButton");
            GameObject homeToggleButtonObject = homeToggleButtonTransform?.gameObject;
            if (homeToggleButtonObject != null)
            {

                Image homeButtonImage = homeToggleButtonObject.GetComponent<Image>();
                if (homeButtonImage != null)
                {
                    if (toggleButtonSprite == null)
                    {
                        string imagePath = Path.Combine(ConfigFolder, "ToggleButton.png");
                        if (!string.IsNullOrEmpty(imagePath) && File.Exists(imagePath))
                        {
                            byte[] imageData = File.ReadAllBytes(imagePath);
                            Texture2D texture = new Texture2D(2, 2);
                            if (texture.LoadImage(imageData))
                            {
                                Sprite newSprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                                toggleButtonSprite = newSprite;
                                homeButtonImage.sprite = newSprite;
                            }
                            else
                            {
                                MelonLogger.Error($"Failed to load image from path: {imagePath}");
                                homeButtonImage.color = new Color(0.2f, 0.6f, 0.2f); // Grüne Farbe für den Button
                            }
                        }
                    }
                    else
                    {
                        homeButtonImage.sprite = toggleButtonSprite;
                    }
                }
                else
                {
                    MelonLogger.Warning("Home Button Image component not found!");
                }
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

            // Lade die Preise für UnownedProperties und OwnedProperties
            if (Property.UnownedProperties == null && Property.OwnedProperties == null)
            {
                MelonLogger.Error("UnownedProperties or OwnedProperties is null! Cannot load price values.");
                return;
            }
            else
            {
                // UnownedProperties
                foreach (Property property in Property.UnownedProperties)
                {
                    if (property == null || property.name == "RV") continue;

                    string displayName = property.name;
                    if (displayName == "MotelRoom")
                    {
                        displayName = "Motel";
                    }
                    if (displayName == "DocksWarehouse")
                    {
                        displayName = "Docks Warehouse";
                    }
                    if (displayName == "StorageUnit")
                    {
                        displayName = "Storage Unit";
                    }

                    float price = property.Price;
                    SetInputFieldValue(priceOptionsTransform, displayName, price);
                }

                // OwnedProperties
                foreach (Property property in Property.OwnedProperties)
                {
                    if (property == null || property.name == "RV") continue;

                    string displayName = property.name;
                    if (displayName == "MotelRoom")
                    {
                        displayName = "Motel";
                    }
                    if (displayName == "DocksWarehouse")
                    {
                        displayName = "Docks Warehouse";
                    }
                    if (displayName == "StorageUnit")
                    {
                        displayName = "Storage Unit";
                    }
                    float price = property.Price;
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

            if (vehicleOptionsTransform != null)
            {
                if (vehicleOptionsTransform.gameObject.activeSelf)
                {
                    vehicleOptionsTransform.gameObject.SetActive(false);
                }
            }

            // Deaktiviere skateboardOptionsTransform
            if (skateboardOptionsTransform != null)
            {
                if (skateboardOptionsTransform.gameObject.activeSelf)
                {
                    skateboardOptionsTransform.gameObject.SetActive(false);
                }
            }

            _selectedBusiness = business;

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

            if (InstructionsTextObject.activeSelf)
            {
                InstructionsTextObject.SetActive(false);
            }

            if (!detailsTitleObject.activeSelf)
            {
                detailsTitleObject.SetActive(true);
            }
            detailsTitleObject.GetComponent<Text>().text = businessName;

            if (!detailsSubtitleObject.activeSelf)
            {
                detailsSubtitleObject.SetActive(true);
            }
            detailsSubtitleObject.GetComponent<Text>().text = subTitleText;

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


        public void AddOptionsForBusiness(Transform parentTransform)
        {
            if (parentTransform == null)
            {
                MelonLogger.Error("parentTransform is null! Cannot add options.");
                return;
            }

            GameObject optionsObject = new GameObject("Options");

            //  LayoutElement layoutElement = generalSettingsObject.AddComponent<LayoutElement>();
            VerticalLayoutGroup contentVerticalLayout = optionsObject.GetComponent<VerticalLayoutGroup>();
            if (contentVerticalLayout == null)
            {
                contentVerticalLayout = optionsObject.gameObject.AddComponent<VerticalLayoutGroup>();
            }
            contentVerticalLayout.childAlignment = TextAnchor.UpperLeft;
            contentVerticalLayout.spacing = 15f; // Abstand zwischen den Optionen 
            contentVerticalLayout.childControlWidth = true;
            contentVerticalLayout.childControlHeight = true;
            contentVerticalLayout.childForceExpandWidth = false;
            contentVerticalLayout.childForceExpandHeight = false;
            // Add padding to the VerticalLayoutGroup using RectOffset
            contentVerticalLayout.padding = new RectOffset(15, 15, 15, 15); // Left: 15, Right: 15, Top: 0, Bottom: 0

            // Spacing from left
            RectTransform optionsRect = optionsObject.GetComponent<RectTransform>();
            if (optionsRect != null)
            {
                optionsRect.offsetMin = new Vector2(60, optionsRect.offsetMin.y + 60);
                // Layout -> RectOffset -> padding
            }

            // Füge einen ContentSizeFitter hinzu
            ContentSizeFitter contentSizeFitter = optionsObject.GetComponent<ContentSizeFitter>();
            if (contentSizeFitter == null)
            {
                contentSizeFitter = optionsObject.AddComponent<ContentSizeFitter>();
            }
            contentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize; // Passt die Höhe an den Inhalt an
            contentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained; // Keine Anpassung der Breite
            optionsObject.transform.SetParent(parentTransform, false);
            optionsTransform = optionsObject.transform;

            // Füge die Optionen hinzu
            AddLabelInputPair("Maximum Launder", optionsTransform, "$");
            AddLabelInputPair("Launder Time", optionsTransform, "hr");
            AddLabelInputPair("Taxation", optionsTransform, "%");
            AddSaveButton(optionsTransform, "BusinessDetails");

            if (parentTransform.FindChild("Space") != null)
            {
                parentTransform.FindChild("Space").SetAsLastSibling();
            }
            optionsObject.SetActive(false);

            //   MelonLogger.Msg("Added options for business to optionsTransform.");
        }

        void AddLabelInputPair(string labelText, Transform parentTransform, string prefixText)
        {
            if (parentTransform == null)
            {
                MelonLogger.Error($"Parent transform is null for label: {labelText}");
                return;
            }

            if (string.IsNullOrEmpty(labelText))
            {
                MelonLogger.Error("Label text is null or empty!");
                return;
            }

            // Erstelle einen Container für das Label und das Inputfeld
            GameObject container = new GameObject($"{labelText} Horizontal Container");
            container.transform.SetParent(parentTransform, false);
            //  container.SetActive(false);

            // Füge eine HorizontalLayoutGroup hinzu, falls sie nicht existiert
            HorizontalLayoutGroup horizontalContainerGroup = container.GetComponent<HorizontalLayoutGroup>();
            if (horizontalContainerGroup == null)
            {
                horizontalContainerGroup = container.AddComponent<HorizontalLayoutGroup>();
                horizontalContainerGroup.spacing = 30f; // Abstand zwischen den Elementen
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
            }
            labelRect.sizeDelta = new Vector2(325, 30); // Breite und Höhe des Labels
            labelRect.anchorMin = new Vector2(0, 0.5f);
            labelRect.anchorMax = new Vector2(0, 0.5f);
            labelRect.pivot = new Vector2(0, 0.5f);

            // Füge ein LayoutElement hinzu, falls es nicht existiert
            LayoutElement layoutElement = labelObject.GetComponent<LayoutElement>();
            if (layoutElement == null)
            {
                layoutElement = labelObject.AddComponent<LayoutElement>();
                layoutElement.minWidth = 325; // Mindestbreite des Input-Felds
                layoutElement.preferredWidth = 325; // Bevorzugte Breite des Input-Felds
                layoutElement.minHeight = 30; // Mindesthöhe des Input-Felds
                layoutElement.preferredHeight = 30; // Bevorzugte Höhe des Input-Felds
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

            // Stelle sicher, dass das RectTransform des Input-Felds korrekt ist
            RectTransform inputRect = inputObject.GetComponent<RectTransform>();
            if (inputRect == null)
            {
                inputRect = inputObject.AddComponent<RectTransform>();
            }
            inputRect.sizeDelta = new Vector2(100, 30); // Setze die Größe des Input-Felds auf die gleiche Größe wie der Hintergrund
            inputRect.anchorMin = new Vector2(0, 0.5f);
            inputRect.anchorMax = new Vector2(0, 0.5f);
            inputRect.pivot = new Vector2(0, 0.5f);
            inputRect.anchoredPosition = new Vector2(0, 0);

            // Füge ein LayoutElement hinzu, falls es nicht existiert
            LayoutElement layoutElementInput = inputObject.GetComponent<LayoutElement>();
            if (layoutElementInput == null)
            {
                layoutElementInput = inputObject.AddComponent<LayoutElement>();
                layoutElementInput.minWidth = 100; // Mindestbreite des Input-Felds
                layoutElementInput.preferredWidth = 100; // Bevorzugte Breite des Input-Felds
                layoutElementInput.minHeight = 30; // Mindesthöhe des Input-Felds
                layoutElementInput.preferredHeight = 30; // Bevorzugte Höhe des Input-Felds
            }
            // Füge eine Text-Komponente hinzu, falls sie nicht existiert
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
                inputTextComponent.alignment = TextAnchor.MiddleRight;
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

            // Setze die Größe des Prefix-Objekts
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
            prefixRect.sizeDelta = new Vector2(20, 30); // Leicht nach rechts verschoben
            prefixRect.anchorMin = new Vector2(0, 0.5f);
            prefixRect.anchorMax = new Vector2(0, 0.5f);
            prefixRect.pivot = new Vector2(0, 0.5f);
            prefixRect.anchoredPosition = new Vector2(5, 0);

            // Verschiebe den Text des Inputfelds nach rechts, um Platz für das "$"-Symbol zu schaffen
            RectTransform inputTextRect = inputTextObject.GetComponent<RectTransform>();
            if (inputTextRect != null)
            {
                inputTextRect.sizeDelta = new Vector2(100, 30); // Setze die Größe des Text-Objekts auf die gleiche Größe wie das Input-Feld
                inputTextRect.offsetMin = new Vector2(-20, inputTextRect.offsetMin.y);
                inputTextRect.offsetMax = new Vector2(40, inputTextRect.offsetMax.y); // Verschiebe den linken Rand nach rechts
            }

            //  MelonLogger.Msg($"Added Label, Prefix '$', and InputField for '{labelText}' to Horizontal Container.");

        }

        void onEndEditCheck(InputField inputFieldComponent, string value, string labelText)
        {

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


            string subTitleString; //= $"for <color=#329AC5>{displayName}</color>";

            string input = value.Trim(); // Entferne Leerzeichen am Anfang und Ende der Eingabe
            if (labelText == "Maximum Launder")
            {
                // Überprüfe, ob die Eingabe leer ist oder kleiner als 1000
                if (string.IsNullOrEmpty(input) || int.TryParse(input, out int parsedValue) && parsedValue < 1000)
                {
                    inputFieldComponent.text = "1000";
                    if (MRLCore.Instance.notificationsManager != null)
                    {
                        subTitleString = $"Has to be <color=#329AC5>at least $1000</color>";
                        MRLCore.Instance.notificationsManager.SendNotification("Maximum Launder", subTitleString, appIconSprite, 3, true);
                    }
                    MelonLogger.Warning($"Maximum Launder value was < 1000. Applying '$1000' as value.");
                }
            }
            else if (labelText == "Launder Time")
            {
                // Überprüfe, ob die Eingabe leer ist oder kleiner als 4
                if (string.IsNullOrEmpty(input) || int.TryParse(input, out int parsedValue) && parsedValue < 4)
                {
                    inputFieldComponent.text = "24";
                    if (MRLCore.Instance.notificationsManager != null)
                    {
                        subTitleString = $"Has to be <color=#329AC5>at least 4 hrs</color>";
                        MRLCore.Instance.notificationsManager.SendNotification("Launder Time", subTitleString, appIconSprite, 3, true);
                    }
                    MelonLogger.Warning($"Launder Time was < 4. Applying '24 hrs' as value.");
                }
            }
            else if (labelText == "Taxation")
            {
                if (string.IsNullOrEmpty(input) || float.TryParse(input, out float parsedValue) && (parsedValue < 0 || parsedValue > 100))
                {
                    inputFieldComponent.text = "19";
                    if (MRLCore.Instance.notificationsManager != null)
                    {
                        subTitleString = $"Has to be <color=#329AC5> 0 to 100 %</color>";
                        MRLCore.Instance.notificationsManager.SendNotification("Taxation", subTitleString, appIconSprite, 3, true);
                    }
                    MelonLogger.Warning($"Taxation < 0 or > 100. Applying '19 %' as value.");
                }
            }
            else if (labelText.Contains("Post Office") || labelText.Contains("PostOffice") || labelText.Contains("Taco Tickler") || labelText.Contains("Laundromat") || labelText.Contains("Car Wash"))
            {
                // Überprüfe, ob die Eingabe leer ist oder kleiner als 1000
                if (string.IsNullOrEmpty(input) || float.TryParse(input, out float parsedValue) && parsedValue < 1000)
                {
                    inputFieldComponent.text = "1000";
                    if (MRLCore.Instance.notificationsManager != null)
                    {
                        subTitleString = $"Has to be <color=#329AC5>at least $1000</color>";
                        MRLCore.Instance.notificationsManager.SendNotification("Property Price", subTitleString, appIconSprite, 3, true);
                    }
                    MelonLogger.Warning($"{labelText} was < 1000. Applying '$1000' as value.");
                }
            }
            else if (labelText.Contains("Bungalow") || labelText.Contains("Barn") || labelText.Contains("Docks Warehouse") || labelText.Contains("Manor") || labelText.Contains("Storage"))
            {
                // Überprüfe, ob die Eingabe leer ist oder kleiner als 100
                if (string.IsNullOrEmpty(input) || float.TryParse(input, out float parsedValue) && parsedValue < 100)
                {
                    inputFieldComponent.text = "1000";
                    if (MRLCore.Instance.notificationsManager != null)
                    {
                        subTitleString = $"Has to be <color=#329AC5>at least $100</color>";
                        MRLCore.Instance.notificationsManager.SendNotification("Property Price", subTitleString, appIconSprite, 3, true);
                    }
                    MelonLogger.Warning($"{labelText} was < 100. Applying '$1000' as value.");
                }
            }
            else if (labelText.Contains("Motel"))
            {
                // Überprüfe, ob die Eingabe leer ist oder kleiner als 75
                if (string.IsNullOrEmpty(input) || float.TryParse(input, out float parsedValue) && parsedValue < 75)
                {
                    inputFieldComponent.text = "1000";
                    if (MRLCore.Instance.notificationsManager != null)
                    {
                        subTitleString = $"Has to be <color=#329AC5>at least $75</color>";
                        MRLCore.Instance.notificationsManager.SendNotification("Property Price", subTitleString, appIconSprite, 3, true);
                    }
                    MelonLogger.Warning($"{labelText} was < 75. Applying '$1000' as value.");
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
            saveSpaceRect.sizeDelta = new Vector2(100, 50); // Abstand zwischen dem letzten Element und dem Button

            LayoutElement spaceLayoutElement = saveSpaceObject.GetComponent<LayoutElement>();
            if (spaceLayoutElement == null)
            {
                spaceLayoutElement = saveSpaceObject.AddComponent<LayoutElement>();
                spaceLayoutElement.minHeight = 0; // Mindesthöhe des Space
                spaceLayoutElement.preferredHeight = 0; // Bevorzugte Höhe des Space
                spaceLayoutElement.minWidth = 100; // Mindestbreite des Space
                spaceLayoutElement.preferredWidth = 100; // Bevorzugte Breite des Space
            }

            // Erstelle ein neues GameObject für den Button
            GameObject saveButtonObject = new GameObject("Save Button");
            saveButtonObject.transform.SetParent(parentTransform, false);
            saveButtonObject.SetActive(true);

            // Füge ein RectTransform hinzu, falls es nicht existiert
            RectTransform buttonRect = saveButtonObject.GetComponent<RectTransform>();
            if (buttonRect == null)
            {
                buttonRect = saveButtonObject.AddComponent<RectTransform>();
                buttonRect.sizeDelta = new Vector2(455, 30); // Standardgröße des Buttons
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
                layoutElement.preferredWidth = 455; // Bevorzugte Breite des Buttons
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

            if (saveString == "Vehicles")
            {
                void FuncThatCallsFunc() => SaveVehicleOptions(buttonText);
                saveButton.onClick.AddListener((UnityAction)FuncThatCallsFunc);
            }

            if (saveString == "Skateboards")
            {
                void FuncThatCallsFunc() => SaveSkateboardOptions(buttonText);
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
            int launderTimeValue = MRLCore.Instance.config.Businesses.Laundromat.Laundromat_Laundering_time_hours; // Standardwert
            float taxationValue = MRLCore.Instance.config.Businesses.Laundromat.Laundromat_Tax_Percentage; // Standardwert

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
                    MRLCore.Instance.config.Businesses.Laundromat.Laundromat_Cap = maxLaunderValue;
                    MRLCore.Instance.config.Businesses.Laundromat.Laundromat_Laundering_time_hours = launderTimeValue;
                    MRLCore.Instance.config.Businesses.Laundromat.Laundromat_Tax_Percentage = taxationValue;
                    break;

                case "taco ticklers":
                case "tacoticklers":
                    MRLCore.Instance.config.Businesses.TacoTicklers.Taco_Ticklers_Cap = maxLaunderValue;
                    MRLCore.Instance.config.Businesses.TacoTicklers.Taco_Ticklers_Laundering_time_hours = launderTimeValue;
                    MRLCore.Instance.config.Businesses.TacoTicklers.Taco_Ticklers_Tax_Percentage = taxationValue;
                    break;

                case "car wash":
                case "carwash":
                    MRLCore.Instance.config.Businesses.CarWash.Car_Wash_Cap = maxLaunderValue;
                    MRLCore.Instance.config.Businesses.CarWash.Car_Wash_Laundering_time_hours = launderTimeValue;
                    MRLCore.Instance.config.Businesses.CarWash.Car_Wash_Tax_Percentage = taxationValue;
                    break;

                case "post office":
                case "postoffice":
                    MRLCore.Instance.config.Businesses.PostOffice.Post_Office_Cap = maxLaunderValue;
                    MRLCore.Instance.config.Businesses.PostOffice.Post_Office_Laundering_time_hours = launderTimeValue;
                    MRLCore.Instance.config.Businesses.PostOffice.Post_Office_Tax_Percentage = taxationValue;
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

            Sprite businessSprite = null;
            if (displayName == "Laundromat")
            {
                businessSprite = laundromatSprite;
            }
            else if (displayName == "Taco Ticklers")
            {
                businessSprite = tacoTicklersSprite;
            }
            else if (displayName == "Car Wash")
            {
                businessSprite = carWashSprite;
            }
            else if (displayName == "Post Office")
            {
                businessSprite = postOfficeSprite;
            }

            if (businessSprite == null)
            {
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
                            businessSprite = newSprite;
                        }
                        else
                        {
                            MelonLogger.Error($"Failed to load image from path: {imagePath}");
                        }
                    }
                }
                else
                {
                    businessSprite = appIconSprite;
                }
            }


            if (MRLCore.Instance.notificationsManager != null)
            {
                string subTitleString = $"for <color=#329AC5>{displayName}</color>";
                MRLCore.Instance.notificationsManager.SendNotification("Saved Changes", subTitleString, businessSprite, 5, true);
            }
            // MelonLogger.Msg($"Details for {displayName} have been saved and applied successfully.");
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

            Transform storageUnitPriceInputTransform = priceOptionsTransform.Find("Storage Unit Horizontal Container/Storage Unit Input");
            Transform motelPriceInputTransform = priceOptionsTransform.Find("Motel Horizontal Container/Motel Input");
            Transform sweatPriceInputTransform = priceOptionsTransform.Find("Sweatshop Horizontal Container/Sweatshop Input");
            Transform bungalowPriceInputTransform = priceOptionsTransform.Find("Bungalow Horizontal Container/Bungalow Input");
            Transform barnPriceInputTransform = priceOptionsTransform.Find("Barn Horizontal Container/Barn Input");
            Transform docksPriceInputTransform = priceOptionsTransform.Find("Docks Warehouse Horizontal Container/Docks Warehouse Input");
            Transform manorPriceInputTransform = priceOptionsTransform.Find("Manor Horizontal Container/Manor Input");

            float laundromatPrice = MRLCore.Instance.config.Properties.BusinessProperties.Laundromat_Price;
            float tacoTicklersPrice = MRLCore.Instance.config.Properties.BusinessProperties.Taco_Ticklers_Price;
            float carWashPrice = MRLCore.Instance.config.Properties.BusinessProperties.Car_Wash_Price;
            float postOfficePrice = MRLCore.Instance.config.Properties.BusinessProperties.Post_Office_Price;

            float storageUnitPrice = MRLCore.Instance.config.Properties.PrivateProperties.Storage_Unit_Price;
            float motelPrice = MRLCore.Instance.config.Properties.PrivateProperties.Motel_Room_Price;
            float sweatshopPrice = MRLCore.Instance.config.Properties.PrivateProperties.Sweatshop_Price;
            float bungalowPrice = MRLCore.Instance.config.Properties.PrivateProperties.Bungalow_Price;
            float barnPrice = MRLCore.Instance.config.Properties.PrivateProperties.Barn_Price;
            float docksPrice = MRLCore.Instance.config.Properties.PrivateProperties.Docks_Warehouse_Price;
            float manorPrice = MRLCore.Instance.config.Properties.PrivateProperties.Manor_Price;

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

            // Aktualisiere den Preis für Storage Unit
            if (storageUnitPriceInputTransform != null)
            {
                InputField storageUnitPriceInputField = storageUnitPriceInputTransform.GetComponent<InputField>();
                if (storageUnitPriceInputField != null && float.TryParse(storageUnitPriceInputField.text, out float parsedStorageUnitPrice))
                {
                    storageUnitPrice = parsedStorageUnitPrice;
                }
            }

            // Aktualisiere die Preise für die Immobilien
            if (motelPriceInputTransform != null)
            {
                InputField motelPriceInputField = motelPriceInputTransform.GetComponent<InputField>();
                if (motelPriceInputField != null && float.TryParse(motelPriceInputField.text, out float parsedMotelPrice))
                {
                    motelPrice = parsedMotelPrice;
                }
            }

            if (sweatPriceInputTransform != null)
            {
                InputField sweatPriceInputField = sweatPriceInputTransform.GetComponent<InputField>();
                if (sweatPriceInputField != null && float.TryParse(sweatPriceInputField.text, out float parsedSweatPrice))
                {
                    sweatshopPrice = parsedSweatPrice;
                }
            }

            if (bungalowPriceInputTransform != null)
            {
                InputField bungalowPriceInputField = bungalowPriceInputTransform.GetComponent<InputField>();
                if (bungalowPriceInputField != null && float.TryParse(bungalowPriceInputField.text, out float parsedBungalowPrice))
                {
                    bungalowPrice = parsedBungalowPrice;
                }
            }

            if (barnPriceInputTransform != null)
            {
                InputField barnPriceInputField = barnPriceInputTransform.GetComponent<InputField>();
                if (barnPriceInputField != null && float.TryParse(barnPriceInputField.text, out float parsedBarnPrice))
                {
                    barnPrice = parsedBarnPrice;
                }
            }

            if (docksPriceInputTransform != null)
            {
                InputField docksPriceInputField = docksPriceInputTransform.GetComponent<InputField>();
                if (docksPriceInputField != null && float.TryParse(docksPriceInputField.text, out float parsedDocksPrice))
                {
                    docksPrice = parsedDocksPrice;
                }
            }

            if (manorPriceInputTransform != null)
            {
                InputField manorPriceInputField = manorPriceInputTransform.GetComponent<InputField>();
                if (manorPriceInputField != null && float.TryParse(manorPriceInputField.text, out float parsedManorPrice))
                {
                    manorPrice = parsedManorPrice;
                }
            }

            //   MelonLogger.Msg($"Updated Prices: {laundromatPrice}, {tacoTicklersPrice}, {carWashPrice}, {postOfficePrice}");

            // Speichere die aktualisierten Preise in der Konfiguration
            MRLCore.Instance.config.Properties.BusinessProperties.Laundromat_Price = laundromatPrice;
            MRLCore.Instance.config.Properties.BusinessProperties.Taco_Ticklers_Price = tacoTicklersPrice;
            MRLCore.Instance.config.Properties.BusinessProperties.Car_Wash_Price = carWashPrice;
            MRLCore.Instance.config.Properties.BusinessProperties.Post_Office_Price = postOfficePrice;
            MRLCore.Instance.config.Properties.PrivateProperties.Storage_Unit_Price = storageUnitPrice;
            MRLCore.Instance.config.Properties.PrivateProperties.Motel_Room_Price = motelPrice;
            MRLCore.Instance.config.Properties.PrivateProperties.Sweatshop_Price = sweatshopPrice;
            MRLCore.Instance.config.Properties.PrivateProperties.Bungalow_Price = bungalowPrice;
            MRLCore.Instance.config.Properties.PrivateProperties.Barn_Price = barnPrice;
            MRLCore.Instance.config.Properties.PrivateProperties.Docks_Warehouse_Price = docksPrice;
            MRLCore.Instance.config.Properties.PrivateProperties.Manor_Price = manorPrice;

            // Speichere die aktualisierte Konfiguration
            ConfigManager.Save(MRLCore.Instance.config);

            // Aktualisiere die Preise in den PropertyListings
            MRLCore.Instance.ApplyPricesToPropertyListings();
            MRLCore.Instance.ApplyPricesToProperties();

            // Zeige eine Benachrichtigung an
            if (MRLCore.Instance.notificationsManager != null)
            {
                string subTitleString = "Config saved";
                Sprite notificationSprite = null;
                if (raysRealEstateSprite)
                {
                    notificationSprite = raysRealEstateSprite;
                }
                if (notificationSprite == null)
                {
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
                                notificationSprite = appIconSprite;
                            }
                            else
                            {
                                MelonLogger.Error($"Failed to load image from path: {imagePath}");
                            }
                        }
                    }
                    else
                    {
                        notificationSprite = appIconSprite;
                    }
                }
                MRLCore.Instance.notificationsManager.SendNotification("Property Prices", subTitleString, notificationSprite, 5, true);
            }

            buttonText.text = "Save & Apply";
            isSaveStillRunning = false;

            // MelonLogger.Msg("Real estate prices have been saved and applied successfully.");
        }

        void SaveSkateboardOptions(Text buttonText)
        {
            isSaveStillRunning = true;
            buttonText.text = "Saving...";

            if (skateboardOptionsTransform == null)
            {
                MelonLogger.Error("skateboardOptionsTransform is null! Cannot save skateboard prices.");
                buttonText.text = "Save & Apply";
                isSaveStillRunning = false;
                return;
            }

            Transform cheapSkateboardPriceInputTransform = skateboardOptionsTransform.Find("Cheap Skateboard Horizontal Container/Cheap Skateboard Input");
            Transform skateboardPriceInputTransform = skateboardOptionsTransform.Find("Skateboard Horizontal Container/Skateboard Input");
            Transform cruiserPriceInputTransform = skateboardOptionsTransform.Find("Cruiser Horizontal Container/Cruiser Input");
            Transform lightweightBoardPriceInputTransform = skateboardOptionsTransform.Find("Lightweight Board Horizontal Container/Lightweight Board Input");
            Transform goldenSkateboardPriceInputTransform = skateboardOptionsTransform.Find("Golden Skateboard Horizontal Container/Golden Skateboard Input");

            float cheapSkateboardPrice = MRLCore.Instance.config.Skateboards.Cheap_Skateboard_Price;
            float skateboardPrice = MRLCore.Instance.config.Skateboards.Skateboard_Price;
            float cruiserPrice = MRLCore.Instance.config.Skateboards.Cruiser_Price;
            float lightweightBoardPrice = MRLCore.Instance.config.Skateboards.Lightweight_Board_Price;
            float goldenSkateboardPrice = MRLCore.Instance.config.Skateboards.Golden_Skateboard_Price;

            // Aktualisiere den Preis für Cheap Skateboard
            if (cheapSkateboardPriceInputTransform != null)
            {
                InputField cheapSkateboardPriceInputField = cheapSkateboardPriceInputTransform.GetComponent<InputField>();
                if (cheapSkateboardPriceInputField != null && float.TryParse(cheapSkateboardPriceInputField.text, out float parsedCheapSkateboardPrice))
                {
                    cheapSkateboardPrice = parsedCheapSkateboardPrice;
                }
            }
            // Aktualisiere den Preis für Skateboard
            if (skateboardPriceInputTransform != null)
            {
                InputField skateboardPriceInputField = skateboardPriceInputTransform.GetComponent<InputField>();
                if (skateboardPriceInputField != null && float.TryParse(skateboardPriceInputField.text, out float parsedSkateboardPrice))
                {
                    skateboardPrice = parsedSkateboardPrice;
                }
            }
            // Aktualisiere den Preis für Cruiser
            if (cruiserPriceInputTransform != null)
            {
                InputField cruiserPriceInputField = cruiserPriceInputTransform.GetComponent<InputField>();
                if (cruiserPriceInputField != null && float.TryParse(cruiserPriceInputField.text, out float parsedCruiserPrice))
                {
                    cruiserPrice = parsedCruiserPrice;
                }
            }
            // Aktualisiere den Preis für Lightweight Skateboard
            if (lightweightBoardPriceInputTransform != null)
            {
                InputField lightweightBoardPriceInputField = lightweightBoardPriceInputTransform.GetComponent<InputField>();
                if (lightweightBoardPriceInputField != null && float.TryParse(lightweightBoardPriceInputField.text, out float parsedLightweightBoardPrice))
                {
                    lightweightBoardPrice = parsedLightweightBoardPrice;
                }
            }
            // Aktualisiere den Preis für Golden Skateboard
            if (goldenSkateboardPriceInputTransform != null)
            {
                InputField goldenSkateboardPriceInputField = goldenSkateboardPriceInputTransform.GetComponent<InputField>();
                if (goldenSkateboardPriceInputField != null && float.TryParse(goldenSkateboardPriceInputField.text, out float parsedGoldenSkateboardPrice))
                {
                    goldenSkateboardPrice = parsedGoldenSkateboardPrice;
                }
            }
            // Speichere die aktualisierten Preise in der Konfiguration
            MRLCore.Instance.config.Skateboards.Cheap_Skateboard_Price = cheapSkateboardPrice;
            MRLCore.Instance.config.Skateboards.Skateboard_Price = skateboardPrice;
            MRLCore.Instance.config.Skateboards.Cruiser_Price = cruiserPrice;
            MRLCore.Instance.config.Skateboards.Lightweight_Board_Price = lightweightBoardPrice;
            MRLCore.Instance.config.Skateboards.Golden_Skateboard_Price = goldenSkateboardPrice;

            // Speichere die aktualisierte Konfiguration
            ConfigManager.Save(MRLCore.Instance.config);
            // Aktualisiere die Preise in den SkateboardListings
            MRLCore.Instance.ApplyPricesToSkateboards();

            if (MRLCore.Instance.notificationsManager != null)
            {
                string subTitleString = "Config saved";
                Sprite notificationSprite = null;
                if (shredShackSprite)
                {
                    notificationSprite = shredShackSprite;
                }
                if (notificationSprite == null)
                {
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
                                notificationSprite = appIconSprite;
                            }
                            else
                            {
                                MelonLogger.Error($"Failed to load image from path: {imagePath}");
                            }
                        }
                    }
                    else
                    {
                        notificationSprite = appIconSprite;
                    }
                }
                MRLCore.Instance.notificationsManager.SendNotification("Skateboard Prices", subTitleString, notificationSprite, 5, true);
            }

            buttonText.text = "Save & Apply";
            isSaveStillRunning = false;
        }
        void SaveVehicleOptions(Text buttonText)
        {
            isSaveStillRunning = true;
            buttonText.text = "Saving...";

            if (vehicleOptionsTransform == null)
            {
                MelonLogger.Error("priceOptionsTransform is null! Cannot save vehicle prices.");
                buttonText.text = "Save & Apply";
                isSaveStillRunning = false;
                return;
            }

            // Greife auf die Eingabefelder zu und speichere die Werte
            Transform shitboxInputTransform = vehicleOptionsTransform.Find("Shitbox Horizontal Container/Shitbox Input");
            Transform veeperPriceInputTransform = vehicleOptionsTransform.Find("Veeper Horizontal Container/Veeper Input");
            Transform bruiserPriceInputTransform = vehicleOptionsTransform.Find("Bruiser Horizontal Container/Bruiser Input");
            Transform dinklerPriceInputTransform = vehicleOptionsTransform.Find("Dinkler Horizontal Container/Dinkler Input");
            Transform hounddogPriceInputTransform = vehicleOptionsTransform.Find("Hounddog Horizontal Container/Hounddog Input");
            Transform cheetahPriceInputTransform = vehicleOptionsTransform.Find("Cheetah Horizontal Container/Cheetah Input");

            float shitboxPrice = MRLCore.Instance.config.Vehicles.Shitbox_Price;
            float veeperPrice = MRLCore.Instance.config.Vehicles.Veeper_Price;
            float bruiserPrice = MRLCore.Instance.config.Vehicles.Bruiser_Price;
            float dinklerPrice = MRLCore.Instance.config.Vehicles.Dinkler_Price;
            float hounddogPrice = MRLCore.Instance.config.Vehicles.Hounddog_Price;
            float cheetahPrice = MRLCore.Instance.config.Vehicles.Cheetah_Price;

            // Aktualisiere den Preis für Shitbox
            if (shitboxInputTransform != null)
            {
                InputField shitboxPriceInputField = shitboxInputTransform.GetComponent<InputField>();
                if (shitboxPriceInputField != null && float.TryParse(shitboxPriceInputField.text, out float parsedShitboxPrice))
                {
                    shitboxPrice = parsedShitboxPrice;
                }
            }
            // Aktualisiere den Preis für Veeper
            if (veeperPriceInputTransform != null)
            {
                InputField veeperPriceInputField = veeperPriceInputTransform.GetComponent<InputField>();
                if (veeperPriceInputField != null && float.TryParse(veeperPriceInputField.text, out float parsedVeeperPrice))
                {
                    veeperPrice = parsedVeeperPrice;
                }
            }
            // Aktualisiere den Preis für Bruiser
            if (bruiserPriceInputTransform != null)
            {
                InputField bruiserPriceInputField = bruiserPriceInputTransform.GetComponent<InputField>();
                if (bruiserPriceInputField != null && float.TryParse(bruiserPriceInputField.text, out float parsedBruiserPrice))
                {
                    bruiserPrice = parsedBruiserPrice;
                }
            }
            // Aktualisiere den Preis für Dinkler
            if (dinklerPriceInputTransform != null)
            {
                InputField dinklerPriceInputField = dinklerPriceInputTransform.GetComponent<InputField>();
                if (dinklerPriceInputField != null && float.TryParse(dinklerPriceInputField.text, out float parsedDinklerPrice))
                {
                    dinklerPrice = parsedDinklerPrice;
                }
            }
            // Aktualisiere den Preis für Hounddog
            if (hounddogPriceInputTransform != null)
            {
                InputField hounddogPriceInputField = hounddogPriceInputTransform.GetComponent<InputField>();
                if (hounddogPriceInputField != null && float.TryParse(hounddogPriceInputField.text, out float parsedHounddogPrice))
                {
                    hounddogPrice = parsedHounddogPrice;
                }
            }
            // Aktualisiere den Preis für Cheetah
            if (cheetahPriceInputTransform != null)
            {
                InputField cheetahPriceInputField = cheetahPriceInputTransform.GetComponent<InputField>();
                if (cheetahPriceInputField != null && float.TryParse(cheetahPriceInputField.text, out float parsedCheetahPrice))
                {
                    cheetahPrice = parsedCheetahPrice;
                }
            }

            // Speichere die aktualisierten Preise in der Konfiguration
            MRLCore.Instance.config.Vehicles.Shitbox_Price = shitboxPrice;
            MRLCore.Instance.config.Vehicles.Veeper_Price = veeperPrice;
            MRLCore.Instance.config.Vehicles.Bruiser_Price = bruiserPrice;
            MRLCore.Instance.config.Vehicles.Dinkler_Price = dinklerPrice;
            MRLCore.Instance.config.Vehicles.Hounddog_Price = hounddogPrice;
            MRLCore.Instance.config.Vehicles.Cheetah_Price = cheetahPrice;
            // Speichere die aktualisierte Konfiguration
            ConfigManager.Save(MRLCore.Instance.config);
            // Aktualisiere die Preise in den VehicleListings
            MRLCore.Instance.ApplyPricesToVehicles();

            if (MRLCore.Instance.notificationsManager != null)
            {
                string subTitleString = "Config saved";
                Sprite notificationSprite = null;
                if (hylandAutoSprite)
                {
                    notificationSprite = hylandAutoSprite;
                }
                if (notificationSprite == null)
                {
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
                                notificationSprite = appIconSprite;
                            }
                            else
                            {
                                MelonLogger.Error($"Failed to load image from path: {imagePath}");
                            }
                        }
                    }
                    else
                    {
                        notificationSprite = appIconSprite;
                    }
                }
                MRLCore.Instance.notificationsManager.SendNotification("Vehicle Prices", subTitleString, notificationSprite, 5, true);
            }

            buttonText.text = "Save & Apply";
            isSaveStillRunning = false;
        }

        public void AddPriceOptionsWithToggleButton(Transform parentTransform)
        {
            if (parentTransform == null)
            {
                MelonLogger.Error("Parent transform is null! Cannot add Toggle Buttons.");
                return;
            }

            GameObject priceOptionsObject = new GameObject("PriceOptions");

            VerticalLayoutGroup contentVerticalLayout = priceOptionsObject.GetComponent<VerticalLayoutGroup>();
            if (contentVerticalLayout == null)
            {
                contentVerticalLayout = priceOptionsObject.gameObject.AddComponent<VerticalLayoutGroup>();
            }
            contentVerticalLayout.childAlignment = TextAnchor.UpperLeft;
            contentVerticalLayout.spacing = 15f; // Abstand zwischen den Optionen 
            contentVerticalLayout.childControlWidth = true;
            contentVerticalLayout.childControlHeight = true;
            contentVerticalLayout.childForceExpandWidth = false;
            contentVerticalLayout.childForceExpandHeight = false;
            // Add padding to the VerticalLayoutGroup using RectOffset
            contentVerticalLayout.padding = new RectOffset(15, 15, 15, 15); // Left: 15, Right: 15, Top: 0, Bottom: 0

            // Spacing from left
            RectTransform priceOptionsRect = priceOptionsObject.GetComponent<RectTransform>();
            if (priceOptionsRect != null)
            {
                priceOptionsRect.offsetMin = new Vector2(60, priceOptionsRect.offsetMin.y + 60);
                // Layout -> RectOffset -> padding
            }

            // Füge einen ContentSizeFitter hinzu
            ContentSizeFitter contentSizeFitter = priceOptionsObject.GetComponent<ContentSizeFitter>();
            if (contentSizeFitter == null)
            {
                contentSizeFitter = priceOptionsObject.AddComponent<ContentSizeFitter>();
            }
            contentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize; // Passt die Höhe an den Inhalt an
            contentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained; // Keine Anpassung der Breite

            priceOptionsObject.transform.SetParent(parentTransform, false);
            priceOptionsTransform = priceOptionsObject.transform;


            /*
                GameObject toggleSpaceObject = new GameObject("ToggleSpace");
                toggleSpaceObject.transform.SetParent(priceOptionsTransform, false);

                RectTransform toggleSpaceRect = toggleSpaceObject.AddComponent<RectTransform>();
                toggleSpaceRect.sizeDelta = new Vector2(0, 35); // Spacing above Toggle Buttons */

            GameObject toggleContainer = new GameObject("ToggleContainer");
            toggleContainer.transform.SetParent(priceOptionsTransform, false);

            // Füge eine HorizontalLayoutGroup hinzu, falls sie nicht existiert
            HorizontalLayoutGroup horizontalContainerGroup = toggleContainer.GetComponent<HorizontalLayoutGroup>();
            float horizontalSpacing = 5f; // Abstand zwischen den Elementen
            if (horizontalContainerGroup == null)
            {
                horizontalContainerGroup = toggleContainer.AddComponent<HorizontalLayoutGroup>();
                horizontalContainerGroup.spacing = horizontalSpacing; // Abstand zwischen den Elementen
                horizontalContainerGroup.childAlignment = TextAnchor.MiddleLeft; // Elemente linksbündig ausrichten
                horizontalContainerGroup.childForceExpandWidth = false; // Breite der Kinder nicht erzwingen
                horizontalContainerGroup.childForceExpandHeight = false; // Höhe der Kinder nicht erzwingen
            }

            GameObject toggleSpaceObject = new GameObject("ToggleSpace");
            toggleSpaceObject.transform.SetParent(priceOptionsTransform, false);
            RectTransform saveSpaceRect = toggleSpaceObject.AddComponent<RectTransform>();
            saveSpaceRect.sizeDelta = new Vector2(100, 50); // Abstand zwischen dem letzten Element und dem Button

            LayoutElement spaceLayoutElement = toggleSpaceObject.GetComponent<LayoutElement>();
            if (spaceLayoutElement == null)
            {
                spaceLayoutElement = toggleSpaceObject.AddComponent<LayoutElement>();
                spaceLayoutElement.minHeight = 0; // Mindesthöhe des Space
                spaceLayoutElement.preferredHeight = 0; // Bevorzugte Höhe des Space
                spaceLayoutElement.minWidth = 100; // Mindestbreite des Space
                spaceLayoutElement.preferredWidth = 100; // Bevorzugte Breite des Space
            }

            // Erstelle ein neues GameObject für den Button
            GameObject toggleButtonObject = new GameObject("ToggleButton");
            toggleButtonObject.transform.SetParent(toggleContainer.transform, false);
            // toggleButtonObject.SetActive(false); // Deaktiviere den Button vorübergehend

            // Füge ein RectTransform hinzu, falls es nicht existiert
            RectTransform buttonRect = toggleButtonObject.GetComponent<RectTransform>();
            if (buttonRect == null)
            {
                buttonRect = toggleButtonObject.AddComponent<RectTransform>();
                buttonRect.sizeDelta = new Vector2((455 - horizontalSpacing) / 2, 35); // Standardgröße des Buttons
            }

            // Füge ein Button-Objekt hinzu
            Button toggleButton = toggleButtonObject.GetComponent<Button>();
            if (toggleButton == null)
            {
                toggleButton = toggleButtonObject.AddComponent<Button>();
            }

            // Füge ein Hintergrundbild hinzu
            Image buttonImage = toggleButtonObject.GetComponent<Image>();
            if (buttonImage == null)
            {
                buttonImage = toggleButtonObject.AddComponent<Image>();
                if (toggleButtonSprite == null)
                {
                    string imagePath = Path.Combine(ConfigFolder, "ToggleButton.png");
                    if (!string.IsNullOrEmpty(imagePath) && File.Exists(imagePath))
                    {
                        byte[] imageData = File.ReadAllBytes(imagePath);
                        Texture2D texture = new Texture2D(2, 2);
                        if (texture.LoadImage(imageData))
                        {
                            Sprite newSprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                            toggleButtonSprite = newSprite;
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
                    buttonImage.sprite = toggleButtonSprite;
                }
            }

            // Erstelle ein neues GameObject für den Text des Buttons
            GameObject buttonTextObject = new GameObject("ToggleButton Text");
            buttonTextObject.transform.SetParent(toggleButtonObject.transform, false);

            // Füge eine Text-Komponente hinzu
            Text buttonText = buttonTextObject.GetComponent<Text>();
            if (buttonText == null)
            {
                buttonText = buttonTextObject.AddComponent<Text>();
                buttonText.text = "Business";
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
            LayoutElement layoutElement = toggleButtonObject.GetComponent<LayoutElement>();
            if (layoutElement == null)
            {
                layoutElement = toggleButtonObject.AddComponent<LayoutElement>();
                layoutElement.minHeight = 35; // Mindesthöhe des Buttons
                layoutElement.preferredHeight = 35; // Bevorzugte Höhe des Buttons
                layoutElement.minWidth = 100; // Mindestbreite des Buttons
                layoutElement.preferredWidth = 455 / 2; // Bevorzugte Breite des Buttons
            }

            //////////////////////

            // Erstelle ein neues GameObject für den zweiten Button
            GameObject secondToggleButtonObject = new GameObject("SecondToggleButton");
            secondToggleButtonObject.transform.SetParent(toggleContainer.transform, false);
            //secondToggleButtonObject.SetActive(false); // Deaktiviere den Button vorübergehend

            // Füge ein RectTransform hinzu, falls es nicht existiert
            RectTransform secondButtonRect = secondToggleButtonObject.GetComponent<RectTransform>();
            if (secondButtonRect == null)
            {
                secondButtonRect = secondToggleButtonObject.AddComponent<RectTransform>();
                secondButtonRect.sizeDelta = new Vector2(455 / 2, 35); // Standardgröße des Buttons
            }

            // Positioniere den zweiten Button rechts neben dem ersten
            secondButtonRect.anchorMin = new Vector2(0.5f, 0.5f);
            secondButtonRect.anchorMax = new Vector2(0.5f, 0.5f);
            secondButtonRect.pivot = new Vector2(0, 0.5f);
            secondButtonRect.anchoredPosition = new Vector2(90, 0); // Abstand von 100 Einheiten nach rechts

            // Füge ein Button-Objekt hinzu
            Button secondToggleButton = secondToggleButtonObject.GetComponent<Button>();
            if (secondToggleButton == null)
            {
                secondToggleButton = secondToggleButtonObject.AddComponent<Button>();
            }

            // Füge ein Hintergrundbild hinzu
            Image secondButtonImage = secondToggleButtonObject.GetComponent<Image>();
            if (secondButtonImage == null)
            {
                secondButtonImage = secondToggleButtonObject.AddComponent<Image>();
                secondButtonImage.sprite = buttonImage.sprite; // Verwende das gleiche Sprite wie der erste Button
            }

            // Erstelle ein neues GameObject für den Text des zweiten Buttons
            GameObject secondButtonTextObject = new GameObject("SecondToggleButton Text");
            secondButtonTextObject.transform.SetParent(secondToggleButtonObject.transform, false);

            // Füge eine Text-Komponente hinzu
            Text secondButtonText = secondButtonTextObject.GetComponent<Text>();
            if (secondButtonText == null)
            {
                secondButtonText = secondButtonTextObject.AddComponent<Text>();
                secondButtonText.text = "Private";
                secondButtonText.font = FontLoader.openSansSemiBold ?? Resources.GetBuiltinResource<Font>("Arial.ttf");
                secondButtonText.fontSize = 18;
                secondButtonText.color = Color.white;
                secondButtonText.alignment = TextAnchor.MiddleCenter;
            }

            // Positioniere den Text innerhalb des zweiten Buttons
            RectTransform secondTextRect = secondButtonTextObject.GetComponent<RectTransform>();
            if (secondTextRect == null)
            {
                secondTextRect = secondButtonTextObject.AddComponent<RectTransform>();
            }
            secondTextRect.anchorMin = Vector2.zero;
            secondTextRect.anchorMax = Vector2.one;
            secondTextRect.offsetMin = Vector2.zero;
            secondTextRect.offsetMax = Vector2.zero;

            // Füge ein LayoutElement hinzu, um die Integration in die VerticalLayoutGroup zu gewährleisten
            LayoutElement secondLayoutElement = secondToggleButtonObject.GetComponent<LayoutElement>();
            if (secondLayoutElement == null)
            {
                secondLayoutElement = secondToggleButtonObject.AddComponent<LayoutElement>();
                secondLayoutElement.minHeight = 35; // Mindesthöhe des Buttons
                secondLayoutElement.preferredHeight = 35; // Bevorzugte Höhe des Buttons
                secondLayoutElement.minWidth = 455 / 2; // Mindestbreite des Buttons
                secondLayoutElement.preferredWidth = 455 / 2; // Bevorzugte Breite des Buttons
            }

            void FuncThatCallsFunc() => ShowBusinessRealEstates(secondToggleButtonObject, toggleButtonObject);
            toggleButton.onClick.AddListener((UnityAction)FuncThatCallsFunc);

            // Füge eine Aktion für den zweiten Button hinzu
            void SecondButtonAction() => ShowHomeRealEstates(secondToggleButtonObject, toggleButtonObject);
            secondToggleButton.onClick.AddListener((UnityAction)SecondButtonAction);

            AddPriceOptionsContentForRealEstate(priceOptionsTransform);
        }

        void ShowHomeRealEstates(GameObject homeButtonObject, GameObject businessButtonObject)
        {

            if (priceOptionsTransform == null)
            {
                MelonLogger.Error("priceOptionsTransform is null! Cannot show home real estates.");
                return;
            }

            // Change the Sprites of the buttons
            if (homeButtonObject != null && homeButtonObject.GetComponent<Image>() != null)
            {
                Image homeButtonImage = homeButtonObject.GetComponent<Image>();
                if (toggleButtonPressedSprite == null)
                {
                    string imagePath = Path.Combine(ConfigFolder, "ToggleButtonPressed.png");
                    if (!string.IsNullOrEmpty(imagePath) && File.Exists(imagePath))
                    {
                        byte[] imageData = File.ReadAllBytes(imagePath);
                        Texture2D texture = new Texture2D(2, 2);
                        if (texture.LoadImage(imageData))
                        {
                            Sprite newSprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                            toggleButtonPressedSprite = newSprite;
                            homeButtonImage.sprite = newSprite;
                        }
                        else
                        {
                            MelonLogger.Error($"Failed to load image from path: {imagePath}");
                            homeButtonImage.color = new Color(0.2f, 0.6f, 0.2f); // Grüne Farbe für den Button
                        }
                    }
                }
                else
                {
                    homeButtonImage.sprite = toggleButtonPressedSprite;
                }
            }

            if (businessButtonObject != null && businessButtonObject.GetComponent<Image>() != null)
            {
                Image businessButtonImage = businessButtonObject.GetComponent<Image>();
                if (toggleButtonSprite == null)
                {
                    string imagePath = Path.Combine(ConfigFolder, "ToggleButton.png");
                    if (!string.IsNullOrEmpty(imagePath) && File.Exists(imagePath))
                    {
                        byte[] imageData = File.ReadAllBytes(imagePath);
                        Texture2D texture = new Texture2D(2, 2);
                        if (texture.LoadImage(imageData))
                        {
                            Sprite newSprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                            toggleButtonSprite = newSprite;
                            businessButtonImage.sprite = newSprite;
                        }
                        else
                        {
                            MelonLogger.Error($"Failed to load image from path: {imagePath}");
                            businessButtonImage.color = new Color(0.2f, 0.6f, 0.2f); // Grüne Farbe für den Button
                        }
                    }
                }
                else
                {
                    businessButtonImage.sprite = toggleButtonSprite;
                }
            }

            // Find the Save Button and activate it
            Transform saveButton = priceOptionsTransform.Find("Save Button");
            if (saveButton != null && saveButton.gameObject != null && !saveButton.gameObject.activeSelf)
            {
                saveButton.gameObject.SetActive(true);
            }

            // Find the first 4 containers
            Transform laundromatContainer = priceOptionsTransform.Find("Laundromat Horizontal Container");
            Transform postOfficeContainer = priceOptionsTransform.Find("Post Office Horizontal Container");
            Transform carWashContainer = priceOptionsTransform.Find("Car Wash Horizontal Container");
            Transform tacoTicklersContainer = priceOptionsTransform.Find("Taco Ticklers Horizontal Container");

            // Find the other 6 containers
            Transform storageUnitContainer = priceOptionsTransform.Find("Storage Unit Horizontal Container");
            Transform motelContainer = priceOptionsTransform.Find("Motel Horizontal Container");
            Transform sweatshopContainer = priceOptionsTransform.Find("Sweatshop Horizontal Container");
            Transform bungalowContainer = priceOptionsTransform.Find("Bungalow Horizontal Container");
            Transform barnContainer = priceOptionsTransform.Find("Barn Horizontal Container");
            Transform docksContainer = priceOptionsTransform.Find("Docks Warehouse Horizontal Container");
            Transform manorContainer = priceOptionsTransform.Find("Manor Horizontal Container");

            // Deactivate the first 4 containers
            if (laundromatContainer != null) laundromatContainer.gameObject.SetActive(false);
            if (postOfficeContainer != null) postOfficeContainer.gameObject.SetActive(false);
            if (carWashContainer != null) carWashContainer.gameObject.SetActive(false);
            if (tacoTicklersContainer != null) tacoTicklersContainer.gameObject.SetActive(false);

            // Activate the other 5 containers
            if (storageUnitContainer != null) storageUnitContainer.gameObject.SetActive(true);
            if (motelContainer != null) motelContainer.gameObject.SetActive(true);
            if (sweatshopContainer != null) sweatshopContainer.gameObject.SetActive(true);
            if (bungalowContainer != null) bungalowContainer.gameObject.SetActive(true);
            if (barnContainer != null) barnContainer.gameObject.SetActive(true);
            if (docksContainer != null) docksContainer.gameObject.SetActive(true);
            if (manorContainer != null) manorContainer.gameObject.SetActive(true);

            ForceLayoutUpdate(priceOptionsTransform);
        }

        public void ForceLayoutUpdate(Transform parentTransform)
        {
            if (parentTransform == null) return;

            // Deaktiviere und aktiviere das GameObject, um das Layout neu zu berechnen
            Canvas.ForceUpdateCanvases();

            foreach (var layoutGroup in parentTransform.GetComponentsInChildren<LayoutGroup>(true))
            {
                layoutGroup.enabled = false;
                layoutGroup.enabled = true;
            }

            foreach (var contentSizeFitter in parentTransform.GetComponentsInChildren<ContentSizeFitter>(true))
            {
                contentSizeFitter.enabled = false;
                contentSizeFitter.enabled = true;
            }

            Canvas.ForceUpdateCanvases();
        }

        void ShowBusinessRealEstates(GameObject homeButtonObject, GameObject businessButtonObject)
        {
            if (priceOptionsTransform == null)
            {
                MelonLogger.Error("priceOptionsTransform is null! Cannot show home real estates.");
                return;
            }

            // Change the Sprites of the buttons
            if (homeButtonObject != null && homeButtonObject.GetComponent<Image>() != null)
            {
                Image homeButtonImage = homeButtonObject.GetComponent<Image>();
                if (toggleButtonSprite == null)
                {
                    string imagePath = Path.Combine(ConfigFolder, "ToggleButton.png");
                    if (!string.IsNullOrEmpty(imagePath) && File.Exists(imagePath))
                    {
                        byte[] imageData = File.ReadAllBytes(imagePath);
                        Texture2D texture = new Texture2D(2, 2);
                        if (texture.LoadImage(imageData))
                        {
                            Sprite newSprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                            toggleButtonSprite = newSprite;
                            homeButtonImage.sprite = newSprite;
                        }
                        else
                        {
                            MelonLogger.Error($"Failed to load image from path: {imagePath}");
                            homeButtonImage.color = new Color(0.2f, 0.6f, 0.2f); // Grüne Farbe für den Button
                        }
                    }
                }
                else
                {
                    homeButtonImage.sprite = toggleButtonSprite;
                }
            }

            if (businessButtonObject != null && businessButtonObject.GetComponent<Image>() != null)
            {
                Image businessButtonImage = businessButtonObject.GetComponent<Image>();
                if (toggleButtonPressedSprite == null)
                {
                    string imagePath = Path.Combine(ConfigFolder, "ToggleButtonPressed.png");
                    if (!string.IsNullOrEmpty(imagePath) && File.Exists(imagePath))
                    {
                        byte[] imageData = File.ReadAllBytes(imagePath);
                        Texture2D texture = new Texture2D(2, 2);
                        if (texture.LoadImage(imageData))
                        {
                            Sprite newSprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                            toggleButtonPressedSprite = newSprite;
                            businessButtonImage.sprite = newSprite;
                        }
                        else
                        {
                            MelonLogger.Error($"Failed to load image from path: {imagePath}");
                            businessButtonImage.color = new Color(0.2f, 0.6f, 0.2f); // Grüne Farbe für den Button
                        }
                    }
                }
                else
                {
                    businessButtonImage.sprite = toggleButtonPressedSprite;
                }
            }

            // Find the Save Button
            Transform saveButton = priceOptionsTransform.Find("Save Button");
            if (saveButton != null && saveButton.gameObject != null && !saveButton.gameObject.activeSelf)
            {
                saveButton.gameObject.SetActive(true);
            }

            // Find the first 4 containers
            Transform laundromatContainer = priceOptionsTransform.Find("Laundromat Horizontal Container");
            Transform postOfficeContainer = priceOptionsTransform.Find("Post Office Horizontal Container");
            Transform carWashContainer = priceOptionsTransform.Find("Car Wash Horizontal Container");
            Transform tacoTicklersContainer = priceOptionsTransform.Find("Taco Ticklers Horizontal Container");

            // Find the other 5 containers
            Transform storageUnitContainer = priceOptionsTransform.Find("Storage Unit Horizontal Container");
            Transform motelContainer = priceOptionsTransform.Find("Motel Horizontal Container");
            Transform sweatshopContainer = priceOptionsTransform.Find("Sweatshop Horizontal Container");
            Transform bungalowContainer = priceOptionsTransform.Find("Bungalow Horizontal Container");
            Transform barnContainer = priceOptionsTransform.Find("Barn Horizontal Container");
            Transform docksContainer = priceOptionsTransform.Find("Docks Warehouse Horizontal Container");
            Transform mannorContainer = priceOptionsTransform.Find("Manor Horizontal Container");

            // Activte the first 4 containers
            if (laundromatContainer != null) laundromatContainer.gameObject.SetActive(true);
            if (postOfficeContainer != null) postOfficeContainer.gameObject.SetActive(true);
            if (carWashContainer != null) carWashContainer.gameObject.SetActive(true);
            if (tacoTicklersContainer != null) tacoTicklersContainer.gameObject.SetActive(true);

            // Deactivate the other 5 containers
            if (storageUnitContainer != null) storageUnitContainer.gameObject.SetActive(false);
            if (motelContainer != null) motelContainer.gameObject.SetActive(false);
            if (sweatshopContainer != null) sweatshopContainer.gameObject.SetActive(false);
            if (bungalowContainer != null) bungalowContainer.gameObject.SetActive(false);
            if (barnContainer != null) barnContainer.gameObject.SetActive(false);
            if (docksContainer != null) docksContainer.gameObject.SetActive(false);
            if (mannorContainer != null) mannorContainer.gameObject.SetActive(false);

            ForceLayoutUpdate(priceOptionsTransform);
        }

        public void AddPriceOptionsContentForRealEstate(Transform parentTransform)
        {
            if (parentTransform == null)
            {
                MelonLogger.Error("parentTransform is null! Cannot add price options.");
                return;
            }

            // Füge die Label-Input-Paare für die vier Unternehmen hinzu
            AddLabelInputPair("Laundromat", parentTransform, "$");
            AddLabelInputPair("Post Office", parentTransform, "$");
            AddLabelInputPair("Car Wash", parentTransform, "$");
            AddLabelInputPair("Taco Ticklers", parentTransform, "$");

            AddLabelInputPair("Storage Unit", parentTransform, "$");
            AddLabelInputPair("Motel", parentTransform, "$");
            AddLabelInputPair("Sweatshop", parentTransform, "$");
            AddLabelInputPair("Bungalow", parentTransform, "$");
            AddLabelInputPair("Barn", parentTransform, "$");
            AddLabelInputPair("Docks Warehouse", parentTransform, "$");

            //Not yet ingame
            AddLabelInputPair("Manor", parentTransform, "$");

            // Füge einen Save-Button hinzu
            AddSaveButton(parentTransform, "RealEstate");

            if (parentTransform.FindChild("Space") != null)
            {
                parentTransform.FindChild("Space").SetAsLastSibling();
            }
            parentTransform.gameObject.SetActive(false);

            // MelonLogger.Msg("Added price options for real estate to priceOptionsTransform.");
        }

        public void AddVehicleOptions(Transform parentTransform)
        {
            if (parentTransform == null)
            {
                MelonLogger.Error("parentTransform is null! Cannot add price options.");
                return;
            }

            GameObject vehicleOptionsObject = new GameObject("VehicleOptions");

            //  LayoutElement layoutElement = generalSettingsObject.AddComponent<LayoutElement>();
            VerticalLayoutGroup contentVerticalLayout = vehicleOptionsObject.GetComponent<VerticalLayoutGroup>();
            if (contentVerticalLayout == null)
            {
                contentVerticalLayout = vehicleOptionsObject.gameObject.AddComponent<VerticalLayoutGroup>();
            }
            contentVerticalLayout.childAlignment = TextAnchor.UpperLeft;
            contentVerticalLayout.spacing = 15f; // Abstand zwischen den Optionen 
            contentVerticalLayout.childControlWidth = true;
            contentVerticalLayout.childControlHeight = true;
            contentVerticalLayout.childForceExpandWidth = false;
            contentVerticalLayout.childForceExpandHeight = false;
            // Add padding to the VerticalLayoutGroup using RectOffset
            contentVerticalLayout.padding = new RectOffset(15, 15, 15, 15); // Left: 15, Right: 15, Top: 0, Bottom: 0

            // Spacing from left
            RectTransform optionsRect = vehicleOptionsObject.GetComponent<RectTransform>();
            if (optionsRect != null)
            {
                optionsRect.offsetMin = new Vector2(60, optionsRect.offsetMin.y + 60);
                // Layout -> RectOffset -> padding
            }

            // Füge einen ContentSizeFitter hinzu
            ContentSizeFitter contentSizeFitter = vehicleOptionsObject.GetComponent<ContentSizeFitter>();
            if (contentSizeFitter == null)
            {
                contentSizeFitter = vehicleOptionsObject.AddComponent<ContentSizeFitter>();
            }
            contentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize; // Passt die Höhe an den Inhalt an
            contentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained; // Keine Anpassung der Breite
            vehicleOptionsObject.transform.SetParent(parentTransform, false);
            vehicleOptionsTransform = vehicleOptionsObject.transform;

            // Füge die Label-Input-Paare für die vier Unternehmen hinzu
            AddLabelInputPair("Shitbox", vehicleOptionsTransform, "$");
            AddLabelInputPair("Veeper", vehicleOptionsTransform, "$");
            AddLabelInputPair("Bruiser", vehicleOptionsTransform, "$");
            AddLabelInputPair("Dinkler", vehicleOptionsTransform, "$");
            AddLabelInputPair("Hounddog", vehicleOptionsTransform, "$");
            AddLabelInputPair("Cheetah", vehicleOptionsTransform, "$");

            // Füge einen Save-Button hinzu
            AddSaveButton(vehicleOptionsTransform, "Vehicles");

            if (parentTransform.FindChild("Space") != null)
            {
                parentTransform.FindChild("Space").SetAsLastSibling();
            }
            vehicleOptionsObject.SetActive(false);
        }

        public void AddSkateboardOptions(Transform parentTransform)
        {
            if (parentTransform == null)
            {
                MelonLogger.Error("detailEntriesTransform is null! Cannot add price options.");
                return;
            }

            GameObject skateboardOptionsObject = new GameObject("SkateboardOptions");
            //  LayoutElement layoutElement = generalSettingsObject.AddComponent<LayoutElement>();
            VerticalLayoutGroup contentVerticalLayout = skateboardOptionsObject.GetComponent<VerticalLayoutGroup>();
            if (contentVerticalLayout == null)
            {
                contentVerticalLayout = skateboardOptionsObject.gameObject.AddComponent<VerticalLayoutGroup>();
            }
            contentVerticalLayout.childAlignment = TextAnchor.UpperLeft;
            contentVerticalLayout.spacing = 15f; // Abstand zwischen den Optionen 
            contentVerticalLayout.childControlWidth = true;
            contentVerticalLayout.childControlHeight = true;
            contentVerticalLayout.childForceExpandWidth = false;
            contentVerticalLayout.childForceExpandHeight = false;
            // Add padding to the VerticalLayoutGroup using RectOffset
            contentVerticalLayout.padding = new RectOffset(15, 15, 15, 15); // Left: 15, Right: 15, Top: 0, Bottom: 0

            // Spacing from left
            RectTransform optionsRect = skateboardOptionsObject.GetComponent<RectTransform>();
            if (optionsRect != null)
            {
                optionsRect.offsetMin = new Vector2(60, optionsRect.offsetMin.y + 60);
                // Layout -> RectOffset -> padding
            }

            // Füge einen ContentSizeFitter hinzu
            ContentSizeFitter contentSizeFitter = skateboardOptionsObject.GetComponent<ContentSizeFitter>();
            if (contentSizeFitter == null)
            {
                contentSizeFitter = skateboardOptionsObject.AddComponent<ContentSizeFitter>();
            }
            contentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize; // Passt die Höhe an den Inhalt an
            contentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained; // Keine Anpassung der Breite
            skateboardOptionsObject.transform.SetParent(parentTransform, false);
            skateboardOptionsTransform = skateboardOptionsObject.transform;

            // Füge die Label-Input-Paare für die vier Unternehmen hinzu
            AddLabelInputPair("Cheap Skateboard", skateboardOptionsTransform, "$");
            AddLabelInputPair("Skateboard", skateboardOptionsTransform, "$");
            AddLabelInputPair("Lightweight Board", skateboardOptionsTransform, "$");
            AddLabelInputPair("Cruiser", skateboardOptionsTransform, "$");
            AddLabelInputPair("Golden Skateboard", skateboardOptionsTransform, "$");

            // Füge einen Save-Button hinzu
            AddSaveButton(skateboardOptionsTransform, "Skateboards");

            if (parentTransform.FindChild("Space") != null)
            {
                parentTransform.FindChild("Space").SetAsLastSibling();
            }
            skateboardOptionsObject.SetActive(false);
        }

        public GameObject DansHardwareTemplate;
        public GameObject GasMartWestTemplate;
        public GameObject viewPortContentSpaceTemplate;
        public Transform launderingAppViewportContentTransform;
        private static readonly string ConfigFolder = Path.Combine(MelonEnvironment.UserDataDirectory, "MoreRealisticLaundering");
        private static readonly string FilePath = Path.Combine(ConfigFolder, "LaunderingIcon.png");
        public static string _appName;
        public bool _isLaunderingAppLoaded = false;
        private GameObject detailsTitleObject;
        private GameObject InstructionsTextObject;
        private GameObject detailsSubtitleObject;
        private Transform settingsContentTransform;
        public Transform optionsTransform = null;
        public Transform priceOptionsTransform = null;
        public Transform vehicleOptionsTransform = null;
        public Transform skateboardOptionsTransform = null;
        public GameObject checkboxTemplate = null;
        Sprite inputBackgroundSprite = null;
        Sprite saveButtonSprite = null;
        Sprite toggleButtonSprite = null;
        Sprite toggleButtonPressedSprite = null;
        Sprite appIconSprite = null;
        Sprite raysRealEstateSprite = null;
        Sprite shredShackSprite = null;
        Sprite hylandAutoSprite = null;
        Sprite laundromatSprite = null;
        Sprite postOfficeSprite = null;
        Sprite carWashSprite = null;
        Sprite tacoTicklersSprite = null;
        private Business _selectedBusiness = null;
        public bool isSaveStillRunning = false;
    }
}