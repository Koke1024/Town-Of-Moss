using System.Linq;
using HarmonyLib;
using TownOfUs.Roles;
using UnityEngine;

namespace TownOfUs.ImpostorRoles.UndertakerMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.SetTarget))]
    public class UndertakerKillButtonTarget
    {
        public static bool Prefix(KillButton __instance)
        {
            if (!PlayerControl.LocalPlayer.CanDrag()) return true;
            return __instance == DestroyableSingleton<HudManager>.Instance.KillButton;
        }

        public static void SetDeadTarget(KillButton __instance, DeadBody target, Undertaker role)
        {
            if (role.CurrentTarget && role.CurrentTarget != target)
                role.CurrentTarget.bodyRenderer.material.SetFloat("_Outline", 0f);
            
            role.CurrentTarget = target;
            
            foreach (Undertaker dragger in Role.AllRoles.ToArray().Where(x => x.Player.CanDrag() && ((Undertaker)x).CurrentlyDragging != null)) {
                if (target && dragger.CurrentlyDragging.ParentId == target.ParentId) {
                    role.CurrentTarget = null;
                    break;
                }
            }
            
            if (role.CurrentTarget && __instance.enabled)
            {
                var component = role.CurrentTarget.bodyRenderer;
                component.material.SetFloat("_Outline", 1f);
                component.material.SetColor("_OutlineColor", Color.yellow);
                __instance.graphic.color = Palette.EnabledColor;
                __instance.graphic.material.SetFloat("_Desat", 0f);
                return;
            }

            __instance.graphic.color = Palette.DisabledClear;
            __instance.graphic.material.SetFloat("_Desat", 1f);
        }
    }
}