using HarmonyLib;
using TownOfUs.Roles;

namespace TownOfUs.NeutralRoles.ExecutionerMod
{
    [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.Start))]
    public static class StartGame
    {
        public static void Postfix(ShipStatus __instance) {
            foreach (var player in Role.GetRoles(RoleEnum.Executioner)) {
                if (player.Player != PlayerControl.LocalPlayer) {
                    return;
                }
                ((Executioner)player).SetExecutionTarget();
            }
        }
    }
}