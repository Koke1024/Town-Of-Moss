using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;
using TownOfUs;
using TownOfUs.Extensions;
using TownOfUs.Roles;
using UnityEngine.UI;

namespace TheOtherRoles.Patches {
    
    [HarmonyPatch(typeof(OptionsMenuBehaviour), nameof(OptionsMenuBehaviour.Update))]
    public class OptionsMenuBehaviourStartPatch {
        private static Vector3? origin;
        public static ToggleButtonBehaviour getHostButton;
        public static ToggleButtonBehaviour streamButton;
        public static ToggleButtonBehaviour settingCheckButton;
        public static ToggleButtonBehaviour roleManualButton;
        public static ToggleButtonBehaviour crewOnButton;
        public static ToggleButtonBehaviour ImpostorOnButton;
        public static ToggleButtonBehaviour NeutralOnButton;
        public static List<ToggleButtonBehaviour> buttons = null;

        public static float xOffset = 1.75f;
        public static float yOffset = -0.3f;

        private static void updateToggle(ToggleButtonBehaviour button, string text, bool on, Color bgColor) {
            if (button == null || button.gameObject == null) return;

            Color color = on ? new Color(0f, 1f, 0.16470589f, 1f) : bgColor;
            button.Background.color = color;
            button.Text.text = $"{text}{(on ? "On" : "Off")}";
            if (button.Rollover) button.Rollover.ChangeOutColor(color);
        }

        private static ToggleButtonBehaviour createCustomToggle(string text, bool on, ToggleButtonBehaviour model, UnityEngine.Events.UnityAction onClick, OptionsMenuBehaviour __instance) {
            if (__instance.CensorChatButton != null) {
                var button = UnityEngine.Object.Instantiate(model);
                PassiveButton passiveButton = button.GetComponent<PassiveButton>();
                passiveButton.OnClick = new Button.ButtonClickedEvent();
                passiveButton.OnClick.AddListener(onClick);
                updateToggle(button, text, on, Color.white);
                
                return button;
            }
            return null;
        }

        private static ToggleButtonBehaviour createCustomButton(string text, ToggleButtonBehaviour model, UnityEngine.Events.UnityAction onClick, OptionsMenuBehaviour __instance, Color bgColor) {
            if (__instance.CensorChatButton != null) {
                var button = UnityEngine.Object.Instantiate(model);
                PassiveButton passiveButton = button.GetComponent<PassiveButton>();
                passiveButton.OnClick = new Button.ButtonClickedEvent();
                passiveButton.OnClick.AddListener(onClick);
                button.Text.text = text;
                button.Background.color = bgColor;
                button.name = text + " Button";
                button.Rollover.OverColor = bgColor;
                button.Rollover.OutColor = bgColor;
                
                return button;
            }
            return null;
        }

        public static void Prefix(OptionsMenuBehaviour __instance) {
            // var customTab = GameObject.Instantiate(__instance.Tabs[0]);
            // customTab.transform.SetParent(__instance.Tabs[0].transform.parent);
            // AmongUsExtensions.Log($"length {__instance.Tabs.Length}");
            // __instance.Tabs.AddItem(customTab);
            // AmongUsExtensions.Log($">{__instance.Tabs.Length}");
            // __instance.Tabs[0].transform.localPosition.Set(-0.7f, 2.4f, -1);
            // __instance.Tabs[1].transform.localPosition.Set(0, 2.4f, -1);
            // __instance.Tabs[2].transform.localPosition.Set(0.7f, 2.4f, -1);
        }

