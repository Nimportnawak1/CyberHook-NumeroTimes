using UnityEngine;
using HarmonyLib;
using MelonLoader;
using UnityEngine.UI;
using TMPro;
using FluffyUnderware.Curvy.Generator;
using System.Collections;

namespace CyberHookNumeroTimes
{
    /* This patch turns the diamonds and the light screen green when needed for the victory screen. */

    [HarmonyPatch(typeof(PauseManager), nameof(PauseManager.OpenVictory))]
    class VictorySceen_Patch
    {
        static Color numeroColor = Color.green;

        static void Postfix(PauseManager __instance, float finalTime)
        {
            // Retrieving best time player has ever achieved
            float bestTime = __instance.PreviousBestTime;
            if (finalTime < bestTime) bestTime = finalTime;
            
            // Starting coroutine
            MelonCoroutines.Start(DelayedPostfix(bestTime));
        }

        static System.Collections.IEnumerator DelayedPostfix(float bestTime)
        {
            yield return new WaitForSecondsRealtime(0.5f);

            // Check if Numero time has been beaten
            string levelId = LevelManager.Instance.LevelDataSO.LevelUniqueID;

            if (!LevelTimes.times.ContainsKey(levelId))
            {
                Melon<Main>.Logger.Error("Unable to find Numero time for level {0}", levelId);
                yield break;
            }

            float timeGoal = LevelTimes.times[levelId];

            if (bestTime >= timeGoal) yield break; // If not beaten, do nothing


            // Changing the light wave color
            GameObject lightWave = GameObject.Find("Light Wave");
            UnityEngine.UI.Image lightWaveImage;
            lightWave.TryGetComponent<UnityEngine.UI.Image>(out lightWaveImage);
            lightWaveImage.color = numeroColor;

            // Changing the light wave particles color
            UnityEngine.ParticleSystem[] particleSystems = Resources.FindObjectsOfTypeAll<UnityEngine.ParticleSystem>();
            UnityEngine.ParticleSystem lightWaveParticleSystem = particleSystems.FirstOrDefault(c => c.name == "LightWave particles");

            UnityEngine.ParticleSystem.MainModule particleSystemMain = lightWaveParticleSystem.main;
            particleSystemMain.startColor = numeroColor;

            // Changing the diamonds color
            GameObject starsContainer = GameObject.Find("Stars Container");
            if (starsContainer == null)
            {
                Melon<Main>.Logger.Error("Diamonds container not found, unable to change diamond color");
                yield break;
            }

            for (int i = 0; i < 3; i++)
            {
                Transform starFill = starsContainer.transform.GetChild(i).Find("Super Mask").Find("StarFill_Super");

                UnityEngine.UI.Image starFillImage;
                starFill.TryGetComponent<UnityEngine.UI.Image>(out starFillImage);

                starFillImage.color = numeroColor;
            }
        }
    }
}