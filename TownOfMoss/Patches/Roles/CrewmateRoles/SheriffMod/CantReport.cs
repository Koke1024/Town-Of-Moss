using System.Linq;
using HarmonyLib;
using Il2CppSystem.Collections.Generic;
using TownOfUs.CrewmateRoles.MedicMod;
using TownOfUs.Extensions;
using UnityEngine;

namespace TownOfUs.CrewmateRoles.SheriffMod
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.FixedUpdate))]
    public class CantReport
    {
        public static void Postfix(PlayerControl __instance)
        {
            if (!__instance.AmOwner) return;
            if (!__instance.CanMove) return;
            if (!__instance.Is(RoleEnum.Sheriff)) return;
            if (CustomGameOptions.SheriffBodyReport) return;
            var truePosition = __instance.GetTruePosition();

            var data = __instance.Data;
            var stuff = Physics2D.OverlapCircleAll(truePosition, __instance.MaxReportDistance, Constants.Usables);
            var flag = (PlayerControl.GameOptions.GhostsDoTasks || !data.IsDead) &&
                       (!AmongUsClient.Instance || !AmongUsClient.Instance.IsGameOver) && __instance.CanMove;
            var flag2 = false;

            foreach (var collider2D in stuff)
                if (flag && !data.IsDead && !flag2 && collider2D.CompareTag("DeadBody"))
                {
                    var component = collider2D.GetComponent<DeadBody>();

                    if (Vector2.Distance(truePosition, component.TruePosition) <= __instance.MaxReportDistance)
                    {
                        if (Utils.KilledPlayers.Any(x =>
                            x.Value.PlayerId == component.ParentId &&
                            x.Value.KillerId != PlayerControl.LocalPlayer.PlayerId)) {
                            flag2 = true;
                        }
                    }
                }

            DestroyableSingleton<HudManager>.Instance.ReportButton.SetActive(flag2);
        }
    }

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.ReportClosest))]
    public static class DontReport
    {
        public static bool Prefix(PlayerControl __instance)
        {
            if (!__instance.Is(RoleEnum.Sheriff)) return true;
            if (CustomGameOptions.SheriffBodyReport) return true;

            if (AmongUsClient.Instance.IsGameOver) return false;
            if (PlayerControl.LocalPlayer.Data.IsDead) return false;
            foreach (var collider2D in Physics2D.OverlapCircleAll(__instance.GetTruePosition(),
                __instance.MaxReportDistance, Constants.PlayersOnlyMask))
                if (collider2D.CompareTag("DeadBody"))
                {
                    var component = collider2D.GetComponent<DeadBody>();
                    if (component && !component.Reported)
                    {
                        if (Utils.KilledPlayers.Any(x =>
                            x.Value.PlayerId == component.ParentId &&
                            x.Value.KillerId != PlayerControl.LocalPlayer.PlayerId)) {
                            component.OnClick();
                        }
                        if (component.Reported) break;
                    }
                }

            return false;
        }
    }
}