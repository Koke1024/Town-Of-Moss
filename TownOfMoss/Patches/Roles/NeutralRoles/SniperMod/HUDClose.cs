using HarmonyLib;
using TownOfUs.CrewmateRoles.EngineerMod;
using TownOfUs.Roles;

namespace TownOfUs.NeutralRoles.SniperMod
{
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
    public static class SniperCountReset
    {
        public static void Postfix(MeetingHud __instance)
        {
            if (CustomGameOptions.SniperPerGame == EngineerFixPer.Game) {
                return;
            }
            foreach (var role in Role.GetRoles(RoleEnum.Sniper))
            {
                var sniper = (Sniper) role;
                sniper.KilledCount = 0;
            }
        }
    }
}