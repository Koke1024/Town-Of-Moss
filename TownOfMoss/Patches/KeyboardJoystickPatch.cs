using HarmonyLib;
using Reactor;
using System;
using System.Collections.Generic;
using System.Text;

namespace TownOfUs
{
    [HarmonyPatch(typeof(KeyboardJoystick), nameof(KeyboardJoystick.HandleHud))]
    public class KeyboardJoystickPatch
    {
        [HarmonyPostfix]
        public static void Postfix()
        {
            if (DestroyableSingleton<HudManager>.Instance != null && DestroyableSingleton<HudManager>.Instance.ImpostorVentButton != null && DestroyableSingleton<HudManager>.Instance.ImpostorVentButton.isActiveAndEnabled && ConsoleJoystick.player.GetButtonDown(50))
                DestroyableSingleton<HudManager>.Instance.ImpostorVentButton.DoClick();
        }
    }
}
