using System.Collections;
using System.Linq;
using TownOfUs.Roles;
using UnityEngine;
using DateTime = System.DateTime;

namespace TownOfUs.ImpostorRoles.KirbyMod
{
    public class KirbyCoroutine
    {
        public static IEnumerator inhaleCoroutine(DeadBody body, Kirby role)
        {
            InhaleButtonTarget.SetTarget(DestroyableSingleton<HudManager>.Instance.KillButton, null, role);
            role.Player.NetTransform.Halt();
            // float maxFrame = CustomGameOptions.CleanDuration * 60;
            Vector3 startPosition = body.transform.position;
            float maxFrame = 1.0f * 60;
            for (var i = 0; i < maxFrame; i++)
            {
                if (body == null || !GameData.Instance.GetPlayerById(body.ParentId).IsDead) {
                    role.Player.moveable = true;
                    yield break;
                }
                body.bodyRenderer.transform.localScale = Vector3.Lerp(new Vector3(0.5f, 0.5f, 0.5f), Vector3.zero, (float)i / (maxFrame * 2.0f));
                body.transform.position = Vector3.Lerp(startPosition, role.Player.transform.position, (float)i / maxFrame);
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
                    taker._dragDropButton.graphic.sprite = TownOfUs.DragSprite;
                }
            }
            role.Player.moveable = true;
            role.SampledPlayer = GameData.Instance.GetPlayerById(body.ParentId)._object;
            role.CopyCrew(body);
        }
    }
}