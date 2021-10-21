using HarmonyLib;
using TownOfUs.Roles;

namespace TownOfUs.NeutralRoles.ZombieMod
{
    [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.Start))]
    public static class StartGame
    {
        public static void Postfix(ShipStatus __instance) {
            foreach (var player in Role.GetRoles(RoleEnum.Mayor)) {
                player.Player.RemainingEmergencies += 1;
            }
        }
    }
}