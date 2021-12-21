using System;
using HarmonyLib;
using TownOfUs.Roles;
using Object = UnityEngine.Object;
using System.Linq;

namespace TownOfUs.ImpostorRoles.KirbyMod
{
    [HarmonyPatch(typeof(Object), nameof(Object.Destroy), typeof(Object))]
    public static class HUDClose
    {
        public static void Postfix(Object obj)
        {
            if (ExileController.Instance == null || obj != ExileController.Instance.gameObject) return;


            foreach (var p in PlayerControl.AllPlayerControls.ToArray().Where(x => x.Is(RoleEnum.Kirby))) {
                var role = Role.GetRole<Kirby>(p);
                role.SampledPlayer = null;
                role._aten = null;
                role.Unmorph();
                role.LastMorphed = DateTime.UtcNow;
            }
        }
    }
}