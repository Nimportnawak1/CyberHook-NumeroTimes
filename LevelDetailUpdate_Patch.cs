using UnityEngine;
using HarmonyLib;
using MelonLoader;
using UnityEngine.UI;
using Org.BouncyCastle.Asn1.TeleTrust;
using FMOD.Studio;
using FluffyUnderware.DevTools.Extensions;
using System.Runtime.CompilerServices;

namespace CyberHookNumeroTimes
{
    /* This patch updates the side panel in the level select screen, modifying behavior for the 3-diamonds state
       and adding new behavior when Numero time is achieved. */

    [HarmonyPatch(typeof(LevelDetail), nameof(LevelDetail.UpdateDetail))]
    class LevelDetailUpdate_Patch
    {
        static Color goldColor = new Color(0.4f, 0.3656f, 0.025f);

        static Color blueColor = new Color(0.0627f, 0.3412f, 0.5294f);

        static Color numeroColor = new Color(0f, 0.5f, 0f);

        static void Postfix(LevelDetail __instance, SO_Level levelToDisplay) 
        {
            // Check for specific gameObject to avoid affecting the LevelDetail for workshop levels
            if (__instance.name != "Level Details")
                return;

            // Retrieving private UILevelDiv player just clicked on
            var levelDivField = AccessTools.Field(typeof(LevelDetail), "_levelDivSender");
            UILevelDiv levelDiv = (UILevelDiv)levelDivField.GetValue(__instance);

            // If UILevelDiv's name was changed by LevelDivsDiamondColor_Patch's Postfix, then we know Numero time was achieved
            // Default behavior will show the diamonds as Numero times, therefore we can stop the Postfix
            if (levelDiv.name == LevelDivsDiamondColor_Patch.numeroObtainedObjName)
                return;

            if (levelToDisplay.GetStarsUnlocked() == 3)
            {
                // Modified behavior when the player only has the vanilla 3 diamonds: we re-use the pre-3 diamonds display instead.
                // This allows to re-use the time goal object to show the player how to achieve the modded Numero time.

                __instance.StarsNormalContainer.SetActive(true);
                __instance.StarsGoldContainer.SetActive(false);

                // Turning the blue diamonds gold
                foreach (GameObject starFill in __instance.StarsNormal)
                {
                    UnityEngine.UI.Image starFillImage;
                    starFill.TryGetComponent<UnityEngine.UI.Image>(out starFillImage);

                    starFillImage.color = goldColor;
                }

                // Changing the timer to the modded time requirement
                string levelId = levelToDisplay.LevelUniqueID;

                if (!LevelTimes.times.ContainsKey(levelId))
                    __instance.TargetTime.text = "--:--.--";
                else
                    __instance.TargetTime.text = LevelTimes.times[levelId].ToTimerString();

                __instance.TargetTime.color = Color.green;

                // Changing the diamond next to the time req to the Numero time color
                Transform diamondTargetTime = __instance.TargetTime.transform.GetChild(0).GetChild(0);

                UnityEngine.UI.Image diamondTargetTimeImage;
                diamondTargetTime.TryGetComponent<UnityEngine.UI.Image>(out diamondTargetTimeImage);

                diamondTargetTimeImage.color = numeroColor;
            }
            else
            {
                // If vanilla 3 diamonds aren't achieved yet, revert pre-3 diamonds display to normal colors

                // Main diamonds display
                foreach (GameObject starFill in __instance.StarsNormal)
                {
                    UnityEngine.UI.Image starFillImage;
                    starFill.TryGetComponent<UnityEngine.UI.Image>(out starFillImage);

                    starFillImage.color = blueColor;
                }

                // Target time
                __instance.TargetTime.color = Color.white;

                // Diamond next to target time
                Transform diamondTargetTime = __instance.TargetTime.transform.GetChild(0).GetChild(0);

                UnityEngine.UI.Image diamondTargetTimeImage;
                diamondTargetTime.TryGetComponent<UnityEngine.UI.Image>(out diamondTargetTimeImage);

                diamondTargetTimeImage.color = blueColor;
            }
        }
    }
}