using Il2CppSystem;
using TownOfUs.ImpostorRoles.JanitorMod;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TownOfUs.Roles
{
    public class Janitor : Assassin
    {
        public KillButton _cleanButton;

        public Janitor(PlayerControl player) : base(player)
        {
            Name = "Janitor";
            ImpostorText = () => "Clean up bodies";
            TaskText = () => "Clean bodies to prevent Crewmates from discovering them.";
            Color = Palette.ImpostorRed;
            RoleType = RoleEnum.Janitor;
            Faction = Faction.Impostors;
        }

        public override void InitializeLocal() {
            base.InitializeLocal();

            lastCleaned = DateTime.UtcNow;
        }

        public DeadBody CurrentTarget { get; set; }

        public DateTime lastCleaned = new DateTime();

        public float CleanTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - lastCleaned;
            var num = CustomGameOptions.CleanCd * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }
        
        public KillButton CleanButton
        {
            get => _cleanButton;
            set
            {
                _cleanButton = value;
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
            }
        }

        public override void OnEndMeeting() {
            base.OnEndMeeting();
            lastCleaned = DateTime.UtcNow;
        }

        public override void PostHudUpdate(HudManager __instance) {
            base.PostHudUpdate(__instance);
            
            if (CleanButton == null)
            {
                CleanButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                CleanButton.graphic.enabled = true;
                CleanButton.GetComponent<AspectPosition>().DistanceFromEdge = TownOfUs.ButtonPosition;
                CleanButton.gameObject.SetActive(false);
            }

            CleanButton.GetComponent<AspectPosition>().Update();
            CleanButton.gameObject.SetActive(!PlayerControl.LocalPlayer.Data.IsDead && !MeetingHud.Instance);
            CleanButton.graphic.sprite = TownOfUs.JanitorClean;


            var data = PlayerControl.LocalPlayer.Data;
            var isDead = data.IsDead;
            var truePosition = PlayerControl.LocalPlayer.GetTruePosition();
            var maxDistance = GameOptionsData.KillDistances[PlayerControl.GameOptions.KillDistance];
            var flag = (PlayerControl.GameOptions.GhostsDoTasks || !data.IsDead) &&
                       (!AmongUsClient.Instance || !AmongUsClient.Instance.IsGameOver) &&
                       PlayerControl.LocalPlayer.CanMove;
            var allocs = Physics2D.OverlapCircleAll(truePosition, maxDistance,
                LayerMask.GetMask(new[] {"Players", "Ghost"}));
            var killButton = CleanButton;
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


            KillButtonTarget.SetTarget(killButton, closestBody, this);
            CleanButton.SetCoolDown(CleanTimer(), PlayerControl.GameOptions.KillCooldown);
        }
    }
}