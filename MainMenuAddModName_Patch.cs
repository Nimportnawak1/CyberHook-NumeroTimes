using UnityEngine;
using HarmonyLib;
using MelonLoader;
using UnityEngine.UI;
using Org.BouncyCastle.Asn1.TeleTrust;
using FMOD.Studio;
using System.Reflection;
using TMPro;
using UnityEngine.SceneManagement;
using FluffyUnderware.DevTools.Extensions;

namespace CyberHookNumeroTimes
{
    /* This patch creates the "+ NUMERO TIMES" text on the title screen. */

    [HarmonyPatch(typeof(MenuPanel), nameof(MenuPanel.OnIsOpen))]
    class MainMenuAddModName_Patch
    {
        public static TextMeshProUGUI textMesh;

        public static bool firstMenuOpen = true;

        static void Postfix(MenuPanel __instance) 
        {
            if (__instance.name != "Main Menu")
                return;

            if (!firstMenuOpen)
            {
                // Don't recreate the text, only do the fade-in
                MelonCoroutines.Start(TextFadeIn());
                return;
            }

            // Create and position correctly text
            GameObject playText = GameObject.Find("Play 2").transform.Find("Text").gameObject;
            Transform cyberHookLogo = GameObject.Find("CyberHook Mini").transform;

            GameObject modText = GameObject.Instantiate(playText, cyberHookLogo);

            modText.transform.localPosition = new Vector3(38f, -28f, 0f);
            modText.transform.Rotate(0f, 180f, 0f);

            // Prevent StringParser from changing back text
            modText.TryGetComponent<StringParser>(out StringParser textStringParser);
            textStringParser.Destroy();

            // Setting text visuals
            modText.TryGetComponent<TMPro.TextMeshProUGUI>(out textMesh);

            textMesh.text = "+ numero times";

            textMesh.enableWordWrapping = false;
            textMesh.fontSize = 5;

            textMesh.faceColor = new Color32(0, 255, 25, 255);

            MelonCoroutines.Start(TextFadeIn());

            textMesh.outlineWidth = 0f;

            firstMenuOpen = false;
        }

        static System.Collections.IEnumerator TextFadeIn()
        {
            textMesh.color = Color.clear;

            float time = 0f;
            float duration = 0.5f;

            yield return new WaitForSeconds(1.5f);

            textMesh.transform.localPosition = new Vector3(38f, -28f, 0f);

            while (time < duration)
            {
                time += Time.deltaTime;

                float a = Mathf.Lerp(0f, 1f, (time / duration));
                textMesh.color = new Color(1f, 1f, 1f, a);

                yield return null;
            }

            textMesh.color = Color.white;
        }
    }

    [HarmonyPatch(typeof(MainManager), nameof(MainManager.LoadGameLevelEnumerator))]
    class MainManagerLoadLevel_Patch
    {
        static void Prefix()
        {
            MainMenuAddModName_Patch.firstMenuOpen = true;
        }
    }

    /* Could not be bothered to figure out why the text appeared for a split second when backing out
     * of the Options and Replay menu, so this patch teleports it somewhere else during that time */
    [HarmonyPatch(typeof(MenuPanel), nameof(MenuPanel.SwitchTo))]
    class MenuPanelSwitch_Patch
    {
        static void Prefix()
        {
            if (MainMenuAddModName_Patch.textMesh != null)
                MelonCoroutines.Start(TextDisappearHack());
        }

        static System.Collections.IEnumerator TextDisappearHack()
        {
            yield return new WaitForSeconds(0.5f);

            // Good code practice
            MainMenuAddModName_Patch.textMesh.transform.position = new Vector3(5000f, -5000f, 0f);
        }
    }
}