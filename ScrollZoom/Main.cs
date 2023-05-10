using ABI_RC.Core.Player;
using ABI_RC.Core.Savior;
using ABI_RC.Core.Util.Object_Behaviour;
using ABI_RC.Core.InteractionSystem;
using HarmonyLib;
using MelonLoader;
using UnityEngine;

namespace Astro.ScrollZoom;

public class ScrollZoom : MelonMod {
    private MelonPreferences_Category mainCategory;
    private MelonPreferences_Entry<bool> holdToZoom;
    private MelonPreferences_Entry<float> savedZoomLevel;
    private MelonPreferences_Entry<float> targetZoomLevel;
    private MelonPreferences_Entry<bool> rememberLastZoomLevel;
    private MelonPreferences_Entry<float> maxZoomLevel;
    private MelonPreferences_Entry<float> zoomStepAmount;

    public override void OnInitializeMelon() {
        mainCategory = MelonPreferences.CreateCategory("ScrollZoom");
        holdToZoom = mainCategory.CreateEntry<bool>("Hold to Zoom", true);
        targetZoomLevel = mainCategory.CreateEntry<float>("Target Zoom Level", 0.5f);
        rememberLastZoomLevel = mainCategory.CreateEntry<bool>("Remember Zoom Level", true);
        maxZoomLevel = mainCategory.CreateEntry<float>("Max Zoom Level", 0.5f);
        zoomStepAmount = mainCategory.CreateEntry<float>("+/- Amount Per Scroll", 0.1f);
        savedZoomLevel = (MelonPreferences_Entry<float>)mainCategory.CreateEntry<float>("Saved Zoom Level", 1f, "Saved Zoom Level", true);

        HarmonyPatches.scrollZoomInstance = this;

        MelonLogger.Msg($"Loaded!");
    }

    [HarmonyPatch]
    public class HarmonyPatches {
        public static ScrollZoom scrollZoomInstance;
        public static bool zoomToggleState;
        public static float currentZoomLevel;
        public static bool debounceInProgress;
        public static float userDefinedFov;
        public static float currentFov;

        [HarmonyPrefix]
        [HarmonyPatch(typeof(CameraFovClone), nameof(CameraFovClone.Update))]
        public static bool before_CameraFovClone_Update(CameraFovClone __instance)
        {
            __instance._camera.fieldOfView = CVR_DesktopCameraController.defaultFov;
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

            if (ViewManager.Instance._gameMenuOpen || CVR_MenuManager.Instance._quickMenuOpen)
            {
                zoomToggleState = false;
            }

            //Toggles Zoom when button is pressed, only allows new toggle after button is let go of.
            if (CVRInputManager.Instance.zoom && !debounceInProgress && !scrollZoomInstance.holdToZoom.Value)
            {
                zoomToggleState = !zoomToggleState;
#if DEBUG
                MelonLogger.Msg($"Zoom Toggle State: {zoomToggleState}");
#endif
                debounceInProgress = true;
            }
            else if (!CVRInputManager.Instance.zoom)
            {
                debounceInProgress = false;
            }

            if (scrollWheelValue > 0f && CVRInputManager.Instance.zoom && scrollZoomInstance.holdToZoom.Value || scrollWheelValue > 0f && !scrollZoomInstance.holdToZoom.Value && zoomToggleState)
            {
                scrollZoomInstance.savedZoomLevel.Value += scrollZoomInstance.zoomStepAmount.Value * (1f - Mathf.Pow(scrollZoomInstance.savedZoomLevel.Value - 0f / (scrollZoomInstance.maxZoomLevel.Value - 0f), 2f)); //Increment the target zoom level when the scroll wheel moves up
                scrollZoomInstance.savedZoomLevel.Value = Mathf.Clamp(scrollZoomInstance.savedZoomLevel.Value, 0f, scrollZoomInstance.maxZoomLevel.Value);
#if DEBUG
                MelonLogger.Msg($"Target Zoom Level: {scrollZoomInstance.savedZoomLevel.Value}");
#endif
            }
            else if (scrollWheelValue < 0f && CVRInputManager.Instance.zoom && scrollZoomInstance.holdToZoom.Value || scrollWheelValue < 0f && !scrollZoomInstance.holdToZoom.Value && zoomToggleState)
            {
                if (scrollZoomInstance.savedZoomLevel.Value >= 0.99f) // Stops the zoom level getting stuck at 0.99...f
                {
                    scrollZoomInstance.savedZoomLevel.Value = 0.98f;
                } else
                {
                    scrollZoomInstance.savedZoomLevel.Value -= scrollZoomInstance.zoomStepAmount.Value * (1f - Mathf.Pow(scrollZoomInstance.savedZoomLevel.Value - 0f / (scrollZoomInstance.maxZoomLevel.Value - 0f), 2f)); //Decrement the target zoom level when the scroll wheel moves down
                }
                scrollZoomInstance.savedZoomLevel.Value = Mathf.Clamp(scrollZoomInstance.savedZoomLevel.Value, 0f, scrollZoomInstance.maxZoomLevel.Value);
#if DEBUG
                MelonLogger.Msg($"Target Zoom Level: {scrollZoomInstance.savedZoomLevel.Value}");
#endif
            }

            if (scrollZoomInstance.holdToZoom.Value && CVRInputManager.Instance.zoom || !scrollZoomInstance.holdToZoom.Value && zoomToggleState)
            {
                currentZoomLevel = Mathf.Lerp(currentZoomLevel, scrollZoomInstance.savedZoomLevel.Value, Time.deltaTime * 10f); //Smoothly interpolate between the current zoom level and the target zoom level
            } else
            {
                currentZoomLevel = Mathf.Lerp(currentZoomLevel, 0f, Time.deltaTime * 10f);
                if (!scrollZoomInstance.rememberLastZoomLevel.Value && !zoomToggleState)
                {
                    scrollZoomInstance.savedZoomLevel.Value = scrollZoomInstance.targetZoomLevel.Value;
                }
            }

            currentFov = Mathf.Lerp(CVR_DesktopCameraController.defaultFov, 1f, currentZoomLevel);

            CVR_DesktopCameraController._cam.fieldOfView = currentFov;
            CVR_DesktopCameraController.currentZoomProgress = currentZoomLevel;
            CVR_DesktopCameraController.currentZoomProgressCurve = currentZoomLevel;

            if (scrollZoomInstance.maxZoomLevel.Value == 0f) // Prevents setting the max zoom level to 0 and breaking everything
            {
                scrollZoomInstance.maxZoomLevel.Value = 0.5f;
            }

            return false;
        }
    }
}