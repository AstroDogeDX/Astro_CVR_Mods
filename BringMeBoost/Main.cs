using HarmonyLib;
using MelonLoader;
using UnityEngine;
using MelonLoader.Preferences;
using ABI_RC.Systems.InputManagement.InputModules;
using ABI_RC.Systems.InputManagement;
using Astro.BringMeBoost.Properties;

namespace Astro.BringMeBoost;

public class BringMeBoost : MelonMod
{
    public override void OnInitializeMelon()
    {
        MelonLogger.Msg($"Loaded! {AssemblyInfoParams.Version}");
    }

    [HarmonyPatch]
    public class HarmonyPatches
    {
        public static BringMeBoost _BringMeBoost;

        [HarmonyPostfix]
        [HarmonyPatch(typeof(CVRInputModule_XR), nameof(CVRInputModule_XR.Update_Vehicle))]
        public static void after_CVRInputModule_XR(CVRInputModule_XR __instance)
        {
            if (!__instance._inputManager.IsControllerPointedAtMenu(false, false))
            {
                __instance._inputManager.boost += Mathf.Clamp01(__instance._rightModule.Primary2DAxis.y * 1f);
            }
        }
    }
}