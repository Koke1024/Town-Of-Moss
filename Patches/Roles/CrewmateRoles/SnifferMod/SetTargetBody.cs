// using System.Linq;
// using HarmonyLib;
// using Reactor;
// using TownOfUs.Roles;
// using UnityEngine;
//
// namespace TownOfUs.CrewmateRoles.SnifferMod
// {
//     [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.MurderPlayer))]
//     public class SetDeadSniff
//     {
//         public static void Postfix(PlayerControl __instance, [HarmonyArgument(0)] PlayerControl target)
//         {
//             if (PlayerControl.LocalPlayer.Is(RoleEnum.Sniffer)) {
//                 var role = Role.GetRole<Sniffer>(PlayerControl.LocalPlayer);
//                 role.SnifferTargets.Add();
//                 Coroutines.Start(SnifferMod.);
//             }
//         }
//     }
// }