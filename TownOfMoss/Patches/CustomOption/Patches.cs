using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BepInEx.IL2CPP.Utils;
using HarmonyLib;
using Reactor.Extensions;
using TownOfUs.Extensions;
using UnhollowerBaseLib;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TownOfUs.CustomOption {
    public static class Patches {
        public static Export ExportButton;
        public static Import ImportButton;
        public static List<OptionBehaviour> DefaultOptions;
        public static float LobbyTextRowHeight { get; set; } = 0.081F;
        public static bool inited = false;
        public static float contentX, contentY, contentZ;

        public static ToggleOption togglePrefab;

        private static List<OptionBehaviour> CreateOptions(GameOptionsMenu __instance) {
            var options = new List<OptionBehaviour>();

            var numberPrefab = Object.FindObjectOfType<NumberOption>();
            var stringPrefab = Object.FindObjectOfType<StringOption>();

            if (ExportButton.Setting != null) {
                ExportButton.Setting.gameObject.SetActive(true);
                options.Add(ExportButton.Setting);
            }
            else {
                var toggle = Object.Instantiate(togglePrefab, togglePrefab.transform.parent).DontDestroy();
                toggle.transform.GetChild(2).gameObject.SetActive(false);
                toggle.transform.GetChild(0).localPosition += new Vector3(1f, 0f, 0f);

                ExportButton.Setting = toggle;
                ExportButton.OptionCreated();
                options.Add(toggle);
            }

            if (ImportButton.Setting != null) {
                ImportButton.Setting.gameObject.SetActive(true);
                options.Add(ImportButton.Setting);
            }
            else {
                var toggle = Object.Instantiate(togglePrefab, togglePrefab.transform.parent).DontDestroy();
                toggle.transform.GetChild(2).gameObject.SetActive(false);
                toggle.transform.GetChild(0).localPosition += new Vector3(1f, 0f, 0f);

                ImportButton.Setting = toggle;
                ImportButton.OptionCreated();
                options.Add(toggle);
            }

            DefaultOptions = __instance.Children.ToList();
            foreach (var defaultOption in __instance.Children) {
                defaultOption.gameObject.SetActive(true);

                if (!AmongUsClient.Instance.AmHost) {
                    defaultOption.gameObject.SetActive(true);
                    if (defaultOption.GetComponent<PassiveButton>()) {
                        defaultOption.GetComponent<PassiveButton>().enabled = false;
                    }
                }

                options.Add(defaultOption);
            }

            foreach (var row
                in CustomOption.AllOptions) {
                // AmongUsExtensions.Log($"{option.Name}");
                if (row.Setting != null) {
                    row.Setting.gameObject.SetActive(true);

                    options.Add(row.Setting);
                    continue;
                }

                // AmongUsExtensions.Log($"{option.Type}");
                switch (row.Type) {
                    case CustomOptionType.Header:
                        if (AmongUsClient.Instance.AmHost) {
                            var toggle = Object.Instantiate(togglePrefab, togglePrefab.transform.parent).DontDestroy();
                            toggle.transform.GetChild(1).gameObject.SetActive(false);
                            toggle.transform.GetChild(2).gameObject.SetActive(false);
                            row.Setting = toggle;
                            options.Add(toggle);
                        }
                        else {
                            var header = Object.Instantiate(togglePrefab, togglePrefab.transform.parent).DontDestroy();
                            header.transform.GetChild(1).gameObject.SetActive(false);
                            header.transform.GetChild(2).gameObject.SetActive(false);
                            header.gameObject.SetActive(true);
                            row.Setting = header;
                            options.Add(header);
                        }

                        break;

                    case CustomOptionType.Toggle:
                        if (AmongUsClient.Instance.AmHost) {
                            var toggle2 = Object.Instantiate(togglePrefab, togglePrefab.transform.parent).DontDestroy();
                            row.Setting = toggle2;
                            options.Add(toggle2);
                        }
                        else {
                            var toggle2 = Object.Instantiate(togglePrefab, togglePrefab.transform.parent).DontDestroy();
                            row.Setting = toggle2;
                            toggle2.gameObject.SetActive(true);
                            toggle2.GetComponent<PassiveButton>().enabled = false;
                            options.Add(toggle2);
                        }

                        break;
                    case CustomOptionType.Number:
                        var number = Object.Instantiate(numberPrefab, numberPrefab.transform.parent).DontDestroy();
                        row.Setting = number;
                        options.Add(number);
                        break;
                    case CustomOptionType.String:
                        var str = Object.Instantiate(stringPrefab, stringPrefab.transform.parent).DontDestroy();
                        row.Setting = str;
                        options.Add(str);
                        break;
                }

                row.OptionCreated();
            }

            return options;
        }


        private static bool OnEnable(OptionBehaviour opt) {
            if (opt == ExportButton.Setting) {
                ExportButton.OptionCreated();
                return false;
            }

            if (opt == ImportButton.Setting) {
                ImportButton.OptionCreated();
                return false;
            }


            var customOption =
                CustomOption.AllOptions.FirstOrDefault(option =>
                    option.Setting == opt); // Works but may need to change to gameObject.name check

            if (customOption == null) {
                customOption = ExportButton.SlotButtons.FirstOrDefault(option => option.Setting == opt);
                if (customOption == null) {
                    customOption = ImportButton.SlotButtons.FirstOrDefault(option => option.Setting == opt);
                    if (customOption == null) return true;
                }
            }

            customOption.OptionCreated();

            return false;
        }

        [HarmonyPatch(typeof(GameSettingMenu), nameof(GameSettingMenu.Start))]
        private class OptionsMenuBehaviour_Start {
            public static void Postfix(GameSettingMenu __instance) {
                var obj = __instance.RolesSettingsHightlight.gameObject.transform.parent.parent;
                var touSettings = Object.Instantiate(__instance.RegularGameSettings,
                    __instance.RegularGameSettings.transform.parent);
                touSettings.SetActive(false);
                touSettings.name = "TOUSettings";

                var gameGroup = touSettings.transform.FindChild("GameGroup");
                var title = gameGroup?.FindChild("Text");

                if (title != null) {
                    title.GetComponent<TextTranslatorTMP>().Destroy();
                    title.GetComponent<TMPro.TextMeshPro>().m_text = "Town Of Moss Settings";
                }

                var sliderInner = gameGroup?.FindChild("SliderInner");
                if (sliderInner != null)
                    sliderInner.GetComponent<GameOptionsMenu>().name = "TouGameOptionsMenu";

                var ourSettingsButton = Object.Instantiate(obj.gameObject, obj.transform.parent);
                ourSettingsButton.transform.localPosition = new Vector3(obj.localPosition.x + 0.906f,
                    obj.localPosition.y, obj.localPosition.z);
                ourSettingsButton.name = "TOUtab";
                var hatButton = ourSettingsButton.transform.GetChild(0); //TODO:  change to FindChild I guess to be sure
                var hatIcon = hatButton.GetChild(0);
                var tabBackground = hatButton.GetChild(1);

                var renderer = hatIcon.GetComponent<SpriteRenderer>();
                renderer.sprite = ModManager.Instance.ModStamp.sprite;
                var touSettingsHighlight = tabBackground.GetComponent<SpriteRenderer>();
                PassiveButton passiveButton = __instance.GameSettingsHightlight.GetComponent<PassiveButton>();
                passiveButton.OnClick = new UnityEngine.UI.Button.ButtonClickedEvent();
                passiveButton.OnClick.AddListener(ToggleButton(__instance, touSettings, touSettingsHighlight, 0));
                passiveButton = __instance.RolesSettingsHightlight.GetComponent<PassiveButton>();
                passiveButton.OnClick = new UnityEngine.UI.Button.ButtonClickedEvent();
                passiveButton.OnClick.AddListener(ToggleButton(__instance, touSettings, touSettingsHighlight, 1));
                passiveButton = tabBackground.GetComponent<PassiveButton>();
                passiveButton.OnClick = new UnityEngine.UI.Button.ButtonClickedEvent();
                passiveButton.OnClick.AddListener(ToggleButton(__instance, touSettings, touSettingsHighlight, 2));

                //fix for scrollbar (bug in among us)
                touSettings.GetComponentInChildren<Scrollbar>().parent = touSettings.GetComponentInChildren<Scroller>();
                __instance.RegularGameSettings.GetComponentInChildren<Scrollbar>().parent =
                    __instance.RegularGameSettings.GetComponentInChildren<Scroller>();
                __instance.RolesSettings.GetComponentInChildren<Scrollbar>().parent =
                    __instance.RolesSettings.GetComponentInChildren<Scroller>();
            }
        }

        public static System.Action ToggleButton(GameSettingMenu settingMenu, GameObject TouSettings,
            SpriteRenderer highlight, int id) {
            return new System.Action(() => {
                settingMenu.RegularGameSettings.SetActive(id == 0);
                settingMenu.GameSettingsHightlight.enabled = id == 0;
                settingMenu.RolesSettings.gameObject.SetActive(id == 1);
                settingMenu.RolesSettingsHightlight.enabled = id == 1;
                highlight.enabled = id == 2;
                TouSettings.SetActive(id == 2);
            });
        }

        [HarmonyPatch(typeof(GameOptionsMenu), nameof(GameOptionsMenu.Start))]
        private class GameOptionsMenu_Start {
            public static bool Prefix(GameOptionsMenu __instance) {
                inited = false;
                if (__instance.name != "TouGameOptionsMenu") {
                    return true;
                }

                togglePrefab = Object.FindObjectOfType<ToggleOption>();

                __instance.Children = new Il2CppReferenceArray<OptionBehaviour>(Array.Empty<OptionBehaviour>());
                var children = new Transform[__instance.gameObject.transform.childCount];
                for (int k = 0; k < children.Length; k++) {
                    children[k] =
                        __instance.gameObject.transform
                            .GetChild(k); //TODO: Make a better fix for this for example caching the options or creating it ourself.
                }


                if (__instance.Children.Any()) {
                    var startOption = __instance.gameObject.transform.GetChild(0);
                    var localPosition = startOption.localPosition;
                    contentY = localPosition.y;
                    contentX = localPosition.x;
                    contentZ = localPosition.z;
                }

                var customOptions = CreateOptions(__instance);
                foreach (var row in children) {
                    row.gameObject.Destroy();
                }

                var i = 0;

                foreach (var option in customOptions) {
                    option.transform.localPosition = new Vector3(contentX, contentY - i++ * 0.25f, contentZ);
                }

                __instance.Children = new Il2CppReferenceArray<OptionBehaviour>(customOptions.ToArray());
                return false;
            }

            public static void Postfix(GameOptionsMenu __instance) {
                if (__instance.Children == null) {
                    return;
                }
                var commonTasksOption = __instance.Children.FirstOrDefault(x => x.name == "NumCommonTasks")
                    ?.TryCast<NumberOption>();
                if (commonTasksOption != null) commonTasksOption.ValidRange = new FloatRange(0f, 4f);

                var shortTasksOption = __instance.Children.FirstOrDefault(x => x.name == "NumShortTasks")
                    ?.TryCast<NumberOption>();
                if (shortTasksOption != null) shortTasksOption.ValidRange = new FloatRange(0f, 23f);

                var longTasksOption = __instance.Children.FirstOrDefault(x => x.name == "NumLongTasks")
                    ?.TryCast<NumberOption>();
                if (longTasksOption != null) longTasksOption.ValidRange = new FloatRange(0f, 15f);
            }
        }

        [HarmonyPatch(typeof(GameOptionsMenu), nameof(GameOptionsMenu.Update))]
        private class GameOptionsMenu_Update {
            public static float bottomY;
            public static float topY;

            public static void Postfix(GameOptionsMenu __instance) {
                if (!inited) {
                    var children = __instance.GetComponentsInChildren<OptionBehaviour>();
                    if (children.Any()) {
                        contentY = children.Max(option => option.transform.localPosition.y);
                    }
                    else {
                        contentY = 0;
                    }

                    if (__instance.Children == null || !__instance.Children.Any()) {
                        contentX = 0;
                        contentZ = -1;
                    }
                    else if (__instance.Children.Length == 1) {
                        contentX = __instance.Children[0].transform.localPosition.x;
                        contentZ = __instance.Children[0].transform.localPosition.z;
                    }
                    else if (__instance.Children.Length > 1) {
                        contentX = __instance.Children[1].transform.localPosition.x;
                        contentZ = __instance.Children[1].transform.localPosition.z;
                    }

                    inited = true;

                    int i = 0;
                    if (__instance.Children != null && __instance.Children.Any()) {
                        foreach (var option in __instance.Children) {
                            var opt =
                                CustomOption.AllOptions.FirstOrDefault(o =>
                                    o.Setting == option);
                            if (opt is CustomHeaderOption header) {
                                if (i % 2 == 1) {
                                    ++i;
                                }
                            }

                            option.transform.localPosition = new Vector3(contentX - 1.25f + 2.5f * (i % 2),
                                contentY - (i / 2) * 0.25f, contentZ);
                            option.transform.localScale = Vector3.one * 0.5f;
                            ++i;
                            if (opt is CustomHeaderOption header2) {
                                if (i % 2 == 1) {
                                    ++i;
                                }
                            }
                        }

                        float myY = PlayerControl.LocalPlayer.transform.position.y;
                        bottomY = -3 + myY + 1.34f - __instance.Children
                            .Min(option => option.transform.localPosition.y);
                        topY = myY - 0.9227f;
                    }
                }

                // var tomSetting = GameObject.Find("TOMSettings");


                var position = __instance.transform.position;
                if (BepInEx.IL2CPP.UnityEngine.Input.GetKeyInt(BepInEx.IL2CPP.UnityEngine.KeyCode.W)) {
                    __instance.transform.position += new Vector3(0, -0.3f, 0);
                }

                if (BepInEx.IL2CPP.UnityEngine.Input.GetKeyInt(BepInEx.IL2CPP.UnityEngine.KeyCode.S)) {
                    __instance.transform.position += new Vector3(0, 0.3f, 0);
                }

                if (BepInEx.IL2CPP.UnityEngine.Input.GetKeyInt(BepInEx.IL2CPP.UnityEngine.KeyCode.End) ||
                    BepInEx.IL2CPP.UnityEngine.Input.GetKeyInt(BepInEx.IL2CPP.UnityEngine.KeyCode.D) ||
                    __instance.transform.position.y > bottomY) {
                    __instance.transform.position = new Vector3(position.x, bottomY, position.z);
                }

                if (BepInEx.IL2CPP.UnityEngine.Input.GetKeyInt(BepInEx.IL2CPP.UnityEngine.KeyCode.Home) ||
                    BepInEx.IL2CPP.UnityEngine.Input.GetKeyInt(BepInEx.IL2CPP.UnityEngine.KeyCode.A) ||
                    __instance.transform.position.y < topY) {
                    __instance.transform.position = new Vector3(position.x, topY, position.z);
                }
            }
        }

        public static void JumpToElement(CustomOption option) {
            var menu = Object.FindObjectOfType<GameOptionsMenu>();
            var target = menu.Children
                .FirstOrDefault(x => x == option.Setting);
            if (target == null) {
                return;
            }

            menu.transform.position =
                new Vector3(menu.transform.position.x, GameOptionsMenu_Update.topY - target.transform.localPosition.y,
                    menu.transform.position.z);
        }

        [HarmonyPatch(typeof(ToggleOption), nameof(ToggleOption.OnEnable))]
        private static class ToggleOption_OnEnable {
            private static bool Prefix(ToggleOption __instance) {
                return OnEnable(__instance);
            }
        }

        [HarmonyPatch(typeof(NumberOption), nameof(NumberOption.OnEnable))]
        private static class NumberOption_OnEnable {
            private static bool Prefix(NumberOption __instance) {
                return OnEnable(__instance);
            }
        }

        [HarmonyPatch(typeof(StringOption), nameof(StringOption.OnEnable))]
        private static class StringOption_OnEnable {
            private static bool Prefix(StringOption __instance) {
                return OnEnable(__instance);
            }
        }


        [HarmonyPatch(typeof(ToggleOption), nameof(ToggleOption.Toggle))]
        private class ToggleButtonPatch {
            public static bool Prefix(ToggleOption __instance) {
                var option =
                    CustomOption.AllOptions.FirstOrDefault(option =>
                        option.Setting == __instance); // Works but may need to change to gameObject.name check
                if (option is CustomToggleOption toggle) {
                    toggle.Toggle();
                    return false;
                }

                if (__instance == ExportButton.Setting) {
                    if (!AmongUsClient.Instance.AmHost) return false;
                    ExportButton.Do();
                    return false;
                }

                if (__instance == ImportButton.Setting) {
                    if (!AmongUsClient.Instance.AmHost) return false;
                    ImportButton.Do();
                    return false;
                }


                if (option is CustomHeaderOption header) {
                    var jump = CustomOption.AllOptions.FirstOrDefault(option =>
                        option.Setting != __instance && header.Name == option.Name);
                    if (jump != null) {
                        JumpToElement(jump);
                    }

                    return false;
                }

                CustomOption option2 = ExportButton.SlotButtons.FirstOrDefault(option => option.Setting == __instance);
                if (option2 is CustomButtonOption button) {
                    if (!AmongUsClient.Instance.AmHost) return false;
                    button.Do();
                    return false;
                }

                CustomOption option3 = ImportButton.SlotButtons.FirstOrDefault(option => option.Setting == __instance);
                if (option3 is CustomButtonOption button2) {
                    if (!AmongUsClient.Instance.AmHost) return false;
                    button2.Do();
                    return false;
                }

                return true;
            }
        }

        [HarmonyPatch(typeof(NumberOption), nameof(NumberOption.Increase))]
        private class NumberOptionPatchIncrease {
            public static bool Prefix(NumberOption __instance) {
                var option =
                    CustomOption.AllOptions.FirstOrDefault(option =>
                        option.Setting == __instance); // Works but may need to change to gameObject.name check
                if (option is CustomNumberOption number) {
                    number.Increase();
                    return false;
                }

                return true;
            }
        }

        [HarmonyPatch(typeof(NumberOption), nameof(NumberOption.Decrease))]
        private class NumberOptionPatchDecrease {
            public static bool Prefix(NumberOption __instance) {
                var option =
                    CustomOption.AllOptions.FirstOrDefault(option =>
                        option.Setting == __instance); // Works but may need to change to gameObject.name check
                if (option is CustomNumberOption number) {
                    number.Decrease();
                    return false;
                }

                return true;
            }
        }

        [HarmonyPatch(typeof(StringOption), nameof(StringOption.Increase))]
        private class StringOptionPatchIncrease {
            public static bool Prefix(StringOption __instance) {
                var option =
                    CustomOption.AllOptions.FirstOrDefault(option =>
                        option.Setting == __instance); // Works but may need to change to gameObject.name check
                if (option is CustomStringOption str) {
                    str.Increase();
                    return false;
                }

                return true;
            }
        }

        [HarmonyPatch(typeof(StringOption), nameof(StringOption.Decrease))]
        private class StringOptionPatchDecrease {
            public static bool Prefix(StringOption __instance) {
                var option =
                    CustomOption.AllOptions.FirstOrDefault(option =>
                        option.Setting == __instance); // Works but may need to change to gameObject.name check
                if (option is CustomStringOption str) {
                    str.Decrease();
                    return false;
                }

                return true;
            }
        }

        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.RpcSyncSettings))]
        private class PlayerControlPatch {
            public static void Postfix() {
                if (PlayerControl.AllPlayerControls.Count < 2 || !AmongUsClient.Instance ||
                    !PlayerControl.LocalPlayer || !AmongUsClient.Instance.AmHost) return;

                Rpc.SendRpc();
            }
        }

        [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
        private class HudManagerUpdate {
            private const float
                MinX = -5.233334F /*-5.3F*/,
                OriginalY = 2.9F,
                MinY = 3F; // Differs to cause excess options to appear cut off to encourage scrolling

            private static Scroller Scroller;
            private static Vector3 LastPosition = new Vector3(MinX, MinY);

            public static void Prefix(HudManager __instance) {
                if (__instance.GameSettings?.transform == null) return;


                // Scroller disabled
                if (!CustomOption.LobbyTextScroller) {
                    // Remove scroller if disabled late
                    if (Scroller != null) {
                        __instance.GameSettings.transform.SetParent(Scroller.transform.parent);
                        __instance.GameSettings.transform.localPosition = new Vector3(MinX, OriginalY);

                        Object.Destroy(Scroller);
                    }

                    return;
                }

                CreateScroller(__instance);

                Scroller.gameObject.SetActive(__instance.GameSettings.gameObject.activeSelf);

                if (!Scroller.gameObject.active) return;

                var rows = __instance.GameSettings.text.Count(c => c == '\n');
                var maxY = Mathf.Max(MinY, rows * LobbyTextRowHeight + (rows - 38) * LobbyTextRowHeight);

                // Scroller.YBounds = new FloatRange(MinY, maxY);

                // Prevent scrolling when the player is interacting with a menu
                if (PlayerControl.LocalPlayer?.CanMove != true) {
                    __instance.GameSettings.transform.localPosition = LastPosition;

                    return;
                }

                if (__instance.GameSettings.transform.localPosition.x != MinX ||
                    __instance.GameSettings.transform.localPosition.y < MinY) return;

                LastPosition = __instance.GameSettings.transform.localPosition;
            }

            private static void CreateScroller(HudManager __instance) {
                if (Scroller != null) return;

                Scroller = new GameObject("SettingsScroller").AddComponent<Scroller>();
                Scroller.transform.SetParent(__instance.GameSettings.transform.parent);
                Scroller.gameObject.layer = 5;

                Scroller.transform.localScale = Vector3.one;
                Scroller.allowX = false;
                Scroller.allowY = true;
                Scroller.active = true;
                Scroller.velocity = new Vector2(0, 0);
                // Scroller.ScrollerYRange = new FloatRange(0, 0);
                // Scroller.XBounds = new FloatRange(MinX, MinX);
                Scroller.enabled = true;

                Scroller.Inner = __instance.GameSettings.transform;
                __instance.GameSettings.transform.SetParent(Scroller.transform);
            }
        }
    }

    [HarmonyPatch(typeof(Console), nameof(Console.CanUse))]
    public class MedBayDuplicate {
        public static bool Prefix(ref float __result, Console __instance, [HarmonyArgument(0)] GameData.PlayerInfo pc,
            [HarmonyArgument(1)] out bool canUse, [HarmonyArgument(2)] out bool couldUse) {
            canUse = couldUse = false;
            if (__instance.gameObject.GetComponent<MedScannerBehaviour>() == null) {
                return true;
            }

            foreach (var live in PlayerControl.AllPlayerControls.ToArray()
                .Where(x => !x.Data.IsDead && x != PlayerControl.LocalPlayer)) {
                if (__instance.gameObject.GetComponent<CircleCollider2D>().IsTouching(live.Collider)) {
                    __result = float.MaxValue;
                    return false;
                }
            }

            return true;
        }
    }
}