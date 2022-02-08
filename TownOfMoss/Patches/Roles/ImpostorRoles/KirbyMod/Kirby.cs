using Il2CppSystem;
using TownOfUs.ImpostorRoles.KirbyMod;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TownOfUs.Roles
{
    public class Kirby : Morphling
    {
        public KillButton _inhaleButton;
        public DeadBody _aten;
        public new DeadBody CurrentTarget { get; set; }
        public DateTime eatTime = new DateTime();

        public Kirby(PlayerControl player) : base(player)
        {
            Name = "Popopo";
            ImpostorText = () => "Twinkle Hungry Ball";
            TaskText = () => "Twinkle Hungry Ball";
            Color = Palette.ImpostorRed;
            RoleType = RoleEnum.Kirby;
            Faction = Faction.Impostors;
            Color = new Color(0.66f, 0.42f, 0.64f);
        }

        public override void Initialize() {
            base.Initialize();

            eatTime = DateTime.UtcNow;
            _aten = null;
        }

        public KillButton InhaleButton
        {
            get => _inhaleButton;
            set
            {
                _inhaleButton = value;
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
            }
        }

        public void CopyCrew(DeadBody body) {
            TimeRemaining = CustomGameOptions.CopyDuration;
            MorphedPlayer = SampledPlayer;
            Utils.Morph(Player, SampledPlayer, true);
            eatTime = DateTime.Now;
            _aten = body;
            body.Reported = true;
            body.myCollider.tag = "Untagged";
            body.bodyRenderer.enabled = false;
        }
        
        public void Spit() {
            if (_aten == null) {
                return;
            }
            _aten.bodyRenderer.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            _aten.bodyRenderer.enabled = true;
            _aten.Reported = false;
            _aten.myCollider.tag = "DeadBody";
            _aten.transform.position = Player.transform.position;
            
            _aten = null;
                    
            TimeRemaining = 0;
            Unmorph();
        }

        public override void OnEndMeeting() {
            base.OnEndMeeting();
            SampledPlayer = null;
            _aten = null;
            Unmorph();
            LastMorphed = DateTime.UtcNow;
        }

        public override void PostFixedUpdateLocal() {
            base.PostFixedUpdateLocal();
            if (Morphed) {
                Player.killTimer += Time.fixedDeltaTime;                
            }
        }

        public override void PostHudUpdate(HudManager __instance) {
            if (InhaleButton == null)
            {
                InhaleButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                InhaleButton.graphic.enabled = true;
                InhaleButton.GetComponent<AspectPosition>().DistanceFromEdge = TownOfUs.ButtonPosition;
                InhaleButton.gameObject.SetActive(false);
                InhaleButton.graphic.sprite = TownOfUs.Inhale;
            }
            InhaleButton.GetComponent<AspectPosition>().Update();
            InhaleButton.gameObject.SetActive(!PlayerControl.LocalPlayer.Data.IsDead && !MeetingHud.Instance && !LobbyBehaviour.Instance);


            var data = PlayerControl.LocalPlayer.Data;
            var isDead = data.IsDead;
            var truePosition = PlayerControl.LocalPlayer.GetTruePosition();
            var maxDistance = GameOptionsData.KillDistances[0];
            var flag = (PlayerControl.GameOptions.GhostsDoTasks || !data.IsDead) &&
                       (!AmongUsClient.Instance || !AmongUsClient.Instance.IsGameOver) &&
                       PlayerControl.LocalPlayer.CanMove;
            var allocs = Physics2D.OverlapCircleAll(truePosition, maxDistance,
                LayerMask.GetMask(new[] {"Players", "Ghost"}));
            var killButton = InhaleButton;
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

            if (Morphed) {
                InhaleButton.SetCoolDown(TimeRemaining, CustomGameOptions.CopyDuration);
                
                InhaleButton.graphic.color = Palette.EnabledColor;
                InhaleButton.graphic.material.SetFloat("_Desat", 0f);
                InhaleButton.graphic.material.SetFloat("_Percent", 0);
                InhaleButton.cooldownTimerText.color = Palette.AcceptedGreen;
                return;
            }
            InhaleButton.cooldownTimerText.color = Palette.EnabledColor;

            InhaleButtonTarget.SetTarget(killButton, closestBody, this);
            InhaleButton.SetCoolDown(0, 1.0f);
            
        }
    }
}