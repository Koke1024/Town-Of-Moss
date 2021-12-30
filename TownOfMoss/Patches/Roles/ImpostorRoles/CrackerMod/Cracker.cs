using System;
using System.Collections.Generic;

namespace TownOfUs.Roles
{
    public class Cracker : Assassin
    {
        public KillButton _crackButton;
        
        public static SystemTypes? MyLastRoom;

        public static Queue<(DateTime, SystemTypes)> BlackoutRooms = new Queue<(DateTime, SystemTypes)>();

        public Cracker(PlayerControl player) : base(player)
        {
            Name = "Cracker";
            ImpostorText = () => "The room is in your palm";
            TaskText = () => "Crack rooms to disturb crews";
            Color = Palette.ImpostorRed;
            RoleType = RoleEnum.Cracker;
            Faction = Faction.Impostors;
            
            MyLastRoom = null;  //local player's current room
            BlackoutRooms.Clear();
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

        public static bool IsBlackout(SystemTypes? roomId) {
            if (roomId == null) {
                return false;
            }
            foreach (var row in BlackoutRooms) {
                if (row.Item2 == roomId) {
                    if (DateTime.UtcNow < row.Item1.AddSeconds(CustomGameOptions.BlackoutDur)) {
                        return true;                        
                    }
                }
            }
            return false;
        }

        public static bool IsCracked(SystemTypes? roomId) {
            if (roomId == null) {
                return false;
            }
            foreach (var row in BlackoutRooms) {
                if (row.Item2 == roomId) {
                    if (DateTime.UtcNow < row.Item1.AddSeconds(CustomGameOptions.CrackDur)) {
                        return true;                        
                    }
                }
            }
            return false;
        }
    }
}