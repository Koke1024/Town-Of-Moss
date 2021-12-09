using Il2CppSystem;
using TownOfUs.Extensions;
using UnityEngine;

namespace TownOfUs.Roles
{
    public class BodyGuard : Role
    {
        public BodyGuard(PlayerControl player) : base(player)
        {
            Name = "BodyGuard";
            ImpostorText = () => "Create a shield to protect a crewmate";
            TaskText = () => "Protect a crewmate with a shield";
            Color = new Color(0f, 0.47f, 0.23f);
            RoleType = RoleEnum.BodyGuard;
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
        
        // internal override bool Criteria()
        // {
        //     return Defended && PlayerControl.LocalPlayer.Data.IsImpostor() ||
        //            base.Criteria();
        // }
    }
}