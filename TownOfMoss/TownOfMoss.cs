using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.IL2CPP;
using HarmonyLib;
using Reactor;
using Reactor.Extensions;
using TownOfUs.CustomOption;
using TownOfUs.RainbowMod;
using UnhollowerBaseLib;
using UnhollowerRuntimeLib;
using UnityEngine;

namespace TownOfUs
{
    [BepInPlugin(Id, "Town Of Moss", "1.4.6")]
    [BepInDependency(ReactorPlugin.Id)]
    public class TownOfUs : BasePlugin
    {
        public static string Version = "1.4.6";
        public const string Id = "jp.spiel.koke";

        public static Sprite JanitorClean;
        public static Sprite Inhale;
        public static Sprite EngineerFix;
        public static Sprite SwapperSwitch;
        public static Sprite SwapperSwitchDisabled;
        public static Sprite Shift;
        public static Sprite Footprint;
        public static Sprite Rewind;
        public static Sprite NormalKill;
        public static Sprite MedicSprite;
        public static Sprite SeerSprite;
        public static Sprite SampleSprite;
        public static Sprite MorphSprite;
        public static Sprite PossessSprite;
        public static Sprite ReleaseSprite;
        public static Sprite Camouflage;
        public static Sprite Arrow;
        // public static Sprite Abstain;
        // public static Sprite Extend;
        public static Sprite MineSprite;
        public static Sprite SwoopSprite;
        public static Sprite DouseSprite;
        public static Sprite IgniteSprite;
        public static Sprite ReviveSprite;
        public static Sprite ButtonSprite;
        public static Sprite PolusSprite;

        public static Sprite TargetSprite;
        public static Sprite HackSprite;
        
        public static Sprite CloseVentButtonSprite;
        public static Sprite PlaceCameraSprite;
        public static Sprite AnimatedVentSprite;
        public static Sprite StaticVentSprite;

        public static Sprite DragSprite;
        public static Sprite DropSprite;
        public static Sprite WaxSprite;
        public static readonly Sprite[] PaintSprite = new Sprite[3];
        public static Sprite InkSprite;
        
        public static Vector3 ButtonPosition { get; private set; } = new Vector3(2.6f, 0.7f, -9f);
        public static Vector3 ButtonOffset { get; private set; } = new Vector3(0.9f, 0.8f, 0);

        private static DLoadImage _iCallLoadImage;


        private Harmony _harmony;

        public ConfigEntry<string> Ip { get; set; }

        public ConfigEntry<ushort> Port { get; set; }

