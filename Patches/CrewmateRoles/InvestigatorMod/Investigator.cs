using System.Collections.Generic;
using Il2CppSystem;
using TownOfUs.CrewmateRoles.InvestigatorMod;
using UnityEngine;

namespace TownOfUs.Roles
{
    public class Investigator : Role
    {
        // public readonly List<Footprint> AllPrints = new List<Footprint>();
        // public List<GameObject> crewIcons = new List<GameObject>();
        public Dictionary<byte, SpriteRenderer> crewPoints = new Dictionary<byte, SpriteRenderer>();
        public DateTime lastUpdated;

        public Investigator(PlayerControl player) : base(player)
        {
            Name = "Investigator";
            ImpostorText = () => "Find all imposters by examining footprints";
            TaskText = () => "You can see everyone's footprints.";
            Color = new Color(0f, 0.7f, 0.7f, 1f);
            RoleType = RoleEnum.Investigator;
            lastUpdated = DateTime.UtcNow;
            Scale = 1.4f;
        }
    }
}