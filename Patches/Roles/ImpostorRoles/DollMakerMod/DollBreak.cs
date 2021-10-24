using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using Hazel;
using TownOfUs.Extensions;
using TownOfUs.Roles;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngineInternal;

namespace TownOfUs.ImpostorRoles.DollMakerMod {
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.FixedUpdate))]
    public static class DollBreak {
        public static void Postfix(PlayerControl __instance) {
            if (!__instance.Is(RoleEnum.DollMaker)) {
                return;
            }
            
            DollMaker role = Role.GetRole<DollMaker>(__instance);
            if (LobbyBehaviour.Instance || MeetingHud.Instance) {
                if (role.DollList.Count > 0) {
                    foreach (var doll in role.DollList) {
                        GameData.Instance.GetPlayerById(doll.Key)._object.RpcMurderPlayer(GameData.Instance.GetPlayerById(doll.Key)._object);
                    }
                    role.DollList.Clear();
                }
                return;
            }


            var breakList = new Queue<byte>();
            foreach (var doll in role.DollList) {
                if (GameData.Instance.GetPlayerById(doll.Key).IsDead) {
                    continue;
                }
                PlayerControl closestPlayer = null;
                System.Collections.Generic.List<PlayerControl> targets = PlayerControl.AllPlayerControls.ToArray()
                    .ToList().FindAll(x =>
                        x.PlayerId != role.Player.PlayerId && x.PlayerId != doll.Key);
                if (Utils.SetClosestPlayerToPlayer(GameData.Instance.GetPlayerById(doll.Key)._object, ref closestPlayer,
                    0.8f, targets
                )) {
                    breakList.Enqueue(doll.Key);
                }
            }

            foreach (var breakQueue in breakList) {
                role.DollList.Remove(breakQueue);
                Utils.RpcMurderPlayer(GameData.Instance.GetPlayerById(breakQueue)._object, GameData.Instance.GetPlayerById(breakQueue)._object);
            }
        }
    }
    
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.FixedUpdate))]
    public static class DollCantMove {
        public static void Postfix(PlayerControl __instance) {
            if (!__instance.Is(RoleEnum.DollMaker)) {
                return;
            }
            DollMaker role = Role.GetRole<DollMaker>(__instance);

            var breakList = new Queue<byte>();
            var keys = role.DollList.Keys.ToArray();
            foreach (var key in keys) {
                if (GameData.Instance.GetPlayerById(key).IsDead) {
                    breakList.Enqueue(key);
                    continue;
                }
                if (key != PlayerControl.LocalPlayer.PlayerId) {
                    continue;
                }
                role.DollList[key] += Time.deltaTime;
                PlayerControl.LocalPlayer.moveable = false;
                if (role.DollList[key] >= CustomGameOptions.DollBreakTime) {
                    breakList.Enqueue(key);
                }
            }
            foreach (var breakQueue in breakList) {
                Utils.RpcMurderPlayer(GameData.Instance.GetPlayerById(breakQueue)._object, GameData.Instance.GetPlayerById(breakQueue)._object);
                role.DollList.Remove(breakQueue);
            }
        }
    }
}