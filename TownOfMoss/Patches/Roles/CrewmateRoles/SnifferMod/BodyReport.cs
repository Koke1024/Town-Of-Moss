using HarmonyLib;

namespace TownOfUs.CrewmateRoles.SniffMod
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.CmdReportDeadBody))]
    internal class BodyReportPass
    {
        private static bool Prefix(PlayerControl __instance, [HarmonyArgument(0)] GameData.PlayerInfo target)
        {
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Sniffer) && !CustomGameOptions.SnifferCanReport && target != null) {
                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class Update {
        public static void Postfix(HudManager __instance) {
        }
    }
}