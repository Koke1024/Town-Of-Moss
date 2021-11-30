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
                role.CrackButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.CrackButton.graphic.enabled = true;
                role.CrackButton.graphic.sprite = CrackSprite;
                role.CrackButton.GetComponent<AspectPosition>().DistanceFromEdge = TownOfUs.ButtonPosition;
                role.CrackButton.gameObject.SetActive(false);
            }
            role.CrackButton.GetComponent<AspectPosition>().Update();

            role.CrackButton.gameObject.SetActive(!PlayerControl.LocalPlayer.Data.IsDead && !MeetingHud.Instance);
            role.CrackButton.SetCoolDown(role.CrackTimer(), CustomGameOptions.CrackCd);

            bool available = role.TargetRoom != null;
            if (available)
            {
                role.CrackButton.graphic.color = Palette.EnabledColor;
                role.CrackButton.graphic.material.SetFloat("_Desat", 0f);
            }
            else
            {
                role.CrackButton.graphic.color = Palette.DisabledClear;
                role.CrackButton.graphic.material.SetFloat("_Desat", 1f);
            }
        }
    }
}