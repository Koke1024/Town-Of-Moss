using System;
using TownOfUs.CrewmateRoles.TimeLordMod;
using TownOfUs.Extensions;
using UnityEngine;

namespace TownOfUs.Roles
{
    public class TimeLord : Role
    {
        public TimeLord(PlayerControl player) : base(player)
        {
            Name = "Time Lord";
            ImpostorText = () => "Rewind Time";
            TaskText = () => "Rewind Time!";
            Color = new Color(0f, 0f, 1f, 1f);
            RoleType = RoleEnum.TimeLord;
            Scale = 1.4f;
        }

        public override void InitializeLocal() {
            base.InitializeLocal();
            FinishRewind = DateTime.UtcNow.AddSeconds(-10);
            StartRewind = DateTime.UtcNow.AddSeconds(-10 - CustomGameOptions.RewindDuration);
        }

        public DateTime StartRewind { get; set; }
        public DateTime FinishRewind { get; set; }

        public float TimeLordRewindTimer()
        {
            var utcNow = DateTime.UtcNow;


            TimeSpan timespan;
            float num;

            if (RecordRewind.rewinding)
            {
                timespan = utcNow - StartRewind;
                num = CustomGameOptions.RewindDuration * 1000f / 3f;
            }
            else
            {
                timespan = utcNow - FinishRewind;
                num = CustomGameOptions.RewindCooldown * 1000f;
            }


            var flag2 = num - (float) timespan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float) timespan.TotalMilliseconds) / 1000f;
        }


        public float GetCooldown()
        {
            return RecordRewind.rewinding ? CustomGameOptions.RewindDuration : CustomGameOptions.RewindCooldown;
        }

        public override void OnEndMeeting() {
            base.OnEndMeeting();
            FinishRewind = DateTime.UtcNow;
            StartRewind = FinishRewind.AddSeconds(-CustomGameOptions.RewindDuration);
        }

        public override void PostFixedUpdateLocal() {
            base.PostFixedUpdateLocal();
            
            var isDead = Player.Data.IsDead;
            var rewindButton = DestroyableSingleton<HudManager>.Instance.KillButton;

            if (isDead)
            {
                rewindButton.gameObject.SetActive(false);
                //  rewindButton.isActive = false;
            }
            else
            {
                rewindButton.gameObject.SetActive(!MeetingHud.Instance && !LobbyBehaviour.Instance);
                //  rewindButton.isActive = !MeetingHud.Instance;
                rewindButton.SetCoolDown(TimeLordRewindTimer(), GetCooldown());
            }

            var renderer = rewindButton.graphic;
            if (!rewindButton.isCoolingDown & !RecordRewind.rewinding & rewindButton.enabled)
            {
                renderer.color = Palette.EnabledColor;
                renderer.material.SetFloat("_Desat", 0f);
                return;
            }

            renderer.color = Palette.DisabledClear;
            renderer.material.SetFloat("_Desat", 1f);
        }
    }
}