using HarmonyLib;
using TownOfUs.Roles;
using TownOfUs.Roles.Modifiers;
using UnityEngine;

namespace TownOfUs
{
    [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.CalculateLightRadius))]
    public static class LowLights {
        public static bool Prefix(ShipStatus __instance, [HarmonyArgument(0)] GameData.PlayerInfo player,
            ref float __result)
        {
            if (player == null || player.Role == null || (player.IsDead && !Utils.ExistBody(PlayerControl.LocalPlayer.PlayerId)))
            {
                __result = __instance.MaxLightRadius;
                return false;
            }

            var switchSystem = __instance.Systems[SystemTypes.Electrical].Cast<SwitchSystem>();
            if (player.Role.IsImpostor || player._object.Is(RoleEnum.Glitch) || player._object.Is(RoleEnum.Sniper)) {
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

            if (Cracker.IsBlackout(Cracker.MyLastRoom)) {
                if (player.Role.IsImpostor || player._object.Is(RoleEnum.Glitch) ||
                    player._object.Is(RoleEnum.Sniper)) {
                    Utils.StartFlash(new Color(0, 0, 0, 1.0f));
                }
                else {
                    t = 0;
                }
            }

            if (player._object.Is(ModifierEnum.Torch)) t = 1;
            __result = Mathf.Lerp(__instance.MinLightRadius, __instance.MaxLightRadius, t) *
                       PlayerControl.GameOptions.CrewLightMod;

            if (player._object.Is(RoleEnum.Charger)) {
                __result *= (0.75f + Role.GetRole<Charger>(player._object).Charge * 0.75f);
            }

            return false;
        }
    }

    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class DeadLowSight {
        public static void Prefix(HudManager __instance) {
            if (LobbyBehaviour.Instance || MeetingHud.Instance || !PlayerControl.LocalPlayer) {
                return;
            }

            if (PlayerControl.LocalPlayer.Data.IsDead && Utils.ExistBody(PlayerControl.LocalPlayer.PlayerId) && __instance.ShadowQuad != null) {
                __instance.ShadowQuad.gameObject.SetActive(true);
            }
        }
    }
}