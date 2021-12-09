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
        BodyGuard = 0,
        Shielded = 1,
        Everyone = 2,
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

                var exPlayer = bodyGuard.exShielded;
                if (exPlayer != null)
                {
                    System.Console.WriteLine(exPlayer.name + " is ex-Shielded and unvisored");
                    exPlayer.myRend.material.SetColor("_VisorColor", Palette.VisorColor);
                    exPlayer.myRend.material.SetFloat("_Outline", 0f);
                    bodyGuard.exShielded = null;
                    continue;
                }

                var player = bodyGuard.ShieldedPlayer;
                if (player == null) continue;

                if (player.Data.IsDead || bodyGuard.Player.Data.IsDead)
                {
                    StopKill.BreakShield(bodyGuard.Player.PlayerId, player.PlayerId, true);
                    continue;
                }


                var showShielded = CustomGameOptions.ShowShielded;
                if (showShielded == ShieldOptions.Everyone)
                {
                    // player.myRend.material.SetColor("_VisorColor", ProtectedColor);
                    player.myRend.material.SetFloat("_Outline", 1f);
                    player.myRend.material.SetColor("_OutlineColor", ProtectedColor);
                }
                else if (PlayerControl.LocalPlayer.PlayerId == player.PlayerId && (showShielded == ShieldOptions.Self ||
                    showShielded == ShieldOptions.SelfAndBodyGuard))
                {
                    //System.Console.WriteLine("Setting " + PlayerControl.LocalPlayer.name + "'s shield");
                    // player.myRend.material.SetColor("_VisorColor", ProtectedColor);
                    player.myRend.material.SetFloat("_Outline", 1f);
                    player.myRend.material.SetColor("_OutlineColor", ProtectedColor);
                }
                else if (PlayerControl.LocalPlayer.Is(RoleEnum.BodyGuard) &&
                         (showShielded == ShieldOptions.BodyGuard || showShielded == ShieldOptions.SelfAndBodyGuard))
                {
                    // player.myRend.material.SetColor("_VisorColor", ProtectedColor);
                    player.myRend.material.SetFloat("_Outline", 1f);
                    player.myRend.material.SetColor("_OutlineColor", ProtectedColor);
                }
            }
        }
    }
}