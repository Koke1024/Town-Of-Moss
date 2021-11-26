using System;
using UnityEngine;

namespace TownOfUs.Roles
{
    public class Shifter : Role
    {
        public Shifter(PlayerControl player) : base(player)
        {
            Name = "Shifter";
            ImpostorText = () => "Shift around different roles";
            TaskText = () => "Steal other people's roles.\nFake Tasks:";
            Color = new Color(0.6f, 0.6f, 0.6f, 1f);
            RoleType = RoleEnum.Shifter;
            Faction = Faction.Neutral;
            
            foreach (var role in Role.GetRoles(RoleEnum.Shifter))
            {
                var shifter = (Shifter) role;
                shifter.LastShifted = DateTime.UtcNow;
            }
        }

        public PlayerControl ClosestPlayer;
        public DateTime LastShifted { get; set; }

        public void Loses()
        {
            Player.Data.Role.TeamType = RoleTeamTypes.Impostor;
        }

        public float ShifterShiftTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastShifted;
            var num = CustomGameOptions.ShifterCd * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }
    }
}