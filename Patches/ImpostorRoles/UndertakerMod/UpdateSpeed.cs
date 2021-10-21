using HarmonyLib;
using TownOfUs.Roles;

namespace TownOfUs.ImpostorRoles.UndertakerMod
{
    [HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.FixedUpdate))]
    public static class PlayerPhysics_FixedUpdate
    {
        public static void Postfix(PlayerPhysics __instance)
        {
            if (__instance.myPlayer.CanDrag())
            {
                var role = Role.GetRole<Undertaker>(__instance.myPlayer);
                if (role.CurrentlyDragging != null)
                    if (__instance.AmOwner && GameData.Instance && __instance.myPlayer.CanMove) {
                        __instance.body.velocity *= CustomGameOptions.DragVel / 100.0f;
                        if (__instance.myPlayer.Is(RoleEnum.Druid)) {
                            Role.GetRole<Druid>(__instance.myPlayer).Drag();
                        }
                    }
            }
        }
    }
}