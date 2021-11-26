using HarmonyLib;
using TownOfUs.Roles;
using UnityEngine;

namespace TownOfUs.ImpostorRoles.CamouflageMod
{
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Update))]
    public class UpdateMeeting
    {
        private static void Postfix(MeetingHud __instance)
        {
            if (CustomGameOptions.MeetingColourblind)
                foreach (var state in __instance.playerStates)
                {
                    if (!PlayerControl.LocalPlayer.Is(RoleEnum.Seer)) state.NameText.text = "";
                    PlayerControl.SetPlayerMaterialColors(Color.grey, state.PlayerIcon.Body);
                    state.PlayerIcon.HatSlot.SetHat(0, 0);
                    var skinById = DestroyableSingleton<HatManager>.Instance.AllSkins.ToArray()[0];
                    state.PlayerIcon.Skin.layer.sprite = skinById.IdleFrame;
                }
        }
    }
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
    public class StartMeeting
    {
        private static void Postfix(MeetingHud __instance)
        {
            foreach (var role in Role.GetRoles(RoleEnum.Camouflager)) {
                ((Camouflager)role).TimeRemaining = 0;
            }
        }
    }
}