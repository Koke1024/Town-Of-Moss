using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using Hazel;
using TownOfUs.Extensions;
using TownOfUs.Roles;
using UnityEngine;
using UnityEngineInternal;

namespace TownOfUs.ImpostorRoles.DollMakerMod {
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.FixedUpdate))]
    public static class DollCrash {
        public static void Postfix(PlayerControl __instance) {
            if (!__instance.Is(RoleEnum.DollMaker)) {
                return;
            }
            
            DollMaker role = Role.GetRole<DollMaker>(__instance);
            if (LobbyBehaviour.Instance || MeetingHud.Instance) {
                if (DollMaker.DollList.Count > 0) {
                    foreach (var doll in DollMaker.DollList) {
                        GameData.Instance.GetPlayerById(doll.Key)._object.RpcMurderPlayer(GameData.Instance.GetPlayerById(doll.Key)._object);
                    }
                    DollMaker.DollList.Clear();
                }
                return;
            }


            foreach (var doll in DollMaker.DollList) {
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
                    Utils.RpcMurderPlayer(GameData.Instance.GetPlayerById(doll.Key)._object, GameData.Instance.GetPlayerById(doll.Key)._object);
                }
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

            foreach (var doll in DollMaker.DollList) {
                if (doll.key != PlayerControl.LocalPlayer.PlayerId) {
                    continue;
                }
                if (GameData.Instance.GetPlayerById(doll.Key).IsDead) {
                    continue;
                }
                doll.value += Time.deltaTime;
                PlayerControl.LocalPlayer.moveable = false;
                if (doll.value >= CustomGameOptions.DollBreakTime) {
                    Utils.RpcMurderPlayer(PlayerControl.LocalPlayer, PlayerControl.LocalPlayer);
                }
            }
        }
    }
}