using HarmonyLib;
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

        public static bool CanVent(PlayerControl player, GameData.PlayerInfo playerInfo)
        { 
            if (player.inVent)
                return true;

            if (playerInfo.IsDead)
                return false;
            
            if (!CustomGameOptions.VentWithBody && player.CanDrag() && Role.GetRole<Undertaker>(player).CurrentlyDragging != null) {
                return false;
            }

            if (player.Is(RoleEnum.Morphling) || player.Is(RoleEnum.Kirby)) {
                if (CustomGameOptions.MorphCanVent == MorphVentOptions.None) {
                    return false;
                }
                if (Role.GetRole<Morphling>(player).Morphed && CustomGameOptions.MorphCanVent == MorphVentOptions.OnNotMorph) {
                    return false;
                }
            }
            if (player.Is(RoleEnum.Jester)) {
                if (CustomGameOptions.JesterUseVent) {
                    return true;
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

            if (player.Is(RoleEnum.Assassin) && CustomGameOptions.MadMateOn && !CustomGameOptions.MadmateCanVent ) {
                return false;
            }

            return playerInfo.IsImpostor();
        }

        public static void Postfix(Vent __instance,
            [HarmonyArgument(0)] GameData.PlayerInfo playerInfo,
            [HarmonyArgument(1)] ref bool canUse,
            [HarmonyArgument(2)] ref bool couldUse,
            ref float __result)
        {
            if (__instance.name.StartsWith("SealedVent_")) {
                canUse = couldUse = false;
                return;
            }
            
            float num = float.MaxValue;
            PlayerControl playerControl = playerInfo.Object;
            couldUse = CanVent(playerControl, playerInfo) && !playerControl.MustCleanVent(__instance.Id) && (!playerInfo.IsDead || playerControl.inVent) && (playerControl.CanMove || playerControl.inVent);

            var ventilationSystem = ShipStatus.Instance.Systems[SystemTypes.Ventilation].Cast<VentilationSystem>();
            if (ventilationSystem != null && ventilationSystem.PlayersCleaningVents != null && !playerControl.Data.IsImpostor())
            {
                foreach (var item in ventilationSystem.PlayersCleaningVents)
                {
                    if (item.value == __instance.Id)
                        couldUse = false;
                }
            }
            canUse = couldUse;
            if (canUse)
            {
                Vector3 center = playerControl.Collider.bounds.center;
                Vector3 position = __instance.transform.position;
                num = Vector2.Distance((Vector2)center, (Vector2)position);
                var usableDistance = playerInfo._object.inVent ? 0.35: (double)__instance.UsableDistance; 
                canUse = ((canUse ? 1 : 0) & ((double)num > usableDistance ? 0 : (!PhysicsHelpers.AnythingBetween(playerControl.Collider, (Vector2)center, (Vector2)position, Constants.ShipOnlyMask, false) ? 1 : 0))) != 0;
            }
            
            __result = num;

        }

        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.MustCleanVent))]
        public static class MustCleanVentPatch {
            public static bool Prefix(PlayerControl __instance, ref bool __result) {
                if (__instance.Data.IsImpostor()) {
                    __result = false;
                    return false;
                }
                return true;
            }

            public static void Postfix(PlayerControl __instance) {
            }
        }
    }
}