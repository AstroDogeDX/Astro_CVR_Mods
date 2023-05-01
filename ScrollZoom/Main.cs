using ABI_RC.Core.Player;
using ABI_RC.Core.Savior;
using HarmonyLib;
using MelonLoader;
using UnityEngine;
using System.Collections;

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
        public static bool debounceInProgress = false;

        public static IEnumerator DebounceCoroutine(float debounceTime)
        {
            debounceInProgress = true;
            yield return new WaitForSeconds(debounceTime);
            debounceInProgress = false;
        }


        [HarmonyPrefix]
        [HarmonyPatch(typeof(CVR_DesktopCameraController), nameof(CVR_DesktopCameraController.Start))]
        public static void before_CVR_DesktopCameraController_Start()
        {
            zoomToggleState = false;
            currentZoomLevel = 0f;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(CVR_DesktopCameraController), nameof(CVR_DesktopCameraController.Update))]
        public static bool before_CVR_DesktopCameraController_Update(CVR_DesktopCameraController __instance) { 
            float scrollWheelValue = Input.GetAxis("Mouse ScrollWheel");

            if (!CVR_DesktopCameraController.enableZoom)
            {
                return false;
            }

            if (zoomToggleState && scrollWheelValue > 0f || CVRInputManager.Instance.zoom && scrollWheelValue > 0f) {
                currentZoomLevel = currentZoomLevel >= 1f ? 1f : currentZoomLevel += scrollZoomInstance.zoomStepAmount.Value;
            }
            else if (zoomToggleState && scrollWheelValue < 0f || CVRInputManager.Instance.zoom && scrollWheelValue < 0f) {
                currentZoomLevel = currentZoomLevel <= 0f ? 0f : currentZoomLevel -= scrollZoomInstance.zoomStepAmount.Value;
            }

            if (!scrollZoomInstance.holdToZoom.Value && !CVRInputManager.Instance.zoom)
            {
                if(zoomToggleState)
                {
                    CVR_DesktopCameraController._cam.fieldOfView = Mathf.Lerp(CVR_DesktopCameraController.defaultFov, Mathf.Lerp(60f, 1f, scrollZoomInstance.maxZoomLevel.Value), currentZoomLevel);
                    CVR_DesktopCameraController.currentZoomProgress = currentZoomLevel;
                    CVR_DesktopCameraController.currentZoomProgressCurve = currentZoomLevel;
                    return false;
                } else
                {
                    CVR_DesktopCameraController._cam.fieldOfView = CVR_DesktopCameraController.defaultFov;
                    CVR_DesktopCameraController.currentZoomProgress = 0f;
                    CVR_DesktopCameraController.currentZoomProgressCurve = 0f;
                    return false;
                }
            }

            if (CVRInputManager.Instance.zoom && !debounceInProgress)
            {
                if (!scrollZoomInstance.holdToZoom.Value)
                {
                    zoomToggleState = !zoomToggleState;
                    MelonLogger.Msg($"Zoom Toggle State: {zoomToggleState}");
                    __instance.StartCoroutine(DebounceCoroutine(0.3f)); // Adjust the debounce time (in seconds) as needed
                }
                else
                {
                    CVR_DesktopCameraController._cam.fieldOfView = Mathf.Lerp(CVR_DesktopCameraController.defaultFov, Mathf.Lerp(60f, 1f, scrollZoomInstance.maxZoomLevel.Value), currentZoomLevel);
                    CVR_DesktopCameraController.currentZoomProgress = currentZoomLevel;
                    CVR_DesktopCameraController.currentZoomProgressCurve = currentZoomLevel;
                }
                return false;
            }

            CVR_DesktopCameraController._cam.fieldOfView = CVR_DesktopCameraController.defaultFov;
            CVR_DesktopCameraController.currentZoomProgress = 0f;
            CVR_DesktopCameraController.currentZoomProgressCurve = 0f;

            if (scrollZoomInstance.rememberLastZoomLevel.Value)
            {
                return false;
            } else
            {
                currentZoomLevel = 0f;
                return false;
            }
        }
    }
}