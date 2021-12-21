using HarmonyLib;
using TownOfUs.Roles;
using UnityEngine;

namespace TownOfUs.ImpostorRoles.KirbyMod
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.FixedUpdate))]
    public class KirbyKillTimer {
        public static void Postfix(PlayerControl __instance) {
            if (!__instance.AmOwner) return;
            if (__instance.Data.IsDead) return;
            if (!__instance.Is(RoleEnum.Kirby)) {
                return;
            }

            if (Role.GetRole<Kirby>(__instance).Morphed) {
                __instance.killTimer += Time.fixedDeltaTime;                
            }
        }
    }
}