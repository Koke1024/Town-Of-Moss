using HarmonyLib;
using Il2CppSystem;
using TownOfUs.Roles;
using Object = UnityEngine.Object;

namespace TownOfUs.CrewmateRoles.BodyGuardMod
{
    [HarmonyPatch(typeof(Object), nameof(Object.Destroy), typeof(Object))]
    public static class HUDClose
    {
        public static void Postfix(Object obj)
        {
            if (ExileController.Instance == null || obj != ExileController.Instance.gameObject) return;
            foreach (var role in Role.GetRoles(RoleEnum.BodyGuard))
            {
                var bodyGuard = (BodyGuard) role;
                bodyGuard.ShieldedTime = DateTime.UtcNow;
                bodyGuard.ShieldedPlayer = null;
            }
        }
    }
}