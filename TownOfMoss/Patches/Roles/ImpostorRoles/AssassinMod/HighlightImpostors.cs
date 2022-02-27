using System.Linq;
using HarmonyLib;

namespace TownOfUs.ImpostorRoles.AssassinMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HighlightImpostors
    {
        private static void UpdateMeeting(MeetingHud __instance)
        {
            if (PlayerControl.LocalPlayer.Data.IsDead) {
                return;
            }
            foreach (var state in __instance.playerStates) {
                bool amMad = PlayerControl.LocalPlayer.Is(RoleEnum.Assassin);
                if (state.TargetPlayerId == PlayerControl.LocalPlayer.PlayerId) {
                    continue;
                }
                PlayerControl player = Utils.PlayerById(state.TargetPlayerId);
                if (player.Data.Disconnected) {
                    continue;
                }
                
                if (amMad || player.Is(RoleEnum.Assassin)) {
                    state.NameText.color = Palette.White;
                }
            }
        }

        public static void Postfix(HudManager __instance)
        {
            if (!CustomGameOptions.MadMateOn) return;
            if (MeetingHud.Instance) UpdateMeeting(MeetingHud.Instance);

            if (!PlayerControl.LocalPlayer) {
                return;
            }
            bool amMad = PlayerControl.LocalPlayer.Is(RoleEnum.Assassin);

            if (PlayerControl.AllPlayerControls == null) {
                return;
            }
            
            foreach (var player in PlayerControl.AllPlayerControls.ToArray().
                Where(x => !x.Data.Disconnected && !x.AmOwner))
            {
                if (amMad || player.Is(RoleEnum.Assassin)) {
                    player.nameText.color = Palette.White;                    
                }
            }
        }
    }
}