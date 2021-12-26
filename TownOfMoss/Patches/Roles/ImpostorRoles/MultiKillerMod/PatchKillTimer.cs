using HarmonyLib;
using TownOfUs.Extensions;
using TownOfUs.Roles;
using UnityEngine;

namespace TownOfUs.ImpostorRoles.MultiKillerMod
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.SetKillTimer))]
    public static class PatchKillTimer
    {
        public static bool Prefix(PlayerControl __instance, [HarmonyArgument(0)] float time)
        {
            if (__instance != PlayerControl.LocalPlayer) {
                return true;
            }

            if (!PlayerControl.LocalPlayer.Is(RoleEnum.MultiKiller)) return true;

            var maxTimer = PlayerControl.GameOptions.KillCooldown * CustomGameOptions.MultiKillerCdRate / 100.0f;
            __instance.killTimer = Mathf.Clamp(time, 0, maxTimer);
            HudManager.Instance.KillButton.SetCoolDown(__instance.killTimer, maxTimer);
            return false;
        }
    }

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.FixedUpdate))]
    public static class MultiKillerInit {
        public static bool Prefix(PlayerControl __instance) {
            if (__instance != PlayerControl.LocalPlayer) {
                return true;
            }
            if (!__instance.Is(RoleEnum.MultiKiller)) return true;
            var role = Role.GetRole<MultiKiller>(__instance);
            if (HudManager._instance.isIntroDisplayed) {
                role.firstInitialize = false;
            }
            if (!role.firstInitialize && !HudManager._instance.isIntroDisplayed) {
                var maxTimer = PlayerControl.GameOptions.KillCooldown * CustomGameOptions.MultiKillerCdRate / 100.0f;
                __instance.SetKillTimer(Mathf.Max(maxTimer - 10.0f, 10.0f));
                role.firstInitialize = true;
            }
            return true;
        }
    }
}
