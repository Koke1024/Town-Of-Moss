using System;
using UnityEngine;

namespace TownOfUs.Roles
{
    public class Police : Role
    {
        public int bulletCount = 0;
        public Police(PlayerControl player) : base(player)
        {
            Name = "Sheriff";
            ImpostorText = () => "Shoot the <color=#FF0000FF>Impostor</color>";
            TaskText = () => "Kill off the impostor but don't kill crewmates.";
            Color = Color.yellow;
            RoleType = RoleEnum.Police;
            
            foreach (var role in Role.GetRoles(RoleEnum.Police))
            {
                var Police = (Police) role;
                Police.LastKilled = DateTime.UtcNow;
                Police.LastKilled = Police.LastKilled.AddSeconds(0.1);
            }
        }

        public PlayerControl ClosestPlayer;
        public DateTime LastKilled { get; set; }

        public float PoliceKillTimer()
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
    }
}