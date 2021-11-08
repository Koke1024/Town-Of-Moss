﻿using System;
using HarmonyLib;
using Hazel;
using Reactor;
using TownOfUs.Extensions;
using TownOfUs.NeutralRoles.ArsonistMod;
using TownOfUs.NeutralRoles.SeerMod;
using TownOfUs.Roles;
using UnityEngine;
using DateTime = Il2CppSystem.DateTime;

namespace TownOfUs.CrewmateRoles.PainterMod
{
    [HarmonyPatch(typeof(KillButtonManager), nameof(KillButtonManager.PerformKill))]
    public class PerformKill
    {
        public static bool Prefix(KillButtonManager __instance)
        {
            var flag = PlayerControl.LocalPlayer.Is(RoleEnum.Painter);
            if (!flag) return true;
            var role = Role.GetRole<Painter>(PlayerControl.LocalPlayer);
            if (!PlayerControl.LocalPlayer.CanMove) return false;
            var flag2 = role.PaintTimer() == 0f;
            if (!flag2) return false;
            if (!__instance.enabled) return false;

            int c = 0;
            foreach (var btn in role._paintButtons) {
                if(btn != __instance) {
                    ++c;
                    continue;
                }

                if (c >= CustomGameOptions.PaintColorMax) {
                    return false;
                }
                
                role.lastPainted = DateTime.UtcNow;
                
                if (__instance.renderer.sprite == TownOfUs.PaintSprite[c]) {
                    Painter.RpcSetPaintPoint(role.Player.GetTruePosition(), (PaintColor)c);
                }
                else {
                    Painter.RpcSetPaintVent(role.closeVent.Id, (PaintColor)c);
                }

                break;
            }

            return false;
        }
    }
}