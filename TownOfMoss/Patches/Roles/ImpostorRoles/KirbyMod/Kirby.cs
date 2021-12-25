using Il2CppSystem;
using Rewired;
using UnityEngine;

namespace TownOfUs.Roles
{
    public class Kirby : Morphling
    {
        public KillButton _inhaleButton;
        public DeadBody _aten;
        public new DeadBody CurrentTarget { get; set; }
        public DateTime eatTime = new DateTime();

        public Kirby(PlayerControl player) : base(player)
        {
            Name = "Popopo";
            ImpostorText = () => "Twinkle Hungry Ball";
            TaskText = () => "Twinkle Hungry Ball";
            Color = Palette.ImpostorRed;
            RoleType = RoleEnum.Kirby;
            Faction = Faction.Impostors;
            Color = new Color(0.66f, 0.42f, 0.64f);
            
            eatTime = DateTime.UtcNow;
            _aten = null;
        }
        
        public KillButton InhaleButton
        {
            get => _inhaleButton;
            set
            {
                _inhaleButton = value;
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
            }
        }

        public void CopyCrew(DeadBody body) {
            TimeRemaining = float.MaxValue;
            MorphedPlayer = SampledPlayer;
            Utils.Morph(Player, SampledPlayer, true);
            eatTime = DateTime.Now;
            _aten = body;
            body.Reported = true;
            body.bodyRenderer.enabled = false;
        }
        
        public void Spit() {
            if (_aten == null) {
                return;
            }
            _aten.bodyRenderer.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            _aten.bodyRenderer.enabled = true;
            _aten.Reported = false;
            _aten.transform.position = Player.transform.position;
            
            _aten = null;
                    
            TimeRemaining = 0;
            Unmorph();
        }
    }
}