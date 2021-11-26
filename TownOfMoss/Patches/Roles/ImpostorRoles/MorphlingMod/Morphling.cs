using System;
using TownOfUs.Extensions;
using TownOfUs.Roles.Modifiers;
using UnityEngine;

namespace TownOfUs.Roles
{
    public class Morphling : Undertaker, IVisualAlteration

    {
        public ActionButton _morphButton;
        public PlayerControl ClosestPlayer;
        public DateTime LastMorphed;
        public PlayerControl MorphedPlayer;

        public PlayerControl SampledPlayer;

        public Color sampledColor;
        public float TimeRemaining;
        
        public static Sprite SampleSprite => TownOfUs.SampleSprite;
        public static Sprite MorphSprite => TownOfUs.MorphSprite;

        public Morphling(PlayerControl player) : base(player)
        {
            Name = "Morphling";
            ImpostorText = () => "Transform into crewmates";
            TaskText = () => "Morph into crewmates to be disguised";
            Color = Palette.ImpostorRed;
            RoleType = RoleEnum.Morphling;
            Faction = Faction.Impostors;
            
            LastMorphed = DateTime.UtcNow;
        }

        public ActionButton MorphButton
        {
            get => _morphButton;
            set
            {
                _morphButton = value;
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
            }
        }

        public bool Morphed => TimeRemaining > 0f;

        public void Morph()
        {
            TimeRemaining -= Time.deltaTime;
            Utils.Morph(Player, MorphedPlayer);
        }

        public void Unmorph()
        {
            MorphedPlayer = null;
            Utils.Unmorph(Player);
            LastMorphed = DateTime.UtcNow;
            TimeRemaining = 0;
        }

        public float MorphTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastMorphed;
            var num = CustomGameOptions.MorphlingCd * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }

        public bool TryGetModifiedAppearance(out VisualAppearance appearance)
        {
            if (Morphed)
            {
                appearance = MorphedPlayer.GetDefaultAppearance();
                var modifier = Modifier.GetModifier(MorphedPlayer);
                if (modifier is IVisualAlteration alteration)
                    alteration.TryGetModifiedAppearance(out appearance);
                return true;
            }

            appearance = Player.GetDefaultAppearance();
            return false;
        }
    }
}