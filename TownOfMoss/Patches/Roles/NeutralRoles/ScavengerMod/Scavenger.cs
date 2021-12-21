using TownOfUs.Roles;
using UnityEngine;

namespace TownOfUs.NeutralRoles.ScavengerMod
{
    public class Scavenger : Role {
        public int eatCount;  
        public KillButton _eatButton;
        public DeadBody CurrentTarget { get; set; }
        public Scavenger(PlayerControl player) : base(player)
        {
            Name = "Scavenger";
            ImpostorText = () => "Eat dead bodies";
            TaskText = () => "Eat dead bodies";
            Color = new Color(0.47f, 0.1f, 0.4f);
            RoleType = RoleEnum.Scavenger;
            Faction = Faction.Neutral;
            eatCount = 0;
        }
        
        public KillButton EatButton
        {
            get => _eatButton;
            set
            {
                _eatButton = value;
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
            }
        }

        protected override void IntroPrefix(IntroCutscene._CoBegin_d__18 __instance)
        {
            var myTeam = new Il2CppSystem.Collections.Generic.List<PlayerControl>();
            myTeam.Add(PlayerControl.LocalPlayer);
            __instance.yourTeam = myTeam;
        }

        public void Wins() {
            eatCount = CustomGameOptions.ScavengerWinCount;
        }

        public void Loses()
        {
            Player.Data.Role.TeamType = RoleTeamTypes.Impostor;
        }
        
        
        public ArrowBehaviour Arrow;
        public static Sprite Sprite => TownOfUs.Arrow;

        public void SetExecutionTarget()
        {
            var gameObj = new GameObject();
            Arrow = gameObj.AddComponent<ArrowBehaviour>();
            gameObj.transform.parent = PlayerControl.LocalPlayer.gameObject.transform;
            var renderer = gameObj.AddComponent<SpriteRenderer>();
            renderer.sprite = Sprite;
            Arrow.image = renderer;
            gameObj.layer = 5;
        }
        
        internal override bool EABBNOODFGL(ShipStatus __instance)
        {
            if (eatCount < CustomGameOptions.ScavengerWinCount) {
                return true;
            }
            Utils.EndGame();
            return false;
        }
        
        public override bool DidWin(GameOverReason gameOverReason) {
            return eatCount >= CustomGameOptions.ScavengerWinCount;
        }
    }
}