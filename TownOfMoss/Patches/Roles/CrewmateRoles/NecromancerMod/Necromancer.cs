using Il2CppSystem;
using Il2CppSystem.Collections.Generic;
using Reactor.Extensions;
using TownOfUs.CrewmateRoles.NecromancerMod;
using UnityEngine;
using Coroutine = TownOfUs.CrewmateRoles.NecromancerMod.Coroutine;

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

        public override void PostFixedUpdate() {
            base.PostFixedUpdate();

            if (Coroutine.Arrow != null) {
                if (LobbyBehaviour.Instance || MeetingHud.Instance || PlayerControl.LocalPlayer.Data.IsDead ||
                    Coroutine.Target.Data.IsDead) {
                    Coroutine.Arrow.gameObject.Destroy();
                    Coroutine.Target = null;
                    return;
                }

                Coroutine.Arrow.target = Coroutine.Target.transform.position;
            }
        }

        public override void PostHudUpdate(HudManager __instance) {
            base.PostHudUpdate(__instance);

            var data = PlayerControl.LocalPlayer.Data;
            var isDead = data.IsDead;
            var truePosition = PlayerControl.LocalPlayer.GetTruePosition();
            var maxDistance = GameOptionsData.KillDistances[0];
            var flag = (PlayerControl.GameOptions.GhostsDoTasks || !data.IsDead) &&
                       (!AmongUsClient.Instance || !AmongUsClient.Instance.IsGameOver) &&
                       PlayerControl.LocalPlayer.CanMove;
            var allocs = Physics2D.OverlapCircleAll(truePosition, maxDistance,
                LayerMask.GetMask(new[] {"Players", "Ghost"}));
            var killButton = __instance.KillButton;
            DeadBody closestBody = null;
            var closestDistance = float.MaxValue;

            foreach (var collider2D in allocs)
            {
                if (!flag || isDead || !collider2D.CompareTag("DeadBody")) continue;
                var component = collider2D.GetComponent<DeadBody>();


                if (!(Vector2.Distance(truePosition, component.TruePosition) <=
                      maxDistance)) continue;

                var distance = Vector2.Distance(truePosition, component.TruePosition);
                if (!(distance < closestDistance)) continue;
                closestBody = component;
                closestDistance = distance;
            }

            if (isDead)
            {
                killButton.gameObject.SetActive(false);
                //   killButton.isActive = false;
            }
            else
            {
                killButton.gameObject.SetActive(!MeetingHud.Instance);
                //      killButton.isActive = !MeetingHud.Instance;
            }

            NecromancerKillButtonTarget.SetTarget(killButton, closestBody, this);
            __instance.KillButton.SetCoolDown(ReviveTimer(), CustomGameOptions.NecroCoolDown);
        }
    }
}