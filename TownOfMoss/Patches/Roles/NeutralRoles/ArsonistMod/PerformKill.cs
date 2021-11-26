using System;
using HarmonyLib;
using Hazel;
using Reactor;
using TownOfUs.NeutralRoles.ArsonistMod;
using TownOfUs.Roles;

namespace TownOfUs.NeutralRoles.ArsonistMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class DoClick
    {
        public static bool Prefix(KillButton __instance)
        {
            var flag = PlayerControl.LocalPlayer.Is(RoleEnum.Arsonist);
            if (!flag) return true;
            if (PlayerControl.LocalPlayer.Data.IsDead) return false;
            if (!PlayerControl.LocalPlayer.CanMove) return false;
            var role = Role.GetRole<Arsonist>(PlayerControl.LocalPlayer);
            if (role.IgniteUsed) return false;
            if (__instance == role.IgniteButton)
            {
                if (!__instance.isActiveAndEnabled) return false;
                if (!role.CheckEveryoneDoused()) return false;
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                    (byte) CustomRPC.Ignite, SendOption.Reliable, -1);
                writer.Write(PlayerControl.LocalPlayer.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                Ignite(role);
                return false;
            }

            if (__instance != DestroyableSingleton<HudManager>.Instance.KillButton) return true;
            if (!__instance.isActiveAndEnabled) return false;
            if (role.ClosestPlayer == null) return false;
            if (role.DouseTimer() != 0) return false;
            if (role.DousedPlayers.Contains(role.ClosestPlayer.PlayerId)) return false;

            __instance.SetTarget(null);
            Coroutines.Start(ArsonistCoroutine.Dousing(role.ClosestPlayer, Role.GetRole<Arsonist>(role.Player)));
            return false;
        }

        public static void Ignite(Arsonist role)
        {
            Coroutines.Start(ArsonistCoroutine.Ignite(role));
        }
    }
}
