using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TownOfUs.Roles
{
    public class Camouflager : Assassin

    {
        public KillButton _camouflageButton;
        public bool Enabled;
        public DateTime LastCamouflaged;
        public float TimeRemaining;

        public Camouflager(PlayerControl player) : base(player)
        {
            Name = "Camouflager";
            ImpostorText = () => "Camouflage and turn everyone grey";
            TaskText = () => "Camouflage and get secret kills";
            Color = Palette.ImpostorRed;
            RoleType = RoleEnum.Camouflager;
            Faction = Faction.Impostors;
            
            LastCamouflaged = DateTime.UtcNow;
        }

        public bool Camouflaged => TimeRemaining > 0f;

        public KillButton CamouflageButton
        {
            get => _camouflageButton;
            set
            {
                _camouflageButton = value;
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
            }
        }

        public void Camouflage()
        {
            Enabled = true;
            TimeRemaining -= Time.deltaTime;
            Utils.Camouflage();
        }

        public void UnCamouflage()
        {
            Enabled = false;
            LastCamouflaged = DateTime.UtcNow;
            Utils.UnCamouflage();
        }

        public float CamouflageTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastCamouflaged;
            var num = CustomGameOptions.CamouflagerCd * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }

        public override void OnEndMeeting() {
            base.OnEndMeeting();
            LastCamouflaged = DateTime.UtcNow;
            LastCamouflaged = LastCamouflaged.AddSeconds(-10f);
        }

        public static Sprite CamouflageSprite => TownOfUs.Camouflage;
        
        public override void PostHudUpdate(HudManager __instance) {
            base.PostHudUpdate(__instance);
            
            if (CamouflageButton == null)
            {
                CamouflageButton = Object.Instantiate(__instance.KillButton, __instance.UseButton.transform.parent);
                CamouflageButton.name = "CamouflageButton";
                CamouflageButton.graphic.enabled = true;
                CamouflageButton.graphic.sprite = CamouflageSprite;
                CamouflageButton.GetComponent<AspectPosition>().DistanceFromEdge = TownOfUs.ButtonPosition;
                CamouflageButton.gameObject.SetActive(false);
            }
            CamouflageButton.GetComponent<AspectPosition>().Update();
            CamouflageButton.gameObject.SetActive(!PlayerControl.LocalPlayer.Data.IsDead && !MeetingHud.Instance);


            if (Enabled)
            {
                CamouflageButton.SetCoolDown(TimeRemaining, CustomGameOptions.CamouflagerDuration);
                return;
            }

            CamouflageButton.SetCoolDown(CamouflageTimer(), CustomGameOptions.CamouflagerCd);
            CamouflageButton.graphic.color = Palette.EnabledColor;
            CamouflageButton.graphic.material.SetFloat("_Desat", 0f);
        }
    }
}