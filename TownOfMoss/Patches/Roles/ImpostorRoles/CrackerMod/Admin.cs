using HarmonyLib;
using TownOfUs.Roles;

namespace TownOfUs.CrewmateRoles.CrackerMod
{
    [HarmonyPatch(typeof(MapCountOverlay), nameof(MapCountOverlay.Update))]
    public static class Admin
    {
        public static void UpdateBlips(MapCountOverlay __instance) {
            foreach (Cracker cracker in Role.GetRoles(RoleEnum.Cracker)) {
                foreach (var area in __instance.CountAreas) {
                    if (cracker.HackingRoom == area.RoomType) {
                        area.UpdateCount(0);
                        break;
                    }
                }
            }
        }

        public static void Postfix(MapCountOverlay __instance)
        {

            foreach (Cracker cracker in Role.GetRoles(RoleEnum.Cracker)) {
                if (cracker.HackingRoom == null) {
                    return;
                }
            }

            UpdateBlips(__instance);
        }
    }
}