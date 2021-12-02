using HarmonyLib;
using TownOfUs.Extensions;
using TownOfUs.Roles;

namespace TownOfUs.NeutralRoles.ExecutionerMod
{
    [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.Start))]
    public class StartGame
    {
        public static void Postfix(ShipStatus __instance) {
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Executioner)) {
                return;
            }
            var role = Role.GetRole<Executioner>(PlayerControl.LocalPlayer);
            role.SetExecutionTarget();
        }
    }
}