using System;
using System.Linq;
using HarmonyLib;
using Hazel;
using TownOfUs.Extensions;
using TownOfUs.Roles;

namespace TownOfUs.ImpostorRoles.CrackerMod {
    [HarmonyPatch(typeof(RoomTracker), nameof(RoomTracker.FixedUpdate))]
    class CrackingRoom {
        public static void Prefix(RoomTracker __instance) {
            if (!__instance.gameObject.active) {
                return;
            }
            if (__instance.LastRoom == null) {
                Cracker.MyLastRoom = null;
                return;
            }
            Cracker.MyLastRoom = __instance.LastRoom.RoomId;
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

                if (Cracker.IsCracked(Cracker.MyLastRoom)) {
                    // AmongUsExtensions.Log($"CloseDoorsOfType {Cracker.MyLastRoom} == {type}");
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
                }

                if (Cracker.BlackoutRooms.Any()) {
                    if (Cracker.BlackoutRooms.First().Item1.AddSeconds(28f) < DateTime.UtcNow) {
                        Cracker.BlackoutRooms.Dequeue();
                    }
                }
            }
        }
    }

    [HarmonyPatch(typeof(PlainDoor), nameof(PlainDoor.DoorDynamics))]
    public static class Door2 {
        public static void Postfix(PlainDoor __instance) {
            if (__instance.Open) {
                return;
            }
            if (Cracker.IsBlackout(__instance.Room)) return;
            Cracker.BlackoutRooms.Enqueue((DateTime.UtcNow, __instance.Room));
        }
    }
    
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.OpenMap))]
    public static class MapBlock {
        public static bool Prefix(HudManager __instance) {
            if (PlayerControl.LocalPlayer.Data.IsImpostor()) {
                return true;
            }
            if (Cracker.IsCracked(Cracker.MyLastRoom)) {
                return false;
            }

            return true;
        }
    }
    
        
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.ShowMap))]
    public static class MapOpen {
        public static bool Prefix(MapBehaviour __instance) {
            if (PlayerControl.LocalPlayer.Data.IsImpostor()) {
                return true;
            }
            if (Cracker.IsCracked(Cracker.MyLastRoom)) {
                return false;
            }
            return true;
        }
    }
}