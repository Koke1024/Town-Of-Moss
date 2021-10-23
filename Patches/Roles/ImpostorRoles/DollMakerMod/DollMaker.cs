using Il2CppSystem;
using Il2CppSystem.Collections.Generic;
using UnityEngine;

namespace TownOfUs.Roles
{
    public class DollMaker : Assassin
    {
        public KillButtonManager _waxButton;
        public static Dictionary<byte, float> DollList { get; set; }
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
            
            lastWaxed = DateTime.UtcNow;
        }

        public PlayerControl CurrentTarget { get; set; }

        public DateTime lastWaxed = new DateTime();

        public float CleanTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - lastWaxed;
            var num = PlayerControl.GameOptions.KillCooldown * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }
        
        public KillButtonManager WaxButton
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