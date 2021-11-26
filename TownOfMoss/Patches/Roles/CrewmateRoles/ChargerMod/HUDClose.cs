using HarmonyLib;
using Reactor;
using TownOfUs.Roles;
using UnityEngine;

namespace TownOfUs.CrewmateRoles.ChargerMod
{
    [HarmonyPatch(typeof(Object), nameof(Object.Destroy), typeof(Object))]
    public static class HUDClose
    {
        public static void Postfix(Object obj)
        {
            
            if (ExileController.Instance == null || obj != ExileController.Instance.gameObject) return;
            
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Charger)) {
                return;
            }
            Charger role = Role.GetRole<Charger>(PlayerControl.LocalPlayer);
            role.Charge = 1.0f;
        }
    }
}