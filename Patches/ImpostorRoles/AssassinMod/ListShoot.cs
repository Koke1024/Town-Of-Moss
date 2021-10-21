using System;
using System.Collections.Generic;
using System.Linq;
using Hazel;
using Il2CppSystem.Web.Util;
using Reactor.Extensions;
using TownOfUs.CrewmateRoles.SwapperMod;
using TownOfUs.Extensions;
using TownOfUs.ImpostorRoles.AssassinMod;
using TownOfUs.Roles;
using UnityEngine;
using UnityEngine.UI;

namespace TownOfUs.Patches.ImpostorRoles.AssassinMod {
    public class ListShoot {
        

        private static GameObject guesserUI;
        public static void guesserOnClick(int buttonTarget, PlayerVoteArea voteArea) {
            MeetingHud __instance = MeetingHud.Instance;
            Assassin role = Role.GetRole<Assassin>(PlayerControl.LocalPlayer);
            if (guesserUI != null || !(__instance.state == MeetingHud.VoteStates.Voted || __instance.state == MeetingHud.VoteStates.NotVoted)) return;
            __instance.playerStates.ToList().ForEach(x => x.gameObject.SetActive(false));

            Transform container = UnityEngine.Object.Instantiate(__instance.transform.FindChild("Background"), __instance.transform);
            container.FindChild("BlackBG").gameObject.SetActive(false);
            container.transform.localPosition = new Vector3(0, 0, -5f);
            guesserUI = container.gameObject;

            int i = 0;
            var buttonTemplate = __instance.playerStates[0].transform.FindChild("votePlayerBase");
            var maskTemplate = __instance.playerStates[0].transform.FindChild("MaskArea");
            var smallButtonTemplate = __instance.playerStates[0].Buttons.transform.Find("CancelButton");
            var textTemplate = __instance.playerStates[0].NameText;

            Transform exitButtonParent = (new GameObject()).transform;
            exitButtonParent.SetParent(container);
            Transform exitButton = UnityEngine.Object.Instantiate(buttonTemplate.transform, exitButtonParent);
            Transform exitButtonMask = UnityEngine.Object.Instantiate(maskTemplate, exitButtonParent);
            exitButton.gameObject.GetComponent<SpriteRenderer>().sprite = smallButtonTemplate.GetComponent<SpriteRenderer>().sprite;
            var transform = exitButtonParent.transform;
            transform.localPosition = new Vector3(2.725f, 2.1f, -5);
            transform.localScale = new Vector3(0.25f, 0.9f, 1);
            PassiveButtonManager.Instance.RemoveOne(exitButton.GetComponent<PassiveButton>());
            exitButton.GetComponent<PassiveButton>().OnClick = new Button.ButtonClickedEvent();
            exitButton.GetComponent<PassiveButton>().OnClick.AddListener((Action)(() => {
                __instance.playerStates.ToList().ForEach(x => x.gameObject.SetActive(true));
                UnityEngine.Object.Destroy(container.gameObject);
            }));

            List<Transform> buttons = new List<Transform>();
            Transform selectedButton = null;

            PlayerControl target = Utils.PlayerById((byte)__instance.playerStates[buttonTarget].TargetPlayerId);
            var targetRole = Role.GetRole(target).Name;
            AmongUsExtensions.Log($"{targetRole}");
            
            foreach (var pair in role.ColorMapping) {
                Transform buttonParent = (new GameObject()).transform;
                buttonParent.SetParent(container);
                Transform button = UnityEngine.Object.Instantiate(buttonTemplate, buttonParent);
                Transform buttonMask = UnityEngine.Object.Instantiate(maskTemplate, buttonParent);
                TMPro.TextMeshPro label = UnityEngine.Object.Instantiate(textTemplate, button);
                buttons.Add(button);
                int row = i/4, col = i%4;
                buttonParent.localPosition = new Vector3(-2.725f + 1.83f * col, 1.5f - 0.45f * row, -5);
                buttonParent.localScale = new Vector3(0.55f, 0.55f, 1f);
                label.text = $"<color=#{pair.Value.ToHtmlStringRGBA()}>{pair.Key}</color>";
                label.alignment = TMPro.TextAlignmentOptions.Center;
                label.transform.localPosition = new Vector3(0, 0, label.transform.localPosition.z);
                label.transform.localScale *= 1.7f;

                button.GetComponent<PassiveButton>().OnClick = new Button.ButtonClickedEvent();
                button.GetComponent<PassiveButton>().OnClick.AddListener((Action)(() => {
                    if (selectedButton != button) {
                        selectedButton = button;
                        buttons.ForEach(x => x.GetComponent<SpriteRenderer>().color = x == selectedButton ? Color.red : Color.white);
                    } else {
                        if (!(__instance.state == MeetingHud.VoteStates.Voted || __instance.state == MeetingHud.VoteStates.NotVoted) || target == null || role.RemainingKills <= 0 ) return;

                        ShootExecute.Guess(role, voteArea, pair.Key);

                        __instance.playerStates.ToList().ForEach(x => x.gameObject.SetActive(true));
                        UnityEngine.Object.Destroy(container.gameObject);
                        __instance.playerStates.ToList().ForEach(x => { if (x.transform.FindChild("ShootButton") != null) UnityEngine.Object.Destroy(x.transform.FindChild("ShootButton").gameObject); });
                    }
                }));

                i++;
            }
            container.transform.localScale *= 0.75f;
        }
    }
}