using HarmonyLib;
using TownOfUs.Roles;

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
                if (player.Disconnected) {
                    continue;
                }
                if (player._object.MyRend) {
                    player._object.MyRend.material.SetColor("_VisorColor", Painter.PaintColors[(int)c]);                    
                }
            }
        }

        static void UpdateMeeting(MeetingHud __instance) {
            foreach (var state in __instance.playerStates) {
                var id = state.TargetPlayerId;
                if (Painter.PaintedPlayers.ContainsKey(id)) {
                    var c = (int)Painter.PaintedPlayers[id];
                    if (state.PlayerIcon.Body) {
                        state.PlayerIcon.Body.material.SetColor("_VisorColor", Painter.PaintColors[c]);                        
                    }
                }
            }
        }
    }
}