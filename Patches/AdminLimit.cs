using HarmonyLib;
using Hazel;
using Il2CppSystem.Collections.Generic;
using Reactor.Extensions;
using TMPro;
using TownOfUs.Extensions;
using UnityEngine;

namespace TownOfUs.Patches {
    public class MechanicLimit {
        public static float timeLimit;
        public static List<byte> AdminWatcher = new List<byte>();
        static TextMeshPro timeText;

        [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.Start))]
        public static class OnGameStart {
            public static void Postfix(ShipStatus __instance) {
                timeLimit = CustomGameOptions.AdminTimeLimitTime;
            }
        }

        [HarmonyPatch(typeof(Object), nameof(Object.Destroy), typeof(Object))]
        public static class AdminTimeReset {
            public static void Postfix(Object obj) {
                if (ExileController.Instance == null || obj != ExileController.Instance.gameObject) return;

                if (CustomGameOptions.AdminTimeLimitTime > 0) {
                    timeLimit = CustomGameOptions.AdminTimeLimitTime;
                    AdminWatcher = new List<byte>();
                }
            }
        }
        
        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.FixedUpdate))]
        public class ConsumeAdmin {
            public static void Prefix(PlayerControl __instance) {
                if (CustomGameOptions.AdminTimeLimitTime == 0) {
                    return;
                }
                if (__instance.PlayerId != PlayerControl.LocalPlayer.PlayerId) {
                    return;
                }
                timeLimit -= Time.fixedDeltaTime * AdminWatcher.Count;
            }
        }

        [HarmonyPatch(typeof(MapBehaviour), nameof(MapBehaviour.ShowCountOverlay))]
        public static class MapOpen {
            public static void Postfix(MapBehaviour __instance) {
                MechanicalOpen(__instance);
            }
        }

        [HarmonyPatch(typeof(MapCountOverlay), nameof(MapCountOverlay.Update))]
        public static class AdminTimeLimit {
            public static bool Prefix(MapCountOverlay __instance) {
                if (!MechanicalUpdate(__instance)) {
                    __instance.BackgroundColor.SetColor(Palette.DisabledGrey);
                    __instance.OnDisable();
                    return false;
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(MapBehaviour), nameof(MapBehaviour.Close))]
        public static class AdminTimeLimitClose {
            public static void Prefix(MapBehaviour __instance) {
                MechanicalClose(__instance);
            }
        }

        [HarmonyPatch(typeof(SurveillanceMinigame), nameof(SurveillanceMinigame.CoAnimateOpen))]
        public static class CameraTimeLimitOpen {
            public static void Postfix(SurveillanceMinigame __instance) {
                MechanicalOpen(__instance);
            }
        }

        [HarmonyPatch(typeof(SurveillanceMinigame), nameof(SurveillanceMinigame.Update))]
        public static class CameraTimeLimit {
            public static bool Prefix(SurveillanceMinigame __instance) {
                if (!MechanicalUpdate(__instance)) {
                    __instance.isStatic = true;
                    for (int j = 0; j < __instance.ViewPorts.Length; j++) {
                        __instance.ViewPorts[j].sharedMaterial = __instance.StaticMaterial;
                    }
                    return false;
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(SurveillanceMinigame), nameof(SurveillanceMinigame.CoDestroySelf))]
        public static class CameraTimeLimitClose {
            public static void Postfix(SurveillanceMinigame __instance) {
                MechanicalClose(__instance);
            }
        }

        [HarmonyPatch(typeof(PlanetSurveillanceMinigame), nameof(PlanetSurveillanceMinigame.CoAnimateOpen))]
        public static class PbCameraTimeLimitOpen {
            public static void Postfix(PlanetSurveillanceMinigame __instance) {
                MechanicalOpen(__instance);
            }
        }

        [HarmonyPatch(typeof(PlanetSurveillanceMinigame), nameof(PlanetSurveillanceMinigame.Update))]
        public static class PolusCameraTimeLimit {
            public static bool Prefix(PlanetSurveillanceMinigame __instance) {
                if (!MechanicalUpdate(__instance)) {
                    __instance.isStatic = true;
                    __instance.ViewPort.sharedMaterial = __instance.StaticMaterial;
                    return false;
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(PlanetSurveillanceMinigame), nameof(PlanetSurveillanceMinigame.CoDestroySelf))]
        public static class PolusCameraTimeLimitClose {
            public static void Postfix(PlanetSurveillanceMinigame __instance) {
                MechanicalClose(__instance);
            }
        }

        // [HarmonyPatch(typeof(SurveillanceMinigame), nameof(SurveillanceMinigame.Close))]
        // public static class CameraTimeLimitClose {
        //     public static void Prefix(SurveillanceMinigame __instance) {
        //         MechanicalClose(__instance);
        //     }
        // }

        // [HarmonyPatch(typeof(Minigame), nameof(Minigame.CoDestroySelf))]
        // public static class DestroySelf {
        //     public static void Postfix(Minigame __instance) {
        //         AmongUsExtensions.Log($"CoDestroySelf {(__instance.GetType().Name)}");
        //         AmongUsExtensions.Log($"VitalsMinigame {__instance as VitalsMinigame != null}");
        //         AmongUsExtensions.Log($"PlanetSurveillanceMinigame {__instance as PlanetSurveillanceMinigame != null}");
        //         AmongUsExtensions.Log($"SurveillanceMinigame {__instance as SurveillanceMinigame != null}");
        //         MechanicalClose(__instance);
        //     }
        // }

        public static bool MechanicalUpdate(MonoBehaviour __instance) {
            if (CustomGameOptions.AdminTimeLimitTime == 0) {
                return true;
            }


            if (timeText) {
                timeText.text = (int)timeLimit > 10 ? $"{(int)timeLimit}" : $"<color=#FF0000FF>{(int)timeLimit}</color>";
            }
            
            if (timeLimit <= 1) {
                timeLimit = 0;
                return false;
            }

            return true;
        }
        
        static void MechanicalClose(MonoBehaviour __instance) {
            if (CustomGameOptions.AdminTimeLimitTime == 0) {
                return;
            }
            DestroyableSingleton<HudManager>.Instance.SetHudActive(true);
            // AmongUsExtensions.Log($"close {(__instance.GetType().Name)}");

            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                (byte)CustomRPC.EndWatchAdmin, SendOption.Reliable, -1);
            writer.Write(PlayerControl.LocalPlayer.PlayerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            if (AdminWatcher.Contains(PlayerControl.LocalPlayer.PlayerId)) {
                AdminWatcher.Remove(PlayerControl.LocalPlayer.PlayerId);                
            }

            timeText.Destroy();
            timeText = null;
        }
        
        static bool MechanicalOpen(MonoBehaviour __instance) {
            if (CustomGameOptions.AdminTimeLimitTime == 0) {
                return true;
            }

            // AmongUsExtensions.Log($"update {(__instance.GetType().Name)}");

            if (timeText == null) {
                timeText = Object.Instantiate(PlayerControl.LocalPlayer.nameText, null);
                timeText.transform.position = HudManager.Instance.UseButton.transform.position;
                timeText.color = Color.white;
                timeText.transform.localScale = Vector3.one * 2.0f;
                timeText.text = "";
                
                timeText.gameObject.transform.SetParent(__instance.transform);
                if (__instance as PlanetSurveillanceMinigame != null) {
                    timeText.transform.position = new Vector3(timeText.transform.position.x, timeText.transform.position.y, 0.0f);
                    timeText.transform.SetParent(((PlanetSurveillanceMinigame)__instance).FillQuad.transform);
                }
            }

            DestroyableSingleton<HudManager>.Instance.SetHudActive(false);
            
            if (!AdminWatcher.Contains(PlayerControl.LocalPlayer.PlayerId) &&
                !PlayerControl.LocalPlayer.Data.IsDead) {
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                    (byte)CustomRPC.StartWatchAdmin, SendOption.Reliable, -1);
                writer.Write(PlayerControl.LocalPlayer.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                AdminWatcher.Add(PlayerControl.LocalPlayer.PlayerId);
            }

            return true;
        }
    }
}