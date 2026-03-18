using UnityEngine;
using HarmonyLib;
using MelonLoader;
using UnityEngine.UI;
using Org.BouncyCastle.Asn1.TeleTrust;
using FMOD.Studio;

namespace CyberHookNumeroTimes
{
    /* This patch turns the diamonds green when needed for the level select screen. */

    [HarmonyPatch(typeof(UILevelDiv), nameof(UILevelDiv.UpdateDiv))]
    class LevelDivsDiamondColor_Patch
    {
        static Color numeroColor = new Color(0f, 0.5f, 0f);

        public static string numeroObtainedObjName = "NUMEROTIMEOBTAINED";

        static void Postfix(UILevelDiv __instance) 
        {
            // If the div corresponds to a workshop level, do nothing
            if (__instance is WorkshopLevelDiv)
                return;

            string levelId = __instance.TargetLevel.LevelUniqueID;

            if (!LevelTimes.times.ContainsKey(levelId))
            {
                Melon<Main>.Logger.Error("Unable to find Numero time for \"{0}\" ({1})", __instance.LevelTitle.TargetTextField.text, levelId);
                return;
            }

            float bestTime = __instance.TargetLevel.SerializedData.BestTime;

            if (bestTime <= 0.0 || bestTime > LevelTimes.times[levelId])
                return;

            // StarThree always contains all of the StarFill_Super, while StarOne and StarTwo only have the StarFill which are hidden behind the Supers
            foreach (GameObject starSuper in __instance.StarThree)
            {
                UnityEngine.UI.Image starSuperImage;
                starSuper.TryGetComponent<UnityEngine.UI.Image>(out starSuperImage);

                if (starSuperImage != null)
                    starSuperImage.color = numeroColor;
            }

            // Little hack to pass the info that the super time has been achieved to the LevelDetail panel
            __instance.name = numeroObtainedObjName;
        }
    }
}