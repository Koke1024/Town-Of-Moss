using System;
using System.Linq;
using HarmonyLib;
using Hazel;
using TownOfUs.Roles;
using Il2CppSystem.Collections;
using Il2CppSystem.Collections.Generic;
using Reactor;
using Reactor.Extensions;
using TownOfUs.Extensions;
using UnityEngine;

namespace TownOfUs.ImpostorRoles.CrackerMod {
    [HarmonyPatch(typeof(RoomTracker), nameof(RoomTracker.SlideOut))]
    class CrackingRoom {
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
                        Cracker.MyLastRoom = null;
                        return;
                    }

                    Cracker.MyLastRoom = __instance.LastRoom.RoomId;
                    if (cracker.HackingRoom == __instance.LastRoom.RoomId) {
                        if (cracker.RoomDetected == null) {
                            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                                (byte)CustomRPC.DetectCrackRoom, SendOption.Reliable, -1);
                            writer.Write((byte)__instance.LastRoom.RoomId);
                            writer.Write((byte)cracker.Player.PlayerId);
                            AmongUsClient.Instance.FinishRpcImmediately(writer);

                            cracker.RoomDetected = DateTime.UtcNow;
                            cracker.BlackOutRoomId = __instance.LastRoom.RoomId;
                        }
                    }
                }
            }
        }


        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.FixedUpdate))]
        public static class BlackoutRoom {
            public static void Prefix(PlayerControl __instance) {
                if (LobbyBehaviour.Instance || MeetingHud.Instance) {
                    return;
                }

                if (!__instance.Is(RoleEnum.Cracker)) {
                    return;
                }

                Cracker role = Role.GetRole<Cracker>(__instance);
                if (role.BlackOutRoomId == null || role.RoomDetected == null) {
                    return;
                }
                if ((DateTime.UtcNow - role.RoomDetected).Value.Seconds > CustomGameOptions.CrackDur) {
                    // var room = ShipStatus.Instance.AllRooms[(int)role.BlackOutRoomId];
                    // room.gameObject.GetComponent<OneWayShadows>().Destroy();
                    
                    role.HackingRoom = null;
                    role.RoomDetected = null;
                    role.BlackOutRoomId = null;

                    Cracker.InCrackedRoom = false;
                    return;
                }

                if (Cracker.MyLastRoom == role.BlackOutRoomId) {
                    if (role.Player.PlayerId != PlayerControl.LocalPlayer.PlayerId) {
                        if (Minigame.Instance) {
                            if (Minigame.Instance.TaskType != TaskTypes.ResetReactor &&
                                Minigame.Instance.TaskType != TaskTypes.RestoreOxy &&
                                Minigame.Instance.TaskType != TaskTypes.FixLights &&
                                Minigame.Instance.TaskType != TaskTypes.FixComms
                            ) {
                                Minigame.Instance.Close();
                            }
                        }
                    }

                    if (MapBehaviour.Instance && !PlayerControl.LocalPlayer.Is(Faction.Impostors)) {
                        MapBehaviour.Instance.Close();
                    }

                    Cracker.InCrackedRoom = true;
                }
                else {
                    Cracker.InCrackedRoom = false;
                }
            }
        }
    }
}