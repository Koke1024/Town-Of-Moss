using System;
using System.Linq;
using HarmonyLib;
using Hazel;
using TownOfUs.Roles;
using Il2CppSystem.Collections;
using Il2CppSystem.Collections.Generic;
using Reactor;
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
                            cracker.blackOutRoomId = __instance.LastRoom.RoomId;
                        }
                    }
                }
            }
        }


        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.FixedUpdate))]
        public static class blackoutRoom {
            public static void Prefix(PlayerControl __instance) {
                if (LobbyBehaviour.Instance || MeetingHud.Instance) {
                    return;
                }

                if (!__instance.Is(RoleEnum.Cracker)) {
                    return;
                }

                Cracker role = Role.GetRole<Cracker>(__instance);
                if (role.blackOutRoomId == null) {
                    return;
                }

                if (HudManager.Instance == null || HudManager.Instance.ReportButton == null) {
                    role.HackingRoom = null;
                    role.RoomDetected = null;
                    role.blackOutRoomId = null;
                    return;
                }

                if (HudManager.Instance.ReportButton) {
                    HudManager.Instance.ReportButton.enabled = true;
                }

                if ((DateTime.UtcNow - role.RoomDetected).Value.Seconds > CustomGameOptions.CrackDur) {
                    role.HackingRoom = null;
                    role.RoomDetected = null;
                    role.blackOutRoomId = null;
                    return;
                }

                if (role.blackOutRoomId != role.HackingRoom) {
                    //another room hacked
                    role.blackOutRoomId = null;
                    return;
                }

                if (Cracker.MyLastRoom != role.blackOutRoomId) {
                    return;
                }

                if (HudManager.Instance.ReportButton) {
                    HudManager.Instance.ReportButton.enabled = false;
                    HudManager.Instance.ReportButton.SetActive(false);
                }

                if (Minigame.Instance) {
                    if (Minigame.Instance.TaskType != TaskTypes.ResetReactor &&
                        Minigame.Instance.TaskType != TaskTypes.RestoreOxy &&
                        Minigame.Instance.TaskType != TaskTypes.FixLights &&
                        Minigame.Instance.TaskType != TaskTypes.FixComms
                    ) {
                        Minigame.Instance.Close();
                        Minigame.Instance.Close();
                    }
                }

                if (MapBehaviour.Instance && !PlayerControl.LocalPlayer.Is(Faction.Impostors)) {
                    MapBehaviour.Instance.Close();
                    MapBehaviour.Instance.Close();
                }

                role.blackOutRoomId = null;
            }
        }
    }
}