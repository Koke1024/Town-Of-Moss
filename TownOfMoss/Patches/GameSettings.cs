using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using HarmonyLib;
using Reactor.Extensions;
using TownOfUs.CustomOption;
using TownOfUs.Extensions;
using UnityEngine;

namespace TownOfUs {
    [HarmonyPatch]
    public static class GameSettings {
        public static bool AllOptions;

        [HarmonyPatch] //ToHudString
        private static class GameOptionsDataPatch {
            public static IEnumerable<MethodBase> TargetMethods() {
                return typeof(GameOptionsData).GetMethods(typeof(string), typeof(int));
            }

            private static void Postfix(ref string __result) {
                var builder = new StringBuilder(AllOptions ? __result : "");
                builder.AppendLine(
                    $"\n\n<color=#FF0000FF>#Impostors</color>: {PlayerControl.GameOptions.NumImpostors}");

                int ratePhase = 0;
                Utils.crewRateString = "";
                Utils.impRateString = "";
                Utils.neutralRateString = "";
                int index = 0;
                foreach (var option in CustomOption.CustomOption.AllOptions) {
                    if (option.Name == "Custom Role Settings" && !AllOptions) break;
                    if (option.Type == CustomOptionType.Button) continue;
                    if (option.Type == CustomOptionType.Header) builder.AppendLine($"\n{option.Name}");
                    else if (option.Indent) builder.AppendLine($"     {option.Name}: {option}");
                    else builder.AppendLine($"{option.Name}: {option}");

                    if (ratePhase == 3) {
                        if (option.Name == "Custom Role Settings") {
                            ratePhase = 4;
                        }
                        else {
                            if (option.ToString() != "0%") {
                                if (Utils.impRateString != "") {
                                    if (index % 2 == 0) {
                                        Utils.impRateString += "\n";
                                    }
                                    else {
                                        Utils.impRateString += "    ";
                                    }
                                }
                                Utils.impRateString += $"{option.Name}: {option}";
                            }
                        }
                    }
                    if (ratePhase == 2) {
                        if (option.Name == "<color=#FF0000FF>Impostor Roles</color>") {
                            index = -1;
                            ratePhase = 3;
                        }
                        else {
                            if (option.ToString() != "0%") {
                                Utils.neutralRateString += $"{option.Name}: {option}\n";
                            }
                        }
                    }
                    if (ratePhase == 1) {
                        if (option.Name == "<color=#FF00FFFF>Neutral Roles</color>") {
                            ratePhase = 2;
                        }
                        else {
                            if (option.ToString() != "0%") {
                                if (Utils.crewRateString != "") {
                                    if (index % 2 == 0) {
                                        Utils.crewRateString += "\n";
                                    }
                                    else {
                                        Utils.crewRateString += "    ";
                                    }
                                }
                                Utils.crewRateString += $"{option.Name}: {option}";
                            }
                        }
                    }
                    if (ratePhase == 0) {
                        if (option.Name == "<color=#00FF00FF>Crewmate Roles</color>") {
                            index = -1;
                            ratePhase = 1;
                        }
                    }
                    ++index;
                }
                __result = builder.ToString();


                if (CustomOption.CustomOption.LobbyTextScroller && __result.Count(c => c == '\n') > 38)
                    __result = __result.Insert(__result.IndexOf('\n'), " (Scroll for more)");
                else __result = __result.Insert(__result.IndexOf('\n'), "Press Tab to see All Options");


                __result = $"<size=1.25>{__result}</size>";
            }
        }

        [HarmonyPatch(typeof(LobbyBehaviour), nameof(LobbyBehaviour.FixedUpdate))]
        private static class LobbyBehaviourPatch {
            private static void Postfix() {
                if (Input.GetKeyInt(KeyCode.Tab)) AllOptions = !AllOptions;

                //                HudManager.Instance.GameSettings.scale = 0.5f;
            }
        }

        [HarmonyPatch(typeof(GameOptionsMenu), nameof(GameOptionsMenu.Update))]
        public static class Update {
            public static void Postfix(ref GameOptionsMenu __instance) {
                var scroller = __instance.GetComponentInParent<Scroller>();
                if (scroller) {
                    scroller.YBounds.max = 90f;                    
                }
            }
        }
    }

    [HarmonyPatch(typeof(GameOptionsData), nameof(GameOptionsData.SetRecommendations), typeof(int), typeof(GameModes))]
    public static class ImpostorNumPatch {
        public static void Postfix(GameOptionsData __instance) {
            if (__instance.NumImpostors > 3) {
                __instance.NumImpostors = 3;
            }
            else if(__instance.NumImpostors == 0){
                __instance.NumImpostors = 1;
            }
        }
    }
}