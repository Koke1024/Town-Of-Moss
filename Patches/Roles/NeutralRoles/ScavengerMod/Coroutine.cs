using System.Collections;
using System.Linq;
using Hazel;
using TownOfUs.Roles;
using UnityEngine;
using DateTime = System.DateTime;

namespace TownOfUs.ImpostorRoles.ScavengerMod
{
    public class ScavengerCoroutine
    {
        public static IEnumerator EatCoroutine(DeadBody body, Scavenger role)
        {
            EatButtonTarget.SetTarget(DestroyableSingleton<HudManager>.Instance.KillButton, null, role);
            role.Player.NetTransform.Halt();
            var renderer = body.bodyRenderer;
            // float maxFrame = CustomGameOptions.CleanDuration * 60;
            Vector3 startPosition = renderer.transform.position;
            float maxFrame = 1.0f * 60;
            for (var i = 0; i < maxFrame; i++)
            {
                if (body == null || !GameData.Instance.GetPlayerById(body.ParentId).IsDead) {
                    role.Player.moveable = true;
                    yield break;
                }
                renderer.transform.localScale = Vector3.Lerp(new Vector3(0.5f, 0.5f, 0.5f), Vector3.zero, (float)i / (maxFrame * 2.0f));
                renderer.transform.position = Vector3.Lerp(startPosition, role.Player.GetTruePosition(), (float)i / maxFrame);
                role.Player.moveable = false;
                yield return null;
            }
            
            foreach (var p in PlayerControl.AllPlayerControls.ToArray().Where(x => x.CanDrag())) {
                var taker = Role.GetRole<Undertaker>(p);
                if (taker.CurrentlyDragging != null &&
                    taker.CurrentlyDragging.ParentId == body.ParentId) {
                    taker.CurrentlyDragging = null;
                    taker.LastDragged = DateTime.UtcNow;
                }

                if (taker.Player == PlayerControl.LocalPlayer) {
                    taker._dragDropButton.renderer.sprite = TownOfUs.DragSprite;
                }
            }
            
            body.Reported = true;
            body.gameObject.SetActive(false);
            role.Player.moveable = true;
            role.eatCount++;
            if (role.eatCount >= CustomGameOptions.ScavengerWinCount) {
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                    (byte) CustomRPC.NeutralWin, SendOption.Reliable, -1);
                writer.Write((byte)RoleEnum.Scavenger);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                Utils.EndGame();
            }
        }
    }
}