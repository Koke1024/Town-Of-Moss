using HarmonyLib;
using TownOfUs.Extensions;
using TownOfUs.Roles;

namespace TownOfUs.NeutralRoles.ExecutionerMod
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.SetInfected))]
    public class StartGame
    {
        public static void Postfix() {
            foreach (var role in Role.GetRoles(RoleEnum.Executioner)) {
                if (role.Player != PlayerControl.LocalPlayer) {
                    return;
                }
                ((Executioner)role).SetExecutionTarget();
                AmongUsExtensions.Log($"Set Execution Target Arrow");
            }
        }
    }
}