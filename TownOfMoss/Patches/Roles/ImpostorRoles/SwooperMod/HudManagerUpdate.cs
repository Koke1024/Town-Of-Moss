using HarmonyLib;
using TownOfUs.Roles;
using UnityEngine;

namespace TownOfUs.ImpostorRoles.SwooperMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HudManagerUpdate
    {
        public static Sprite SwoopSprite => TownOfUs.SwoopSprite;

        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Swooper)) return;
            var role = Role.GetRole<Swooper>(PlayerControl.LocalPlayer);
            if (role.SwoopButton == null)
            {
                role.SwoopButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.SwoopButton.graphic.enabled = true;
                role.SwoopButton.GetComponent<AspectPosition>().DistanceFromEdge = TownOfUs.ButtonPosition;
                role.SwoopButton.gameObject.SetActive(false);
            }
            role.SwoopButton.GetComponent<AspectPosition>().Update();
            role.SwoopButton.graphic.sprite = SwoopSprite;
            role.SwoopButton.gameObject.SetActive(!PlayerControl.LocalPlayer.Data.IsDead && !MeetingHud.Instance);

            if (role.IsSwooped)
            {
                role.SwoopButton.SetCoolDown(role.TimeRemaining, CustomGameOptions.SwoopDuration);
                return;
            }

            role.SwoopButton.SetCoolDown(role.SwoopTimer(), CustomGameOptions.SwoopCd);


            role.SwoopButton.graphic.color = Palette.EnabledColor;
            role.SwoopButton.graphic.material.SetFloat("_Desat", 0f);
        }
    }
}