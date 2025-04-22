using UnityEngine;
using MelonLoader;
using UnityEngine.UI;
using System.Collections;

namespace MoreRealisticLaundering.Util
{
    public static class FontLoader
    {
        public static IEnumerator InitializeFonts()
        {
            openSansBold = FindFontFromOtherApp("openSansBold");
            openSansSemiBold = FindFontFromOtherApp("openSansSemiBold");
            yield break;
        }
        public static Font FindFontFromOtherApp(string fontName)
        {
            Font font = null;
            if (fontName == "openSansBold")
            {
                GameObject canvas = Utils.GetAppCanvasByName("ProductManagerApp/Container/Topbar");
                Text textComponent = canvas.GetComponentInChildren<Text>();
                if (textComponent != null)
                {
                    font = textComponent.font;
                    openSansBoldIsInitialized = true;
                    MelonLogger.Msg("Initialized openSansBold Font");
                }
                return font;
            }
            else if (fontName == "openSansSemiBold")
            {
                GameObject canvas = Utils.GetAppCanvasByName("DeliveryApp/Container/Scroll View/Viewport/Content/Albert Hoover/Header");
                Text textComponent = canvas.GetComponentInChildren<Text>();
                if (textComponent != null)
                {
                    font = textComponent.font;
                    openSansSemiBoldIsInitialized = true;

                    MelonLogger.Msg("Initialized openSansSemiBold Font");
                }
                return font;
            }
            else return Resources.GetBuiltinResource<Font>("Arial.ttf");
        }

        public static Font openSansBold;
        public static bool openSansBoldIsInitialized;
        public static Font openSansSemiBold;
        public static bool openSansSemiBoldIsInitialized;
    }
}
