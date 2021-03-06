using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Reactor.Extensions;
using TownOfUs.Extensions;
using TownOfUs.Roles;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TownOfUs.CrewmateRoles.NecromancerMod {
    public class Coroutine {
        public static ArrowBehaviour Arrow;
        public static PlayerControl Target;
        public static Sprite Sprite => TownOfUs.Arrow;

        public static IEnumerator NecromancerRevive(DeadBody target, Necromancer role) {
            var myPosition = role.Player.GetTruePosition();
            var parentId = target.ParentId;
            var position = target.TruePosition;

            var revived = new List<PlayerControl>();

            // Utils.MurderPlayer(role.Player, role.Player);
            //
            // if (AmongUsClient.Instance.AmHost) Utils.RpcMurderPlayer(role.Player, role.Player);

            // if (CustomGameOptions.NecromancerTargetBody) {
            //     if (target != null && Utils.KilledPlayers.ContainsKey(target.ParentId)) {
            //         Utils.KilledPlayers[target.ParentId].Body.gameObject.Destroy();
            //         Utils.KilledPlayers.Remove(target.ParentId);
            //     }
            // }

            var startTime = DateTime.UtcNow;
            while (true) {
                var now = DateTime.UtcNow;
                var seconds = (now - startTime).TotalSeconds;
                if (seconds < CustomGameOptions.ReviveDuration)
                    yield return null;
                else break;

                if (MeetingHud.Instance) yield break;
            }

            var body = Utils.GetBody(role.Player.PlayerId);
            if (body != null) {
                body.gameObject.Destroy();
                Utils.KilledPlayers.Remove(role.Player.PlayerId);
            }

            var player = Utils.PlayerById(parentId);

            // if (player == null || AmongUsClient.Instance.GameState != InnerNetClient.GameStates.Started)
            //     yield break;

            player.Revive();
            revived.Add(player);

            role.RevivedPlayer.Enqueue(player);

            if (target != null) Object.Destroy(target.gameObject);

            if (player.isLover() && CustomGameOptions.BothLoversDie) {
                var lover = Role.GetRole<Lover>(player).OtherLover.Player;

                lover.Revive();
                revived.Add(lover);

                foreach (DeadBody deadBody in GameObject.FindObjectsOfType<DeadBody>()) {
                    if (deadBody.ParentId == lover.PlayerId) {
                        deadBody.gameObject.Destroy();
                    }
                }
            }

            if (revived.Any(x => x.AmOwner))
                try {
                    Minigame.Instance.Close();
                    Minigame.Instance.Close();
                }
                catch {
                }


            if (PlayerControl.LocalPlayer.Data.IsImpostor() || PlayerControl.LocalPlayer.Is(RoleEnum.Glitch)) {
                if (Arrow != null) {
                    Arrow.gameObject.SetActive(false);
                    Arrow.Destroy();
                }
                var gameObj = new GameObject();
                Arrow = gameObj.AddComponent<ArrowBehaviour>();
                gameObj.transform.parent = PlayerControl.LocalPlayer.gameObject.transform;
                var renderer = gameObj.AddComponent<SpriteRenderer>();
                renderer.sprite = Sprite;
                Arrow.image = renderer;
                gameObj.layer = 5;
                Target = player;
                yield return Utils.FlashCoroutine(role.Color, 1f, 0.5f);
            }

            yield break;
        }
    }
}