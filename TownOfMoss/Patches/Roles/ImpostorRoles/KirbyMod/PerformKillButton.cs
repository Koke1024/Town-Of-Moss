using HarmonyLib;
using Hazel;
using Reactor;
using TownOfUs.Roles;
using UnityEngine;

namespace TownOfUs.ImpostorRoles.KirbyMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class DoClickButton

    {
        public static bool Prefix(KillButton __instance)
        {
            var flag = PlayerControl.LocalPlayer.Is(RoleEnum.Kirby);
            if (!flag) return true;
            if (!PlayerControl.LocalPlayer.CanMove) return false;
            if (PlayerControl.LocalPlayer.Data.IsDead) return false;
            var role = Role.GetRole<Kirby>(PlayerControl.LocalPlayer);

            if (__instance == role.InhaleButton)
            {
                if (role.Morphed) {
                    var writer2 = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                        (byte) CustomRPC.Spit,
                        SendOption.Reliable, -1);
                    writer2.Write(PlayerControl.LocalPlayer.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer2);
                    role.Spit();
                    return false;
                }

                if (role.CurrentTarget == null) {
                    return false;
                }
                var flag2 = __instance.isCoolingDown;
                if (flag2) return false;
                if (!__instance.enabled) return false;
                var maxDistance = GameOptionsData.KillDistances[0];
                if (Vector2.Distance(role.CurrentTarget.TruePosition,
                    PlayerControl.LocalPlayer.GetTruePosition()) > maxDistance) return false;
                var playerId = role.CurrentTarget.ParentId;

                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                    (byte) CustomRPC.Inhale, SendOption.Reliable, -1);
                writer.Write(PlayerControl.LocalPlayer.PlayerId);
                writer.Write(playerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);

                Coroutines.Start(KirbyCoroutine.inhaleCoroutine(role.CurrentTarget, role));
                return false;
            }

            return true;
        }
    }
}