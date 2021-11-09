using System.Linq;
using HarmonyLib;
using Rewired;
using TownOfUs.Extensions;
using TownOfUs.Roles;
using UnityEngine;

namespace TownOfUs.CrewmateRoles.PainterMod {
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class SeePlayerColors {
        public static void Prefix() {
            if (MeetingHud.Instance) {
                UpdateMeeting(MeetingHud.Instance);
                return;
            }

            foreach (var (id, c) in Painter.PaintedPlayers) {
                var player = GameData.Instance.GetPlayerById(id);
                player._object.myRend.material.SetColor("_VisorColor", Painter.PaintColors[(int)c]);
            }

            foreach (var (id, c) in Painter.PaintedVent) {
                Vent vent = ShipStatus.Instance.AllVents[id];
                vent.myRend.color = Painter.PaintColors[(int)c];
                vent.myRend.material.SetColor("_OutlineColor", Painter.PaintColors[(int)c]);
            }
        }

        static void UpdateMeeting(MeetingHud __instance) {
            foreach (var state in __instance.playerStates) {
                var id = state.TargetPlayerId;
                if (Painter.PaintedPlayers.ContainsKey(id)) {
                    var c = (int)Painter.PaintedPlayers[id];
                    state.PlayerIcon.Body.material.SetColor("_VisorColor", Painter.PaintColors[c]);
                }
            }
        }
    }
}