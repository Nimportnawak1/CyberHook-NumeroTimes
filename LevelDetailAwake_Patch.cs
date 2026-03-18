using UnityEngine;
using HarmonyLib;
using MelonLoader;
using UnityEngine.UI;
using Org.BouncyCastle.Asn1.TeleTrust;
using FMOD.Studio;

namespace CyberHookNumeroTimes
{
    /* This patch modifies the structure of the side panel in the level select screen.
       This allows for the panel to be updated when needed in LevelDetailUpdate_Patch. */

    [HarmonyPatch(typeof(LevelDetail), "Awake")]
    class LevelDetailAwake_Patch
    {
        static Color numeroColor = new Color(0f, 0.5f, 0f);

        static void Postfix(LevelDetail __instance) 
        {
            // Check for specific gameObject to avoid affecting the LevelDetail for workshop levels
            if (__instance.name != "Level Details")
                return;

            Transform starsGoldContainer = __instance.StarsGoldContainer.transform;

            // 3 diamonds display is permanently replaced by Numero diamonds display
            // 3 diamonds times will be shown using a modified pre-3 diamonds display; see LevelDetailUpdate_Patch.cs
            for (int i = 0; i < 3; i++)
            {
                Transform starFill = starsGoldContainer.GetChild(i).Find("StarFill_Super");

                UnityEngine.UI.Image starFillImage;
                starFill.TryGetComponent<UnityEngine.UI.Image>(out starFillImage);

                if (starFillImage != null)
                    starFillImage.color = numeroColor;
            }
        }
    }
}