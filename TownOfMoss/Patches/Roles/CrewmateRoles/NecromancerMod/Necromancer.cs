using UnityEngine;

namespace TownOfUs.Roles
{
    public class Necromancer : Role
    {
        public DeadBody CurrentTarget;
        public PlayerControl revivedPlayer;

        public Necromancer(PlayerControl player) : base(player)
        {
            Name = "Necromancer";
            ImpostorText = () => "Summon dead temporary";
            TaskText = () => "Revive a dead body until one meeting.";
            Color = new Color(0.4f, 0f, 0f, 1f);
            RoleType = RoleEnum.Necromancer;
        }
    }
}