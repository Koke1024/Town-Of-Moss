﻿using System.Linq;
using HarmonyLib;
using TownOfUs.Roles;
using UnityEngine;

namespace TownOfUs.NeutralRoles.ArsonistMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class HudManagerUpdate
    {
        public static Sprite IgniteSprite => TownOfUs.IgniteSprite;
        
        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Arsonist)) return;
            var role = Role.GetRole<Arsonist>(PlayerControl.LocalPlayer);

            foreach (var playerId in role.DousedPlayers)
            {
                var player = Utils.PlayerById(playerId);
                var data = player?.Data;
                if (data == null || data.Disconnected || data.IsDead)
                    continue;

                // player.myRend.material.SetColor("_VisorColor", role.Color);
                player.nameText.color = Color.black;
            }

            if (role.IgniteButton == null)
            {
                role.IgniteButton = Object.Instantiate(__instance.KillButton, HudManager.Instance.transform);
                role.IgniteButton.graphic.enabled = true;
            }

            role.IgniteButton.graphic.sprite = IgniteSprite;
            var position = __instance.KillButton.transform.localPosition;
            role.IgniteButton.transform.localPosition = new Vector3(position.x,
                __instance.ReportButton.transform.localPosition.y, position.z);

            role.IgniteButton.gameObject.SetActive(!PlayerControl.LocalPlayer.Data.IsDead && !MeetingHud.Instance);
            __instance.KillButton.gameObject.SetActive(!PlayerControl.LocalPlayer.Data.IsDead && !MeetingHud.Instance);
            role.IgniteButton.SetCoolDown(0f, 1f);
            __instance.KillButton.SetCoolDown(role.DouseTimer(), CustomGameOptions.DouseCd);

            var notDoused = PlayerControl.AllPlayerControls.ToArray().Where(
                player => !role.DousedPlayers.Contains(player.PlayerId)
            ).ToList();

            Utils.SetTarget(ref role.ClosestPlayer, __instance.KillButton, float.NaN, notDoused);


            if (!role.IgniteButton.isCoolingDown & role.IgniteButton.isActiveAndEnabled & !role.IgniteUsed &
                role.CheckEveryoneDoused())
            {
                role.IgniteButton.graphic.color = Palette.EnabledColor;
                role.IgniteButton.graphic.material.SetFloat("_Desat", 0f);
                return;
            }

            role.IgniteButton.graphic.color = Palette.DisabledClear;
            role.IgniteButton.graphic.material.SetFloat("_Desat", 1f);
        }
    }
}
