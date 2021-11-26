using HarmonyLib;
using TownOfUs.Roles;
using UnityEngine;

namespace TownOfUs.CrewmateRoles.DruidMod
{

    [HarmonyPatch(typeof(Object), nameof(Object.Destroy), typeof(Object))]
    public static class HUDClose
    {
        public static void Postfix(Object obj)
        {
            if (ExileController.Instance == null || obj != ExileController.Instance.gameObject) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Druid)) {
                return;
            }
            var druid = Role.GetRole<Druid>(PlayerControl.LocalPlayer);
            druid.revivedCount = 0;
            druid._dragDropButton.graphic.color = new Color(1, 1, 1, 1);;
        }
    }
}