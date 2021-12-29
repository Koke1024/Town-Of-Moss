using HarmonyLib;
using Il2CppSystem;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace TownOfUs.Roles
{
    public class MultiKiller : Assassin {
        public bool KilledOnce = false;
        public DateTime? FirstKillTime = null;
        public MultiKiller(PlayerControl player) : base(player)
        {
            Name = "MultiKiller";
            ImpostorText = () => "kill crews at once";
            TaskText = () => "you can kill continuous";
            Color = Palette.ImpostorRed;
            RoleType = RoleEnum.MultiKiller;
            Faction = Faction.Impostors;
        }

        public float MaxTimer => PlayerControl.GameOptions.KillCooldown * CustomGameOptions.MultiKillerCdRate / 100.0f;

        public override void InitializeLocal() {
            base.InitializeLocal();
            Player.SetKillTimer(Mathf.Max(MaxTimer - 10.0f, 10.0f));
            KilledOnce = false;
            firstInitialize = false;
        }

        public override void PostKill(PlayerControl target) {
            if (!KilledOnce) {
                Player.SetKillTimer(0);
                FirstKillTime = DateTime.UtcNow;
            }
            else {
                Player.SetKillTimer(MaxTimer);
            }
            KilledOnce = !KilledOnce;
        }

        public override void OnEndMeeting() {
            base.OnEndMeeting();
            if (KilledOnce) {
                KilledOnce = false;
                var maxTimer = PlayerControl.GameOptions.KillCooldown * CustomGameOptions.MultiKillerCdRate / 100.0f;
                Player.SetKillTimer(maxTimer);
                HudManager.Instance.KillButton.SetCoolDown(maxTimer, maxTimer);
            }
        }

        public override void PostFixedUpdateLocal() {
            base.PostFixedUpdateLocal();
            
            if (FirstKillTime == null) {
                return;
            }

            if (FirstKillTime.Value.AddSeconds(CustomGameOptions.MultiKillEnableTime) < DateTime.UtcNow) {
                KilledOnce = false;
                FirstKillTime = null;
                Player.SetKillTimer(MaxTimer);
            }
        }
    }
}
