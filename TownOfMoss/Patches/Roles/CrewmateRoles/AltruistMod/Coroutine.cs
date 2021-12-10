using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using InnerNet;
using Reactor.Extensions;
using TownOfUs.CrewmateRoles.MedicMod;
using TownOfUs.Extensions;
using TownOfUs.Roles;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TownOfUs.CrewmateRoles.AltruistMod {
    public class Coroutine {
        public static ArrowBehaviour Arrow;
        public static PlayerControl Target;
        public static Sprite Sprite => TownOfUs.Arrow;

        public static IEnumerator AltruistRevive(DeadBody target, Altruist role) {
            var myPosition = role.Player.GetTruePosition();
            var parentId = target.ParentId;
            var position = target.TruePosition;

            var revived = new List<PlayerControl>();
            
            Utils.MurderPlayer(role.Player, role.Player);
            
            if (target != null && Utils.KilledPlayers.ContainsKey(target.ParentId)) {
                Utils.KilledPlayers[target.ParentId].Body.gameObject.Destroy();
                Utils.KilledPlayers.Remove(target.ParentId);
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
            // if (CustomGameOptions.AltruistLendBody) {
                player.myRend.flipX = role.Player.myRend.flipX;
                player.myRend.flipY = role.Player.myRend.flipY;
                player.NetTransform.SnapTo(role.Player.GetTruePosition() - role.Player.Collider.offset);
                role.Player.myRend.enabled = false;
            // }
            // else {
            //     player.NetTransform.SnapTo(position);
            // }

            role.revivedPlayer = player;

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

            yield return new WaitForSeconds(1.0f);
            role.Player.myRend.enabled = true;
            yield break;
        }
    }
}