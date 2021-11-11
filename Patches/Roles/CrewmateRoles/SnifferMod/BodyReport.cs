using System;
using System.Linq;
using HarmonyLib;

namespace TownOfUs.CrewmateRoles.SniffMod
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.CmdReportDeadBody))]
    internal class BodyReportPass
    {
        private static bool Prefix(PlayerControl __instance, [HarmonyArgument(0)] GameData.PlayerInfo info)
        {
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Sniffer) && !CustomGameOptions.SnifferCanReport) {
                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class Update {
        public static void Postfix(HudManager __instance) {
            if(PlayerControl.LocalPlayer.Is(RoleEnum.Sniffer) && !CustomGameOptions.SnifferCanReport) {
                __instance.ReportButton.enabled = false;
                __instance.ReportButton.SetActive(false);
            }
        }
    }
}