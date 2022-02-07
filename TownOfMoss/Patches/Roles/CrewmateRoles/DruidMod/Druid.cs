using System;
using Hazel;
using UnityEngine;
using System.Linq;
using InnerNet;
using Object = UnityEngine.Object;

namespace TownOfUs.Roles
{
    public class Druid : Undertaker {
        private static Vector2 _dragStartPosition;
        private int _revivedCount;
        
        public Druid(PlayerControl player) : base(player)
        {
            Name = "Druid";
            ImpostorText = () => "Revive dead by sorcery";
            TaskText = () => "Revive dead by sorcery";
            Color = new Color(0.4f, 0f, 0.56f);
            RoleType = RoleEnum.Druid;
            Faction = Faction.Crewmates;
            _revivedCount = 0;
        }
        
        public void DragStart(Vector2 startPos) {
            _dragStartPosition = startPos;
        }
        public void Drag() {
            var distance = Vector2.Distance(PlayerControl.LocalPlayer.GetTruePosition(), _dragStartPosition);
            if(distance > CustomGameOptions.DruidReviveRange / 2.0f){
                Revive();
            }
        }

        public bool CanRevive() {
            return _revivedCount < (int)CustomGameOptions.DruidReviveLimit;
        }

        public bool Revive() {
            var flag = PlayerControl.LocalPlayer.Is(RoleEnum.Druid);
            if (!flag) return true;
            if (!PlayerControl.LocalPlayer.CanMove) return false;
            if (PlayerControl.LocalPlayer.Data.IsDead) return false;
            var role = Role.GetRole<Druid>(PlayerControl.LocalPlayer);
            
            if (role.CurrentlyDragging == null) {
                return false;
            }
            
            var playerId = role.CurrentlyDragging.ParentId;

            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                (byte) CustomRPC.DruidRevive, SendOption.Reliable, -1);
            writer.Write(PlayerControl.LocalPlayer.PlayerId);
            writer.Write(playerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);

            DruidRevive(role.CurrentlyDragging, role);
            
            role.CurrentlyDragging = null;
            role.LastDragged = DateTime.UtcNow;
            return false;
        }
        
        public void DruidRevive(DeadBody target, Druid role)
        {
            var parentId = target.ParentId;
            var position = target.TruePosition;

            var revived = new System.Collections.Generic.List<PlayerControl>();

            var player = Utils.PlayerById(parentId);

            if (player == null || AmongUsClient.Instance.GameState != InnerNetClient.GameStates.Started) {
                return;
            }

            player.Revive();
            revived.Add(player);

            if (target != null) Object.Destroy(target.gameObject);

            if (revived.Any(x => x.AmOwner)) {
                try {
                    Minigame.Instance.Close();
                    Minigame.Instance.Close();
                }
                catch {
                }
            }
            _revivedCount += 1;
            if (!CanRevive()) {
                if (_dragDropButton) {
                    _dragDropButton.graphic.color = new Color(0, 0, 0, 0);
                }    
            }
        }

        public override void OnEndMeeting() {
            base.OnEndMeeting();
            _revivedCount = 0;
            if (Player.AmOwner) {
                _dragDropButton.graphic.color = new Color(1, 1, 1, 1);                
            }
        }
    }
}