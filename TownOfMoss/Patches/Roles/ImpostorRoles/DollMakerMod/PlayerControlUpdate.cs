using HarmonyLib;
using TownOfUs.Roles;
using UnityEngine;

namespace TownOfUs.ImpostorRoles.DollMakerMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class PlayerControlUpdate
    {
        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.DollMaker)) return;

            var role = Role.GetRole<DollMaker>(PlayerControl.LocalPlayer);
            if (role.WaxButton == null)
            {
                role.WaxButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.WaxButton.graphic.enabled = true;
                role.WaxButton.graphic.sprite = TownOfUs.WaxSprite;
            }

            role.WaxButton.gameObject.SetActive(!PlayerControl.LocalPlayer.Data.IsDead && !MeetingHud.Instance);
            role.WaxButton.transform.localPosition = __instance.KillButton.transform.localPosition;

            role.WaxButton.SetCoolDown(role.Player.killTimer, PlayerControl.GameOptions.KillCooldown);
        }
    }
}