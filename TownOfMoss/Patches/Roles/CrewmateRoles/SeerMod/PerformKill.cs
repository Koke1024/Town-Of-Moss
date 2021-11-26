using System;
using HarmonyLib;
using Hazel;
using Reactor;
using TownOfUs.NeutralRoles.ArsonistMod;
using TownOfUs.NeutralRoles.SeerMod;
using TownOfUs.Roles;
using UnityEngine;

namespace TownOfUs.CrewmateRoles.SeerMod
{
    [HarmonyPatch(typeof(ActionButton), nameof(ActionButton.DoClick))]
    public class DoClick
    {
        public static bool Prefix(ActionButton __instance)
        {
            if (__instance != DestroyableSingleton<HudManager>.Instance.KillButton) return true;
            var flag = PlayerControl.LocalPlayer.Is(RoleEnum.Seer);
            if (!flag) return true;
            var role = Role.GetRole<Seer>(PlayerControl.LocalPlayer);
            if (!PlayerControl.LocalPlayer.CanMove || role.ClosestPlayer == null) return false;
            var flag2 = role.SeerTimer() == 0f;
            if (!flag2) return false;
            if (!__instance.enabled) return false;
            var maxDistance = GameOptionsData.KillDistances[PlayerControl.GameOptions.KillDistance];
            if (Vector2.Distance(role.ClosestPlayer.GetTruePosition(),
                PlayerControl.LocalPlayer.GetTruePosition()) > maxDistance) return false;
            if (role.ClosestPlayer == null) return false;
            
            Coroutines.Start(SeerCoroutine.Investigating(role.ClosestPlayer, Role.GetRole<Seer>(role.Player)));
            return false;
        }
    }
}
