using HarmonyLib;
using TownOfUs.Roles;
using UnityEngine;

namespace TownOfUs.ImpostorRoles.JanitorMod
{
    [HarmonyPatch(typeof(ActionButton), nameof(ActionButton.SetTarget))]
    public class KillButtonTarget
    {
        public static bool Prefix(ActionButton __instance)
        {
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Janitor)) return true;
            return __instance == DestroyableSingleton<HudManager>.Instance.KillButton;
        }

        public static void SetTarget(ActionButton __instance, DeadBody target, Janitor role)
        {
            if (role.CurrentTarget && role.CurrentTarget != target)
                role.CurrentTarget.bodyRenderer.material.SetFloat("_Outline", 0f);

            role.CurrentTarget = target;
            if (role.CurrentTarget && __instance.enabled)
            {
                var component = role.CurrentTarget.bodyRenderer;
                component.material.SetFloat("_Outline", 1f);
                component.material.SetColor("_OutlineColor", Color.yellow);
                __instance.renderer.color = Palette.EnabledColor;
                __instance.renderer.material.SetFloat("_Desat", 0f);
                return;
            }

            __instance.renderer.color = Palette.DisabledClear;
            __instance.renderer.material.SetFloat("_Desat", 1f);
        }
    }
}