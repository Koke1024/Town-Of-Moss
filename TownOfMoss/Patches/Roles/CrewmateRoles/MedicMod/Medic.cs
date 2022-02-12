using UnityEngine;
using System.Linq;

namespace TownOfUs.Roles
{
    public class Medic : Role
    {
        public Medic(PlayerControl player) : base(player)
        {
            Name = "Medic";
            ImpostorText = () => "Find victims to find killer";
            TaskText = () => "You will get more information when you report.";
            Color = new Color(0f, 0.4f, 0f, 1f);
            RoleType = RoleEnum.Medic;
        }

        public System.Collections.Generic.IEnumerable<PlayerControl> SusList;

        public override void OnEndMeeting() {
            base.OnEndMeeting();
            SusList = null;
        }

        public override void PostHudUpdate(HudManager __instance) {
            base.PostHudUpdate(__instance);
            
            if (MeetingHud.Instance == null || SusList == null) {
                return;
            }

            foreach (var player in MeetingHud.Instance.playerStates) {
                if (SusList.Any(x => x.PlayerId == player.TargetPlayerId)) {
                    player.NameText.color = Color.black;
                }
            }
        }
    }
}