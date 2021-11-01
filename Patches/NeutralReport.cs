using System;
using System.Linq;
using HarmonyLib;
using TownOfUs;
using TownOfUs.Roles;

namespace TownOfUs.NeutralReport {
    static class NeutralReporter {
        private static bool reported = false;
        private static string neutralReport = "";
        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
        public class MeetingHudStart {

            private static void Postfix(MeetingHud __instance) {
                if (!CustomGameOptions.NoticeNeutral || reported || PlayerControl.LocalPlayer.Data.IsDead) {
                    return;
                }

                if (PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead) <=
                    GameData.Instance.AllPlayers.Count / 2) {
                    if (DestroyableSingleton<HudManager>.Instance) {
                        DestroyableSingleton<HudManager>.Instance.Chat.AddChat(PlayerControl.LocalPlayer, neutralReport);
                        reported = true;
                    }
                }
            }
        }

        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.SetInfected))]
        public class StartGame {
            public static void Postfix() {
                if (!CustomGameOptions.NoticeNeutral) {
                    return;
                }

                reported = false;
                var neutralList = Role.AllRoles.Where(x => x.Faction == Faction.Neutral);
                if (!neutralList.Any()) {
                    neutralReport = "No Neutral Roles have been assigned in this game.";
                    return;
                }
                neutralReport = neutralList.Join(x => x.Name, ", ") + " are assigned in this game.";
            }
        }
    }
}