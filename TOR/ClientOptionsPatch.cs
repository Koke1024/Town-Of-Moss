//   
// using HarmonyLib;
// using UnityEngine;
// using System.Collections.Generic;
// using Hazel;
// using System;
// using Reactor.Extensions;
// using TownOfUs;
// using UnityEngine.UI;
// using UnityEngine.Events;
//
// namespace TheOtherRoles.Patches {
//     [HarmonyPatch(typeof(OptionsMenuBehaviour), nameof(OptionsMenuBehaviour.Start))]
//     public class OptionsMenuBehaviourStartPatch {
//         private static Vector3? origin;
//         public static ToggleButtonBehaviour getHostButton;
//
//         public static float xOffset = 1.75f;
//         public static float yOffset = -0.5f;
//
//         private static void updateToggle(ToggleButtonBehaviour button, string text, bool on) {
//             if (button == null || button.gameObject == null) return;
//
//             Color color = on ? new Color(0f, 1f, 0.16470589f, 1f) : Color.white;
//             button.Background.color = color;
//             button.Text.text = $"{text}{(on ? "On" : "Off")}";
//             if (button.Rollover) button.Rollover.ChangeOutColor(color);
//         }
//
//         private static ToggleButtonBehaviour createCustomToggle(string text, bool on, Vector3 offset, UnityEngine.Events.UnityAction onClick, OptionsMenuBehaviour __instance) {
//             if (__instance.CensorChatButton != null) {
//                 var button = UnityEngine.Object.Instantiate(__instance.CensorChatButton, __instance.CensorChatButton.transform.parent);
//                 button.transform.localPosition = (origin ?? Vector3.zero) + offset;
//                 PassiveButton passiveButton = button.GetComponent<PassiveButton>();
//                 passiveButton.OnClick = new Button.ButtonClickedEvent();
//                 passiveButton.OnClick.AddListener(onClick);
//                 updateToggle(button, text, on);
//                 
//                 return button;
//             }
//             return null;
//         }
//
//         public static void Postfix(OptionsMenuBehaviour __instance) {
//             if (__instance.CensorChatButton != null) {
//                 if (origin == null) origin = __instance.CensorChatButton.transform.localPosition + Vector3.up * 0.25f;
//                 __instance.CensorChatButton.transform.localPosition = origin.Value + Vector3.left * xOffset;
//                 __instance.CensorChatButton.transform.localScale = Vector3.one * 2f / 3f;
//             }
//
//             if ((getHostButton == null || getHostButton.gameObject == null)) {
//                 getHostButton = createCustomToggle("Room Host: ", AmongUsClient.Instance.AmHost, Vector3.zero, (UnityEngine.Events.UnityAction)beHost, __instance);
//
//                 void beHost() {
//                     if (!AmongUsClient.Instance.AmHost && GameStartManager.Instance != null) {
//                         var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
//                             (byte) CustomRPC.PassHost, SendOption.Reliable, -1);
//                         writer.Write(AmongUsClient.Instance.ClientId);
//                         AmongUsClient.Instance.FinishRpcImmediately(writer);
//                         AmongUsClient.Instance.HostId = AmongUsClient.Instance.ClientId;
//                         
//                         PlayerControl.LocalPlayer.RpcSyncSettings(PlayerControl.GameOptions);
//                     }
//                 }
//             }
//         }
//     }
//
//     [HarmonyPatch(typeof(TextBoxTMP), nameof(TextBoxTMP.SetText))]
// 	public static class HiddenTextPatch
// 	{
// 		private static void Postfix(TextBoxTMP __instance)
// 		{
// 			bool flag = true && (__instance.name == "GameIdText" || __instance.name == "IpTextBox" || __instance.name == "PortTextBox");
// 			if (flag) __instance.outputText.text = new string('*', __instance.text.Length);
// 		}
// 	}
//
//     [HarmonyPatch(typeof(OptionsMenuBehaviour), nameof(OptionsMenuBehaviour.Update))]
//     public class ButtonUpdate {
//         
//         private static void updateToggle(ToggleButtonBehaviour button, string text, bool on) {
//             if (button == null || button.gameObject == null) return;
//
//             Color color = on ? new Color(0f, 1f, 0.16470589f, 1f) : Color.white;
//             button.Background.color = color;
//             button.Text.text = $"{text}{(on ? "On" : "Off")}";
//             if (button.Rollover) button.Rollover.ChangeOutColor(color);
//         }
//
//         static void Prefix() {
//             updateToggle(OptionsMenuBehaviourStartPatch.getHostButton, "Room Host: ", AmongUsClient.Instance.AmHost);
//             if (GameStartManager.Instance != null && GameStartManager.Instance.StartButton != null) {
//                 GameStartManager.Instance.StartButton.gameObject.SetActive(AmongUsClient.Instance.AmHost);                
//             }
//         }
//     }
// }
