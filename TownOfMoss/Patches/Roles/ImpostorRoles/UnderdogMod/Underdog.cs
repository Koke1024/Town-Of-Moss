using TownOfUs.ImpostorRoles.UnderdogMod;

namespace TownOfUs.Roles
{
    public class Underdog : Assassin
    {
        public Underdog(PlayerControl player) : base(player)
        {
            Name = "Underdog";
            ImpostorText = () => "Use your comeback power to win";
            TaskText = () => "long kill cooldown when 2 imps, short when 1 imp";
            Color = Palette.ImpostorRed;
            RoleType = RoleEnum.Underdog;
            Faction = Faction.Impostors;
        }

        public float MaxTimer() => PlayerControl.GameOptions.KillCooldown * (
            DoClick.LastImp() ? 0.5f : 1.5f
        );

        public void SetKillTimer()
        {
            Player.SetKillTimer(MaxTimer());
        }

        public override void PostKill(PlayerControl target) {
            SetKillTimer();
        }

        public override void OnEndMeeting() {
            base.OnEndMeeting();
            SetKillTimer();
        }
    }
}
