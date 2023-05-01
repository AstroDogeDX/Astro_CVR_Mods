using ABI_RC.Core.Player;
using ABI_RC.Core.Savior;
using HarmonyLib;
using MelonLoader;
using UnityEngine;

namespace Astro.ScrollZoom;

public class ScrollZoom : MelonMod
{

    public override void OnInitializeMelon()
    {
        var AstroString = "Lookitmee!";
        MelonLogger.Msg($"Hello! {AstroString}");
    }

    [HarmonyPatch]
    public class HarmonyPatches
    {
        public static float maxZoom;
        [HarmonyPrefix]
        [HarmonyPatch(typeof(CVR_DesktopCameraController), nameof(CVR_DesktopCameraController.Start))]
        public static bool before_CVR_DesktopCameraController_Start()
        {
            MelonLogger.Msg("I'm gonna do a run.");
            return true;
        }
        [HarmonyPostfix]
        [HarmonyPatch(typeof(CVR_DesktopCameraController), nameof(CVR_DesktopCameraController.Start))]
        public static void after_CVR_DesktopCameraController_Start(CVR_DesktopCameraController __instance)
        {
            MelonLogger.Msg("Woah, I did the run!");
            maxZoom = 69f;
        }
        [HarmonyPrefix]
        [HarmonyPatch(typeof(CVR_DesktopCameraController), nameof(CVR_DesktopCameraController.Update))]
        public static bool before_CVR_DesktopCameraController_Update(CVR_DesktopCameraController __instance)
        {
            if (!CVR_DesktopCameraController.enableZoom)
            {
                return false;
            }
            var isZoom = CVRInputManager.Instance.zoom ? 1f : 0f;
            CVR_DesktopCameraController._cam.fieldOfView = Mathf.Lerp(CVR_DesktopCameraController.defaultFov, CVR_DesktopCameraController.zoomFov, isZoom);
            return false;
        }
    }
}