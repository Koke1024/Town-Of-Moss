using HarmonyLib;
using TownOfUs.Roles;
using UnityEngine;

namespace TownOfUs.CrewmateRoles.PainterMod
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.FixedUpdate))]
    public class PlayerPainted
    {
        public static void Prefix(PlayerControl __instance) {
            if (__instance != PlayerControl.LocalPlayer) {
                return;
            }

            if (__instance.inVent) {
                return;
            }
            foreach (var (pos, color) in Painter.PaintedPoint) {
                var dist = Vector2.Distance(pos, PlayerControl.LocalPlayer.GetTruePosition());
                if (dist < 1.2f) {
                    Painter.RpcSetPaintPlayer(PlayerControl.LocalPlayer.PlayerId, color);
                }
            }
        }
    }

    [HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.CoEnterVent), typeof(int))]
    public static class GetPaintedOnVent {
        public static void Prefix(PlayerPhysics __instance, [HarmonyArgument(0)]int id) {
            PaintColor color = PaintColor.PaintNone;
            Painter.IsColoredVent(id, ref color);
            if (color != PaintColor.PaintNone) {
                Painter.RpcSetPaintPlayer(__instance.myPlayer.PlayerId, color);
            }
        }

        public static void Postfix(PlayerPhysics __instance) {
        }
    }

    [HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.CoExitVent), typeof(int))]
    public static class GetPaintedOnVentExit {
        public static void Prefix(PlayerPhysics __instance, [HarmonyArgument(0)]int id) {
            PaintColor color = PaintColor.PaintNone;
            Painter.IsColoredVent(id, ref color);
            if (color != PaintColor.PaintNone) {
                Painter.RpcSetPaintPlayer(__instance.myPlayer.PlayerId, color);
            }
        }

        public static void Postfix(PlayerPhysics __instance) {
        }
    }
}