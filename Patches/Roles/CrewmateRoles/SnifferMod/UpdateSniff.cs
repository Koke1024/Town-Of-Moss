using System;
using System.Linq;
using HarmonyLib;
using Il2CppSystem.Runtime.Serialization.Formatters.Binary;
using Reactor.Extensions;
using TownOfUs.Extensions;
using TownOfUs.Roles;
using UnityEngine;
using UnityEngine.UIElements.Experimental;
using DateTime = Il2CppSystem.DateTime;
using Object = UnityEngine.Object;

namespace TownOfUs.CrewmateRoles.SnifferMod
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.FixedUpdate))]
    public class UpdateArrows {
        public static void Postfix(PlayerControl __instance)
        {
            var flag = PlayerControl.LocalPlayer.Is(RoleEnum.Sniffer);
            if (!flag) return;
            var role = Role.GetRole<Sniffer>(PlayerControl.LocalPlayer);
            if (!PlayerControl.LocalPlayer.CanMove) return;
            if (PlayerControl.LocalPlayer.Data.IsDead) return;

            role.sniffInterval -= Time.deltaTime;
            if (role.sniffInterval > 0) {
                return;
            }
            
            var bodies = Object.FindObjectsOfType<DeadBody>();

            if (bodies.Count == 0) {
                role.sniffInterval = 5.0f;
                return;
            }
            float closestDistance = float.MaxValue;
            foreach (var body in bodies) {
                closestDistance = Mathf.Min(closestDistance, Vector2.SqrMagnitude(role.Player.GetTruePosition() - body.TruePosition));
            }

            closestDistance = Mathf.Sqrt(closestDistance);
            AmongUsExtensions.Log($"Distance: {closestDistance}");

            role.sniffInterval = Mathf.Lerp(0.5f, 5.0f, closestDistance / 50.0f);
        }
    }
}