using System;
using System.Numerics;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;

namespace TownOfUs.Roles
{
    public class Undertaker : Assassin
    {
        public KillButton _dragDropButton;

        public Undertaker(PlayerControl player) : base(player)
        {
            if (GetType() != typeof(Undertaker)) {
                return;
            }
            Name = "Undertaker";
            ImpostorText = () => "Drag bodies and hide them";
            TaskText = () => "Drag bodies around to hide them from being reported";
            Color = Palette.ImpostorRed;
            RoleType = RoleEnum.Undertaker;
            Faction = Faction.Impostors;
        }

        public DateTime LastDragged { get; set; }
        public DeadBody CurrentTarget { get; set; }
        public DeadBody CurrentlyDragging { get; set; }

        public KillButton DragDropButton
        {
            get => _dragDropButton;
            set
            {
                _dragDropButton = value;
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
            }
        }

        public float DragTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastDragged;
            var num = CustomGameOptions.DragCd * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }
    }
}