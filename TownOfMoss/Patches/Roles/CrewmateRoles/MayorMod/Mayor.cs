using System.Collections.Generic;
using TownOfUs.CrewmateRoles.MayorMod;
using UnityEngine;

namespace TownOfUs.Roles
{
    public class Mayor : Role
    {
        public List<byte> ExtraVotes = new List<byte>();
        public KillButton ButtonButton;
        public bool ButtonUsed;

        public float reportDelay;
        // public bool extended = false;

        public Mayor(PlayerControl player) : base(player)
        {
            Name = "Mayor";
            ImpostorText = () => "Tiebreaker and have extra emergency button";
            TaskText = () => "You are Tiebreaker and have extra emergency button";
            Color = new Color(0.44f, 0.31f, 0.66f, 1f);
            RoleType = RoleEnum.Mayor;
            // VoteBank = CustomGameOptions.MayorVoteBank;
            ExtraVotes = new List<byte>();
        }

        public int VoteBank { get; set; }
        public bool SelfVote { get; set; }

        public bool VotedOnce { get; set; }

        public PlayerVoteArea Abstain { get; set; }
        // public PlayerVoteArea Extend { get; set; }

        public bool CanVote => VoteBank > 0 && !SelfVote;

        public static Sprite Button => TownOfUs.ButtonSprite;
        
        public override void PostHudUpdate(HudManager __instance) {
            base.PostHudUpdate(__instance);
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Mayor)) return;

            var data = PlayerControl.LocalPlayer.Data;
            var isDead = data.IsDead;

            var role = Role.GetRole<Mayor>(PlayerControl.LocalPlayer);

            if (role.ButtonButton == null) {
                role.ButtonButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.ButtonButton.graphic.enabled = true;
                role.ButtonButton.graphic.sprite = Button;

                var position1 = __instance.UseButton.transform.position;
                role.ButtonButton.transform.position = new Vector3(
                    Camera.main.ScreenToWorldPoint(new Vector3(0, 0)).x + 0.75f, position1.y,
                    position1.z);
            }

            role.ButtonButton.graphic.sprite = Button;


            role.ButtonButton.gameObject.SetActive(!PlayerControl.LocalPlayer.Data.IsDead && !MeetingHud.Instance);

            role.ButtonButton.SetCoolDown(0f, 1f);
            var renderer = role.ButtonButton.graphic;

            if (!role.ButtonUsed && PlayerControl.LocalPlayer.RemainingEmergencies > 0){
                renderer.color = Palette.EnabledColor;
                renderer.material.SetFloat("_Desat", 0f);
                return;
            }

            renderer.color = Palette.DisabledClear;
            renderer.material.SetFloat("_Desat", 1f);
        }

        public override void PostFixedUpdateLocal() {
            base.PostFixedUpdateLocal();
            
            if (Player.Data.IsDead && !ButtonUsed && CustomGameOptions.MayorMeetingOnDead && PlayerControl.LocalPlayer.RemainingEmergencies > 0) {
                ButtonUsed = true;
                if (MeetingHud.Instance != null) {
                    return;
                }
                reportDelay -= Time.fixedDeltaTime;
                if (reportDelay <= 0f) {
                    DoClick.MayorEmergencyMeeting(this);
                }
            }
        }
    }
}