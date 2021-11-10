using HarmonyLib;
using Hazel;
using Il2CppSystem.Collections.Generic;
using Reactor.Extensions;
using TMPro;
using TownOfUs;
using UnityEngine;

namespace TownOfUs.CrewmateRoles.SniffMod {

    public class AdminLimit {
        [HarmonyPatch(typeof(MapCountOverlay), nameof(MapCountOverlay.Update))]
        public static class AdminTimeLimit {
            public static bool Prefix(MapCountOverlay __instance) {
                if (!PlayerControl.LocalPlayer.Is(RoleEnum.Sniffer)) {
                    return true;
                }
                __instance.BackgroundColor.SetColor(Palette.DisabledGrey);
                __instance.OnDisable();
                return false;
            }
        }
    }
}