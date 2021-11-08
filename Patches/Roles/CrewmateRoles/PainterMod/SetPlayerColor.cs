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
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Painter)) {
                return;
            }

            if (MeetingHud.Instance) {
                UpdateMeeting(MeetingHud.Instance);
            }

            foreach (var (id, c) in Painter.PaintedPlayers) {
                var player = GameData.Instance.GetPlayerById(id);
                player._object.myRend.material.SetColor("_VisorColor", Painter.PaintColors[(int)c]);
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