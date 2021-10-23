using Il2CppSystem;

namespace TownOfUs.Roles
{
    public class DollMaker : Assassin
    {
        public KillButtonManager _waxButton;

        public DollMaker(PlayerControl player) : base(player)
        {
            Name = "DollMaker";
            ImpostorText = () => "Make crews art";
            TaskText = () => "Wax Crews to make them dolls.";
            Color = Palette.ImpostorRed;
            RoleType = RoleEnum.DollMaker;
            Faction = Faction.Impostors;
            
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
        
        public KillButtonManager WaxButton
        {
            get => _waxButton;
            set
            {
                _waxButton = value;
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
            }
        }
    }
}