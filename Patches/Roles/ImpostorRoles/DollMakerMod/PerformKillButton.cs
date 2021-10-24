using HarmonyLib;
using Hazel;
using Il2CppSystem;
using Reactor;
using TownOfUs.Extensions;
using TownOfUs.Roles;
using UnityEngine;

namespace TownOfUs.ImpostorRoles.DollMakerMod
{
    [HarmonyPatch(typeof(KillButtonManager), nameof(KillButtonManager.PerformKill))]
    public class PerformKillButton

    {
        public static bool Prefix(KillButtonManager __instance)
        {
            var flag = PlayerControl.LocalPlayer.Is(RoleEnum.DollMaker);
            if (!flag) return true;
            if (!PlayerControl.LocalPlayer.CanMove) return false;
            if (PlayerControl.LocalPlayer.Data.IsDead) return false;
            var role = Role.GetRole<DollMaker>(PlayerControl.LocalPlayer);

            var flag2 = __instance.isCoolingDown;
            if (flag2) return false;
            AmongUsExtensions.Log($"{__instance.enabled}");
            if (!__instance.enabled) return false;
            
            var maxDistance = GameOptionsData.KillDistances[PlayerControl.GameOptions.KillDistance];
            if (Vector2.Distance(role.ClosestPlayer.GetTruePosition(),
                PlayerControl.LocalPlayer.GetTruePosition()) > maxDistance) return false;
            
            AmongUsExtensions.Log($"nearest:{role.ClosestPlayer.PlayerId}");
            if (role.DollList.ContainsKey(role.ClosestPlayer.PlayerId)) {
                return false;
            }

            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                (byte) CustomRPC.Wax, SendOption.Reliable, -1);
            writer.Write(role.Player.PlayerId);
            writer.Write(role.ClosestPlayer.PlayerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            role.DollList.Add(role.ClosestPlayer.PlayerId, 0);

            Utils.AirKill(role.Player, role.ClosestPlayer);
            SoundManager.Instance.PlaySound(PlayerControl.LocalPlayer.KillSfx, false, 0.8f);
            __instance.SetCoolDown(role.Player.killTimer, PlayerControl.GameOptions.KillCooldown);
            role.Player.killTimer = PlayerControl.GameOptions.KillCooldown;
            return false;
        }
    }
}