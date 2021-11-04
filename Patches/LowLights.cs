using System;
using HarmonyLib;
using SteamKit2.GC.TF2.Internal;
using TownOfUs.Extensions;
using TownOfUs.Patches;
using TownOfUs.Roles;
using TownOfUs.Roles.Modifiers;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

namespace TownOfUs
{
    [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.CalculateLightRadius))]
    public static class LowLights {
        public static bool Prefix(ShipStatus __instance, [HarmonyArgument(0)] GameData.PlayerInfo player,
            ref float __result)
        {
            if (player == null || (player.IsDead && !CanMove.CanMovePatch.GetMyBody()))
            {
                __result = __instance.MaxLightRadius;
                return false;
            }

            var switchSystem = __instance.Systems[SystemTypes.Electrical].Cast<SwitchSystem>();
            if (player.IsImpostor || player._object.Is(RoleEnum.Glitch) || player._object.Is(RoleEnum.Sniper)) {
                if (switchSystem.Value < 230) {
                    Utils.StartFlash(new Color(0, 0, 0, 1.0f));
                }
                else if(switchSystem.Value >= 230 && switchSystem.Value < 255){
                    Utils.EndFlash();
                }
                __result = __instance.MaxLightRadius * PlayerControl.GameOptions.ImpostorLightMod;
                if (player.Object.Is(ModifierEnum.ButtonBarry))
                    if (Modifier.GetModifier<ButtonBarry>(PlayerControl.LocalPlayer).ButtonUsed)
                        __result *= 0.5f;
                return false;
            }

            var t = switchSystem.Value / 255f;
            foreach (Cracker cracker in Role.GetRoles(RoleEnum.Cracker)) {
                if (cracker.MyLastRoom == cracker.HackingRoom &&
                    cracker.RoomDetected > DateTime.UtcNow.AddSeconds(-CustomGameOptions.CrackDur)) {
                    t = 0;
                }
            }

            // foreach (Vent vent in ShipStatus.Instance.AllVents) {
            //     float distance = Vector2.Distance(vent.transform.position, player._object.GetTruePosition());
            //     if (distance <= vent.UsableDistance) {
            //         if (player._object.Is(RoleEnum.Charger)) {
            //             t = 1;
            //             break;
            //         }
            //     }
            // }
            if (player._object.Is(ModifierEnum.Torch)) t = 1;
            __result = Mathf.Lerp(__instance.MinLightRadius, __instance.MaxLightRadius, t) *
                       PlayerControl.GameOptions.CrewLightMod;

            if (player._object.Is(RoleEnum.Charger)) {
                __result *= (0.75f + Role.GetRole<Charger>(player._object).Charge * 0.75f);
            }
            if (player.Object.Is(ModifierEnum.ButtonBarry))
                if (Modifier.GetModifier<ButtonBarry>(PlayerControl.LocalPlayer).ButtonUsed)
                    __result *= 0.5f;

            return false;
        }
    }

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.FixedUpdate))]
    public static class deadLowSight {
        public static void Prefix(PlayerControl __instance) {
            if (PlayerControl.LocalPlayer.Data.IsDead && CanMove.CanMovePatch.GetMyBody()) {
                DestroyableSingleton<HudManager>.Instance.ShadowQuad.gameObject.SetActive(true);                
            }
        }
    }
}