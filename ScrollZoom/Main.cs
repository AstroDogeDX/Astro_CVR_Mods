using ABI_RC.Core.Player;
using ABI_RC.Core.Savior;
using HarmonyLib;
using MelonLoader;
using UnityEngine;
using System.Collections;
using System;

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
        private static bool toggleSet = false;

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

            if (!CVR_DesktopCameraController.enableZoom)
            {
                return false;
            }

            if (zoomToggleState || CVRInputManager.Instance.zoom)
            {
                currentZoomLevel += scrollZoomInstance.zoomStepAmount.Value * Math.Sign(scrollWheelValue);
                currentZoomLevel = Math.Max(Math.Min(currentZoomLevel,1),0); //clamp currentZoomLevel 0> <1
            }


            if (!scrollZoomInstance.holdToZoom.Value && !CVRInputManager.Instance.zoom)
            {
                if (zoomToggleState)
                {
                    CVR_DesktopCameraController._cam.fieldOfView = Mathf.Lerp(CVR_DesktopCameraController.defaultFov, Mathf.Lerp(60f, 1f, scrollZoomInstance.maxZoomLevel.Value), currentZoomLevel);
                    CVR_DesktopCameraController.currentZoomProgress = currentZoomLevel;
                    CVR_DesktopCameraController.currentZoomProgressCurve = currentZoomLevel;
                    return false;
                }
                else
                {
                    CVR_DesktopCameraController._cam.fieldOfView = CVR_DesktopCameraController.defaultFov;
                    CVR_DesktopCameraController.currentZoomProgress = 0f;
                    CVR_DesktopCameraController.currentZoomProgressCurve = 0f;
                    return false;
                }
            }




            if (CVRInputManager.Instance.zoom )
            {
       
                    CVR_DesktopCameraController._cam.fieldOfView = Mathf.Lerp(CVR_DesktopCameraController.defaultFov, Mathf.Lerp(60f, 1f, scrollZoomInstance.maxZoomLevel.Value), currentZoomLevel);
                    CVR_DesktopCameraController.currentZoomProgress = currentZoomLevel;
                    CVR_DesktopCameraController.currentZoomProgressCurve = currentZoomLevel;

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