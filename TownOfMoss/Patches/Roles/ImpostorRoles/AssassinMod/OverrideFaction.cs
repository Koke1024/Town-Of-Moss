using HarmonyLib;
using TownOfUs.Extensions;
using TownOfUs.Roles;
using UnityEngine;
using UnityEngine.SocialPlatforms;

namespace TownOfUs.ImpostorRoles.AssassinMod
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.FixedUpdate))]
    public class MadMateCantKill
    {
        public static bool Prefix(PlayerControl __instance) {
            if (!HudManager.Instance) {
                return true;
            }
            // if ((CustomGameOptions.MadMateOn && PlayerControl.LocalPlayer.Is(RoleEnum.Assassin))) return false;
            if (CustomGameOptions.MadMateOn) {
                foreach(var role in Role.GetRoles(RoleEnum.Assassin))
                {
                    role.Player.Data.Role.TeamType = RoleTeamTypes.Impostor;
                }
            }
            return true;
        }
        // public static void Postfix(PlayerControl __instance) {
        //     if (CustomGameOptions.MadMateOn) {
        //         foreach(var role in Role.GetRoles(RoleEnum.Assassin))
        //         {
        //             role.Player.Data.Role.TeamType = RoleTeamTypes.Impostor;
        //         }
        //     }
        // }
    }
}