// using System.Linq;
// using BepInEx.Logging;
// using HarmonyLib;
// using TownOfUs.Extensions;
// using TownOfUs.Roles;
//
// namespace TownOfUs.ImpostorRoles.MultiKillerMod
// {
//     [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.MurderPlayer))]
//     public class PerformKill
//     {
//         public static void Postfix(PlayerControl __instance, [HarmonyArgument(0)] PlayerControl target)
//         {
//             var role = Role.GetRole(__instance);
//             if (role?.RoleType == RoleEnum.MultiKiller) {
//                 ((MultiKiller)role).SetKillTimer();
//                 Role.GetRole<MultiKiller>(__instance).killedOnce = !Role.GetRole<MultiKiller>(__instance).killedOnce;
//                 AmongUsExtensions.Log(LogLevel.Message, $"killedOnce update {Role.GetRole<MultiKiller>(__instance).killedOnce}");
//             }
//         }
//     }
// }
