using MelonLoader;
using HarmonyLib;
using ABI_RC.Core.Savior;
using UnityEngine;
using Astro.TurnKeys.Properties;

namespace Astro.TurnKeys;
public class TurnKeys : MelonMod
{
    private MelonPreferences_Category mainCategory;
    private MelonPreferences_Entry<float> turnSensitivity;
    public override void OnInitializeMelon()
    {
        mainCategory = MelonPreferences.CreateCategory("TurnKeys");
        turnSensitivity = mainCategory.CreateEntry<float>("Turn Sensitivity", 35f,
            description: "How much per frame you turn while holding the key down.");
        MelonLogger.Msg($"Loaded! {AssemblyInfoParams.Version}");

        HarmonyPatches.__TurnKeys = this;
    }

    [HarmonyPatch]
    public class HarmonyPatches
    {
        public static TurnKeys __TurnKeys;

        [HarmonyPostfix]
        [HarmonyPatch(typeof(InputModuleMouseKeyboard), nameof(InputModuleMouseKeyboard.UpdateInput))]
        public static void after_InputModuleMouseKeyboard(InputModuleMouseKeyboard __instance)
        {
            if (Input.GetKey(KeyCode.Q))
            {
                __instance._inputManager.lookVector -= new Vector2(__TurnKeys.turnSensitivity.Value, 0f) * Time.deltaTime;
            }
            if (Input.GetKey(KeyCode.E))
            {
                __instance._inputManager.lookVector += new Vector2(__TurnKeys.turnSensitivity.Value, 0f) * Time.deltaTime;
            }
        }
    }
}