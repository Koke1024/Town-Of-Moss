using System;
using Hazel;
using Reactor.Extensions;
using TownOfUs.ImpostorRoles.UndertakerMod;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TownOfUs.Roles
{
    public class Undertaker : Assassin
    {
        public KillButton _dragDropButton;

        public Undertaker(PlayerControl player) : base(player)
        {
            if (GetType() != typeof(Undertaker)) {
                return;
            }
            Name = "Undertaker";
            ImpostorText = () => "Drag bodies and hide them";
            TaskText = () => "Drag bodies around to hide them from being reported";
            Color = Palette.ImpostorRed;
            RoleType = RoleEnum.Undertaker;
            Faction = Faction.Impostors;
        }

        public DateTime LastDragged { get; set; }
        public DeadBody CurrentTarget { get; set; }
        public DeadBody CurrentlyDragging { get; set; }

        public KillButton DragDropButton
        {
            get => _dragDropButton;
            set
            {
                _dragDropButton = value;
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
            }
        }

        public float DragTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastDragged;
            var num = CustomGameOptions.DragCd * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }

        public override void OnEndMeeting() {
            base.OnEndMeeting();
            if (PlayerControl.LocalPlayer.CanDrag())
            {
                if (Player.AmOwner) {
                    DragDropButton.graphic.sprite = TownOfUs.DragSprite;
                }
                CurrentlyDragging = null;
                LastDragged = DateTime.UtcNow;
            }
        }

        public override void PostFixedUpdate() {
            base.PostFixedUpdate();
            
            if (!Player.CanDrag()) return;
            var body = CurrentlyDragging;
            if (body == null) return;

            if (Player.Data.IsDead)
            {
                if (Player.AmOwner)
                {
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                        (byte) CustomRPC.Drop, SendOption.Reliable, -1);
                    writer.Write(PlayerControl.LocalPlayer.PlayerId);
                    var position = PlayerControl.LocalPlayer.GetTruePosition();
                    writer.Write(position);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    body.transform.position = position;

                    CurrentlyDragging = null;
                    body.bodyRenderer.material.SetFloat("_Outline", 0f);
                    LastDragged = DateTime.UtcNow;

                }
                return;
            }

            var currentPosition = Player.GetTruePosition();
            var velocity = Player.gameObject.GetComponent<Rigidbody2D>().velocity.normalized;
            var newPos = ((Vector2) Player.transform.position) - (velocity * (CustomGameOptions.DragVel / 100.0f) / 3) + body.myCollider.offset;

            if (PhysicsHelpers.AnythingBetween(
                currentPosition,
                newPos,
                Constants.ShipAndObjectsMask,
                false
            ))
            {
                body.transform.position = new Vector3(currentPosition.x, currentPosition.y, currentPosition.y / 1000.0f);
            }
            else
            {
                body.transform.position = new Vector3(newPos.x, newPos.y, newPos.y / 1000.0f);
            }

            if (!Player.AmOwner) return;
            var material = body.bodyRenderer.material;
            material.SetColor("_OutlineColor", Color.green);
            material.SetFloat("_Outline", 1f);
        }

        public override void PostHudUpdate(HudManager __instance) {
            base.PostHudUpdate(__instance);
            
            if (!PlayerControl.LocalPlayer.CanDrag()) return;

            if (DragDropButton == null)
            {
                DragDropButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                DragDropButton.graphic.enabled = true;
                DragDropButton.graphic.sprite = TownOfUs.DragSprite;
                DragDropButton.GetComponent<AspectPosition>().DistanceFromEdge = TownOfUs.ButtonPosition;
                DragDropButton.gameObject.SetActive(false);
            }
            DragDropButton.GetComponent<AspectPosition>().Update();

            if (DragDropButton.graphic.sprite != TownOfUs.DragSprite &&
                DragDropButton.graphic.sprite != TownOfUs.DropSprite)
                DragDropButton.graphic.sprite = TownOfUs.DragSprite;
                
            if (DragDropButton.graphic.sprite == TownOfUs.DropSprite && CurrentlyDragging == null)
                DragDropButton.graphic.sprite = TownOfUs.DragSprite;

            DragDropButton.gameObject.SetActive(!PlayerControl.LocalPlayer.Data.IsDead && !MeetingHud.Instance);

            
            if (DragDropButton.graphic.sprite == TownOfUs.DragSprite)
            {
                var data = PlayerControl.LocalPlayer.Data;
                var isDead = data.IsDead;
                var truePosition = PlayerControl.LocalPlayer.GetTruePosition();
                var maxDistance = GameOptionsData.KillDistances[0];
                var flag = (PlayerControl.GameOptions.GhostsDoTasks || !data.IsDead) &&
                           (!AmongUsClient.Instance || !AmongUsClient.Instance.IsGameOver) &&
                           PlayerControl.LocalPlayer.CanMove;
                var allocs = Physics2D.OverlapCircleAll(truePosition, maxDistance,
                    LayerMask.GetMask(new[] {"Players", "Ghost"}));
                var killButton = DragDropButton;
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
                
                
                UndertakerKillButtonTarget.SetDeadTarget(killButton, closestBody, this);
            }
            
            if (DragDropButton.graphic.sprite == TownOfUs.DragSprite)
            {
                DragDropButton.SetCoolDown(DragTimer(), CustomGameOptions.DragCd);
            }
            else
            {
                DragDropButton.SetCoolDown(0f, 1f);
                DragDropButton.graphic.color = Palette.EnabledColor;
                DragDropButton.graphic.material.SetFloat("_Desat", 0f);
            }
        }
    }
}