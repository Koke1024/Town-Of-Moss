using System.Collections;
using Il2CppSystem;
using TownOfUs.Roles;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TownOfUs.ImpostorRoles.JanitorMod
{
    public class Coroutine
    {
        private static readonly int BodyColor = Shader.PropertyToID("_BodyColor");
        private static readonly int BackColor = Shader.PropertyToID("_BackColor");

        public static IEnumerator CleanCoroutine(DeadBody body, Janitor role)
        {
            KillButtonTarget.SetTarget(DestroyableSingleton<HudManager>.Instance.KillButton, null, role);
            role.Player.NetTransform.Halt();
            role.lastCleaned = DateTime.UtcNow;
            var renderer = body.bodyRenderer;
            var backColor = renderer.material.GetColor(BackColor);
            var bodyColor = renderer.material.GetColor(BodyColor);
            var newColor = new Color(1f, 1f, 1f, 0f);
            float maxFrame = CustomGameOptions.CleanDuration * 60;
            for (var i = 0; i < maxFrame; i++)
            {
                if (body == null) {
                    role.Player.moveable = true;
                    yield break;
                }
                renderer.color = Color.Lerp(backColor, newColor, i / maxFrame);
                renderer.color = Color.Lerp(bodyColor, newColor, i / maxFrame);
                role.Player.moveable = false;
                yield return null;
            }
            role.Player.moveable = true;

            Object.Destroy(body.gameObject);
        }
    }
}