using System.Linq;
using HarmonyLib;
using Hazel;
using Il2CppSystem;
using TheOtherRoles.Objects;
using TownOfUs.Extensions;
using TownOfUs.Patches.CrewmateRoles.SecurityGuardMod;
using UnityEngine;
using static Il2CppSystem.BitConverter;
using Buffer = System.Buffer;

namespace TownOfUs.Roles
{
    public class Painter : Role {
        private KillButtonManager[] _paintButtons;
        
        public Painter(PlayerControl player) : base(player) {
            Name = "Painter";
            ImpostorText = () => "Paint Players";
            TaskText = () => "Paint Players";
            Color = new Color(0.81f, 0.81f, 0.81f, 0.23f);
            RoleType = RoleEnum.Painter;
            
            // _paintButtons = new []{new KillButtonManager()}
        }
        
        
    }
}