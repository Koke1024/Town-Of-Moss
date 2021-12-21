using System.Linq;
using HarmonyLib;
using TownOfUs.Roles;
using UnityEngine;

namespace TownOfUs.CrewmateRoles.MedicMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class SuspectColor
    {
        private static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Medic)) return;
            var role = Role.GetRole<Medic>(PlayerControl.LocalPlayer);
            if (MeetingHud.Instance == null || role.SusList == null) {
                return;
            }

            foreach (var player in MeetingHud.Instance.playerStates) {
                if (role.SusList.Any(x => x.PlayerId == player.TargetPlayerId)) {
                    player.NameText.color = Color.gray;
                }
            }
        }
    }
}