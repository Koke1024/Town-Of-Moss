using System.Collections.Generic;
using System.Linq;
using Hazel;
using Il2CppSystem;
using UnityEngine;

namespace TownOfUs.Roles
{
    public class Vulture : Role {
        public int eatCount;  
        public KillButtonManager _eatButton;
        public new DeadBody CurrentTarget { get; set; }
        public Vulture(PlayerControl player) : base(player)
        {
            Name = "Vulture";
            ImpostorText = () => "You won't be killed!";
            TaskText = () => "You will revive after killed";
            Color = new Color(0.47f, 0.22f, 0f);
            RoleType = RoleEnum.Vulture;
            Faction = Faction.Neutral;
            eatCount = 0;
        }
        
        public KillButtonManager EatButton
        {
            get => _eatButton;
            set
            {
                _eatButton = value;
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
            }
        }

        protected override void IntroPrefix(IntroCutscene._CoBegin_d__14 __instance)
        {
            var myTeam = new Il2CppSystem.Collections.Generic.List<PlayerControl>();
            myTeam.Add(PlayerControl.LocalPlayer);
            __instance.yourTeam = myTeam;
        }

        public void Wins() {
            eatCount = CustomGameOptions.VultureWinCount;
        }

        public void Loses()
        {
            Player.Data.IsImpostor = true;
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
            if (eatCount < CustomGameOptions.VultureWinCount) {
                return true;
            }
            Utils.EndGame();
            return false;
        }
    }
}