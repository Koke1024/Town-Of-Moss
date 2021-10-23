using Il2CppSystem;
using UnityEngine;

namespace TownOfUs.Roles
{
    public class Puppeteer : Assassin

    {
        public KillButtonManager _possessButton;
        public PlayerControl ClosestPlayer;
        public PlayerControl PossessPlayer;
        public DateTime PossStart;
        public float PossessTime;
        public float duration;
        public DateTime lastPossess;
        
        public static Sprite PossessSprite => TownOfUs.PossessSprite;
        public static Sprite UnPossessSprite => TownOfUs.ReleaseSprite;

        public Puppeteer(PlayerControl player) : base(player)
        {
            Name = "Puppeteer";
            ImpostorText = () => "Control Crew to Kill";
            TaskText = () => "Control Crew to Kill";
            Color = Palette.ImpostorRed;
            RoleType = RoleEnum.Puppeteer;
            Faction = Faction.Impostors;

            PossessPlayer = null;
            ClosestPlayer = null;
            _possessButton = null;
            
            lastPossess = DateTime.UtcNow.AddSeconds(-10.0f);
        }

        public KillButtonManager PossessButton
        {
            get => _possessButton;
            set
            {
                _possessButton = value;
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
            }
        }

        public void UnPossess() {
            PossessPlayer = null;
            Player.moveable = true;
            if (PlayerControl.LocalPlayer == Player) {
                PossessButton.renderer.sprite = Puppeteer.PossessSprite;
            }
            // duration = Mathf.Max(PossessTime, 3.0f);
            duration = CustomGameOptions.ReleaseWaitTime;
        }
        public void KillUnPossess() {
            if (PlayerControl.LocalPlayer == Player) {
                Player.SetKillTimer(PlayerControl.GameOptions.KillCooldown);                
            }
            UnPossess();
        }
    }
}