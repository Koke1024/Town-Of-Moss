using System;
using System.Linq;
using HarmonyLib;
using Hazel;
using Reactor;
using TownOfUs.CrewmateRoles.SwapperMod;
using TownOfUs.Roles;
using UnityEngine;

namespace TownOfUs.ImpostorRoles.CrackerMod {
    [HarmonyPatch(typeof(RoomTracker), nameof(RoomTracker.SlideOut))]
    class RoomSlideOut {
        public static void Prefix(RoomTracker __instance) {
            var localPlayer = PlayerControl.LocalPlayer;
            if (localPlayer.Is(RoleEnum.Cracker)) {
                var cracker = Role.GetRole<Cracker>(localPlayer);
                if (__instance.LastRoom == null) {
                    cracker.TargetRoom = null;
                    return;
                }
                cracker.TargetRoom = __instance.LastRoom.RoomId;
            }
            else {
                foreach (Cracker cracker in Role.GetRoles(RoleEnum.Cracker)) {
                    if (__instance.LastRoom == null) {
                        cracker.MyLastRoom = null;
                        return;
                    }

                    cracker.MyLastRoom = __instance.LastRoom.RoomId;
                    if (cracker.HackingRoom == __instance.LastRoom.RoomId) {
                        if (cracker.RoomDetected == null) {
                            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                                (byte)CustomRPC.DetectCrackRoom, SendOption.Reliable, -1);
                            writer.Write((byte)__instance.LastRoom.RoomId);
                            writer.Write((byte)cracker.Player.PlayerId);
                            AmongUsClient.Instance.FinishRpcImmediately(writer);
                            Coroutines.Start(Cracker.HackRoomCoroutine(__instance.LastRoom.RoomId, cracker));
                        }
                    }
                }
            }
        }
    }
}