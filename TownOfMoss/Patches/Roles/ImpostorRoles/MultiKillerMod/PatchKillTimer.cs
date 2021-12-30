using HarmonyLib;
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
}
