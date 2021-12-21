using HarmonyLib;
using TownOfUs.Roles;
using DateTime = Il2CppSystem.DateTime;

namespace TownOfUs.CrewmateRoles.PainterMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class DoClick
    {
        public static bool Prefix(KillButton __instance)
        {
            var flag = PlayerControl.LocalPlayer.Is(RoleEnum.Painter);
            if (!flag) return true;
            var role = Role.GetRole<Painter>(PlayerControl.LocalPlayer);
            if (!PlayerControl.LocalPlayer.CanMove) return false;
            var flag2 = role.PaintTimer() == 0f;
            if (!flag2) return false;
            if (!__instance.enabled) return false;

            int c = 0;
            foreach (var btn in role._paintButtons) {
                if(btn != __instance) {
                    ++c;
                    continue;
                }

                if (c >= CustomGameOptions.PaintColorMax) {
                    return false;
                }
                
                role.lastPainted = DateTime.UtcNow;
                
                if (__instance.graphic.sprite == TownOfUs.PaintSprite[c]) {
                    Painter.RpcSetPaintPoint(role.Player.GetTruePosition(), (PaintColor)c);
                }
                else {
                    Painter.RpcSetPaintVent(role.closeVent.Id, (PaintColor)c);
                }

                break;
            }

            return false;
        }
    }
}
