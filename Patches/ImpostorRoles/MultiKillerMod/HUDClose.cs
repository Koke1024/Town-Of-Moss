using System;
using HarmonyLib;
using TownOfUs.Extensions;
using TownOfUs.Roles;
using Object = UnityEngine.Object;

namespace TownOfUs.ImpostorRoles.MultiKillerMod
{
    [HarmonyPatch(typeof(Object), nameof(Object.Destroy), typeof(Object))]
    public static class HUDClose
    {
        public static void Postfix(Object obj)
        {
            if (ExileController.Instance == null || obj != ExileController.Instance.gameObject) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.MultiKiller)) {
                return;
            }

            MultiKiller mk = Role.GetRole<MultiKiller>(PlayerControl.LocalPlayer);
            if (mk.killedOnce) {
                mk.killedOnce = false;
                var maxTimer = PlayerControl.GameOptions.KillCooldown * CustomGameOptions.MultiKillerCdRate / 100.0f;
                mk.Player.SetKillTimer(maxTimer);
                HudManager.Instance.KillButton.SetCoolDown(maxTimer, maxTimer);
            }
        }
    }
}