using Il2CppSystem;
using Reactor.Extensions;
using TownOfUs.CrewmateRoles.BodyGuardMod;
using UnityEngine;

namespace TownOfUs.Roles
{
    public enum NotificationOptions
    {
        Everyone = 0,
        Shielded = 1,
        BodyGuard = 2,
        Nobody = 3
    }
    public class BodyGuard : Role
    {
        public BodyGuard(PlayerControl player) : base(player)
        {
            Name = "BodyGuard";
            ImpostorText = () => "Create a shield to protect a crewmate";
            TaskText = () => "Protect a crewmate with a shield";
            Color = new Color(0f, 0.47f, 0.23f);
            RoleType = RoleEnum.BodyGuard;
        }

        public override void InitializeLocal() {
            base.InitializeLocal();
            
            ShieldedPlayer = null;
            ShieldedTime = DateTime.UtcNow;
        }

        public PlayerControl ClosestPlayer;
        public PlayerControl ShieldedPlayer { get; set; }
        public DateTime ShieldedTime;
        // public bool Defended = false;
        
        public float ShieldTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - ShieldedTime;
            var num = CustomGameOptions.GuardCoolDown * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }
        
        public ArrowBehaviour Arrow;
        public static Sprite Sprite => TownOfUs.Arrow;

        public void SetProtectionTarget()
        {
            var gameObj = new GameObject();
            Arrow = gameObj.AddComponent<ArrowBehaviour>();
            gameObj.transform.parent = PlayerControl.LocalPlayer.gameObject.transform;
            var renderer = gameObj.AddComponent<SpriteRenderer>();
            renderer.sprite = Sprite;
            Arrow.image = renderer;
            gameObj.layer = 5;
        }

        public override void OnEndMeeting() {
            base.OnEndMeeting();
            foreach (var role in GetRoles(RoleEnum.BodyGuard))
            {
                ShieldedTime = DateTime.UtcNow;
                ShieldedPlayer = null;
                if (Arrow) {
                    Arrow.gameObject.Destroy();
                }
            }
        }

        public override void PostFixedUpdateLocal() {
            base.PostFixedUpdateLocal();
            
            var protectButton = DestroyableSingleton<HudManager>.Instance.KillButton;

            if (Player.Data.IsDead)
            {
                protectButton.gameObject.SetActive(false);
            }
            else
            {
                protectButton.gameObject.SetActive(!MeetingHud.Instance);
                protectButton.SetCoolDown(ShieldTimer(), CustomGameOptions.GuardCoolDown);
                Utils.SetTarget(ref ClosestPlayer, protectButton);
            }
            
            if (Arrow == null || ShieldedPlayer == null) return;
            if (LobbyBehaviour.Instance || MeetingHud.Instance || PlayerControl.LocalPlayer.Data.IsDead ||
                ShieldedPlayer.Data.IsDead)
            {
                ShieldedPlayer = null;
                Arrow.Destroy();
                return;
            }

            Arrow.target = ShieldedPlayer.transform.position;
            Arrow.gameObject.SetActive(!(Vector2.Distance(Player.GetTruePosition(), ShieldedPlayer.GetTruePosition()) >
                                         CustomGameOptions.GuardRange / 2.0f));
            if (ShieldedTime.AddSeconds(CustomGameOptions.GuardDuration) > DateTime.UtcNow) return;
            ShieldedTime = DateTime.UtcNow;
            ShieldedPlayer = null;
            Arrow.gameObject.SetActive(false);
            Arrow.Destroy();
        }

        public static Color ProtectedColor = Color.cyan;
        public override void PostHudUpdate(HudManager __instance) {
            base.PostHudUpdate(__instance);

            var player = ShieldedPlayer;
            if (player == null) return;

            if (player.Data.IsDead || Player.Data.IsDead)
            {
                StopKill.BreakShield(Player.PlayerId, player.PlayerId, true);
            }
        }
    }
}