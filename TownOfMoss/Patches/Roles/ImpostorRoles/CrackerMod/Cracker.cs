using System;
using System.Collections;

namespace TownOfUs.Roles
{
    public class Cracker : Assassin
    {
        public KillButton _crackButton;
        public DateTime LastCracked;
        public SystemTypes? TargetRoom;

        public SystemTypes? HackingRoom;
        public DateTime? RoomDetected;
        
        public static SystemTypes? MyLastRoom;
        public static bool InCrackedRoom;

        public SystemTypes? BlackOutRoomId;

        public Cracker(PlayerControl player) : base(player)
        {
            Name = "Cracker";
            ImpostorText = () => "The room is in your palm";
            TaskText = () => "Crack rooms to disturb crews";
            Color = Palette.ImpostorRed;
            RoleType = RoleEnum.Cracker;
            Faction = Faction.Impostors;
            
            MyLastRoom = null;  //local player's current room
            LastCracked = DateTime.UtcNow;
            HackingRoom = null;
        }

        public KillButton CrackButton
        {
            get => _crackButton;
            set
            {
                _crackButton = value;
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
            }
        }

        public float CrackTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastCracked;
            var num = CustomGameOptions.CrackCd * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }
        
        public void CrackRoom(SystemTypes? targetRoom) {
            HackingRoom = targetRoom;
            RoomDetected = null;
            LastCracked = DateTime.UtcNow;
        }
    }
}