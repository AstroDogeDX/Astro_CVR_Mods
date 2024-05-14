using ABI_RC.Core.Player;
using ABI_RC.Core.Util.Object_Behaviour;
using ABI_RC.Core.InteractionSystem;
using ABI_RC.Systems.InputManagement;
using HarmonyLib;
using MelonLoader;
using UnityEngine;
using MelonLoader.Preferences;
using Astro.ScrollZoom.Properties;

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
        holdToZoom = mainCategory.CreateEntry<bool>("Hold to Zoom", true,
            description: "If true, zoom works like default CVR. If false, zoom is a toggle");
        targetZoomLevel = mainCategory.CreateEntry<float>("Target Zoom Level", 0.75f,
            description: "Your 'default' zoom level between no zoom and your max zoom",
            validator: new ValueRange<float>(0f, 1f));
        rememberLastZoomLevel = mainCategory.CreateEntry<bool>("Remember Zoom Level", true,
            description: "If true, your zoom will snap to the last zoom level you were at. If false, snap to your target zoom level");
        maxZoomLevel = mainCategory.CreateEntry<float>("Max Zoom Level", 0.5f, 
            description: "Maximum zoom between your defaultFov and 1deg FOV", 
            validator: new ValueRange<float>(0.001f, 1f));
        zoomStepAmount = mainCategory.CreateEntry<float>("+/- Amount Per Scroll", 0.1f, 
            description: "Amount per scroll wheel turn to zoom in/out",
            validator: new ValueRange<float>(0.001f, 1f));
        savedZoomLevel = mainCategory.CreateEntry<float>("Saved Zoom Level", 1f, 
            description: "[Hidden] Your last zoom level - used by 'remember zoom level'", 
            is_hidden: true);

        HarmonyPatches.scrollZoomInstance = this;

        MelonLogger.Msg($"Loaded! {AssemblyInfoParams.Version}");
    }

    [HarmonyPatch]
    public class HarmonyPatches {
        public static ScrollZoom scrollZoomInstance;
        public static bool zoomToggleState;
        public static float currentZoomLevel;
        public static bool debounceInProgress;
        public static float currentFov;
        public static float cappedZoom;

        [HarmonyPrefix]
        [HarmonyPatch(typeof(CameraFovClone), nameof(CameraFovClone.Update))]
        public static bool before_CameraFovClone_Update(CameraFovClone __instance)
        {
            __instance._camera.fieldOfView = CVR_DesktopCameraController.defaultFov;
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(CVR_DesktopCameraController), nameof(CVR_DesktopCameraController.Update))]
        public static bool before_CVR_DesktopCameraController_Update()
        {
            float scrollWheelValue = Input.GetAxis("Mouse ScrollWheel");

            cappedZoom = Mathf.Lerp(CVR_DesktopCameraController.defaultFov, 1f, scrollZoomInstance.maxZoomLevel.Value);

            if (!CVR_DesktopCameraController.enableZoom)
            {
                return false;
            }

            if (ViewManager.Instance.IsAnyMenuOpen)
            {
                zoomToggleState = false;
            }

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

            bool scrollWheelPositive = scrollWheelValue > 0f;
            bool scrollWheelNegative = scrollWheelValue < 0f;
            bool zoomingState = CVRInputManager.Instance.zoom && scrollZoomInstance.holdToZoom.Value || !scrollZoomInstance.holdToZoom.Value && zoomToggleState;

            if (scrollWheelPositive && zoomingState || scrollWheelNegative && zoomingState)
            {
                float zoomChange = scrollZoomInstance.zoomStepAmount.Value * (1f - Mathf.Pow(scrollZoomInstance.savedZoomLevel.Value - 0f / (1f - 0f), 2f));
                scrollZoomInstance.savedZoomLevel.Value += scrollWheelPositive ? zoomChange : (scrollZoomInstance.savedZoomLevel.Value >= 0.99f ? -0.01f : -zoomChange);
                scrollZoomInstance.savedZoomLevel.Value = Mathf.Clamp01(scrollZoomInstance.savedZoomLevel.Value);
#if DEBUG
                MelonLogger.Msg($"Target Zoom Level: {scrollZoomInstance.savedZoomLevel.Value}");
#endif
            }

            if (zoomingState)
            {
                currentZoomLevel = Mathf.Lerp(currentZoomLevel, scrollZoomInstance.savedZoomLevel.Value, Time.deltaTime * 10f);
            }
            else
            {
                currentZoomLevel = Mathf.Lerp(currentZoomLevel, 0f, Time.deltaTime * 10f);
                if (!scrollZoomInstance.rememberLastZoomLevel.Value && !zoomToggleState)
                {
                    scrollZoomInstance.savedZoomLevel.Value = scrollZoomInstance.targetZoomLevel.Value;
                }
            }

            currentFov = Mathf.Lerp(CVR_DesktopCameraController.defaultFov, cappedZoom, currentZoomLevel);

            CVR_DesktopCameraController._cam.fieldOfView = currentFov;
            CVR_DesktopCameraController.currentZoomProgress = CVR_DesktopCameraController.currentZoomProgressCurve = currentZoomLevel;

            return false;
        }
    }
}