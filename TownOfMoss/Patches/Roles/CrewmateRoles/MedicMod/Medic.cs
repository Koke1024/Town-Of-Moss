using System.Collections.Generic;
using UnityEngine;

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
    }
}