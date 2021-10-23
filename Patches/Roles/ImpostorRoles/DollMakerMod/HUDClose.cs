using System;
using HarmonyLib;
using TownOfUs.Roles;
using Object = UnityEngine.Object;
using Il2CppSystem;
using Il2CppSystem.Collections.Generic;
using DateTime = Il2CppSystem.DateTime;

namespace TownOfUs.ImpostorRoles.DollMakerMod
{
    [HarmonyPatch(typeof(Object), nameof(Object.Destroy), typeof(Object))]
    public static class HUDClose
    {
        public static void Postfix(Object obj)
        {
            if (ExileController.Instance == null || obj != ExileController.Instance.gameObject) return;
            
            foreach (var role in Role.GetRoles(RoleEnum.DollMaker))
            {
                ((DollMaker)role).lastWaxed = DateTime.UtcNow;
                ((DollMaker)role).DollList.Clear();
            }
        }
    }
}