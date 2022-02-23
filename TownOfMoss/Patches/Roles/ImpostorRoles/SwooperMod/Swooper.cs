using System;
using TownOfUs.Extensions;
using UnityEngine;
using Object = UnityEngine.Object;

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
        }

        public override void InitializeLocal() {
            base.InitializeLocal();
            
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

        public override void OnEndMeeting() {
            base.OnEndMeeting();
            LastSwooped = DateTime.UtcNow;
        }
        
        public static Sprite SwoopSprite => TownOfUs.SwoopSprite;

        public override void PostHudUpdate(HudManager __instance) {
            base.PostHudUpdate(__instance);

            if (SwoopButton == null)
            {
                SwoopButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                SwoopButton.graphic.enabled = true;
                SwoopButton.GetComponent<AspectPosition>().DistanceFromEdge = TownOfUs.ButtonPosition;
                SwoopButton.gameObject.SetActive(false);
            }
            SwoopButton.GetComponent<AspectPosition>().Update();
            SwoopButton.graphic.sprite = SwoopSprite;
            SwoopButton.gameObject.SetActive(!PlayerControl.LocalPlayer.Data.IsDead && !MeetingHud.Instance);

            if (IsSwooped)
            {
                SwoopButton.SetCoolDown(TimeRemaining, CustomGameOptions.SwoopDuration);
                SwoopButton.graphic.material.SetFloat("_Percent", 0);
                SwoopButton.cooldownTimerText.color = Palette.AcceptedGreen;
                return;
            }
            SwoopButton.cooldownTimerText.color = Palette.EnabledColor;

            SwoopButton.SetCoolDown(SwoopTimer(), CustomGameOptions.SwoopCd);


            SwoopButton.graphic.color = Palette.EnabledColor;
            SwoopButton.graphic.material.SetFloat("_Desat", 0f);
        }
    }
}