using System;
using System.Linq;
using HarmonyLib;
using Hazel;
using Reactor.Extensions;
using TownOfUs.Extensions;
using TownOfUs.Roles;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TownOfUs.ImpostorRoles.CrackerMod
{
    [HarmonyPatch(typeof(ActionButton), nameof(ActionButton.PerformKill))]
    public class PerformKill
    {
        public static bool Prefix(ActionButton __instance)
        {
            var flag = PlayerControl.LocalPlayer.Is(RoleEnum.Cracker);
            if (!flag) return true;
            if (!PlayerControl.LocalPlayer.CanMove) return false;
            if (PlayerControl.LocalPlayer.Data.IsDead) return false;
            var role = Role.GetRole<Cracker>(PlayerControl.LocalPlayer);
            if (__instance == role.CrackButton && role.TargetRoom != null)
            {
                if (__instance.isCoolingDown) return false;
                if (!__instance.isActiveAndEnabled) return false;
                if (role.CrackTimer() != 0) return false;

                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                    (byte) CustomRPC.CrackRoom, SendOption.Reliable, -1);
                var position = PlayerControl.LocalPlayer.transform.position;
                writer.Write(PlayerControl.LocalPlayer.PlayerId);
                writer.Write((byte)role.TargetRoom);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                role.CrackRoom(role.TargetRoom);
                return false;
            }

            return true;
        }

    }
}