using HarmonyLib;
using Hazel;
using TownOfUs.Roles;
using UnityEngine;

namespace TownOfUs.ImpostorRoles.KirbyMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.SetTarget))]
    public class InhaleButtonTarget
    {
        public static bool Prefix(KillButton __instance)
        {
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Kirby)) return true;
            return __instance == DestroyableSingleton<HudManager>.Instance.KillButton;
        }

        public static void SetTarget(ActionButton __instance, DeadBody target, Kirby role)
        {
            if (role.Morphed) {
                __instance.graphic.color = Palette.EnabledColor;
                __instance.graphic.material.SetFloat("_Desat", 0f);
                return;
            }
            if (role.CurrentTarget && role.CurrentTarget != target)
                role.CurrentTarget.bodyRenderer.material.SetFloat("_Outline", 0f);

            role.CurrentTarget = target;
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