using System.Collections;
using System.Linq;
using Assets.CoreScripts;
using HarmonyLib;
using Reactor;
using TownOfUs.Roles;
using UnityEngine;

namespace TownOfUs.NeutralReport {
    static class NeutralReporter {
        private static bool reported = false;
        private static string neutralReport = "";
        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
        public class MeetingHudStart {

            private static void Postfix(MeetingHud __instance) {
                if (!CustomGameOptions.NoticeNeutral || reported) {
                    return;
                }

                if (PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead) <=
                    GameData.Instance.AllPlayers.Count / 2) {
                    if (DestroyableSingleton<HudManager>.Instance) {
                        DestroyableSingleton<HudManager>.Instance.Chat.AddChat(PlayerControl.LocalPlayer, neutralReport);
                        
                        Coroutines.Start(Notice());
                        reported = true;
                    }
                }
            }

            private static IEnumerator Notice() {
                yield return new WaitForSeconds(6.0f);
                DestroyableSingleton<HudManager>.Instance.ShowPopUp(neutralReport);
                yield break;
            }
        }

        [HarmonyPatch(typeof(Telemetry), nameof(Telemetry.StartGame))]
        public class StartGame {
            public static void Postfix() {
                if (!CustomGameOptions.NoticeNeutral) {
                    return;
                }

                reported = false;
                var neutralList = Role.AllRoles.Where(x => x.Faction == Faction.Neutral);
                if (!neutralList.Any()) {
                    neutralReport = "Assigned Neutral Roles:\nNone";
                    return;
                }
                neutralReport = "Assigned Neutral Roles:\n" + neutralList.Join(x => x.Name, ", ");
            }
        }
    }
}