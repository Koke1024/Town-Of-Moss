﻿using System.Linq;
using HarmonyLib;
using TownOfUs.Roles;

namespace TownOfUs.CrewmateRoles.PoliceMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HUDKill
    {
        private static ActionButton KillButton;

        public static void Postfix(HudManager __instance)
        {
            UpdateKillButton(__instance);
        }

        private static void UpdateKillButton(HudManager __instance)
        {
            KillButton = __instance.KillButton;
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            var flag7 = PlayerControl.AllPlayerControls.Count > 1;
            if (!flag7) return;
            var flag8 = PlayerControl.LocalPlayer.Is(RoleEnum.Police);
            if (flag8)
            {
                var role = Role.GetRole<Police>(PlayerControl.LocalPlayer);
                var isDead = PlayerControl.LocalPlayer.Data.IsDead;
                if (isDead)
                {
                    KillButton.gameObject.SetActive(false);
                    KillButton.isActive = false;
                }
                else
                {
                    KillButton.gameObject.SetActive(!MeetingHud.Instance);
                    KillButton.isActive = !MeetingHud.Instance;
                    KillButton.SetCoolDown(role.PoliceKillTimer(), PlayerControl.GameOptions.KillCooldown + 15f);

                    // if (role.bulletCount < role.Player.myTasks.ToArray().Count(x => x.IsComplete)) {
                        Utils.SetTarget(ref role.ClosestPlayer, KillButton);                        
                    // }
                }
            }
            else
            {
                var isImpostor = PlayerControl.LocalPlayer.Data.IsImpostor;
                if (!isImpostor) return;
                if (PlayerControl.LocalPlayer.Is(RoleEnum.Assassin) && CustomGameOptions.MadMateOn) {
                    return;
                }
                var isDead2 = PlayerControl.LocalPlayer.Data.IsDead;
                if (isDead2)
                {
                    KillButton.gameObject.SetActive(false);
                    KillButton.isActive = false;
                }
                else
                {
                    __instance.KillButton.gameObject.SetActive(!MeetingHud.Instance);
                    __instance.KillButton.isActive = !MeetingHud.Instance;
                }
            }
        }
    }
}
