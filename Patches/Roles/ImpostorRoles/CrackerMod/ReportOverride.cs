using System;
using System.Linq;
using HarmonyLib;
using Hazel;
using TownOfUs.Roles;
using Il2CppSystem.Collections;
using Il2CppSystem.Collections.Generic;
using Reactor;
using TownOfUs.Extensions;
using UnityEngine;

namespace TownOfUs.ImpostorRoles.CrackerMod {
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.FixedUpdate))]
    public static class ReportOverride {
        public static void Postfix(PlayerControl __instance) {
            if (Cracker.InCrackedRoom) {
                DestroyableSingleton<HudManager>.Instance.ReportButton.SetActive(false);
            }
        }
    }

    [HarmonyPatch(typeof(DeadBody), nameof(DeadBody.OnClick))]
    public static class ReportBody {
        public static bool Prefix(DeadBody __instance) {
            return !Cracker.InCrackedRoom;
        }
    }
}