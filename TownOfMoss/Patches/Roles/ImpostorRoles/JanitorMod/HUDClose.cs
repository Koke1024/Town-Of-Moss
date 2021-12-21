using HarmonyLib;
using TownOfUs.Roles;
using DateTime = Il2CppSystem.DateTime;
using Object = UnityEngine.Object;

namespace TownOfUs.ImpostorRoles.JanitorMod
{
    [HarmonyPatch(typeof(Object), nameof(Object.Destroy), typeof(Object))]
    public static class HUDClose
    {
        public static void Postfix(Object obj)
        {
            if (ExileController.Instance == null || obj != ExileController.Instance.gameObject) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Janitor)) {
                return;
            }

            Janitor janitor = Role.GetRole<Janitor>(PlayerControl.LocalPlayer);
            janitor.lastCleaned = DateTime.UtcNow;
        }
    }
}