using HarmonyLib;
using TownOfUs.Roles;

namespace TownOfUs.ImpostorRoles.AssassinMod
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.FixedUpdate))]
    public class MadMateCantKill
    {
        public static bool Prefix(PlayerControl __instance) {
            if (!HudManager.Instance) {
                return true;
            }
            if (CustomGameOptions.MadMateOn) {
                foreach(var role in Role.GetRoles(RoleEnum.Assassin))
                {
                    role.Player.Data.Role.TeamType = RoleTeamTypes.Impostor;
                }
            }
            return true;
        }
        public static void Postfix(PlayerControl __instance) {
            if (CustomGameOptions.MadMateOn) {
                foreach(var role in Role.GetRoles(RoleEnum.Assassin))
                {
                    role.Player.Data.Role.TeamType = RoleTeamTypes.Crewmate;
                }
            }
        }
    }

    [HarmonyPatch(typeof(MapBehaviour), nameof(MapBehaviour.ShowNormalMap))]
    public static class ShowSabotageMapPatch {
        public static void Prefix(MapBehaviour __instance) {
            if (CustomGameOptions.MadMateOn && PlayerControl.LocalPlayer.Is(RoleEnum.Assassin)) {
                PlayerControl.LocalPlayer.Data.Role.TeamType = RoleTeamTypes.Impostor;                
            }
        }

        public static void Postfix(MapBehaviour __instance) {
            if (CustomGameOptions.MadMateOn && PlayerControl.LocalPlayer.Is(RoleEnum.Assassin)) {
                PlayerControl.LocalPlayer.Data.Role.TeamType = RoleTeamTypes.Crewmate;
            }
        }
    }
}