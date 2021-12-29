using Il2CppSystem;
using Il2CppSystem.Collections.Generic;
using Reactor.Extensions;
using UnityEngine;

namespace TownOfUs.Roles
{
    public class Necromancer : Role
    {
        public DeadBody CurrentTarget;
        public readonly Queue<PlayerControl> RevivedPlayer = new();
        public DateTime RevivedTime;

        public Necromancer(PlayerControl player) : base(player)
        {
            Name = "Necromancer";
            ImpostorText = () => "Summon dead temporary";
            TaskText = () => "Revive a dead body until one meeting.";
            Color = new Color(0.4f, 0f, 0f, 1f);
            RoleType = RoleEnum.Necromancer;
        }
        
        public float ReviveTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - RevivedTime;
            var num = CustomGameOptions.NecroCoolDown * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }

        public override void OnEndMeeting() {
            base.OnEndMeeting();
            foreach (var revived in RevivedPlayer) {
                Utils.MurderPlayer(revived, revived);
                Utils.GetBody(revived.PlayerId).gameObject.Destroy();
            }
            RevivedPlayer.Clear();
        }
    }
}