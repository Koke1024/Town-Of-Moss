using System.Linq;
using HarmonyLib;
using Il2CppSystem.Collections.Generic;
using TownOfUs.CrewmateRoles.MedicMod;
using TownOfUs.Extensions;
using TownOfUs.Roles;
using UnityEngine;

namespace TownOfUs.ImpostorRoles.PuppeteerMod
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.FixedUpdate))]
    public class CantReport
    {
        public static void Postfix(PlayerControl __instance)
        {
            if (!__instance.AmOwner) return;
            if (!__instance.CanMove) return;
            if (Puppeteer.CantReportPlayer.Count == 0) {
                return;
            }
            if (CustomGameOptions.PossessBodyReport) return;
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
                            !Puppeteer.CantReportPlayer.Contains(x.Value.PlayerId))) {
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
            if (CustomGameOptions.PossessBodyReport) return true;
            
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
                            !Puppeteer.CantReportPlayer.Contains(x.Value.PlayerId))) {
                            component.OnClick();
                        }
                        if (component.Reported) break;
                    }
                }

            return false;
        }
    }
}