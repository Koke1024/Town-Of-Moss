using HarmonyLib;
using TownOfUs.Roles;

namespace TownOfUs.ImpostorRoles.SwooperMod
{
    [HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.FixedUpdate))]
    public static class SwoopDash
    {
        public static void Postfix(PlayerPhysics __instance)
        {
            if(!__instance.myPlayer.Is(RoleEnum.Swooper)) {
                return;
            }
            var role = Role.GetRole<Swooper>(__instance.myPlayer);

            if (role.IsSwooped) {
                __instance.body.velocity *= CustomGameOptions.SwooperVelocity / 100.0f;                
            }
        }
    }
    
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    [HarmonyPriority(Priority.Last)]
    public class SwoopUnswoop
    {
        [HarmonyPriority(Priority.Last)]
        public static void Postfix(HudManager __instance)
        {
            foreach (var role in Role.GetRoles(RoleEnum.Swooper))
            {
                var swooper = (Swooper) role;
                if (swooper.IsSwooped)
                    swooper.Swoop();
                else if (swooper.Enabled) swooper.UnSwoop();
            }
        }
    }
}