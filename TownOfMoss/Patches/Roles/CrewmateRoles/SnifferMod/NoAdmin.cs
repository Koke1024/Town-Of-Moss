using HarmonyLib;

namespace TownOfUs.CrewmateRoles.SniffMod {

    public class NoAdmin {
        [HarmonyPatch(typeof(MapCountOverlay), nameof(MapCountOverlay.Update))]
        public static class AdminTimeLimit {
            public static bool Prefix(MapCountOverlay __instance) {
                if (!PlayerControl.LocalPlayer.Is(RoleEnum.Sniffer)) {
                    return true;
                }
                __instance.BackgroundColor.SetColor(Palette.DisabledGrey);
                __instance.OnDisable();
                MapBehaviour.Instance.Close();
                MapBehaviour.Instance.Close();
                return false;
            }
        }
    }
}