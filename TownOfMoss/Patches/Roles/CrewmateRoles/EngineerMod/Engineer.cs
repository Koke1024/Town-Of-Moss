using TownOfUs.CrewmateRoles.EngineerMod;
using UnityEngine;

namespace TownOfUs.Roles
{
    public enum EngineerFixPer
    {
        Round,
        Game
    }
    public class Engineer : Role
    {
        public Engineer(PlayerControl player) : base(player)
        {
            Name = "Engineer";
            ImpostorText = () => "Maintain important systems on the ship";
            TaskText = () => "Vent and fix a sabotage from anywhere!";
            Color = new Color(1f, 0.65f, 0.04f, 1f);
            RoleType = RoleEnum.Engineer;
        }

        public bool UsedThisRound { get; set; } = false;

        public override void OnEndMeeting() {
            base.OnEndMeeting();
            if (CustomGameOptions.EngineerFixPer == EngineerFixPer.Round) UsedThisRound = false;
        }
    }
}