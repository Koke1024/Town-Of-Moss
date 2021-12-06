using System;
using TownOfUs.Extensions;
using UnityEngine;

namespace TownOfUs.Roles
{
    public class Swooper : Assassin
    {
        public KillButton _swoopButton;
        public bool Enabled;
        public DateTime LastSwooped;
        public float TimeRemaining;

        public Swooper(PlayerControl player) : base(player)
        {
            Name = "Swooper";
            ImpostorText = () => "Turn invisible temporarily";
            TaskText = () => "Turn invisible and sneakily kill";
            Color = Palette.ImpostorRed;
            RoleType = RoleEnum.Swooper;
            Faction = Faction.Impostors;
            
            LastSwooped = DateTime.UtcNow;
            LastSwooped = LastSwooped.AddSeconds(-10f);
        }

        public bool IsSwooped => TimeRemaining > 0f;

        public KillButton SwoopButton
        {
            get => _swoopButton;
            set
            {
                _swoopButton = value;
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
            }
        }

        public float SwoopTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastSwooped;
            ;
            var num = CustomGameOptions.SwoopCd * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }

        public void Swoop()
        {
            Enabled = true;
            TimeRemaining -= Time.deltaTime;
            var color = Color.clear;
            if (PlayerControl.LocalPlayer.Data.IsImpostor() || PlayerControl.LocalPlayer.Data.IsDead) color.a = 0.1f;

            if (Player.GetCustomOutfitType() != CustomPlayerOutfitType.Swooper)
            {
                Player.SetOutfit(CustomPlayerOutfitType.Swooper, new GameData.PlayerOutfit()
                {
                    ColorId = Player.CurrentOutfit.ColorId,
                    HatId = "",
                    SkinId = "",
                    VisorId = "",
                    _playerName = " "
                });
            Player.MyRend.color = color;
            }

        }


        public void UnSwoop()
        {
            Enabled = false;
            LastSwooped = DateTime.UtcNow;
            Utils.Unmorph(Player);
            Player.MyRend.color = Color.white;
            PlayerControl.LocalPlayer.NetTransform.Halt();
            if (Player == PlayerControl.LocalPlayer) {
                PlayerControl.LocalPlayer.NetTransform.Halt();                
            }
        }
    }
}