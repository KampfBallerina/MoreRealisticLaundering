using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using MelonLoader.Utils;


namespace MoreRealisticLaundering.Util
{
    public static class Utils
    {
        public static GameObject GetAppIconByName(string Name, int? Index)
        {
            int valueOrDefault = Index.GetValueOrDefault();
            return (from t in (IEnumerable<Transform>)GameObject.Find("Player_Local/CameraContainer/Camera/OverlayCamera/GameplayMenu/Phone/phone/HomeScreen/AppIcons/").GetComponentsInChildren<Transform>(includeInactive: true)
                    let labelTransform = t.gameObject.transform.Find("Label")
                    let textComponent = (labelTransform != null) ? labelTransform.GetComponent<Text>() : null
                    where textComponent != null && textComponent.text != null && textComponent.text.StartsWith(Name)
                    select t.gameObject).ToArray()[valueOrDefault];
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


    }
}