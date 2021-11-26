using HarmonyLib;
using Hazel;
using Il2CppSystem.Collections.Generic;
using Reactor.Extensions;
using TMPro;
using TownOfUs;
using TownOfUs.Extensions;
using TownOfUs.Roles;
using UnityEngine;

namespace GameCustomize {
//
// [HarmonyPatch(typeof(WeaponsMinigame), nameof(WeaponsMinigame.Begin))]
// public static class HackWeaponMinigame {
//
// 	private static void Postfix(WeaponsMinigame __instance) {
// 		__instance.TimeToSpawn = new FloatRange(0.15f, 0.35f);
// 		__instance.TimeToSpawn.Next();
// 		__instance.MyNormTask.MaxStep = 100;
// 	}
// }
// [HarmonyPatch(typeof(Asteroid), nameof(Asteroid.Reset))]
// public static class HackAsteroid {
// 	private static void Postfix(Asteroid __instance) {
// 		__instance.MoveSpeed = new FloatRange(5.0f, 20.0f);
// 		// __instance.MoveSpeed = new FloatRange(2f, 5f);
// 		__instance.MoveSpeed.Next();
// 	}
// }
//
// [HarmonyPatch(typeof(UnlockManifoldsMinigame), nameof(UnlockManifoldsMinigame.ResetAll))]
// public static class HackUnlockManifoldsMinigame {
//
// 	private static void Postfix(WeaponsMinigame __instance) {
// 		__instance.MyNormTask.MaxStep = 100;
// 	}
// }

    [HarmonyPatch(typeof(ReactorSystemType), nameof(ReactorSystemType.Detoriorate))]
    public static class MeltdownBooster {
        public static void Prefix(ReactorSystemType __instance, float deltaTime) {
            if (!__instance.IsActive) {
                return;
            }
            
            if (ShipStatus.Instance.Type == ShipStatus.MapType.Pb) {
                if (__instance.Countdown >= CustomGameOptions.PolusReactorTimeLimit) {
                    __instance.Countdown = CustomGameOptions.PolusReactorTimeLimit;
                }
                return;
            }
            return;
        }
    }

    [HarmonyPatch(typeof(HeliSabotageSystem), nameof(HeliSabotageSystem.Detoriorate))]
    public static class HeliMeltdownBooster {
        public static void Prefix(HeliSabotageSystem __instance, float deltaTime) {
            if (!__instance.IsActive) {
                return;
            }
            
            if (AirshipStatus.Instance != null) {
                if (__instance.Countdown >= CustomGameOptions.AirshipReactorTimeLimit) {
                    __instance.Countdown = CustomGameOptions.AirshipReactorTimeLimit;
                }
            }
        }
    }

//
//
// [HarmonyPatch(typeof(InfectedOverlay), nameof(InfectedOverlay.CanUseDoors))]
// public static class SkeldDoorUpdate {
// }
//
//
// [HarmonyPatch(typeof(ReactorSystemType), nameof(ReactorSystemType.ReactorSystemType))]
// public static class MapStatusOverride {
//     public static void Postfix(PolusShipStatus __instance){
//         __instance.Systems.Add(SystemTypes.Laboratory, new ReactorSystemType(60f, SystemTypes.Laboratory));
//     }
//
// }
//
// [HarmonyPatch(typeof(AutoDoorsSystemType), nameof(AutoDoorsSystemType.CloseDoorsOfType))]
// public static class SkeldDoorUpdate {
//     static public void Postfix(AutoDoorsSystemType __instance, [HarmonyArgument(0)]SystemTypes room) {
//         for (int i = 0; i < ShipStatus.Instance.AllDoors.Length; i++)
//         {
//             PlainDoor plainDoor = ShipStatus.Instance.AllDoors[i];
//             if (plainDoor.Room == room)
//             {
//                 plainDoor.SetDoorway(false);
//             }
//         }
//     }
// }
//
}