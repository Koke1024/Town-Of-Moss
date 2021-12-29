using HarmonyLib;
using Hazel;
using Il2CppSystem;
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
        public static void BreakShield(byte bodyGuardId, byte playerId, bool flag)
        {
            if (CustomGameOptions.NotificationShield == NotificationOptions.Everyone) {
                Coroutines.Start(Utils.FlashCoroutine(new Color(0f, 0.47f, 0.23f)));
            } else if (CustomGameOptions.NotificationShield == NotificationOptions.Shielded && PlayerControl.LocalPlayer.PlayerId == playerId) {
                Coroutines.Start(Utils.FlashCoroutine(new Color(0f, 0.47f, 0.23f)));
            } else if (CustomGameOptions.NotificationShield == NotificationOptions.BodyGuard && PlayerControl.LocalPlayer.PlayerId == bodyGuardId) {
                Coroutines.Start(Utils.FlashCoroutine(new Color(0f, 0.47f, 0.23f)));
            }

            if (!flag)
                return;

            foreach (var role in Role.GetRoles(RoleEnum.BodyGuard)) {
                if (((BodyGuard)role).ShieldedPlayer.PlayerId == playerId) {
                    ((BodyGuard)role).ShieldedPlayer = null;
                    // ((BodyGuard)role).Defended = true;
                    if (role.Player.AmOwner) {
                        ((BodyGuard)role).Arrow.gameObject.Destroy();
                    }

                    if (CustomGameOptions.DieOnGuard) {
                        Utils.RpcMurderPlayer(role.Player, role.Player);                        
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
                    AmongUsClient.Instance.FinishRpcImmediately(writer);

                    System.Console.WriteLine(CustomGameOptions.ShieldBreaks + "- shield break");
                    if (CustomGameOptions.ShieldBreaks) {
                        if (killer.Is(RoleEnum.MultiKiller)) {
                            MultiKiller mk = Role.GetRole<MultiKiller>(killer);
                            if (!mk.KilledOnce) {
                                killer.SetKillTimer(0);
                            }
                            else {
                                killer.SetKillTimer(mk.MaxTimer);
                            }
                            mk.KilledOnce = !mk.KilledOnce;
                            mk.FirstKillTime = DateTime.UtcNow;
                        }
                        else {
                            PlayerControl.LocalPlayer.SetKillTimer(PlayerControl.GameOptions.KillCooldown);
                        }
                    }

                    BreakShield(target.getBodyGuard().Player.PlayerId, target.PlayerId, CustomGameOptions.ShieldBreaks);
                }


                return false;
            }


            return true;
        }
    }
}