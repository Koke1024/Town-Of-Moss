using HarmonyLib;
using TownOfUs.Roles;

namespace TownOfUs.CrewmateRoles.MayorMod
{
    [HarmonyPatch(typeof(ExileController), nameof(ExileController.Begin))]
    internal class MeetingExiledEnd
    {
        private static void Postfix(ExileController __instance)
        {
            var exiled = __instance.exiled;
            if (exiled == null) return;
            var player = exiled.Object;

            var role = Role.GetRole(player);
            if (role == null) return;
            if (role.RoleType == RoleEnum.Mayor) ((Mayor) role).ButtonUsed = true;
        }
    }
}