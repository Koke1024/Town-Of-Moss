using HarmonyLib;
using Hazel;
using Il2CppSystem.Collections.Generic;
using Reactor.Extensions;
using TMPro;
using UnityEngine;

namespace TownOfUs.Patches {
    public class MechanicLimit {
        public static float timeLimit;
        public static List<byte> AdminWatcher = new List<byte>();
        static TextMeshPro timeText = null;
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

        [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.Start))]
        public static class OnGameStart {
            public static void Postfix(ShipStatus __instance) {
                timeLimit = CustomGameOptions.AdminTimeLimitTime;
            }
        }

        [HarmonyPatch(typeof(MapCountOverlay), nameof(MapCountOverlay.Update))]
        public static class AdminTimeLimit {
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


                if (!AdminWatcher.Contains(PlayerControl.LocalPlayer.PlayerId) &&
                    !PlayerControl.LocalPlayer.Data.IsDead) {
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                        (byte)CustomRPC.StartWatchAdmin, SendOption.Reliable, -1);
                    writer.Write(PlayerControl.LocalPlayer.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    AdminWatcher.Add(PlayerControl.LocalPlayer.PlayerId);
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
            public static void Prefix(MapBehaviour __instance) {
                if (CustomGameOptions.AdminTimeLimitTime == 0) {
                    return;
                }

                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                    (byte)CustomRPC.EndWatchAdmin, SendOption.Reliable, -1);
                writer.Write(PlayerControl.LocalPlayer.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                AdminWatcher.Remove(PlayerControl.LocalPlayer.PlayerId);

                timeText.Destroy();
                timeText = null;
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

        [HarmonyPatch(typeof(SurveillanceMinigame), nameof(SurveillanceMinigame.Update))]
        public static class CameraTimeLimit {
            public static bool Prefix(SurveillanceMinigame __instance) {
                if (!MechanicalUpdate()) {
                    __instance.isStatic = true;
                    for (int j = 0; j < __instance.ViewPorts.Length; j++) {
                        __instance.ViewPorts[j].sharedMaterial = __instance.StaticMaterial;
                    }
                    return false;
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(VitalsMinigame), nameof(VitalsMinigame.Update))]
        public static class VitalTimeLimit {
            public static bool Prefix(VitalsMinigame __instance) {
                if (!MechanicalUpdate()) {
                    for (int j = 0; j < __instance.vitals.Length; j++)
                    {
                        __instance.vitals[j].gameObject.SetActive(false);
                    }
                    return false;
                }
                return true;
            }
        }

        public static bool MechanicalUpdate() {
            if (CustomGameOptions.AdminTimeLimitTime == 0) {
                return true;
            }

            if (timeText == null) {
                timeText = Object.Instantiate(PlayerControl.LocalPlayer.nameText, null);
                timeText.transform.position = HudManager.Instance.UseButton.transform.position;
                timeText.transform.position += Vector3.forward;
                timeText.color = Color.white;
                timeText.transform.localScale = Vector3.one * 2.0f;
            }

            timeText.text = $"{(int)timeLimit}";


            if (!AdminWatcher.Contains(PlayerControl.LocalPlayer.PlayerId) &&
                !PlayerControl.LocalPlayer.Data.IsDead) {
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                    (byte)CustomRPC.StartWatchAdmin, SendOption.Reliable, -1);
                writer.Write(PlayerControl.LocalPlayer.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                AdminWatcher.Add(PlayerControl.LocalPlayer.PlayerId);
            }

            if (timeLimit <= 0) {
                timeLimit = 0;
                return false;
            }

            return true;
        }

        [HarmonyPatch(typeof(SurveillanceMinigame), nameof(SurveillanceMinigame.Close))]
        public static class CameraTimeLimitClose {
            public static void Prefix(SurveillanceMinigame __instance) {
                MechanicalClose();
            }
        }

        [HarmonyPatch(typeof(VitalsMinigame), nameof(VitalsMinigame.Close))]
        public static class VitalTimeLimitClose {
            public static void Prefix(VitalsMinigame __instance) {
                MechanicalClose();
            }
        }

        static void MechanicalClose() {
            if (CustomGameOptions.AdminTimeLimitTime == 0) {
                return;
            }

            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                (byte)CustomRPC.EndWatchAdmin, SendOption.Reliable, -1);
            writer.Write(PlayerControl.LocalPlayer.PlayerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            AdminWatcher.Remove(PlayerControl.LocalPlayer.PlayerId);

            timeText.Destroy();
            timeText = null;
        }
    }
}