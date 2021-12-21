using System;
using System.Linq;
using HarmonyLib;
using Hazel;
using Reactor.Extensions;
using TownOfUs.Roles;
using UnityEngine;

namespace TownOfUs.ImpostorRoles.UndertakerMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class UndertakerDoClickButton
    {
        public static bool Prefix(KillButton __instance)
        {
            var flag = PlayerControl.LocalPlayer.CanDrag();
            if (!flag) return true;
            if (!PlayerControl.LocalPlayer.CanMove) return false;
            if (PlayerControl.LocalPlayer.Data.IsDead) return false;
            var role = Role.GetRole<Undertaker>(PlayerControl.LocalPlayer);

            if (__instance == role.DragDropButton)
            {
                if (role.DragDropButton.graphic.sprite == TownOfUs.DragSprite)
                {
                    if (__instance.isCoolingDown) return false;
                    if (!__instance.enabled) return false;
                    if (!role.CurrentTarget) return false;
                    var maxDistance = GameOptionsData.KillDistances[0];
                    if (Vector2.Distance(role.CurrentTarget.TruePosition,
                        PlayerControl.LocalPlayer.GetTruePosition()) > maxDistance) return false;
                    var playerId = role.CurrentTarget.ParentId;
                    
                    foreach (Undertaker dragger in Role.AllRoles.Where(x => x.Player.CanDrag() && ((Undertaker)x).CurrentlyDragging != null)) {
                        if (dragger.CurrentlyDragging.ParentId == playerId) {
                            return false;
                        }
                    }

                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                        (byte) CustomRPC.Drag, SendOption.Reliable, -1);
                    writer.Write(PlayerControl.LocalPlayer.PlayerId);
                    writer.Write(playerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);

                    role.CurrentlyDragging = role.CurrentTarget;

                    if (PlayerControl.LocalPlayer.Is(RoleEnum.Druid)) {
                        var druid = Role.GetRole<Druid>(PlayerControl.LocalPlayer);
                        druid.DragStart(role.CurrentlyDragging.TruePosition);
                    }

                    UndertakerKillButtonTarget.SetDeadTarget(__instance, null, role);
                    __instance.graphic.sprite = TownOfUs.DropSprite;
                    return false;
                }
                else
                {
                    if (!__instance.enabled) return false;
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                        (byte) CustomRPC.Drop, SendOption.Reliable, -1);
                    writer.Write(PlayerControl.LocalPlayer.PlayerId);
                    var body = role.CurrentlyDragging;
                    var position = PlayerControl.LocalPlayer.GetTruePosition();
                    writer.Write(position);
                    writer.Write(body.transform.position.z);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);

                    body.bodyRenderer.material.SetFloat("_Outline", 0f);
                    role.CurrentlyDragging = null;
                    __instance.graphic.sprite = TownOfUs.DragSprite;
                    role.LastDragged = DateTime.UtcNow;

                    //body.transform.position = position;


                    return false;
                }
            }

            return true;
        }
    }
}
