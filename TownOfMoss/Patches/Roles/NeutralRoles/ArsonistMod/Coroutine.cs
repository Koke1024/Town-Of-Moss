using System;
using System.Collections;
using Hazel;
using Reactor;
using TownOfUs.Roles;
using UnityEngine;

namespace TownOfUs.NeutralRoles.ArsonistMod
{
    public class ArsonistCoroutine
    {
        public static ArrowBehaviour Arrow;
        public static PlayerControl Target;
        public static Sprite Sprite => TownOfUs.Arrow;

        public static IEnumerator Dousing(PlayerControl target, Arsonist arso)
        {
            var start = DateTime.UtcNow;
            arso.LastDoused = DateTime.UtcNow.AddSeconds(-CustomGameOptions.DouseCd + CustomGameOptions.ArsonistDouseTime);
            while (true) {
                var distBetweenPlayers = Utils.getDistBetweenPlayers(PlayerControl.LocalPlayer, target);
                var flag3 = distBetweenPlayers <
                            GameOptionsData.KillDistances[0];
                if (!flag3) {
                    arso.LastDoused = DateTime.UtcNow;
                    yield break;
                }
                yield return null;
                
                if ((DateTime.UtcNow - start).TotalMilliseconds >= CustomGameOptions.ArsonistDouseTime * 1000.0f) {
                    // arso.LastDoused = DateTime.UtcNow.AddSeconds(-CustomGameOptions.DouseCd);
                    break;
                }
            }

            var writer2 = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                (byte) CustomRPC.Douse, SendOption.Reliable, -1);
            writer2.Write(PlayerControl.LocalPlayer.PlayerId);
            writer2.Write(target.PlayerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer2);
            arso.DousedPlayers.Add(target.PlayerId);
            arso.LastDoused = DateTime.UtcNow;
            
            SoundManager.Instance.PlaySound(PlayerControl.LocalPlayer.KillSfx, false, 0.8f);
            yield break;
        }

        public static IEnumerator Ignite(Arsonist role) {
            role.IgniteUsed = true;
            role.Wins();
            Coroutines.Start(Utils.FlashCoroutine(new Color(1.0f, 0, 0, 1.0f), 5.0f));
            ShipStatus.Instance.StartCoroutine(Effects.SwayX(Camera.main.transform, 5.0f, 0.15f));
            yield return new WaitForSeconds(3.0f);
            Utils.EndGame();
        }
    }
}