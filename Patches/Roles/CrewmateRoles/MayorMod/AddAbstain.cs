// using HarmonyLib;
// using TMPro;
// using TownOfUs.Roles;
// using UnityEngine;
//
// namespace TownOfUs.CrewmateRoles.MayorMod
// {
//     public class AddAbstain
//     {
//         private static Sprite Abstain => TownOfUs.Abstain;
//         // private static Sprite Extend => TownOfUs.Extend;
//
//         public static void UpdateButton(Mayor role, MeetingHud __instance)
//         {
//             var skip = __instance.SkipVoteButton;
//             if (role.Abstain == null) {
//                 role.Abstain = Object.Instantiate(skip, skip.transform.parent);
//                 role.Abstain.Parent = __instance;
//                 role.Abstain.SetTargetPlayerId(251);
//                 role.Abstain.transform.localPosition = skip.transform.localPosition +
//                                                        new Vector3(0f, -0.17f, 0f);
//                 skip.transform.localPosition += new Vector3(0f, 0.20f, 0f);
//             }
//             role.Abstain.gameObject.SetActive(skip.gameObject.active && !role.VotedOnce);
//             role.Abstain.voteComplete = skip.voteComplete;
//             role.Abstain.GetComponent<SpriteRenderer>().enabled = skip.GetComponent<SpriteRenderer>().enabled;
//             role.Abstain.skipVoteText.text = "Abstain";
//             
//             // role.Extend.gameObject.SetActive(skip.gameObject.active && !role.VotedOnce);
//             // role.Extend.voteComplete = skip.voteComplete;
//             // role.Extend.GetComponent<SpriteRenderer>().enabled = skip.GetComponent<SpriteRenderer>().enabled;
//             // role.Extend.skipVoteText.text = "Extend";
//         }
//
//
//         [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
//         public class MeetingHudStart
//         {
//             public static void Postfix(MeetingHud __instance)
//             {
//                 if (!PlayerControl.LocalPlayer.Is(RoleEnum.Mayor)) return;
//                 var mayorRole = Role.GetRole<Mayor>(PlayerControl.LocalPlayer);
//                 GenButton(mayorRole, __instance);
//             }
//             
//             public static void GenButton(Mayor role, MeetingHud __instance)
//             {
//                 UpdateButton(role, __instance);
//             }
//         }
//
//         [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.ClearVote))]
//         public class MeetingHudClearVote
//         {
//             public static void Postfix(MeetingHud __instance)
//             {
//                 if (!PlayerControl.LocalPlayer.Is(RoleEnum.Mayor)) return;
//                 var mayorRole = Role.GetRole<Mayor>(PlayerControl.LocalPlayer);
//                 UpdateButton(mayorRole, __instance);
//             }
//         }
//
//         [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Confirm))]
//         public class MeetingHudConfirm
//         {
//             public static void Postfix(MeetingHud __instance)
//             {
//                 if (!PlayerControl.LocalPlayer.Is(RoleEnum.Mayor)) return;
//                 var mayorRole = Role.GetRole<Mayor>(PlayerControl.LocalPlayer);
//                 mayorRole.Abstain.ClearButtons();
//                 // mayorRole.Extend.ClearButtons();
//                 UpdateButton(mayorRole, __instance);
//             }
//         }
//
//         [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Select))]
//         public class MeetingHudSelect
//         {
//             public static void Postfix(MeetingHud __instance, int __0)
//             {
//                 if (!PlayerControl.LocalPlayer.Is(RoleEnum.Mayor)) return;
//                 var mayorRole = Role.GetRole<Mayor>(PlayerControl.LocalPlayer);
//                 if (__0 != 251) {
//                     mayorRole.Abstain.ClearButtons();
//                     // mayorRole.Extend.ClearButtons();
//                 }
//
//                 UpdateButton(mayorRole, __instance);
//             }
//         }
//
//         [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.VotingComplete))]
//         public class MeetingHudVotingComplete
//         {
//             public static void Postfix(MeetingHud __instance)
//             {
//                 if (!PlayerControl.LocalPlayer.Is(RoleEnum.Mayor)) return;
//                 var mayorRole = Role.GetRole<Mayor>(PlayerControl.LocalPlayer);
//                 UpdateButton(mayorRole, __instance);
//             }
//         }
//
//         [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Update))]
//         public class MeetingHudUpdate
//         {
//             public static void Postfix(MeetingHud __instance)
//             {
//                 if (!PlayerControl.LocalPlayer.Is(RoleEnum.Mayor)) return;
//                 var mayorRole = Role.GetRole<Mayor>(PlayerControl.LocalPlayer);
//                 switch (__instance.state)
//                 {
//                     case MeetingHud.VoteStates.Discussion:
//                         if (__instance.discussionTimer < PlayerControl.GameOptions.DiscussionTime)
//                         {
//                             mayorRole.Abstain.SetDisabled();
//                             // mayorRole.Extend.SetDisabled();
//                             break;
//                         }
//                         mayorRole.Abstain.SetEnabled();
//                         // mayorRole.Extend.SetEnabled();
//                         break;
//                 }
//
//                 UpdateButton(mayorRole, __instance);
//             }
//         }
//     }
// }