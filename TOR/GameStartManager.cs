using System.Collections;
using HarmonyLib;
using Reactor;
using Rewired;
using TownOfUs;
using TownOfUs.Extensions;
using UnityEngine;

namespace TownOfUs.Patches {
    public class GameStartManagerPatch {
        private static string lobbyCodeText = "";
        
        [HarmonyPatch(typeof(GameStartManager), nameof(GameStartManager.Start))]
        public class GameStartManagerStartPatch {
            public static void Postfix(GameStartManager __instance) {
                if (__instance && __instance.GameRoomName != null) {
                    lobbyCodeText = __instance.GameRoomName.text;                    
                }
            }
        }

        [HarmonyPatch(typeof(GameStartManager), nameof(GameStartManager.Update))]
        public class GameStartManagerUpdatePatch {
            public static void Prefix(GameStartManager __instance) {
            }

            public static void Postfix(GameStartManager __instance) {
                if (__instance && __instance.GameRoomName != null) {
                    if (Utils.IsStreamMode) {
                        __instance.GameRoomName.text = "******";
                    }
                    else {
                        __instance.GameRoomName.text = lobbyCodeText;
                    }
                }
            }
        }
    }
    
    [HarmonyPatch(typeof(GameStartManager), nameof(GameStartManager.FinallyBegin))]
    public static class HostHandicap2 {
        public static void Postfix(GameStartManager __instance) {
            if (__instance.startState == GameStartManager.StartingStates.Countdown)
            {
                return;
            }
            Coroutines.Start(StartingWait());
        }
    
        public static IEnumerator StartingWait() {
            PlayerControl.LocalPlayer.moveable = false;
            yield return new WaitForSeconds(1.0f);
            if (AmongUsClient.Instance.AmHost) {
                yield return new WaitForSeconds(2.0f);
            }
            PlayerControl.LocalPlayer.moveable = true;
        }
    }
}