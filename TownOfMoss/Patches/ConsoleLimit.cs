using System.Linq;
using HarmonyLib;
using Hazel;
using Il2CppSystem.Collections.Generic;
using Reactor.Extensions;
using TMPro;
using TownOfUs.Extensions;
using UnityEngine;

namespace TownOfUs.Patches {
    public class ConsoleLimit {
        static TextMeshPro _timeText;
        public static float TimeLimit;
        public static List<byte> AdminWatcher = new List<byte>();

        [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.Start))]
        public static class OnGameStart {
            public static void Postfix(ShipStatus __instance) {
                TimeLimit = CustomGameOptions.AdminTimeLimitTime;
            }
        }
        [HarmonyPatch(typeof(Object), nameof(Object.Destroy), typeof(Object))]
        public static class AdminTimeReset {
            public static void Postfix(Object obj) {
                if (ExileController.Instance == null || obj != ExileController.Instance.gameObject) return;

                if (CustomGameOptions.AdminTimeLimitTime > 0) {
                    TimeLimit = CustomGameOptions.AdminTimeLimitTime;
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
                TimeLimit -= Time.fixedDeltaTime * AdminWatcher.Count;
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
            public static void Prefix(SurveillanceMinigame __instance) {
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
            public static void Prefix(PlanetSurveillanceMinigame __instance) {
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
        
        [HarmonyPatch(typeof(PlanetSurveillanceMinigame), nameof(PlanetSurveillanceMinigame.NextCamera))]
        public static class PolusCameraTimeLimitNext {
            public static bool Prefix(PlanetSurveillanceMinigame __instance, [HarmonyArgument(0)]int direction) {
                if (CustomGameOptions.AdminTimeLimitTime == 0) {
                    return true;
                }
        
                return !(TimeLimit <= 1);
            }
        }
        
        public static bool MechanicalUpdate(MonoBehaviour __instance) {
            if (CustomGameOptions.AdminTimeLimitTime == 0) {
                return true;
            }

            if (_timeText == null) {
                if (__instance as SurveillanceMinigame != null) {
                    _timeText = Object.Instantiate(((SurveillanceMinigame)__instance).SabText[0], __instance.transform);
                    _timeText.gameObject.GetComponent<AlphaBlink>().enabled = false;
                }
                if (__instance as PlanetSurveillanceMinigame != null) {
                    _timeText = Object.Instantiate(((PlanetSurveillanceMinigame)__instance).SabText, __instance.transform);
                    _timeText.fontSizeMax = _timeText.fontSize = 1.9f;
                    _timeText.fontSizeMin = 1;
                    _timeText.gameObject.GetComponent<AlphaBlink>().enabled = false;
                }
                if (_timeText == null) {
                    _timeText = Object.Instantiate(PlayerControl.LocalPlayer.nameText, __instance.transform);
                }
                _timeText.transform.localScale = Vector3.one * 3.0f;

                _timeText.name = "ConsoleTimeLimitText";
                _timeText.text = "";
                _timeText.color = Color.white;
                var pos = DestroyableSingleton<HudManager>.Instance.UseButton.transform.position;
                _timeText.transform.position = new Vector3(pos.x, pos.y, -70);
                _timeText.gameObject.transform.SetParent(__instance.transform);
                _timeText.gameObject.SetActive(true);
            }
            
            // AmongUsExtensions.Log($"{_timeText.transform.position.z}");
            
            _timeText.text = (int)TimeLimit > 10 ? $"{(int)TimeLimit}" : $"<color=#FF0000FF>{(int)TimeLimit}</color>";
            if (TimeLimit <= 1) {
                TimeLimit = 0;
                _timeText.text = $"<color=#FF0000FF>--</color>";
                return false;
            }
            return true;
        }
        
        static void MechanicalClose(MonoBehaviour __instance) {
            if (CustomGameOptions.AdminTimeLimitTime == 0) {
                return;
            }
            // DestroyableSingleton<HudManager>.Instance.SetHudActive(true);

            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                (byte)CustomRPC.EndWatchAdmin, SendOption.Reliable, -1);
            writer.Write(PlayerControl.LocalPlayer.PlayerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            if (AdminWatcher.Contains(PlayerControl.LocalPlayer.PlayerId)) {
                AdminWatcher.Remove(PlayerControl.LocalPlayer.PlayerId);                
            }

            if (_timeText) {
                _timeText.gameObject.Destroy();
                _timeText = null;                
            }
        }
        
        static bool MechanicalOpen(MonoBehaviour __instance) {
            if (CustomGameOptions.AdminTimeLimitTime == 0) {
                return true;
            }
            
            if (TimeLimit <= 1) {
                TimeLimit = 0;
                return false;
            }

            // DestroyableSingleton<HudManager>.Instance.SetHudActive(false);
            
            if (!AdminWatcher.Contains(PlayerControl.LocalPlayer.PlayerId) &&
                !PlayerControl.LocalPlayer.Data.IsDead) {
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                    (byte)CustomRPC.StartWatchAdmin, SendOption.Reliable, -1);
                writer.Write(PlayerControl.LocalPlayer.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                AdminWatcher.Add(PlayerControl.LocalPlayer.PlayerId);
            }

            MechanicalUpdate(__instance);

            return true;
        }
    }
}