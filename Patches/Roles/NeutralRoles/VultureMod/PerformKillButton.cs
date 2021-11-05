using HarmonyLib;
using Hazel;
using Reactor;
using TownOfUs.ImpostorRoles.VultureMod;
using TownOfUs.Roles;
using UnityEngine;

namespace TownOfUs.NeutralRoles.VultureMod
{
    [HarmonyPatch(typeof(KillButtonManager), nameof(KillButtonManager.PerformKill))]
    public class PerformKillButton

    {
        public static bool Prefix(KillButtonManager __instance)
        {
            var flag = PlayerControl.LocalPlayer.Is(RoleEnum.Vulture);
            if (!flag) return true;
            if (!PlayerControl.LocalPlayer.CanMove) return false;
            if (PlayerControl.LocalPlayer.Data.IsDead) return false;
            var role = Role.GetRole<Vulture>(PlayerControl.LocalPlayer);

            if (__instance == role.EatButton)
            {
                if (role.CurrentTarget == null) {
                    return false;
                }
                var flag2 = __instance.isCoolingDown;
                if (flag2) return false;
                if (!__instance.enabled) return false;
                var maxDistance = GameOptionsData.KillDistances[PlayerControl.GameOptions.KillDistance];
                if (Vector2.Distance(role.CurrentTarget.TruePosition,
                    PlayerControl.LocalPlayer.GetTruePosition()) > maxDistance) return false;
                var playerId = role.CurrentTarget.ParentId;

                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                    (byte) CustomRPC.Inhale, SendOption.Reliable, -1);
                writer.Write(PlayerControl.LocalPlayer.PlayerId);
                writer.Write(playerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);

                Coroutines.Start(VultureCoroutine.EatCoroutine(role.CurrentTarget, role));
                return false;
            }

            return true;
        }
    }
}