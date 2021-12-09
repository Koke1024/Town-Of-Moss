using System;
using HarmonyLib;
using Hazel;
using TownOfUs.Roles;
using Il2CppSystem;
using DateTime = Il2CppSystem.DateTime;

namespace TownOfUs.CrewmateRoles.BodyGuardMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class Protect
    {
        public static bool Prefix(KillButton __instance)
        {
            if (__instance != DestroyableSingleton<HudManager>.Instance.KillButton) return true;
            var flag = PlayerControl.LocalPlayer.Is(RoleEnum.BodyGuard);
            if (!flag) return true;
            var role = Role.GetRole<BodyGuard>(PlayerControl.LocalPlayer);
            if (!PlayerControl.LocalPlayer.CanMove) return false;
            if (PlayerControl.LocalPlayer.Data.IsDead) return false;
            if (role.ClosestPlayer == null) return false;
            var flag2 = role.ShieldTimer() == 0f;
            if (!flag2) return false;

            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                (byte) CustomRPC.Protect, SendOption.Reliable, -1);
            writer.Write(PlayerControl.LocalPlayer.PlayerId);
            writer.Write(role.ClosestPlayer.PlayerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);

            role.ShieldedPlayer = role.ClosestPlayer;
            role.ShieldedTime = DateTime.UtcNow;
            role.SetProtectionTarget();
            return false;
        }
    }
}
