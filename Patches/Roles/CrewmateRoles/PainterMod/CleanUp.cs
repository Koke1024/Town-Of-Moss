using HarmonyLib;
using TownOfUs.Roles;

namespace TownOfUs.Patches.Roles.CrewmateRoles.PainterMod {
    public class CleanUp {


        [HarmonyPatch(typeof(EndGameManager), nameof(EndGameManager.Start))]
        public class EndGameManager_SetEverythingUp {
            public static void Prefix() {
                foreach (var (id, c) in Painter.PaintedPlayers) {
                    var player = GameData.Instance.GetPlayerById(id);
                    player._object.myRend.material.SetColor("_VisorColor", Palette.VisorColor);
                }
                

                Painter.PaintedPoint.Clear();
                Painter.PaintedPointBefore.Clear();
            
                Painter.PaintedVent.Clear();
                Painter.PaintedVentBefore.Clear();
            
                Painter.PaintedPlayers.Clear();
            }
        }
    }
}