using System;
using UnityEngine;

namespace TownOfUs.Roles
{
    public class Sheriff : Role
    {
        public int bulletCount = 0;
        public Sheriff(PlayerControl player) : base(player)
        {
            Name = "Sheriff";
            ImpostorText = () => "Shoot the <color=#FF0000FF>Impostor</color>";
            TaskText = () => "Kill off the impostor but don't kill crewmates.";
            Color = Color.yellow;
            RoleType = RoleEnum.Sheriff;
            
            foreach (var role in Role.GetRoles(RoleEnum.Sheriff))
            {
                var sheriff = (Sheriff) role;
                sheriff.LastKilled = DateTime.UtcNow;
                sheriff.LastKilled = sheriff.LastKilled.AddSeconds(0.1);
            }
        }

        public PlayerControl ClosestPlayer;
        public DateTime LastKilled { get; set; }

        public float SheriffKillTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastKilled;
            var num = CustomGameOptions.SheriffKillCd * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }

        internal override bool Criteria()
        {
            return CustomGameOptions.ShowSheriff || base.Criteria();
        }

        public override void OnEndMeeting() {
            base.OnEndMeeting();
            LastKilled = DateTime.UtcNow;
        }
    }
}