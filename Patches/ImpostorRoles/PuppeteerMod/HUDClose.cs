using System;
using HarmonyLib;
using TownOfUs.Roles;
using DateTime = Il2CppSystem.DateTime;
using Object = UnityEngine.Object;

namespace TownOfUs.ImpostorRoles.PuppeteerMod
{
    [HarmonyPatch(typeof(Object), nameof(Object.Destroy), typeof(Object))]
    public static class PossessButton
    {
        public static void Postfix(Object obj)
        {
            if (ExileController.Instance == null || obj != ExileController.Instance.gameObject) return;
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Puppeteer))
            {
                var role = Role.GetRole<Puppeteer>(PlayerControl.LocalPlayer);
                role.PossessButton.renderer.sprite = Puppeteer.PossessSprite;
                role.PossessPlayer = null;
                role.lastPossess = DateTime.UtcNow;
            }
        }
    }
}