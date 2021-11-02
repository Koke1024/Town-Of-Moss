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
        public static bool Prefix(ReactorSystemType __instance, float deltaTime) {
            if (ShipStatus.Instance.Type != ShipStatus.MapType.Pb || !__instance.IsActive) {
                return true;
            }

            if (__instance.Countdown >= CustomGameOptions.PolusReactorTimeLimit) {
                __instance.Countdown = CustomGameOptions.PolusReactorTimeLimit;
            }

            return true;
            // if (__instance.IsActive)
            // {
            //     if (!PlayerTask.PlayerHasTaskOfType<ReactorTask>(PlayerControl.LocalPlayer))
            //     {
            //         PlayerControl.LocalPlayer.AddSystemTask(__instance.system);
            //     }
            //
            //     __instance.timer += deltaTime * 1.5f;
            //     if (__instance.timer > 2f)
            //     {
            //         __instance.timer = 0f;
            //         __instance.IsDirty = true;
            //         return false;
            //     }
            // }
            // else
            // {
            //     DestroyableSingleton<HudManager>.Instance.StopReactorFlash();
            // }
            //
            // return false;
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

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.FixedUpdate))]
    public class ConsumeAdmin {
        public static void Prefix(PlayerControl __instance) {
            if (CustomGameOptions.AdminTimeLimitTime == 0) {
                return;
            }

            if (__instance.PlayerId != PlayerControl.LocalPlayer.PlayerId) {
                return;
            }

            AdminTimeLimit.timeLimit -= Time.fixedDeltaTime * AdminTimeLimit.AdminWatcher.Count;
        }
    }

    [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.Start))]
    public static class OnGameStart {
        public static void Postfix(ShipStatus __instance) {
            AdminTimeLimit.timeLimit = CustomGameOptions.AdminTimeLimitTime;
        }
    }

    [HarmonyPatch(typeof(MapCountOverlay), nameof(MapCountOverlay.Update))]
    public static class AdminTimeLimit {
        public static float timeLimit;
        public static List<byte> AdminWatcher = new List<byte>();
        public static TextMeshPro timeText = null;

        public static bool Prefix(MapCountOverlay __instance) {
            if (CustomGameOptions.AdminTimeLimitTime == 0) {
                return true;
            }

            if (timeText == null) {
                timeText = Object.Instantiate(PlayerControl.LocalPlayer.nameText, null);
                timeText.transform.position = HudManager.Instance.UseButton.transform.position;
                timeText.color = Color.white;
                timeText.transform.localScale = Vector3.one * 2.0f;
            }

            timeText.text = $"{(int)timeLimit}";


            if (!AdminWatcher.Contains(PlayerControl.LocalPlayer.PlayerId) && !PlayerControl.LocalPlayer.Data.IsDead) {
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                    (byte)CustomRPC.StartWatchAdmin, SendOption.Reliable, -1);
                writer.Write(PlayerControl.LocalPlayer.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                AdminTimeLimit.AdminWatcher.Add(PlayerControl.LocalPlayer.PlayerId);
            }

            if (timeLimit <= 0) {
                timeLimit = 0;
                __instance.BackgroundColor.SetColor(Palette.DisabledGrey);
                __instance.OnDisable();
                return false;
            }

            return true;
        }
    }

    [HarmonyPatch(typeof(MapBehaviour), nameof(MapBehaviour.Close))]
    public static class AdminTimeLimitClose {
        public static void Prefix(MapCountOverlay __instance) {
            if (CustomGameOptions.AdminTimeLimitTime == 0) {
                return;
            }

            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                (byte)CustomRPC.EndWatchAdmin, SendOption.Reliable, -1);
            writer.Write(PlayerControl.LocalPlayer.PlayerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            AdminTimeLimit.AdminWatcher.Remove(PlayerControl.LocalPlayer.PlayerId);

            AdminTimeLimit.timeText.Destroy();
            AdminTimeLimit.timeText = null;
        }
    }


    [HarmonyPatch(typeof(Object), nameof(Object.Destroy), typeof(Object))]
    public static class AdminTimeReset {
        public static void Postfix(Object obj) {
            if (ExileController.Instance == null || obj != ExileController.Instance.gameObject) return;

            if (CustomGameOptions.AdminTimeLimitTime > 0) {
                AdminTimeLimit.timeLimit = CustomGameOptions.AdminTimeLimitTime;
                AdminTimeLimit.AdminWatcher = new List<byte>();
            }
        }
    }
}