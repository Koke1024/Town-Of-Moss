using HarmonyLib;
using Hazel;
using Il2CppSystem.Collections.Generic;
using Reactor.Extensions;
using TMPro;
using UnityEngine;

namespace TownOfUs.Patches {
    public class VitalLimit {
        [HarmonyPatch(typeof(SurveillanceMinigame), nameof(SurveillanceMinigame.Update))]
        public static class CameraTimeLimit {
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

                timeText.text = $"{(int)AdminLimit.AdminTimeLimit.timeLimit}";


                if (!AdminLimit.AdminTimeLimit.AdminWatcher.Contains(PlayerControl.LocalPlayer.PlayerId) &&
                    !PlayerControl.LocalPlayer.Data.IsDead) {
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                        (byte)CustomRPC.StartWatchAdmin, SendOption.Reliable, -1);
                    writer.Write(PlayerControl.LocalPlayer.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    AdminLimit.AdminTimeLimit.AdminWatcher.Add(PlayerControl.LocalPlayer.PlayerId);
                }

                if (AdminLimit.AdminTimeLimit.timeLimit <= 0) {
                    AdminLimit.AdminTimeLimit.timeLimit = 0;
                    __instance.BackgroundColor.SetColor(Palette.DisabledGrey);
                    __instance.OnDisable();
                    return false;
                }

                return true;
            }
        }

        [HarmonyPatch(typeof(VitalsMinigame), nameof(VitalsMinigame.Close))]
        public static class CameraTimeLimitClose {
            public static void Prefix(MapCountOverlay __instance) {
                if (CustomGameOptions.AdminTimeLimitTime == 0) {
                    return;
                }

                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                    (byte)CustomRPC.EndWatchAdmin, SendOption.Reliable, -1);
                writer.Write(PlayerControl.LocalPlayer.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                AdminLimit.AdminTimeLimit.AdminWatcher.Remove(PlayerControl.LocalPlayer.PlayerId);

                AdminLimit.AdminTimeLimit.timeText.Destroy();
                AdminLimit.AdminTimeLimit.timeText = null;
            }
        }
    }
}