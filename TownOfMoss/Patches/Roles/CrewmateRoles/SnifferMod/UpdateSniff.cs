using System.Linq;
using HarmonyLib;
using TownOfUs.Roles;
using UnityEngine;

namespace TownOfUs.CrewmateRoles.SnifferMod
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.FixedUpdate))]
    public class UpdateArrows {
        public static void Postfix(PlayerControl __instance)
        {
            if (__instance != PlayerControl.LocalPlayer) {
                return;
            }
            if (!__instance.Is(RoleEnum.Sniffer)) {
                return;
            }
            var role = Role.GetRole<Sniffer>(PlayerControl.LocalPlayer);
            if (!PlayerControl.LocalPlayer.CanMove) return;
            if (PlayerControl.LocalPlayer.Data.IsDead) return;

            role.sniffInterval -= Time.deltaTime;
            if (role.sniffInterval > 0) {
                return;
            }

            if (!Utils.KilledPlayers.Any()) {
                role.sniffInterval = 1.0f;
                DestroyableSingleton<HudManager>.Instance.ShadowQuad.material.color = new Color(0.27451f, 0.27451f, 0.27451f);
                return;
            }
            float closestDistance = CustomGameOptions.SnifferMaxRange * CustomGameOptions.SnifferMaxRange;
            foreach (var killed in Utils.KilledPlayers) { 
                closestDistance = Mathf.Min(closestDistance, Vector2.SqrMagnitude(role.Player.GetTruePosition() - killed.Value.Body.TruePosition));
            }

            closestDistance = Mathf.Sqrt(closestDistance);

            var clamp = Mathf.Clamp(closestDistance / CustomGameOptions.SnifferMaxRange, 0, 1.0f);

            if (DestroyableSingleton<HudManager>.Instance.ShadowQuad) {
                DestroyableSingleton<HudManager>.Instance.ShadowQuad.material.color =
                    Color.Lerp(new Color(1f, 0f, 0.2f), new Color(0.27451f, 0.27451f, 0.27451f), clamp);
            }

            role.sniffInterval = 1.0f;
        }
    }
}