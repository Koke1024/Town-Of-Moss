using System;
using System.Collections;
using System.Collections.Generic;
using Epic.OnlineServices.PlayerDataStorage;
using Reactor;
using UnityEngine;

namespace TownOfUs.Roles
{
    public class Cracker : Assassin
    {
        public KillButtonManager _crackButton;
        public DateTime LastCracked;
        public SystemTypes? TargetRoom;

        public SystemTypes? HackingRoom;
        public DateTime? RoomDetected;
        
        public SystemTypes? MyLastRoom;

        public Cracker(PlayerControl player) : base(player)
        {
            Name = "Cracker";
            ImpostorText = () => "Crack rooms to disturb crews";
            TaskText = () => "Crack rooms to disturb crews";
            Color = Palette.ImpostorRed;
            RoleType = RoleEnum.Cracker;
            Faction = Faction.Impostors;
            
            MyLastRoom = null;  //local player's current room
            LastCracked = DateTime.UtcNow;
            HackingRoom = null;
        }

        public KillButtonManager CrackButton
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

        public static IEnumerator HackRoomCoroutine(SystemTypes roomId, Cracker role) {
            role.RoomDetected = DateTime.UtcNow;
            while (true) {
                HudManager.Instance.ReportButton.enabled = true;
                
                if ((DateTime.UtcNow - role.RoomDetected).Value.Seconds > CustomGameOptions.CrackDur) {
                    role.HackingRoom = null;
                    role.RoomDetected = null;
                    break;
                }

                if (roomId != role.HackingRoom) {   //another room hacked
                    break;
                }

                if (role.MyLastRoom != roomId) {
                    yield return null;
                    continue;
                }
                    
                HudManager.Instance.ReportButton.enabled = false;
                HudManager.Instance.ReportButton.SetActive(false);

                if (Minigame.Instance) {
                    if (Minigame.Instance.TaskType != TaskTypes.ResetReactor &&
                        Minigame.Instance.TaskType != TaskTypes.RestoreOxy &&
                        Minigame.Instance.TaskType != TaskTypes.FixLights &&
                        Minigame.Instance.TaskType != TaskTypes.FixComms
                        ) {
                        Minigame.Instance.Close();
                        Minigame.Instance.Close();
                    }
                }

                if (MapBehaviour.Instance && !PlayerControl.LocalPlayer.Is(Faction.Impostors)) {
                    MapBehaviour.Instance.Close();
                    MapBehaviour.Instance.Close();
                }
                yield return null;
            }

            yield break;
        }
    }
}