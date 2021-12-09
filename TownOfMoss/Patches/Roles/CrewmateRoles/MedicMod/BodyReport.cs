using System;
using System.Linq;
using HarmonyLib;
using Il2CppSystem.Collections.Generic;
using Reactor.Extensions;
using TownOfUs.Extensions;
using TownOfUs.Roles;
using UnityEngine;
using Random = System.Random;

namespace TownOfUs.CrewmateRoles.MedicMod
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.CmdReportDeadBody))]
    internal class BodyReportPatch
    {
        private static void Postfix(PlayerControl __instance, [HarmonyArgument(0)] GameData.PlayerInfo info) {
            //System.Console.WriteLine("Report Body!");
            if (info == null) return;
            var match = Utils.KilledPlayers.FirstOrDefault(x => x.Value.PlayerId == info.PlayerId);
            if (match.IsNullOrDestroyed()) {
                return;
            }

            DeadPlayer body = match.Value;

            if (body == null)
                //System.Console.WriteLine("RBTWOOF");
                return;

            var isMedicAlive = __instance.Is(RoleEnum.Medic);

            Medic role = Role.GetRole<Medic>(__instance);

            if (!isMedicAlive)
                return;

            var isUserMedic = PlayerControl.LocalPlayer.Is(RoleEnum.Medic);
            if (!isUserMedic)
                return;
            //System.Console.WriteLine("RBTHREEF");
            var br = new BodyReport {
                Killer = Utils.PlayerById(body.KillerId),
                Reporter = __instance,
                Body = Utils.PlayerById(body.PlayerId),
                KillAge = (float)(DateTime.UtcNow - body.KillTime).TotalMilliseconds
            };

            role.SusList = GetKillerSuspect(br);
        }

        public static System.Collections.Generic.IEnumerable<PlayerControl> GetKillerSuspect(BodyReport report) {
            int susNum = (int)((report.KillAge / 1000.0f) / CustomGameOptions.MedicReportDegradation) + 2;

            if (report.Killer.Data.IsDead) {
                return new[] { report.Killer };
            }
            
            var susList = PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Data.IsDead).OrderBy(x =>
                x.PlayerId == report.Killer.PlayerId ? -10 : new Random().NextDouble() +
                (x.AmOwner ? 5 : 0));
            return susList.Take(Mathf.Clamp(susNum, 1, PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead) - 2)).ToArray();
        }
    }
    
    [HarmonyPatch(typeof(ExileController), nameof(ExileController.WrapUp))]
    public static class AdminTimeReset {
        public static void Postfix(ExileController __instance) {
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Medic)) {
                return;
            }
            var role = Role.GetRole<Medic>(PlayerControl.LocalPlayer);
            role.SusList = null;
        }
    }
}