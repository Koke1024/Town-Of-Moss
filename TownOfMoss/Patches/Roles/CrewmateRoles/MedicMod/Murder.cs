using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using TownOfUs.Extensions;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TownOfUs.CrewmateRoles.MedicMod
{
    [HarmonyPatch(typeof(KillAnimation), nameof(KillAnimation.CoPerformKill))]
    public class Murder {
    }
}