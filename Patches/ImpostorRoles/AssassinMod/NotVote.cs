using HarmonyLib;
using TownOfUs.Roles;

namespace TownOfUs.ImpostorRoles.AssassinMod
{
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.VotingComplete))] // BBFDNCCEJHI
    public static class VotingComplete
    {
        public static void Postfix(MeetingHud __instance)
        {
            if (PlayerControl.LocalPlayer.CanSnipe())
            {
                if (CustomGameOptions.AssassinCanKillAfterVote) {
                    return;
                }
                var assassin = Role.GetRole<Assassin>(PlayerControl.LocalPlayer);
                ShowHideButtons.HideButtons(assassin);
            }
        }
    }
}