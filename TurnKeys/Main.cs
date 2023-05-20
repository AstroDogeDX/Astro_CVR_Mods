using MelonLoader;
using HarmonyLib;
using ABI_RC.Core.Savior;
using UnityEngine;

namespace Astro.TurnKeys
{
    public class TurnKeys : MelonMod
    {
        public override void OnInitializeMelon()
        {
            MelonLogger.Msg("Loaded!");
        }
    }

    [HarmonyPatch]
    public class HarmonyPatches
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(InputModuleMouseKeyboard), nameof(InputModuleMouseKeyboard.UpdateInput))]
        public static void after_InputModuleMouseKeyboard(InputModuleMouseKeyboard __instance)
        {
            if (Input.GetKey(KeyCode.LeftBracket))
            {
                MelonLogger.Msg("[ key pressed!");
                __instance._inputManager.lookVector -= new Vector2(1f, 0f);
            }
            if (Input.GetKey(KeyCode.RightBracket))
            {
                MelonLogger.Msg("] key pressed!");
                __instance._inputManager.lookVector += new Vector2(1f, 0f);
            }
        }
    }
}