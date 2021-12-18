using HarmonyLib;
using TownOfUs.Extensions;

namespace TownOfUs.CrewmateRoles.SwapperMod
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.SetRole))]
    public class NoButtons
    {
        public static void Postfix(PlayerControl __instance)
        {
            if (!__instance.AmOwner) {
                return;
            }
            if (__instance.Is(RoleEnum.Swapper)) PlayerControl.LocalPlayer.RemainingEmergencies = 0;
        }
    }
}