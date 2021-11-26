using System.Collections.Generic;
using TownOfUs.CustomHats;
using TownOfUs.ImpostorRoles.CamouflageMod;
using UnityEngine;

namespace TownOfUs.Roles
{
    public class Sniffer : Role
    {
        public float sniffInterval = 5f;

        public Sniffer(PlayerControl player) : base(player)
        {
            Name = "Sniffer";
            ImpostorText = () => "Finding a dead body with your sense of smell";
            TaskText = () =>  "Finding a dead body with your sense of smell";
            Color = new Color(0.65f, 0f, 0.83f);
            RoleType = RoleEnum.Sniffer;

            sniffInterval = 5.0f;
        }
    }
}