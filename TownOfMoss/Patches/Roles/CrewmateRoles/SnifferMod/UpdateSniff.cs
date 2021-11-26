using System;
using System.Linq;
using HarmonyLib;
using Il2CppSystem.Runtime.Serialization.Formatters.Binary;
using Reactor;
using Reactor.Extensions;
using TownOfUs.Extensions;
using TownOfUs.Roles;
using UnityEngine;
using DateTime = Il2CppSystem.DateTime;
using Object = UnityEngine.Object;

namespace TownOfUs.CrewmateRoles.SnifferMod
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.FixedUpdate))]
    public class UpdateArrows {
        public static void Postfix(PlayerControl __instance)
        {
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Sniffer)) {
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
            
            var bodies = Object.FindObjectsOfType<DeadBody>();

            if (bodies.Count == 0) {
                role.sniffInterval = 1.0f;
                return;
            }
            float closestDistance = CustomGameOptions.SnifferMaxRange * CustomGameOptions.SnifferMaxRange;
            foreach (var body in bodies) {
                closestDistance = Mathf.Min(closestDistance, Vector2.SqrMagnitude(role.Player.GetTruePosition() - body.TruePosition));
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

    namespace TownOfUs.CrewmateRoles.SnifferMod
    {
        [HarmonyPatch(typeof(ExileController), nameof(ExileController.Begin))]
        internal class DeadBodyClean
        {
            private static void Postfix(ExileController __instance)
            {
                if (!PlayerControl.LocalPlayer.Is(RoleEnum.Sniffer)) {
                    return;
                }
                
                DestroyableSingleton<HudManager>.Instance.ShadowQuad.material.color =
                    new Color(0.27451f, 0.27451f, 0.27451f);
            }
        }
    }
}