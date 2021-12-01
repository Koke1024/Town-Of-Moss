using HarmonyLib;
using Reactor;
using System.Linq;
using TownOfUs.Extensions;
using TownOfUs.ImpostorRoles.MorphlingMod;
using TownOfUs.Roles;
using UnityEngine;

namespace TownOfUs
{
    [HarmonyPatch(typeof(HudManager))]
    public static class HudManagerVentPatch
    {
        [HarmonyPatch(nameof(HudManager.Update))]
        public static void Postfix(HudManager __instance)
        {
            if(__instance.ImpostorVentButton == null || __instance.ImpostorVentButton.gameObject == null || __instance.ImpostorVentButton.IsNullOrDestroyed())
                return;

            bool active = PlayerControl.LocalPlayer != null && VentPatches.CanVent(PlayerControl.LocalPlayer, PlayerControl.LocalPlayer._cachedData) && !MeetingHud.Instance;
            if (active != __instance.ImpostorVentButton.gameObject.active) {
                __instance.ImpostorVentButton.gameObject.SetActive(active);
            }
        }
    }

    [HarmonyPatch(typeof(Vent), nameof(Vent.CanUse))]
    public static class VentPatches
    {
        private static bool CheckUndertaker(PlayerControl player)
        {
            var role = Role.GetRole<Undertaker>(player);
            return player.Data.IsDead || role.CurrentlyDragging != null;
        }

        public static bool CanVent(PlayerControl player, GameData.PlayerInfo playerInfo)
        { 
            if (player.inVent)
                return true;

            if (playerInfo.IsDead)
                return false;
            
            if (player.Is(RoleEnum.Kirby)
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
            if (player.Is(RoleEnum.Swooper)) {
                if (CustomGameOptions.SwooperCanVent == MorphVentOptions.None) {
                    return false;
                }
                if (Role.GetRole<Swooper>(player).IsSwooped && CustomGameOptions.SwooperCanVent == MorphVentOptions.OnNotMorph) {
                    return false;
                }
            }

            if (player.Is(RoleEnum.Charger) || player.Is(RoleEnum.Engineer) || (player.roleAssigned && playerInfo.Role?.Role == RoleTypes.Engineer) ||
                player.Is(RoleEnum.Glitch)) {
                
                return true;
            }

            return playerInfo.IsImpostor();
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
                if (__result > 0.2f) {
                    __result = float.MaxValue;
                    canUse = couldUse = false;
                    return false;
                }
                else {
                    canUse = couldUse = true;
                    return false;                    
                }
            }
                
            // if (player.closest != null) {
            //     return false;
            // }

            if (player.Is(RoleEnum.Kirby)
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
            if (player.Is(RoleEnum.Swooper)) {
                if (CustomGameOptions.SwooperCanVent == MorphVentOptions.None) {
                    return false;
                }
                if (Role.GetRole<Swooper>(player).IsSwooped && CustomGameOptions.SwooperCanVent == MorphVentOptions.OnNotMorph) {
                    return false;
                }
            }


            if (player.Is(RoleEnum.Engineer))
                playerInfo.Role.CanVent = true;
            if (player.Is(RoleEnum.Glitch))
                playerInfo.Role.CanVent = true;
            if (player.Is(RoleEnum.Jester) && CustomGameOptions.JesterUseVent)
                playerInfo.Role.CanVent = true;
            if (player.Is(RoleEnum.Charger))
                playerInfo.Role.CanVent = true;
            // if (player.Is(RoleEnum.Defect))
            //     playerInfo.Role.CanVent = true;
            // if (player.Is(RoleEnum.Assassin) && CustomGameOptions.MadMateOn)
            //     playerInfo.Role.CanVent = false;
            
            return true;
        }

        public static void Postfix(Vent __instance,
            [HarmonyArgument(0)] GameData.PlayerInfo playerInfo,
            [HarmonyArgument(1)] ref bool canUse,
            [HarmonyArgument(2)] ref bool couldUse,
            ref float __result)
        {
            float num = float.MaxValue;
            PlayerControl playerControl = playerInfo.Object;
            couldUse = CanVent(playerControl, playerInfo) && !playerControl.MustCleanVent(__instance.Id) && (!playerInfo.IsDead || playerControl.inVent) && (playerControl.CanMove || playerControl.inVent);

            var ventitaltionSystem = ShipStatus.Instance.Systems[SystemTypes.Ventilation].Cast<VentilationSystem>();
            if (ventitaltionSystem != null && ventitaltionSystem.PlayersCleaningVents != null)
            {
                foreach (var item in ventitaltionSystem.PlayersCleaningVents.Values)
                {
                    if (item == __instance.Id)
                        couldUse = false;
                }

            }
            canUse = couldUse;
            if (canUse)
            {
                Vector3 center = playerControl.Collider.bounds.center;
                Vector3 position = __instance.transform.position;
                num = Vector2.Distance((Vector2)center, (Vector2)position);
                canUse = ((canUse ? 1 : 0) & ((double)num > (double)__instance.UsableDistance ? 0 : (!PhysicsHelpers.AnythingBetween(playerControl.Collider, (Vector2)center, (Vector2)position, Constants.ShipOnlyMask, false) ? 1 : 0))) != 0;
            }
            __result = num;

        }
    }
}