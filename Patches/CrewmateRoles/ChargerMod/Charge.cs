using System;
using System.Collections.Generic;
using HarmonyLib;
using Reactor;
using TownOfUs.Roles;
using UnityEngine;

namespace TownOfUs.CrewmateRoles.ChargerMod
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.FixedUpdate))]
    public class Charge {
        public static void Postfix(PlayerControl __instance) {
            if (!__instance.AmOwner) return;
            if (__instance.Data.IsDead) return;
            if (!__instance.AmOwner) return;
            if (!__instance.Is(RoleEnum.Charger)) {
                return;
            }
            var charger = Role.GetRole<Charger>(PlayerControl.LocalPlayer);
            if (__instance.inVent) {
                if (charger.Charge > 1.0f) {
                    if (!charger.flashed) {
                        Coroutines.Start(Utils.FlashCoroutine(charger.Color));
                        charger.flashed = true;
                    }
                    return;
                }
                charger.Charge += 1 / (60.0f * CustomGameOptions.MaxChargeTime);
            }
            else {
                charger.flashed = false;
                if (charger.Charge < 0) {
                    charger.Charge = 0;
                    return;
                }
                charger.Charge -= 1 / (60.0f * CustomGameOptions.ConsumeChargeTime);
            }
        }
    }

    [HarmonyPatch(typeof(Vent), nameof(Vent.SetButtons))]
    public class NoButton {
        public static bool Prefix(PlayerControl __instance) {
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Charger)) {
                return false;                
            }
            return true;
        }
    }
}