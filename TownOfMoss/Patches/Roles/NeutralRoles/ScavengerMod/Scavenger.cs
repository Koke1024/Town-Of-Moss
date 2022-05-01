using TownOfUs.ImpostorRoles.ScavengerMod;
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

        protected override void IntroPrefix(IntroCutscene._CoBegin_d__19 __instance)
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

        public override void Outro(EndGameManager __instance) {
            base.Outro(__instance);
            NeutralOutro(__instance);
        }

        public override void PostHudUpdate(HudManager __instance) {
            base.PostHudUpdate(__instance);
            
            if (EatButton == null)
            {
                EatButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                
                EatButton.GetComponent<AspectPosition>().DistanceFromEdge = TownOfUs.ButtonPosition;
                EatButton.gameObject.SetActive(false);
                EatButton.graphic.sprite = TownOfUs.Inhale;
            }
            EatButton.GetComponent<AspectPosition>().Update();
            EatButton.gameObject.SetActive(!PlayerControl.LocalPlayer.Data.IsDead && !MeetingHud.Instance);


            var data = PlayerControl.LocalPlayer.Data;
            var isDead = data.IsDead;
            var truePosition = PlayerControl.LocalPlayer.GetTruePosition();
            var maxDistance = GameOptionsData.KillDistances[PlayerControl.GameOptions.KillDistance];
            var flag = (PlayerControl.GameOptions.GhostsDoTasks || !data.IsDead) &&
                       (!AmongUsClient.Instance || !AmongUsClient.Instance.IsGameOver) &&
                       PlayerControl.LocalPlayer.CanMove;
            var allocs = Physics2D.OverlapCircleAll(truePosition, maxDistance,
                LayerMask.GetMask(new[] {"Players", "Ghost"}));
            var killButton = EatButton;
            DeadBody closestBody = null;
            var closestDistance = float.MaxValue;

            foreach (var collider2D in allocs)
            {
                if (!flag || isDead || !collider2D.CompareTag("DeadBody")) continue;
                var component = collider2D.GetComponent<DeadBody>();
                if (!(Vector2.Distance(truePosition, component.TruePosition) <=
                      maxDistance)) continue;

                var distance = Vector2.Distance(truePosition, component.TruePosition);
                if (!(distance < closestDistance)) continue;
                closestBody = component;
                closestDistance = distance;
            }


            EatButtonTarget.SetTarget(killButton, closestBody, this);
            EatButton.SetCoolDown(0, 1.0f);
        }
    }
}