        public static void Postfix(OptionsMenuBehaviour __instance) {

            if (buttons != null) {
                AmongUsExtensions.Log($"button exists");
                return;
            }

            if (GameObject.Find("FullScreenButton") == null || GameObject.Find("VSyncButton") == null) {
                AmongUsExtensions.Log($"null");
                return;
            }

            var transform1 = DestroyableSingleton<HudManager>.Instance.Dialogue.transform;
            var dialogPosition = transform1.position;
            transform1.position =
                new Vector3(dialogPosition.x, dialogPosition.y, __instance.transform.position.z - 10);

            var fullScreenButton = GameObject.Find("FullScreenButton").GetComponent<ToggleButtonBehaviour>();

            if (streamButton == null || streamButton.gameObject == null) {
                streamButton = createCustomToggle("Hide Room Code: ", Utils.IsStreamMode, fullScreenButton, (UnityEngine.Events.UnityAction)SetStreamMode, __instance);

                void SetStreamMode() {
                    if (LobbyBehaviour.Instance != null) {
                        Utils.IsStreamMode = !Utils.IsStreamMode;
                        updateToggle(streamButton, "Hide Room Code: ", Utils.IsStreamMode, Color.white);
                    }
                }
            }

            if (settingCheckButton == null || settingCheckButton.gameObject == null) {
                settingCheckButton = createCustomButton("Game Settings", fullScreenButton, (UnityEngine.Events.UnityAction)SettingCheck, __instance, Color.white);
                // settingCheckButton.UpdateText(false);

                void SettingCheck() {
                    // if (MeetingHud.Instance != null) {
                        if (DestroyableSingleton<HudManager>.Instance) {
                            var settingString = $"<color=#FF0000FF>Impostor: {PlayerControl.GameOptions.NumImpostors}</color>　<color=#FF0000FF>Madmate: {(CustomGameOptions.MadMateOn? "On": "Off")}</color>\n";
                            settingString += $"<color=#FF00FFFF>Neutral Roles: {CustomGameOptions.MaxNeutralRoles}</color>　<color=#00FF00FF>Glitch: {(CustomGameOptions.GlitchOn? "On": "Off")}</color>\n";
                            settingString += $"Kill Cooldown: {PlayerControl.GameOptions.KillCooldown}s\n";
                            settingString += $"Kill Cooldown Reset On Meeting: {((CustomGameOptions.KillCoolResetOnMeeting)? "On": "Off")}\n";
                            settingString += $"Last Impostor Can Snipe: {(CustomGameOptions.LastImpCanGuess? "On": "Off")}\n";
                            if (ShipStatus.Instance == null || ShipStatus.Instance.Type == ShipStatus.MapType.Pb) {
                                settingString += $"Polus Vital Position: {(new[] {"Default", "Labo", "Ship", "O2"}[(int)CustomGameOptions.PolusVitalMove])}";
                            }

                            // DestroyableSingleton<HudManager>.Instance.Chat.AddChat(PlayerControl.LocalPlayer, settingString);
                            DestroyableSingleton<HudManager>.Instance.ShowPopUp(settingString);
                        }
                    // }
                }
            }

            if (roleManualButton == null || roleManualButton.gameObject == null) {
                roleManualButton = createCustomButton("About My Role", fullScreenButton, (UnityEngine.Events.UnityAction)ShowRoleInfo, __instance, Color.white);
                // roleManualButton.UpdateText(false);

                void ShowRoleInfo() {
                    if (DestroyableSingleton<HudManager>.Instance) {
                        var role = Role.GetRole(PlayerControl.LocalPlayer);
                        if (role == null) {
                            return;
                        }
                        var settingString = RoleManual.roleManual[role.RoleType];

                        DestroyableSingleton<HudManager>.Instance.ShowPopUp(settingString);
                    }
                }
            }

            if (crewOnButton == null || crewOnButton.gameObject == null) {
                crewOnButton = createCustomButton("Crewmates Assign", fullScreenButton, (UnityEngine.Events.UnityAction)ShowCrewRoleInfo, __instance, Color.green);
                // roleManualButton.UpdateText(false);

                void ShowCrewRoleInfo() {
                    if (DestroyableSingleton<HudManager>.Instance) {
                        var settingString = Utils.crewRateString;
                        DestroyableSingleton<HudManager>.Instance.ShowPopUp(settingString);
                    }
                }
            }

            if (ImpostorOnButton == null || ImpostorOnButton.gameObject == null) {
                ImpostorOnButton = createCustomButton("Impostor Assign", fullScreenButton, (UnityEngine.Events.UnityAction)ShowImpRoleInfo, __instance, Color.red);
                // roleManualButton.UpdateText(false);

                void ShowImpRoleInfo() {
                    if (DestroyableSingleton<HudManager>.Instance) {
                        var settingString = Utils.impRateString;
                        DestroyableSingleton<HudManager>.Instance.ShowPopUp(settingString);
                    }
                }
            }

            if (NeutralOnButton == null || NeutralOnButton.gameObject == null) {
                NeutralOnButton = createCustomButton("Neutral Assign", fullScreenButton, (UnityEngine.Events.UnityAction)ShowNeutralRoleInfo, __instance, Color.magenta);
                // roleManualButton.UpdateText(false);

                void ShowNeutralRoleInfo() {
                    if (DestroyableSingleton<HudManager>.Instance) {
                        var settingString = Utils.neutralRateString;
                        DestroyableSingleton<HudManager>.Instance.ShowPopUp(settingString);
                    }
                }
            }
            
            buttons = new List<ToggleButtonBehaviour> {
                fullScreenButton,
                GameObject.Find("VSyncButton").GetComponent<ToggleButtonBehaviour>(),
                streamButton,
                crewOnButton,
                ImpostorOnButton,
                NeutralOnButton,
                settingCheckButton,
                roleManualButton,
            };
            
            if (origin == null) origin = fullScreenButton.transform.localPosition + Vector3.up * 0.25f;
            var index = 0;
            var parent = GameObject.Find("GraphicsTab").transform;
            foreach (var button in buttons) {
                Transform transform;
                (transform = button.transform).SetParent(parent);
                transform.localScale = Vector3.one * 1f / 2f;
                transform.localPosition = (origin ?? Vector3.zero) + new Vector3((index % 3 - 1) * xOffset, ((int)index / 3) * yOffset);
                ++index;
            }

            // if ((getHostButton == null || getHostButton.gameObject == null)) {
            //     getHostButton = createCustomToggle("Room Host: ", AmongUsClient.Instance.AmHost, new Vector2(xOffset, yOffset), (UnityEngine.Events.UnityAction)beHost, __instance);
            //
            //     void beHost() {
            //         if (!AmongUsClient.Instance.AmHost && GameStartManager.Instance != null) {
            //             var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
            //                 (byte) CustomRPC.PassHost, SendOption.Reliable, -1);
            //             writer.Write(AmongUsClient.Instance.ClientId);
            //             AmongUsClient.Instance.FinishRpcImmediately(writer);
            //             AmongUsClient.Instance.HostId = AmongUsClient.Instance.ClientId;
            //             
            //             PlayerControl.LocalPlayer.RpcSyncSettings(PlayerControl.GameOptions);
            //         }
            //     }
            // }
        }
    }