        public override void Load()
        {
            System.Console.WriteLine("000.000.000.000/000000000000000000");

            _harmony = new Harmony("jp.spiel.koke");

            Generate.GenerateAll();

            JanitorClean = CreateSprite("TownOfMoss.Resources.Janitor.png");
            Inhale = CreateSprite("TownOfMoss.Resources.Inhale.png");
            EngineerFix = CreateSprite("TownOfMoss.Resources.Engineer.png");
            //EngineerArrow = CreateSprite("TownOfMoss.Resources.EngineerArrow.png");
            SwapperSwitch = CreateSprite("TownOfMoss.Resources.SwapperSwitch.png");
            SwapperSwitchDisabled = CreateSprite("TownOfMoss.Resources.SwapperSwitchDisabled.png");
            Shift = CreateSprite("TownOfMoss.Resources.Shift.png");
            Footprint = CreateSprite("TownOfMoss.Resources.Footprint.png");
            Rewind = CreateSprite("TownOfMoss.Resources.Rewind.png");
            NormalKill = CreateSprite("TownOfMoss.Resources.NormalKill.png");
            MedicSprite = CreateSprite("TownOfMoss.Resources.Medic.png");
            SeerSprite = CreateSprite("TownOfMoss.Resources.Seer.png");
            SampleSprite = CreateSprite("TownOfMoss.Resources.Sample.png");
            MorphSprite = CreateSprite("TownOfMoss.Resources.Morph.png");
            PossessSprite = CreateSprite("TownOfMoss.Resources.Possess.png");
            ReleaseSprite = CreateSprite("TownOfMoss.Resources.Release.png");
            Camouflage = CreateSprite("TownOfMoss.Resources.Camouflage.png");
            Arrow = CreateSprite("TownOfMoss.Resources.Arrow.png");
            // Abstain = CreateSprite("TownOfMoss.Resources.Abstain.png");
            // Extend = CreateSprite("TownOfMoss.Resources.Extend.png");
            MineSprite = CreateSprite("TownOfMoss.Resources.Mine.png");
            SwoopSprite = CreateSprite("TownOfMoss.Resources.Swoop.png");
            DouseSprite = CreateSprite("TownOfMoss.Resources.Douse.png");
            IgniteSprite = CreateSprite("TownOfMoss.Resources.Ignite.png");
            ReviveSprite = CreateSprite("TownOfMoss.Resources.Revive.png");
            ButtonSprite = CreateSprite("TownOfMoss.Resources.Button.png");
            DragSprite = CreateSprite("TownOfMoss.Resources.Drag.png");
            DropSprite = CreateSprite("TownOfMoss.Resources.Drop.png");
            PolusSprite = CreateSprite("TownOfMoss.Resources.polus.gg.png");
            TargetSprite = CreateSprite("TownOfMoss.Resources.TargetIcon.png");
            HackSprite = CreateSprite("TownOfMoss.Resources.Hack.png");
            WaxSprite = CreateSprite("TownOfMoss.Resources.Wax.png");
            PaintSprite[0] = CreateSprite("TownOfMoss.Resources.PaintRed.png");
            PaintSprite[1] = CreateSprite("TownOfMoss.Resources.PaintBlue.png");
            PaintSprite[2] = CreateSprite("TownOfMoss.Resources.PaintYellow.png");
            InkSprite = CreateSprite("TownOfMoss.Resources.Ink.png");
            CloseVentButtonSprite = CreateSprite("TownOfMoss.Resources.CloseVentButton.png");
            PlaceCameraSprite = CreateSprite("TownOfMoss.Resources.PlaceCameraButton.png");
            AnimatedVentSprite = CreateSprite("TownOfMoss.Resources.AnimatedVentSealed.png");
            StaticVentSprite = CreateSprite("TownOfMoss.Resources.StaticVentSealed.png");

            PalettePatch.Load();
            ClassInjector.RegisterTypeInIl2Cpp<RainbowBehaviour>();

            // RegisterInIl2CppAttribute.Register();

            Ip = Config.Bind("Custom", "Ipv4 or Hostname", "127.0.0.1");
            Port = Config.Bind("Custom", "Port", (ushort) 22023);
            var defaultRegions = ServerManager.DefaultRegions.ToList();
            var ip = Ip.Value;
            if (Uri.CheckHostName(Ip.Value).ToString() == "Dns")
                foreach (var address in Dns.GetHostAddresses(Ip.Value))
                {
                    if (address.AddressFamily != AddressFamily.InterNetwork)
                        continue;
                    ip = address.ToString();
                    break;
                }

            ServerManager.DefaultRegions = defaultRegions.ToArray();

            //SceneManager.add_sceneLoaded((Action<Scene, LoadSceneMode>) ((scene, loadSceneMode) =>
            //{
            //    ModManager.Instance.ShowModStamp();
            //}));

            _harmony.PatchAll();
            DirtyPatches.Initialize(_harmony);
        }

        public static Sprite CreateSprite(string name)
        {
            var pixelsPerUnit = 100f;
            var pivot = new Vector2(0.5f, 0.5f);

            var assembly = Assembly.GetExecutingAssembly();
            var tex = GUIExtensions.CreateEmptyTexture();
            var imageStream = assembly.GetManifestResourceStream(name);
            var img = imageStream.ReadFully();
            LoadImage(tex, img, true);
            tex.DontDestroy();
            var sprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), pivot, pixelsPerUnit);
            sprite.DontDestroy();
            return sprite;
        }

        public static void LoadImage(Texture2D tex, byte[] data, bool markNonReadable)
        {
            _iCallLoadImage ??= IL2CPP.ResolveICall<DLoadImage>("UnityEngine.ImageConversion::LoadImage");
            var il2CPPArray = (Il2CppStructArray<byte>) data;
            _iCallLoadImage.Invoke(tex.Pointer, il2CPPArray.Pointer, markNonReadable);
        }

        private delegate bool DLoadImage(IntPtr tex, IntPtr data, bool markNonReadable);
    }

    [HarmonyPatch]
    public static class CredentialsPatch {
        [HarmonyPriority(Priority.VeryLow)]
        [HarmonyPatch(typeof(MainMenuManager), nameof(MainMenuManager.LateUpdate))]
        private static class LogoPatchUpd {
            
            static void Postfix(MainMenuManager __instance) {
                
                var ToRLogo = GameObject.Find("bannerLogo_TOR");
                
                if (ToRLogo != null) {
                    var vShower = GameObject.FindObjectOfType<VersionShower>();
                    if (vShower != null) {
                        vShower.text.text = " <color=#FF0000FF>ERROR!!異なるMODが混在しています！</color>";
                    }
    
                    var pingTracker = GameObject.FindObjectOfType<PingTracker>();
                    if (pingTracker != null) {
                        pingTracker.text.text = " <color=#FF0000FF>ERROR!!異なるMODが混在しています！</color>";
                    }
                    var _onlineButton = GameObject.Find("PlayOnlineButton");
                    if (_onlineButton) {
                        ButtonRolloverHandler component = _onlineButton.GetComponent<ButtonRolloverHandler>();
                        if (component != null) {
                            component.SetDisabledColors();
                        }
    
                        _onlineButton.GetComponent<PassiveButton>().enabled = false;
                    }
                }
            }
        }
    }
}
