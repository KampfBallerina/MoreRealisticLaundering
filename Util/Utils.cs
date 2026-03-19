using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Reflection;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using MelonLoader.Utils;
using MelonLoader;

namespace MoreRealisticLaundering.Util
{
    public static class Utils
    {
        /// <summary>
        /// Finds the AppIcons container, handling S1API 2.9.6+ scroll view compatibility.
        /// S1API moves icons into HomeScreen/AppIconsScrollView/Viewport/AppIcons,
        /// leaving a stub at the original path. We target the real container first.
        /// </summary>
        public static GameObject FindAppIconsContainer()
        {
            GameObject scrollViewIcons = GameObject.Find(
                "Player_Local/CameraContainer/Camera/OverlayCamera/GameplayMenu/Phone/phone/HomeScreen/AppIconsScrollView/Viewport/AppIcons");
            if (scrollViewIcons != null)
                return scrollViewIcons;

            return GameObject.Find(
                "Player_Local/CameraContainer/Camera/OverlayCamera/GameplayMenu/Phone/phone/HomeScreen/AppIcons");
        }

        public static GameObject GetAppIconByName(string Name, int? Index, string newObjectName = null)
        {
            // MelonLogger.Msg($"Searching for AppIcon with Name: {Name} and Index: {Index}");
            int valueOrDefault = Index.GetValueOrDefault();

            GameObject iconsContainer = FindAppIconsContainer();
            if (iconsContainer == null)
            {
                MelonLogger.Error("Could not find AppIcons container.");
                return null;
            }

            var appIcons = (from t in (IEnumerable<Transform>)iconsContainer.GetComponentsInChildren<Transform>(includeInactive: true)
                            let labelTransform = t.gameObject.transform.Find("Label")
                            let textComponent = (labelTransform != null) ? labelTransform.GetComponent<Text>() : null
                            where textComponent != null && textComponent.text != null && textComponent.text.StartsWith(Name)
                            select t.gameObject).ToArray();

            // MelonLogger.Msg($"Found {appIcons.Length} AppIcons matching the criteria.");

            if (valueOrDefault >= appIcons.Length)
            {
                MelonLogger.Error($"Index {valueOrDefault} is out of range. Returning null.");
                return null;
            }

            var selectedAppIcon = appIcons[valueOrDefault];
            if (newObjectName != null)
            {
                selectedAppIcon.name = newObjectName;
                //  MelonLogger.Msg($"Renamed AppIcon to: {newObjectName}");
            }

            // MelonLogger.Msg($"Returning AppIcon: {selectedAppIcon.name}");
            return selectedAppIcon;
        }

        public static GameObject ChangeLabelFromAppIcon(string appIconName, string newLabel)
        {
            if (string.IsNullOrEmpty(appIconName) || string.IsNullOrEmpty(newLabel))
            {
                MelonLogger.Error("AppIcon name or new label is null or empty.");
                return null;
            }

            // Try index 1 first (for when clones exist), fallback to 0
            GameObject appIconByName = Utils.GetAppIconByName(appIconName, 1, newLabel);
            appIconByName ??= Utils.GetAppIconByName(appIconName, 0, newLabel);

            if (appIconByName == null)
            {
                MelonLogger.Error($"Could not find app icon: {appIconName}");
                return null;
            }

            Transform labelTransform = appIconByName.transform.Find("Label");
            if (labelTransform != null)
            {
                Text labelText = labelTransform.GetComponent<Text>();
                if (labelText != null)
                {
                    labelText.text = newLabel;
                }
            }
            return appIconByName;
        }

        public static GameObject GetAppCanvasByName(string Name)
        {
            return GameObject.Find("Player_Local/CameraContainer/Camera/OverlayCamera/GameplayMenu/Phone/phone/AppsCanvas/").transform.Find(Name).gameObject;
        }

        public static IEnumerator WaitForObjectByFrame(string path, Action<GameObject> callback)
        {
            GameObject obj = null;
            while (obj == null)
            {
                obj = GameObject.Find(path);
                yield return null;
            }
            callback?.Invoke(obj);
        }

        public static IEnumerator WaitForObject(string path, Action<GameObject> callback, float timeout = 30f)
        {
            GameObject obj = null;
            float timer = 0f;
            while (obj == null && timer < timeout)
            {
                obj = GameObject.Find(path);
                if (obj != null)
                {
                    break;
                }
                yield return new WaitForSeconds(0.1f);
                timer += 0.1f;
            }
            callback?.Invoke(obj);
        }

        public static void ClearChildren(Transform parent, Func<GameObject, bool> keepFilter = null)
        {
            GameObject[] array = (from t in (IEnumerable<Transform>)parent.GetComponentsInChildren<Transform>(includeInactive: true)
                                  select t.gameObject into obj
                                  where obj.transform != parent
                                  select obj).ToArray();
            foreach (GameObject gameObject in array)
            {
                if (keepFilter == null || !keepFilter(gameObject))
                {
                    gameObject.transform.parent = null;
                    UnityEngine.Object.Destroy(gameObject);
                }
            }
        }

        public static Texture2D LoadCustomImage(string path)
        {
            string path2 = Path.Combine(MelonEnvironment.UserDataDirectory, path);
            Texture2D result;
            if (!File.Exists(path2))
            {
                result = null;
                Debug.LogError("Specified path does not exist.");
            }
            else
            {
                byte[] array = File.ReadAllBytes(path2);
                Texture2D texture2D = new Texture2D(2, 2);
                ImageConversion.LoadImage(texture2D, array);
                result = texture2D;
            }
            return result;
        }

        /// <summary>
        /// Loads an embedded resource image from the assembly and returns it as a byte array.
        /// </summary>
        /// <param name="resourceName">The name of the resource file (e.g., "LaunderingIcon.png")</param>
        /// <returns>Byte array of the image data, or null if not found</returns>
        public static byte[] LoadEmbeddedResource(string resourceName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            // Resource names are prefixed with namespace and folder: MoreRealisticLaundering.Assets.filename.png
            string fullResourceName = $"MoreRealisticLaundering.Assets.{resourceName}";

            using (var stream = assembly.GetManifestResourceStream(fullResourceName))
            {
                if (stream == null)
                {
                    MelonLogger.Warning($"Embedded resource not found: {fullResourceName}");
                    return null;
                }

                byte[] buffer = new byte[stream.Length];
                stream.Read(buffer, 0, buffer.Length);
                return buffer;
            }
        }

        /// <summary>
        /// Loads an embedded resource image and creates a Texture2D from it.
        /// </summary>
        /// <param name="resourceName">The name of the resource file (e.g., "LaunderingIcon.png")</param>
        /// <returns>Texture2D or null if resource not found</returns>
        public static Texture2D LoadEmbeddedTexture(string resourceName)
        {
            byte[] imageData = LoadEmbeddedResource(resourceName);
            if (imageData == null)
                return null;

            Texture2D texture = new Texture2D(2, 2);
            if (ImageConversion.LoadImage(texture, imageData))
            {
                return texture;
            }

            MelonLogger.Error($"Failed to load texture from embedded resource: {resourceName}");
            return null;
        }

        /// <summary>
        /// Loads an embedded resource image and creates a Sprite from it.
        /// </summary>
        /// <param name="resourceName">The name of the resource file (e.g., "LaunderingIcon.png")</param>
        /// <returns>Sprite or null if resource not found</returns>
        public static Sprite LoadEmbeddedSprite(string resourceName)
        {
            Texture2D texture = LoadEmbeddedTexture(resourceName);
            if (texture == null)
                return null;

            return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        }
    }
}