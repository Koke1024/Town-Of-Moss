using System;
using System.Diagnostics.Tracing;
using HarmonyLib;
using Hazel;
using Reactor.Extensions;
using Rewired;
using TMPro;
using TownOfUs.Extensions;
using TownOfUs.Patches.ImpostorRoles.AssassinMod;
using TownOfUs.Roles;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace TownOfUs.ImpostorRoles.AssassinMod
{
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
    public class AddButton
    {
        private static Sprite TargetSprite => TownOfUs.TargetSprite;

        public static bool IsExempt(PlayerVoteArea voteArea) {
            if (voteArea.AmDead) return true;
            var player = Utils.PlayerById(voteArea.TargetPlayerId);
            if (
                player == null ||
                player.Data.IsDead ||
                player.Data.Disconnected
            ) return true;
            if (CustomGameOptions.AssassinGuessImpostors && player.Is(Faction.Impostors)) {
                return false;
            }
            var role = Role.GetRole(player);
            return role != null && role.Criteria();
        }
        
        public static void GenButton(Assassin role, PlayerVoteArea voteArea)
        {
            var targetId = voteArea.TargetPlayerId;
            if (IsExempt(voteArea))
            {
                role.Buttons[targetId] = (null);
                return;
            }
            
            var confirmButton = voteArea.Buttons.transform.GetChild(0).gameObject;
            var parent = confirmButton.transform.parent.parent;
        
            var guess = Object.Instantiate(confirmButton, voteArea.transform);
            
            var guessRenderer = guess.GetComponent<SpriteRenderer>();
            guessRenderer.sprite = TargetSprite;
            guess.transform.localPosition = new Vector3(-0.35f, 0f, -2f);
            guess.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            guess.layer = 5;
            guess.transform.parent = parent;
            var guessEvent = new Button.ButtonClickedEvent();
            guessEvent.AddListener((Action)(() => { ListShoot.guesserOnClick((int)voteArea.TargetPlayerId, voteArea); }));
            guess.GetComponent<PassiveButton>().OnClick = guessEvent;
            var bounds = guess.GetComponent<SpriteRenderer>().bounds;
            bounds.size = new Vector3(0.52f, 0.3f, 0.16f);
            var guessCollider = guess.GetComponent<BoxCollider2D>();
            guessCollider.size = guessRenderer.sprite.bounds.size;
            guessCollider.offset = Vector2.zero;
            guess.transform.GetChild(0).gameObject.Destroy();


            role.Guesses.Add(targetId, "None");
            role.Buttons[targetId] = guess;
        }

        // private static Action Cycle(Assassin role, PlayerVoteArea voteArea, TextMeshPro nameText)
        // {
        //     void Listener()
        //     {
        //         if (MeetingHud.Instance.state == MeetingHud.VoteStates.Discussion) return;
        //         var currentGuess = role.Guesses[voteArea.TargetPlayerId];
        //         var guessIndex = currentGuess == "None"
        //             ? -1
        //             : role.PossibleGuesses.IndexOf(currentGuess);
        //         if (++guessIndex == role.PossibleGuesses.Count)
        //             guessIndex = 0;
        //
        //         var newGuess = role.Guesses[voteArea.TargetPlayerId] = role.PossibleGuesses[guessIndex];
        //
        //         nameText.text = newGuess == "None"
        //             ? "Guess"
        //             : $"<color=#{role.ColorMapping[newGuess].ToHtmlStringRGBA()}>{newGuess}</color>";
        //     }
        //
        //     return Listener;
        // }

        public static void Postfix(MeetingHud __instance) {
            if (!PlayerControl.LocalPlayer.CanSnipe()) {
                return;
            }

            if (PlayerControl.LocalPlayer.Data.IsDead) return;

            var assassin = Role.GetRole<Assassin>(PlayerControl.LocalPlayer);
            assassin.Guesses.Clear();
            assassin.Buttons.Clear();
            assassin.GuessedThisMeeting = false;

            if (assassin.RemainingKills <= 0) return;
            foreach (var voteArea in __instance.playerStates)
            {
                GenButton(assassin, voteArea);
            }
        }
    }

    public static class ShootExecute {
        

        public static void Guess(Assassin role, PlayerVoteArea voteArea, string currentGuess)
        {
            if (
                MeetingHud.Instance.state == MeetingHud.VoteStates.Discussion ||
                AddButton.IsExempt(voteArea) || PlayerControl.LocalPlayer.Data.IsDead
            ) return;
            var targetId = voteArea.TargetPlayerId;
            if (currentGuess == "None") return;

            var playerRole = Role.GetRole(voteArea);

            var toDie = playerRole.Name == currentGuess ? playerRole.Player : role.Player;
            if (currentGuess == "Impostor" && playerRole.Faction == Faction.Impostors) {
                toDie = playerRole.Player;
            }
            
            if (role.Player.Is(RoleEnum.Sniper) && toDie.PlayerId != role.Player.PlayerId) {
                ((Sniper)role).KilledCount++;
                
                if (((Sniper)role).KilledCount >= CustomGameOptions.SniperWinCnt) {
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                        (byte) CustomRPC.SniperWin, SendOption.Reliable, -1);
                    writer.Write(role.Player.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    Utils.EndGame();
                }
            }

            AssassinKill.RpcMurderPlayer(toDie);
            role.RemainingKills--;
            ShowHideButtons.HideSingle(role, targetId, toDie == role.Player);
            if (toDie.isLover() && CustomGameOptions.BothLoversDie)
            {
                var lover = ((Lover)playerRole).OtherLover.Player;
                ShowHideButtons.HideSingle(role, lover.PlayerId, false);
            }

            return;
        }
    }
}
