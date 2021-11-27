
using System.Collections.Generic;
using Discord;
using HarmonyLib;
using UnityEngine;
using TownOfUs;
using TownOfUs.Extensions;
using TownOfUs.Roles;
using UnityEngine.UI;

namespace TheOtherRoles.Patches {
    [HarmonyPatch(typeof(OptionsMenuBehaviour), nameof(OptionsMenuBehaviour.Open))]
    public static class TabPatch {
        public static void Prefix(OptionsMenuBehaviour __instance) {
            
        }

        public static void Postfix(OptionsMenuBehaviour __instance) {
            AmongUsExtensions.Log($"open");
        }
    }
    
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
        private static List<ToggleButtonBehaviour> buttons = new List<ToggleButtonBehaviour>();

        public static float xOffset = 1.75f;
        public static float yOffset = -0.3f;

        private static void updateToggle(ToggleButtonBehaviour button, string text, bool on) {
            if (button == null || button.gameObject == null) return;

            Color color = on ? new Color(0f, 1f, 0.16470589f, 1f) : Color.white;
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
                updateToggle(button, text, on);
                
                return button;
            }
            return null;
        }

        private static ToggleButtonBehaviour createCustomButton(string text, ToggleButtonBehaviour model, UnityEngine.Events.UnityAction onClick, OptionsMenuBehaviour __instance) {
            if (__instance.CensorChatButton != null) {
                var button = UnityEngine.Object.Instantiate(model);
                PassiveButton passiveButton = button.GetComponent<PassiveButton>();
                passiveButton.OnClick = new Button.ButtonClickedEvent();
                passiveButton.OnClick.AddListener(onClick);
                button.Text.text = text;
                button.Background.color = Color.yellow;
                button.name = text + " Button";
                
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

            if (buttons.Count > 0) {
                return;
            }

            if (GameObject.Find("FullScreenButton") == null || GameObject.Find("VSyncButton") == null) {
                AmongUsExtensions.Log($"No Button");
                return;
            }
            buttons = new List<ToggleButtonBehaviour> {
                GameObject.Find("FullScreenButton").GetComponent<ToggleButtonBehaviour>(),
                GameObject.Find("VSyncButton").GetComponent<ToggleButtonBehaviour>()
            };
            if (origin == null) origin = buttons[0].transform.localPosition + Vector3.up * 0.25f;

            if (streamButton == null || streamButton.gameObject == null) {
                streamButton = createCustomToggle("Hide Room Code: ", Utils.IsStreamMode, buttons[0], (UnityEngine.Events.UnityAction)SetStreamMode, __instance);

                void SetStreamMode() {
                    if (LobbyBehaviour.Instance != null) {
                        Utils.IsStreamMode = !Utils.IsStreamMode;
                        updateToggle(streamButton, "Hide Room Code: ", Utils.IsStreamMode);
                    }
                }
                buttons.Add(streamButton);
            }

            if (settingCheckButton == null || settingCheckButton.gameObject == null) {
                settingCheckButton = createCustomButton("Game Setting Check", buttons[0], (UnityEngine.Events.UnityAction)SettingCheck, __instance);
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
                            __instance.Close();
                            DestroyableSingleton<HudManager>.Instance.ShowPopUp(settingString);
                        }
                    // }
                }
                buttons.Add(settingCheckButton);
            }

            if (roleManualButton == null || roleManualButton.gameObject == null) {
                roleManualButton = createCustomButton("Show Role Manual", buttons[0], (UnityEngine.Events.UnityAction)ShowRoleInfo, __instance);
                // roleManualButton.UpdateText(false);

                void ShowRoleInfo() {
                    if (DestroyableSingleton<HudManager>.Instance) {
                        var role = Role.GetRole(PlayerControl.LocalPlayer);
                        if (role == null) {
                            return;
                        }
                        var settingString = RoleManual.roleManual[role.RoleType];

                        __instance.Close();
                        DestroyableSingleton<HudManager>.Instance.ShowPopUp(settingString);
                    }
                }
                buttons.Add(roleManualButton);
            }

            if (crewOnButton == null || crewOnButton.gameObject == null) {
                crewOnButton = createCustomButton("Show Crewmates Role ", buttons[0], (UnityEngine.Events.UnityAction)ShowCrewRoleInfo, __instance);
                // roleManualButton.UpdateText(false);

                void ShowCrewRoleInfo() {
                    if (DestroyableSingleton<HudManager>.Instance) {
                        var settingString = Utils.crewRateString;
                        __instance.Close();
                        DestroyableSingleton<HudManager>.Instance.ShowPopUp(settingString);
                    }
                }
                buttons.Add(crewOnButton);
            }

            if (ImpostorOnButton == null || ImpostorOnButton.gameObject == null) {
                ImpostorOnButton = createCustomButton("Show Impostor Role ", buttons[0], (UnityEngine.Events.UnityAction)ShowImpRoleInfo, __instance);
                // roleManualButton.UpdateText(false);

                void ShowImpRoleInfo() {
                    if (DestroyableSingleton<HudManager>.Instance) {
                        var settingString = Utils.impRateString;
                        __instance.Close();
                        DestroyableSingleton<HudManager>.Instance.ShowPopUp(settingString);
                    }
                }
                buttons.Add(ImpostorOnButton);
            }

            if (NeutralOnButton == null || NeutralOnButton.gameObject == null) {
                NeutralOnButton = createCustomButton("Show Neutral Role ", buttons[0], (UnityEngine.Events.UnityAction)ShowNeutralRoleInfo, __instance);
                // roleManualButton.UpdateText(false);

                void ShowNeutralRoleInfo() {
                    if (DestroyableSingleton<HudManager>.Instance) {
                        var settingString = Utils.neutralRateString;
                        __instance.Close();
                        DestroyableSingleton<HudManager>.Instance.ShowPopUp(settingString);
                    }
                }
                buttons.Add(NeutralOnButton);
            }

            var index = 0;
            var parent = GameObject.Find("GraphicsTab").transform;
            foreach (var button in buttons) {
                button.transform.SetParent(parent);
                button.transform.localScale = Vector3.one * 1f / 2f;
                button.transform.localPosition = (origin ?? Vector3.zero) + new Vector3((index % 3 - 1) * xOffset, ((int)index / 3) * yOffset);
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
