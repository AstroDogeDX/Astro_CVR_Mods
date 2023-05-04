using ABI_RC.Core.Player;
using ABI_RC.Core.Savior;
using ABI_RC.Core.Util.Object_Behaviour;
using HarmonyLib;
using MelonLoader;
using UnityEngine;

namespace Astro.ScrollZoom;

public class ScrollZoom : MelonMod {
    private MelonPreferences_Category mainCategory;
    private MelonPreferences_Entry<bool> holdToZoom;
    private MelonPreferences_Entry<bool> rememberLastZoomLevel;
    private MelonPreferences_Entry<float> maxZoomLevel;
    private MelonPreferences_Entry<float> zoomStepAmount;

    public override void OnInitializeMelon() {
        mainCategory = MelonPreferences.CreateCategory("ScrollZoom");
        holdToZoom = mainCategory.CreateEntry<bool>("Hold to Zoom", true);
        rememberLastZoomLevel = mainCategory.CreateEntry<bool>("Remember Zoom Level", true);
        maxZoomLevel = mainCategory.CreateEntry<float>("Max Zoom Level", 0.5f);
        zoomStepAmount = mainCategory.CreateEntry<float>("+/- Amount Per Scroll", 0.1f);

        HarmonyPatches.scrollZoomInstance = this;

        MelonLogger.Msg($"Loaded!");
    }

    [HarmonyPatch]
    public class HarmonyPatches {
        public static ScrollZoom scrollZoomInstance;
        public static bool zoomToggleState;
        public static float currentZoomLevel;
        public static bool debounceInProgress;
        public static float targetZoomLevel;
        public static float userDefinedFov;
        public static float currentFov;

        [HarmonyPrefix]
        [HarmonyPatch(typeof(CameraFovClone), nameof(CameraFovClone.Update))]
        public static bool before_CameraFovClone_Update()
        {
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(CVR_DesktopCameraController), nameof(CVR_DesktopCameraController.Update))]
        public static bool before_CVR_DesktopCameraController_Update(CVR_DesktopCameraController __instance)
        {
            float scrollWheelValue = Input.GetAxis("Mouse ScrollWheel");

            if (!CVR_DesktopCameraController.enableZoom)
            {
                return false;
            }

            //Toggles Zoom when button is pressed, only allows new toggle after button is let go of.
            if (CVRInputManager.Instance.zoom && !debounceInProgress && !scrollZoomInstance.holdToZoom.Value)
            {
                zoomToggleState = !zoomToggleState;
                MelonLogger.Msg($"Zoom Toggle State: {zoomToggleState}");
                debounceInProgress = true;
            }
            else if (!CVRInputManager.Instance.zoom)
            {
                debounceInProgress = false;
            }

            if (scrollWheelValue > 0f && CVRInputManager.Instance.zoom && scrollZoomInstance.holdToZoom.Value || scrollWheelValue > 0f && !scrollZoomInstance.holdToZoom.Value && zoomToggleState)
            {
                targetZoomLevel += scrollZoomInstance.zoomStepAmount.Value; //Increment the target zoom level when the scroll wheel moves up
                targetZoomLevel = Mathf.Clamp(targetZoomLevel, 0f, scrollZoomInstance.maxZoomLevel.Value);
                MelonLogger.Msg($"Target Zoom Level: {targetZoomLevel}");
            }
            else if (scrollWheelValue < 0f && CVRInputManager.Instance.zoom && scrollZoomInstance.holdToZoom.Value || scrollWheelValue < 0f && !scrollZoomInstance.holdToZoom.Value && zoomToggleState)
            {
                targetZoomLevel -= scrollZoomInstance.zoomStepAmount.Value; //Decrement the target zoom level when the scroll wheel moves down
                targetZoomLevel = Mathf.Clamp(targetZoomLevel, 0f, scrollZoomInstance.maxZoomLevel.Value);
                MelonLogger.Msg($"Target Zoom Level: {targetZoomLevel}");
            }

            if (scrollZoomInstance.holdToZoom.Value && CVRInputManager.Instance.zoom || !scrollZoomInstance.holdToZoom.Value && zoomToggleState)
            {
                currentZoomLevel = Mathf.Lerp(currentZoomLevel, targetZoomLevel, Time.deltaTime * 10f); //Smoothly interpolate between the current zoom level and the target zoom level
            } else
            {
                currentZoomLevel = Mathf.Lerp(currentZoomLevel, 0f, Time.deltaTime * 10f);
            }

            currentFov = Mathf.Lerp(CVR_DesktopCameraController.defaultFov, 1f, currentZoomLevel);

            CVR_DesktopCameraController._cam.fieldOfView = currentFov;
            CVR_DesktopCameraController.currentZoomProgress = currentZoomLevel;
            CVR_DesktopCameraController.currentZoomProgressCurve = currentZoomLevel;

            if (!scrollZoomInstance.rememberLastZoomLevel.Value && !zoomToggleState)
            {
                currentZoomLevel = 0f;
            }

            return false;
        }
    }
}