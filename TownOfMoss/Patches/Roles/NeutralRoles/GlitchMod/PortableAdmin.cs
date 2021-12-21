using HarmonyLib;
using TownOfUs;

namespace TownOfMoss.Patches.Roles.NeutralRoles.GlitchMod {
    public class AnywhereMap {
        [HarmonyPatch(typeof(MapBehaviour), nameof(MapBehaviour.ShowNormalMap))]
        public static class PortableAdmin {
            public static void Postfix(MapBehaviour __instance) {
                if (!__instance.IsOpen || !CustomGameOptions.GlitchAdmin) {
                    return;
                }
                if (PlayerControl.LocalPlayer.Is(RoleEnum.Glitch)) {
                    __instance.ShowCountOverlay();
                }
            }
        }

        [HarmonyPatch(typeof(MapBehaviour), nameof(MapBehaviour.FixedUpdate))]
        public static class positionUpdate {
            private static void Postfix(MapBehaviour __instance) {
                if (!PlayerControl.LocalPlayer.Is(RoleEnum.Glitch)) {
                    return;
                }
                if (!CustomGameOptions.GlitchAdmin) {
                    return;
                }
                PlayerControl.LocalPlayer.moveable = false;
                PlayerControl.LocalPlayer.NetTransform.Halt();
            }
        }
        
        [HarmonyPatch(typeof(MapBehaviour), nameof(MapBehaviour.Close))]
        public static class InvestigatorMapClose {
            private static void Postfix(MapTaskOverlay __instance) {
                if (!PlayerControl.LocalPlayer.Is(RoleEnum.Glitch)) {
                    return;
                }
                if (!CustomGameOptions.GlitchAdmin) {
                    return;
                }
                PlayerControl.LocalPlayer.moveable = true;
            }
        }
    }
}