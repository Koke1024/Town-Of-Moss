using System.Collections;
using System.Linq;
using HarmonyLib;
using Reactor;
using TownOfUs.Extensions;
using UnityEngine;
using UnhollowerBaseLib;

namespace TownOfUs.Patches {
    public class GameStartManagerPatch {
        private static string lobbyCodeText = "";
        public static GameObject StartPanel;
        
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
            public static bool startable = false;
            public static void Prefix(GameStartManager __instance) {
                if (StartPanel == null) {
                    return;
                }

                startable = true;
                var color = Color.white;
                foreach (var player in PlayerControl.AllPlayerControls) {
                    if (!player.Collider.IsTouching(StartPanel.GetComponent<PolygonCollider2D>())) {
                        startable = false;
                        if (player == PlayerControl.LocalPlayer) {
                            color = Color.gray;
                        }
                    }
                }

                if (AmongUsClient.Instance.AmHost) {
                    if (startable) {
                        __instance.StartButton.color = new Color(1, 1, 1, 1);
                    }
                    else {
                        __instance.StartButton.color = new Color(0.5f, 0.5f, 0.5f, 0.4f);
                    }
                }
                StartPanel.GetComponent<SpriteRenderer>().color = startable? Color.green: color;
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

        // [HarmonyPatch(typeof(OptionsConsole), nameof(OptionsConsole.Use))]
        // public static class OptionMenu {
        //     public static void Postfix(OptionsConsole __instance) {
        //         var menu = Camera.main.transform.GetComponentInChildren<PlayerCustomizationMenu>();
        //     }
        // }
        
        [HarmonyPatch(typeof(GameStartManager), nameof(GameStartManager.BeginGame))]
        public static class BeginGameBefore {
            public static bool Prefix(GameStartManager __instance) {
                if (!GameStartManagerUpdatePatch.startable) {
                    return false;
                }
                
                if (__instance.startState != GameStartManager.StartingStates.NotStarting)
                {
                    return false;
                }
                if (SaveManager.ShowMinPlayerWarning && GameData.Instance.PlayerCount == __instance.MinPlayers)
                {
                    __instance.GameSizePopup.SetActive(true);
                    return false;
                }
                if (GameData.Instance.PlayerCount < __instance.MinPlayers)
                {
                    __instance.StartCoroutine(Effects.SwayX(__instance.PlayerCounter.transform, 0.75f, 0.25f));
                    return false;
                }
                __instance.ReallyBegin(false);
                
                return true;
            }
        }
        

        [HarmonyPatch(typeof(LobbyBehaviour), nameof(LobbyBehaviour.Start))]
        public static class RearrangeLobby {
            public static void Postfix(LobbyBehaviour __instance) {
                Camera main = Camera.main;
                FollowerCamera component = main.GetComponent<FollowerCamera>();
                if (component)
                {
                    component.shakeAmount = 0f;
                    component.shakePeriod = 0;
                }
                
                var box = __instance.GetComponentsInChildren<PolygonCollider2D>().FirstOrDefault(x => x.name == "RightBox");
                if (box) {
                    box.gameObject.transform.position = new Vector3(1.77f, 0.86f, Utils.getZfromY(0.86f));                    
                }
                box = __instance.GetComponentsInChildren<PolygonCollider2D>().FirstOrDefault(x => x.name == "Leftbox");
                if (box) {
                    box.gameObject.transform.position = new Vector3(-1.77f, 0.34f, Utils.getZfromY(0.34f));                    
                }

                StartPanel = new GameObject("StartPanel");
                Vector3 position = new Vector3(0.18f, 0.58f, 7); // just behind player
                StartPanel.transform.position = position;
                StartPanel.transform.localScale = new Vector3(3, 3, 1);
                var collider = StartPanel.AddComponent<PolygonCollider2D>();
                collider.isTrigger = true;
                collider.points = new Il2CppStructArray<Vector2>(3) {
                    [0] = new Vector2(-1, 0.5f),
                    [1] = new Vector2(1, 0),
                    [2] = new Vector2(-1, -0.5f)
                };

                var panelRenderer = StartPanel.AddComponent<SpriteRenderer>();
                panelRenderer.sprite = GameStartManager.Instance.StartButton.sprite;
                // panelRenderer.sprite = Object.Instantiate(GameStartManager.Instance.StartButton);
                StartPanel.gameObject.transform.SetParent(__instance.transform);
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
