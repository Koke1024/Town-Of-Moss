using HarmonyLib;
using TownOfUs.ImpostorRoles.MorphlingMod;
using TownOfUs.Roles;
using UnityEngine;

namespace TownOfUs
{
    [HarmonyPatch(typeof(Vent), nameof(Vent.CanUse))]
    public static class PlayerVentTimeExtension
    {
        private static bool CheckUndertaker(PlayerControl player)
        {
            var role = Role.GetRole<Undertaker>(player);
            return player.Data.IsDead || role.CurrentlyDragging != null;
        }
        public static bool Prefix(Vent __instance,
            [HarmonyArgument(0)] GameData.PlayerInfo playerInfo,
            [HarmonyArgument(1)] ref bool canUse,
            [HarmonyArgument(2)] ref bool couldUse,
            ref float __result)
        {
            __result = float.MaxValue;
            canUse = couldUse = false;

            var player = playerInfo.Object;
            
            if (__instance.name.StartsWith("SealedVent_")) {
                canUse = couldUse = false;
                return false;
            }
            
            if (player.inVent)
            {
                __result = Vector2.Distance(player.Collider.bounds.center, __instance.transform.position);
                canUse = couldUse = true;
                return false;
            }
                

            if (player.Is(RoleEnum.Swooper)
                || player.Is(RoleEnum.Kirby)
                || (player.CanDrag() && Role.GetRole<Undertaker>(player).CurrentlyDragging != null))
                return false;
            if (player.Is(RoleEnum.Morphling)) {
                if (CustomGameOptions.MorphCanVent == MorphVentOptions.None) {
                    return false;
                }
                if (Role.GetRole<Morphling>(player).Morphed && CustomGameOptions.MorphCanVent == MorphVentOptions.OnNotMorph) {
                    return false;
                }
                
            }


            if (player.Is(RoleEnum.Engineer))
                playerInfo.IsImpostor = true;
            if (player.Is(RoleEnum.Glitch))
                playerInfo.IsImpostor = true;
            if (player.Is(RoleEnum.Jester) && CustomGameOptions.JesterUseVent)
                playerInfo.IsImpostor = true;
            if (player.Is(RoleEnum.Charger))
                playerInfo.IsImpostor = true;
            // if (player.Is(RoleEnum.Defect))
            //     playerInfo.IsImpostor = true;
            // if (player.Is(RoleEnum.Assassin) && CustomGameOptions.MadMateOn)
            //     playerInfo.IsImpostor = false;
            
            return true;
        }

        public static void Postfix(Vent __instance, [HarmonyArgument(0)] GameData.PlayerInfo playerInfo)
        {
            if (playerInfo.Object.Is(RoleEnum.Engineer))
                playerInfo.IsImpostor = false;
            if (playerInfo.Object.Is(RoleEnum.Glitch))
                playerInfo.IsImpostor = false;
            if (playerInfo.Object.Is(RoleEnum.Charger))
                playerInfo.IsImpostor = false;
            if (playerInfo.Object.Is(RoleEnum.Jester) && CustomGameOptions.JesterUseVent)
                playerInfo.IsImpostor = false;
            // if (playerInfo.Object.Is(RoleEnum.Defect))
            //     playerInfo.IsImpostor = false;
        }
    }
}