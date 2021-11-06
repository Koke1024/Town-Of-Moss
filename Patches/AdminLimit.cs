using HarmonyLib;
using Hazel;
using Il2CppSystem.Collections.Generic;
using Reactor.Extensions;
using TMPro;
using UnityEngine;

namespace TownOfUs.Patches {
    public class AdminLimit {

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
}