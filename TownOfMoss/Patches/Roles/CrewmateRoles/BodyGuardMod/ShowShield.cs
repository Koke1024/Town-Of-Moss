using HarmonyLib;
using TownOfUs.Roles;
using UnityEngine;

namespace TownOfUs.CrewmateRoles.BodyGuardMod
{
    public enum ShieldOptions
    {
        BodyGuard = 0,
        Self = 1,
        SelfAndBodyGuard = 2,
        Everyone = 3
    }

    public enum NotificationOptions
    {
        Everyone = 0,
        Shielded = 1,
        BodyGuard = 2,
        Nobody = 3
    }

    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class ShowShield
    {
        public static Color ProtectedColor = Color.cyan;

        public static void Postfix(HudManager __instance)
        {
            foreach (var role in Role.GetRoles(RoleEnum.BodyGuard))
            {
                var bodyGuard = (BodyGuard) role;

                var player = bodyGuard.ShieldedPlayer;
                if (player == null) continue;

                if (player.Data.IsDead || bodyGuard.Player.Data.IsDead)
                {
                    StopKill.BreakShield(bodyGuard.Player.PlayerId, player.PlayerId, true);
                    continue;
                }
            }
        }
    }
}