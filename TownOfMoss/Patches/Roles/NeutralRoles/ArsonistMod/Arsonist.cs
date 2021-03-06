using System;
using System.Collections.Generic;
using System.Linq;
using Hazel;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TownOfUs.Roles
{
    public class Arsonist : Role
    {
        private KillButton _igniteButton;
        public bool ArsonistWins;
        public PlayerControl ClosestPlayer;
        public List<byte> DousedPlayers = new List<byte>();
        public bool IgniteUsed;
        public DateTime LastDoused;


        public Arsonist(PlayerControl player) : base(player)
        {
            Name = "Arsonist";
            ImpostorText = () => "Douse players and ignite the light";
            TaskText = () => "Douse players and ignite to kill everyone\nFake Tasks:";
            Color = new Color(1f, 0.3f, 0f);
            RoleType = RoleEnum.Arsonist;
            Faction = Faction.Neutral;
        }

        public override void InitializeLocal() {
            base.InitializeLocal();
            
            LastDoused = DateTime.UtcNow;
        }

        public KillButton IgniteButton
        {
            get => _igniteButton;
            set
            {
                _igniteButton = value;
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
            }
        }

        internal override bool EABBNOODFGL(ShipStatus __instance)
        {
            if (PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected) == 0)
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(
                    PlayerControl.LocalPlayer.NetId,
                    (byte) CustomRPC.NeutralWin,
                    SendOption.Reliable,
                    -1
                );
                writer.Write((byte)RoleEnum.Arsonist);
                Wins();
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                Utils.EndGame();
                return false;
            }

            if (IgniteUsed || Player.Data.IsDead) return true;

            return !CustomGameOptions.ArsonistGameEnd;
        }


        public void Wins()
        {
            //System.Console.WriteLine("Reached Here - Glitch Edition");
            ArsonistWins = true;
        }

        public void Loses()
        {
            Player.Data.Role.TeamType = RoleTeamTypes.Impostor;
        }

        public bool CheckEveryoneDoused()
        {
            var arsoId = Player.PlayerId;
            foreach (var player in PlayerControl.AllPlayerControls)
            {
                if (
                    player.PlayerId == arsoId ||
                    player.Data.IsDead ||
                    player.Data.Disconnected
                ) continue;
                if (!DousedPlayers.Contains(player.PlayerId)) return false;
            }

            return true;
        }

        protected override void IntroPrefix(IntroCutscene._CoBegin_d__19 __instance)
        {
            var arsonistTeam = new Il2CppSystem.Collections.Generic.List<PlayerControl>();
            arsonistTeam.Add(PlayerControl.LocalPlayer);
            //__instance.yourTeam = arsonistTeam;
        }

        public float DouseTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastDoused;
            var num = CustomGameOptions.DouseCd * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }
        
        public override bool DidWin(GameOverReason gameOverReason) {
            return IgniteUsed;
        }

        public override void OnEndMeeting() {
            base.OnEndMeeting();
            LastDoused = DateTime.UtcNow;
        }
        public static Sprite IgniteSprite => TownOfUs.IgniteSprite;

        public override void PostHudUpdate(HudManager __instance) {
            base.PostHudUpdate(__instance);
            
            foreach (var playerId in DousedPlayers)
            {
                var player = Utils.PlayerById(playerId);
                var data = player?.Data;
                if (data == null || data.Disconnected || data.IsDead)
                    continue;

                // player.MyRend.material.SetColor("_VisorColor", Color);
                player.nameText.color = Color.black;
            }

            if (IgniteButton == null)
            {
                IgniteButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                IgniteButton.graphic.enabled = true;
                IgniteButton.GetComponent<AspectPosition>().DistanceFromEdge = TownOfUs.ButtonPosition;
                IgniteButton.gameObject.SetActive(false);
                IgniteButton.graphic.sprite = IgniteSprite;
            }
            IgniteButton.GetComponent<AspectPosition>().Update();

            IgniteButton.gameObject.SetActive(!PlayerControl.LocalPlayer.Data.IsDead && !MeetingHud.Instance);
            __instance.KillButton.gameObject.SetActive(!PlayerControl.LocalPlayer.Data.IsDead && !MeetingHud.Instance);
            IgniteButton.SetCoolDown(0f, 1f);
            __instance.KillButton.SetCoolDown(DouseTimer(), CustomGameOptions.DouseCd);

            var notDoused = PlayerControl.AllPlayerControls.ToArray().Where(
                player => !DousedPlayers.Contains(player.PlayerId)
            ).ToList();

            Utils.SetTarget(ref ClosestPlayer, __instance.KillButton, float.NaN, notDoused);


            if (!IgniteButton.isCoolingDown & IgniteButton.isActiveAndEnabled & !IgniteUsed &
                CheckEveryoneDoused())
            {
                IgniteButton.graphic.color = Palette.EnabledColor;
                IgniteButton.graphic.material.SetFloat("_Desat", 0f);
                return;
            }

            IgniteButton.graphic.color = Palette.DisabledClear;
            IgniteButton.graphic.material.SetFloat("_Desat", 1f);
        }

        public override void Outro(EndGameManager __instance) {
            base.Outro(__instance);
            NeutralOutro(__instance);
        }
    }
}
