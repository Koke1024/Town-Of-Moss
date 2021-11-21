
using Discord;
using HarmonyLib;
using UnityEngine;
using TownOfUs;
using TownOfUs.Extensions;
using TownOfUs.Roles;
using UnityEngine.UI;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

namespace TheOtherRoles.Patches {
    [HarmonyPatch(typeof(OptionsMenuBehaviour), nameof(OptionsMenuBehaviour.Start))]
    public class OptionsMenuBehaviourStartPatch {
        private static Vector3? origin;
        public static ToggleButtonBehaviour getHostButton;
        public static ToggleButtonBehaviour streamButton;
        public static ToggleButtonBehaviour settingCheckButton;
        public static ToggleButtonBehaviour roleManualButton;
        public static ToggleButtonBehaviour crewOnButton;
        public static ToggleButtonBehaviour ImpostorOnButton;
        public static ToggleButtonBehaviour NeutralOnButton;

        public static float xOffset = 1.75f;
        public static float yOffset = -0.3f;

        private static void updateToggle(ToggleButtonBehaviour button, string text, bool on) {
            if (button == null || button.gameObject == null) return;

            Color color = on ? new Color(0f, 1f, 0.16470589f, 1f) : Color.white;
            button.Background.color = color;
            button.Text.text = $"{text}{(on ? "On" : "Off")}";
            if (button.Rollover) button.Rollover.ChangeOutColor(color);
        }

        private static ToggleButtonBehaviour createCustomToggle(string text, bool on, Vector3 offset, UnityEngine.Events.UnityAction onClick, OptionsMenuBehaviour __instance) {
            if (__instance.CensorChatButton != null) {
                var button = UnityEngine.Object.Instantiate(__instance.CensorChatButton, __instance.CensorChatButton.transform.parent);
                button.transform.localPosition = (origin ?? Vector3.zero) + offset;
                PassiveButton passiveButton = button.GetComponent<PassiveButton>();
                passiveButton.OnClick = new Button.ButtonClickedEvent();
                passiveButton.OnClick.AddListener(onClick);
                updateToggle(button, text, on);
                
                return button;
            }
            return null;
        }

        private static ToggleButtonBehaviour createCustomButton(string text, Vector3 offset, UnityEngine.Events.UnityAction onClick, OptionsMenuBehaviour __instance) {
            if (__instance.CensorChatButton != null) {
                var button = UnityEngine.Object.Instantiate(__instance.CensorChatButton, __instance.CensorChatButton.transform.parent);
                button.transform.localPosition = (origin ?? Vector3.zero) + offset;
                PassiveButton passiveButton = button.GetComponent<PassiveButton>();
                passiveButton.OnClick = new Button.ButtonClickedEvent();
                passiveButton.OnClick.AddListener(onClick);
                button.Text.text = text;
                button.Background.color = Color.yellow;
                
                return button;
            }
            return null;
        }

        public static void Postfix(OptionsMenuBehaviour __instance) {
            if (__instance.CensorChatButton != null) {
                if (origin == null) origin = __instance.CensorChatButton.transform.localPosition + Vector3.up * 0.25f;
                __instance.CensorChatButton.transform.localPosition = origin.Value + Vector3.left * xOffset;
                __instance.CensorChatButton.transform.localScale = Vector3.one * 2f / 3f;
            }

            if (streamButton == null || streamButton.gameObject == null) {
                streamButton = createCustomToggle("Hide Room Code: ", Utils.IsStreamMode, Vector3.zero, (UnityEngine.Events.UnityAction)SetStreamMode, __instance);

                void SetStreamMode() {
                    if (LobbyBehaviour.Instance != null) {
                        Utils.IsStreamMode = !Utils.IsStreamMode;
                        updateToggle(streamButton, "Hide Room Code: ", Utils.IsStreamMode);
                    }
                }
            }

            if (settingCheckButton == null || settingCheckButton.gameObject == null) {
                settingCheckButton = createCustomButton("Game Setting Check", Vector3.right * xOffset, (UnityEngine.Events.UnityAction)SettingCheck, __instance);
                // settingCheckButton.UpdateText(false);

                void SettingCheck() {
                    // if (MeetingHud.Instance != null) {
                        if (DestroyableSingleton<HudManager>.Instance) {
                            var settingString = $"<color=#FF0000FF>Impostor: {PlayerControl.GameOptions.NumImpostors}</color>　<color=#FF0000FF>Madmate: {(CustomGameOptions.MadMateOn? "On": "Off")}</color>\n";
                            settingString += $"<color=#00FF00FF>Neutral Roles: {CustomGameOptions.MaxNeutralRoles}</color>　<color=#00FF00FF>Glitch: {(CustomGameOptions.GlitchOn? "On": "Off")}</color>\n";
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
            }

            if (roleManualButton == null || roleManualButton.gameObject == null) {
                roleManualButton = createCustomButton("Show Role Manual", new Vector2(-xOffset, yOffset), (UnityEngine.Events.UnityAction)ShowRoleInfo, __instance);
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
            }

            if (crewOnButton == null || crewOnButton.gameObject == null) {
                crewOnButton = createCustomButton("Show Crewmates Role ", new Vector2(-xOffset, yOffset * 2), (UnityEngine.Events.UnityAction)ShowCrewRoleInfo, __instance);
                // roleManualButton.UpdateText(false);

                void ShowCrewRoleInfo() {
                    if (DestroyableSingleton<HudManager>.Instance) {
                        var settingString = Utils.crewRateString;
                        __instance.Close();
                        DestroyableSingleton<HudManager>.Instance.ShowPopUp(settingString);
                    }
                }
            }

            if (ImpostorOnButton == null || ImpostorOnButton.gameObject == null) {
                ImpostorOnButton = createCustomButton("Show Impostor Role ", new Vector2(0, yOffset * 2), (UnityEngine.Events.UnityAction)ShowImpRoleInfo, __instance);
                // roleManualButton.UpdateText(false);

                void ShowImpRoleInfo() {
                    if (DestroyableSingleton<HudManager>.Instance) {
                        var settingString = Utils.impRateString;
                        __instance.Close();
                        DestroyableSingleton<HudManager>.Instance.ShowPopUp(settingString);
                    }
                }
            }

            if (NeutralOnButton == null || NeutralOnButton.gameObject == null) {
                NeutralOnButton = createCustomButton("Show Neutral Role ", new Vector2(xOffset, yOffset * 2), (UnityEngine.Events.UnityAction)ShowNeutralRoleInfo, __instance);
                // roleManualButton.UpdateText(false);

                void ShowNeutralRoleInfo() {
                    if (DestroyableSingleton<HudManager>.Instance) {
                        var settingString = Utils.neutralRateString;
                        __instance.Close();
                        DestroyableSingleton<HudManager>.Instance.ShowPopUp(settingString);
                    }
                }
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
