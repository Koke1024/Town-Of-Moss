using Il2CppSystem;

namespace TownOfUs.Roles
{
    public class Janitor : Assassin
    {
        public ActionButton _cleanButton;

        public Janitor(PlayerControl player) : base(player)
        {
            Name = "Janitor";
            ImpostorText = () => "Clean up bodies";
            TaskText = () => "Clean bodies to prevent Crewmates from discovering them.";
            Color = Palette.ImpostorRed;
            RoleType = RoleEnum.Janitor;
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
        
        public ActionButton CleanButton
        {
            get => _cleanButton;
            set
            {
                _cleanButton = value;
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
            }
        }
    }
}