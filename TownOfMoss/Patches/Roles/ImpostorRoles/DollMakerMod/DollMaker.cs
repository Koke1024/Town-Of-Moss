using Il2CppSystem;
using Il2CppSystem.Collections.Generic;
using UnityEngine;

namespace TownOfUs.Roles
{
    public class DollMaker : Assassin
    {
        public KillButton _waxButton;
        public System.Collections.Generic.Dictionary<byte, float> DollList = new System.Collections.Generic.Dictionary<byte, float>();
        public PlayerControl ClosestPlayer;
        public static Sprite _waxSprite => TownOfUs.WaxSprite;

        public DollMaker(PlayerControl player) : base(player)
        {
            Name = "DollMaker";
            ImpostorText = () => "Make crews art";
            TaskText = () => "Wax Crews to make them dolls.";
            Color = Palette.ImpostorRed;
            RoleType = RoleEnum.DollMaker;
            Faction = Faction.Impostors;
        }
        
        public KillButton WaxButton
        {
            get => _waxButton;
            set
            {
                _waxButton = value;
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
            }
        }
    }
}