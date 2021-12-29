// using System.Linq;
// using HarmonyLib;
// using TownOfUs.Roles;
// using UnityEngine;
//
// namespace TownOfUs.CrewmateRoles.PainterMod
// {
//     [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
//     public class HudManagerUpdate
//     {
//         public static Vent ClosestVent() {
//             if (ShipStatus.Instance.AllVents.Count == 0) {
//                 return null;
//             }
//             Vent target = null;
//             Vector2 truePosition = PlayerControl.LocalPlayer.GetTruePosition();
//             float closestDistance = float.MaxValue;
//             for (int i = 0; i < ShipStatus.Instance.AllVents.Length; i++) {
//                 Vent vent = ShipStatus.Instance.AllVents[i];
//                 if (vent.gameObject.name.StartsWith("JackInTheBoxVent_") || vent.gameObject.name.StartsWith("SealedVent_") || vent.gameObject.name.StartsWith("FutureSealedVent_")) continue;
//                 float distance = Vector2.Distance(vent.transform.position, truePosition);
//                 if (distance <= vent.UsableDistance && distance < closestDistance) {
//                     closestDistance = distance;
//                     target = vent;
//                 }
//             }
//
//             return target;
//         }
//     }
// }