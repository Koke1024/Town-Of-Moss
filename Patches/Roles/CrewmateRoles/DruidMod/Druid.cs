using System;
using HarmonyLib;
using Hazel;
using Reactor;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using InnerNet;
using Reactor.Extensions;
using TownOfUs.CrewmateRoles.MedicMod;
using Il2CppSystem.Collections.Generic;
using Object = UnityEngine.Object;

namespace TownOfUs.Roles
{
    public enum ReviveLimit
    {
        NoLimit = 0,
        One = 1,
        Two = 2,
        Three = 3
    }
    public class Druid : Undertaker {
        private static Vector2 _dragStartPosition;
        public int revivedCount = 0;
        
        public Druid(PlayerControl player) : base(player)
        {
            Name = "Druid";
            ImpostorText = () => "Revive dead by sorcery";
            TaskText = () => "Revive dead by sorcery";
            Color = new Color(0.4f, 0f, 0.56f);
            RoleType = RoleEnum.Druid;
            Faction = Faction.Crewmates;
            revivedCount = 0;
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
            if (CustomGameOptions.DruidReviveLimit == ReviveLimit.NoLimit) {
                return true;
            }
            return revivedCount < (int)CustomGameOptions.DruidReviveLimit;
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
            Murder.KilledPlayers.Remove(
                Murder.KilledPlayers.FirstOrDefault(x => x.PlayerId == player.PlayerId));
            revived.Add(player);
            player.NetTransform.SnapTo(position);

            if (target != null) Object.Destroy(target.gameObject);

            if (revived.Any(x => x.AmOwner)) {
                try {
                    Minigame.Instance.Close();
                    Minigame.Instance.Close();
                }
                catch {
                }
            }
            revivedCount += 1;
            if (!CanRevive()) {
                _dragDropButton.renderer.color = Palette.DisabledClear;
                _dragDropButton.renderer.material.SetFloat("_Desat", 1f);    
            }
        }
    }
}