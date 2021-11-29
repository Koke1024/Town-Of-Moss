using System.Linq;
using HarmonyLib;
using TownOfUs.Roles;
using UnityEngine;

namespace TownOfUs.ImpostorRoles.CrackerMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HudManagerUpdate
    {
        public static Sprite CrackSprite => TownOfUs.HackSprite;
        

        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Cracker)) return;
            var role = Role.GetRole<Cracker>(PlayerControl.LocalPlayer);
            if (role.CrackButton == null)
            {
                role.CrackButton = Object.Instantiate(__instance.KillButton, HudManager.Instance.transform);
                role.CrackButton.renderer.enabled = true;
                role.CrackButton.renderer.sprite = CrackSprite;
            }
            role.CrackButton.gameObject.SetActive(!PlayerControl.LocalPlayer.Data.IsDead && !MeetingHud.Instance);

            var position = __instance.KillButton.transform.localPosition;
            role.CrackButton.transform.localPosition = new Vector3(position.x,
                __instance.ReportButton.transform.localPosition.y, position.z);
            role.CrackButton.SetCoolDown(role.CrackTimer(), CustomGameOptions.CrackCd);

            bool available = role.TargetRoom != null;
            if (available)
            {
                role.CrackButton.renderer.color = Palette.EnabledColor;
                role.CrackButton.renderer.material.SetFloat("_Desat", 0f);
            }
            else
            {
                role.CrackButton.renderer.color = Palette.DisabledClear;
                role.CrackButton.renderer.material.SetFloat("_Desat", 1f);
            }
        }
    }
}