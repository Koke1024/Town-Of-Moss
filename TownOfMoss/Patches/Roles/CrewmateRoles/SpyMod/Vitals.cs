// using System;
// using System.Linq;
// using HarmonyLib;
// using TownOfUs.CrewmateRoles.MedicMod;
// using TownOfUs.Extensions;
// using TownOfUs.Roles;
// using UnityEngine;
//
// namespace TownOfUs.CrewmateRoles.SpyMod
// {
//     [HarmonyPatch(typeof(VitalsMinigame), nameof(VitalsMinigame.Update))]
//     public class Vitals
//     {
//         public static void Postfix(VitalsMinigame __instance) {
//             // var vital = GameObject.FindObjectsOfType<SystemConsole>().First(x => x.GetType() == typeof(VitalsPanel));
//             //
//             // var position = vital.transform.position;
//             // AmongUsExtensions.Log($"{position.ToString()}");
//             // position = new Vector3(position.x + 0.5f, position.y + 0.5f, position.z);
//             // vital.transform.position = position;
//
//             if (!PlayerControl.LocalPlayer.Is(RoleEnum.Charger)) return;
//             var charger = Role.GetRole<Charger>(PlayerControl.LocalPlayer);
//             bool charged = charger.Charge > 0;
//             if (charger.Charge <= 0) {
//                 charger.Charge = 0;
//             }
//             for (var i = 0; i < __instance.vitals.Count; i++)
//             {
//                 var panel = __instance.vitals[i];
//                 var info = GameData.Instance.AllPlayers.ToArray()[i];
//                 if (!panel.IsDead) continue;
//                 var deadBody = Utils.KilledPlayers.First(x => x.PlayerId == info.PlayerId);
//                 var num = (float) (DateTime.UtcNow - deadBody.KillTime).TotalMilliseconds;
//                 // panel. = Math.Round(num/1000f) + "s";
//             }
//         }
//     }
// }