    [HarmonyPatch(typeof(TextBoxTMP), nameof(TextBoxTMP.SetText))]
	public static class HiddenTextPatch
	{
		private static void Postfix(TextBoxTMP __instance)
		{
			bool flag = true && (__instance.name == "GameIdText" || __instance.name == "IpTextBox" || __instance.name == "PortTextBox");
			if (flag) __instance.outputText.text = new string('*', __instance.text.Length);
		}
	}

    [HarmonyPatch(typeof(OptionsMenuBehaviour), nameof(OptionsMenuBehaviour.Open))]
    public static class OnDisable {
        public static void Postfix(OptionsMenuBehaviour __instance) {
            OptionsMenuBehaviourStartPatch.buttons = null;
        }
    }

    // [HarmonyPatch(typeof(OptionsMenuBehaviour), nameof(OptionsMenuBehaviour.Update))]
    // public class ButtonUpdate {
    //     
    //     private static void updateToggle(ToggleButtonBehaviour button, string text, bool on) {
    //         if (button == null || button.gameObject == null) return;
    //
    //         Color color = on ? new Color(0f, 1f, 0.16470589f, 1f) : Color.white;
    //         button.Background.color = color;
    //         button.Text.text = $"{text}{(on ? "On" : "Off")}";
    //         if (button.Rollover) button.Rollover.ChangeOutColor(color);
    //     }
    //
    //     static void Prefix() {
    //         updateToggle(OptionsMenuBehaviourStartPatch.getHostButton, "Room Host: ", AmongUsClient.Instance.AmHost);
    //         if (GameStartManager.Instance != null && GameStartManager.Instance.StartButton != null) {
    //             GameStartManager.Instance.StartButton.gameObject.SetActive(AmongUsClient.Instance.AmHost);                
    //         }
    //     }
    // }
}
