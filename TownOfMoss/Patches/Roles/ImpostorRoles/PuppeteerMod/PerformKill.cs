using System;
using HarmonyLib;
using Hazel;
using Reactor;
using TownOfUs.Extensions;
using TownOfUs.Roles;
using UnityEngine;
using DateTime = Il2CppSystem.DateTime;

namespace TownOfUs.ImpostorRoles.PuppeteerMod
{
    [HarmonyPatch(typeof(ActionButton), nameof(ActionButton.PerformKill))]
    public class PossessButtonKill
    {

        public static bool Prefix(ActionButton __instance)
        {
            var flag = PlayerControl.LocalPlayer.Is(RoleEnum.Puppeteer);
            if (!flag) return true;
            // if (!PlayerControl.LocalPlayer.CanMove) return false;
            if (PlayerControl.LocalPlayer.Data.IsDead) return false;

            var role = Role.GetRole<Puppeteer>(PlayerControl.LocalPlayer);
            var target = role.ClosestPlayer;


            if ((role.lastPossess - DateTime.UtcNow).TotalMilliseconds / 1000.0f + PlayerControl.GameOptions.KillCooldown > 0) {
                return false;
            }
            if (role.Player.killTimer > 0) {
                return false;
            }
            
            if (__instance.renderer.sprite == Puppeteer.PossessSprite)
            {
                if (target == null) return false;
                if (role.duration > 0) return false;

                Coroutines.Start(PuppeteerCoroutine.Possessing(target, role));
                return false;
            }
            if(__instance.renderer.sprite == Puppeteer.UnPossessSprite){
                role.duration = CustomGameOptions.ReleaseWaitTime;
                // role.duration = Mathf.Max(role.PossessTime, 3.0f);
                
                var writer2 = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                    (byte) CustomRPC.UnPossess,
                    SendOption.Reliable, -1);
                writer2.Write(PlayerControl.LocalPlayer.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer2);
                
                role.UnPossess();
                return false;
            }

            return true;
        }
    }
    
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.MurderPlayer))]
    public class PerformKill
    {
        public static void Postfix(PlayerControl __instance, [HarmonyArgument(0)] PlayerControl target)
        {
            var role = Role.GetRole(__instance);
            if (role?.RoleType == RoleEnum.Puppeteer) {
                ((Puppeteer)role).lastPossess = DateTime.UtcNow;
            }
        }
    }
}
