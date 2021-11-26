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
                role.DollList[key] += Time.fixedDeltaTime;
                PlayerControl.LocalPlayer.moveable = false;
                PlayerControl.LocalPlayer.NetTransform.Halt();
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

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.CoStartMeeting))]
    class StartMeetingPatch {
        public static void Prefix(PlayerControl __instance, [HarmonyArgument(0)]GameData.PlayerInfo meetingTarget) {
            if (__instance == null) {
                return;
            }
            foreach (var role in Role.GetRoles(RoleEnum.DollMaker)) {
                if (((DollMaker)role).DollList.Count <= 0) continue;
                foreach (var (key, _) in ((DollMaker)role).DollList) {
                    role.Player.MurderPlayer(GameData.Instance.GetPlayerById(key)._object);
                }
                ((DollMaker)role).DollList.Clear();
            }
        }
    }
}