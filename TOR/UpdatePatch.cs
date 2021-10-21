using HarmonyLib;
using System;
using System.IO;
using System.Net.Http;
using UnityEngine;
using TheOtherRoles.Objects;
using System.Collections.Generic;
using System.Linq;
using TownOfUs.Extensions;

namespace TheOtherRoles.Patches {
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    class HudManagerUpdatePatch
    {
    static void Postfix(HudManager __instance)
        {
            if (AmongUsClient.Instance.GameState != InnerNet.InnerNetClient.GameStates.Started) return;
            CustomButton.HudUpdate();
        }
    }
}
