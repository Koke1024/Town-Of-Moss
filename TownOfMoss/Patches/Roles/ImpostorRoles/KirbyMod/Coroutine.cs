using System.Collections;
using System.Linq;
using Hazel;
using Il2CppSystem;
using Reactor.Extensions;
using TownOfUs.Extensions;
using TownOfUs.ImpostorRoles.MorphlingMod;
using TownOfUs.Roles;
using UnityEngine;
using DateTime = System.DateTime;
using Object = UnityEngine.Object;

namespace TownOfUs.ImpostorRoles.KirbyMod
{
    public class KirbyCoroutine
    {
        public static IEnumerator inhaleCoroutine(DeadBody body, Kirby role)
        {
            InhaleButtonTarget.SetTarget(DestroyableSingleton<HudManager>.Instance.KillButton, null, role);
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
                    taker._dragDropButton.graphic.sprite = TownOfUs.DragSprite;
                }
            }
            role.Player.moveable = true;
            role.SampledPlayer = GameData.Instance.GetPlayerById(body.ParentId)._object;
            role.CopyCrew(body);
        }
    }
}