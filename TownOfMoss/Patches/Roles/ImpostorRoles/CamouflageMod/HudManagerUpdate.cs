using HarmonyLib;
using TownOfUs.Roles;
using UnityEngine;

namespace TownOfUs.ImpostorRoles.CamouflageMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HudManagerUpdate
    {
        public static Sprite Camouflage => TownOfUs.Camouflage;

        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Camouflager)) return;
            var role = Role.GetRole<Camouflager>(PlayerControl.LocalPlayer);
            if (role.CamouflageButton == null)
            {
                role.CamouflageButton = Object.Instantiate(__instance.KillButton, HudManager.Instance.transform);
                role.CamouflageButton.graphic.enabled = true;
                role.CamouflageButton.graphic.sprite = Camouflage;
            }

            role.CamouflageButton.graphic.sprite = Camouflage;

            role.CamouflageButton.gameObject.SetActive(!PlayerControl.LocalPlayer.Data.IsDead && !MeetingHud.Instance);
            var position = __instance.KillButton.transform.localPosition;
            role.CamouflageButton.transform.localPosition = new Vector3(position.x,
                __instance.ReportButton.transform.localPosition.y, position.z);

            if (role.Enabled)
            {
                role.CamouflageButton.SetCoolDown(role.TimeRemaining, CustomGameOptions.CamouflagerDuration);
                return;
            }

            role.CamouflageButton.SetCoolDown(role.CamouflageTimer(), CustomGameOptions.CamouflagerCd);
            role.CamouflageButton.graphic.color = Palette.EnabledColor;
            role.CamouflageButton.graphic.material.SetFloat("_Desat", 0f);
        }
    }
}