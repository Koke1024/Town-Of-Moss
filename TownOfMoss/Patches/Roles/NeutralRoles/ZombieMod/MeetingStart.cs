using System;
using HarmonyLib;
using TownOfUs.Roles;
using Object = UnityEngine.Object;

namespace TownOfUs.NeutralRoles.ZombieMod {
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
    public static class NoRevive {
        public static void Postfix(MeetingHud __instance) {
            foreach (var role in Role.GetRoles(RoleEnum.Zombie)) {
                var zombie = (Zombie)role;
                zombie.deadTime = null;
            }
        }
    }
}