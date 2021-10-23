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
                role.WaxButton = Object.Instantiate(__instance.KillButton, HudManager.Instance.transform);
                role.WaxButton.renderer.enabled = true;
                role.WaxButton.renderer.sprite = TownOfUs.WaxSprite;
            }

            role.WaxButton.gameObject.SetActive(!PlayerControl.LocalPlayer.Data.IsDead && !MeetingHud.Instance);
            var position = __instance.KillButton.transform.localPosition;
            role.WaxButton.transform.localPosition = new Vector3(position.x,
                __instance.ReportButton.transform.localPosition.y, position.z);

            role.WaxButton.SetCoolDown(role.CleanTimer(), PlayerControl.GameOptions.KillCooldown);
        }
    }
}