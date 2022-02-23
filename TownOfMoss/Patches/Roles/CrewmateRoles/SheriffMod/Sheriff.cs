using System;
using UnityEngine;

namespace TownOfUs.Roles
{
    public class Sheriff : Role
    {
        public int bulletCount = 0;
        public Sheriff(PlayerControl player) : base(player)
        {
            Name = "Sheriff";
            ImpostorText = () => "Shoot the <color=#FF0000FF>Impostor</color>";
            TaskText = () => "Kill off the impostor but don't kill crewmates.";
            Color = Color.yellow;
            RoleType = RoleEnum.Sheriff;
        }
        
        public override void InitializeLocal() {
            base.InitializeLocal();

            LastKilled = DateTime.UtcNow;
        }

        public PlayerControl ClosestPlayer;
        public DateTime LastKilled { get; set; }

        public float SheriffKillTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastKilled;
            var num = CustomGameOptions.SheriffKillCd * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }

        internal override bool Criteria()
        {
            return CustomGameOptions.ShowSheriff || base.Criteria();
        }

        public override void OnEndMeeting() {
            base.OnEndMeeting();
            LastKilled = DateTime.UtcNow;
        }

        private static KillButton killButton;
        
        public override void PostHudUpdate(HudManager __instance) {
            base.PostHudUpdate(__instance);
            
            killButton = __instance.KillButton;
            var isDead = PlayerControl.LocalPlayer.Data.IsDead;
            if (isDead)
            {
                killButton.gameObject.SetActive(false);
                // killButton.graphic.enabled = false;
            }
            else
            {
                killButton.gameObject.SetActive(!MeetingHud.Instance);
                // killButton.graphic.enabled = !MeetingHud.Instance;
                killButton.SetCoolDown(SheriffKillTimer(), PlayerControl.GameOptions.KillCooldown + 15f);

                // if (role.bulletCount < role.Player.myTasks.ToArray().Count(x => x.IsComplete)) {
                    Utils.SetTarget(ref ClosestPlayer, killButton);                        
                // }
            }
        }
    }
}