using HarmonyLib;
using Hazel;
using Reactor;
using Reactor.Extensions;
using TownOfUs.Extensions;
using TownOfUs.Roles;
using UnityEngine;

namespace TownOfUs.CrewmateRoles.BodyGuardMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class StopKill
    {
        public static void BreakShield(byte bodyGuardId, byte targetId, byte killerId, bool flag)
        {
            switch (CustomGameOptions.NotificationShield) {
                case NotificationOptions.Everyone:
                case NotificationOptions.Shielded when PlayerControl.LocalPlayer.PlayerId == targetId:
                case NotificationOptions.BodyGuard when PlayerControl.LocalPlayer.PlayerId == bodyGuardId:
                    Coroutines.Start(Utils.FlashCoroutine(new Color(0f, 0.47f, 0.23f)));
                    if (PlayerControl.LocalPlayer.PlayerId != killerId) {
                        SoundManager.Instance.PlaySound(PlayerControl.LocalPlayer.KillSfx, false, 0.8f);                        
                    }
                    break;
            }

            if (!flag)
                return;

            foreach (var role in Role.GetRoles(RoleEnum.BodyGuard)) {
                if (((BodyGuard)role).ShieldedPlayer.PlayerId == targetId) {
                    ((BodyGuard)role).ShieldedPlayer = null;
                    // ((BodyGuard)role).Defended = true;
                    if (role.Player.AmOwner) {
                        ((BodyGuard)role).Arrow.gameObject.Destroy();
                    }

                    if (CustomGameOptions.DieOnGuard) {
                        Utils.RpcMurderPlayer(role.Player, role.Player);
                        Utils.RpcOverrideDeadBodyInformation(bodyGuardId, killerId);
                    }
                }
            }
        }

        [HarmonyPriority(Priority.First)]
        public static bool Prefix(KillButton __instance)
        {
            if (__instance != DestroyableSingleton<HudManager>.Instance.KillButton) return true;
            if (!PlayerControl.LocalPlayer.Data.IsImpostor()) return true;
            var target = __instance.currentTarget;
            if (target == null) return true;
            var killer = PlayerControl.LocalPlayer;
            if (target.isShielded())
            {
                if (__instance.isActiveAndEnabled && !__instance.isCoolingDown)
                {
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                        (byte) CustomRPC.AttemptSound, SendOption.Reliable, -1);
                    writer.Write(target.getBodyGuard().Player.PlayerId);
                    writer.Write(target.PlayerId);
                    writer.Write(PlayerControl.LocalPlayer.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);

                    System.Console.WriteLine(CustomGameOptions.ShieldBreaks + "- shield break");
                    if (CustomGameOptions.ShieldBreaks) {
                        PlayerControl.LocalPlayer.SetKillTimer(PlayerControl.GameOptions.KillCooldown);
                        killer.GetRole().PostKill(target);
                    }

                    BreakShield(target.getBodyGuard().Player.PlayerId, target.PlayerId, PlayerControl.LocalPlayer.Data.PlayerId, CustomGameOptions.ShieldBreaks);
                }
                return false;
            }
            return true;
        }
    }
}