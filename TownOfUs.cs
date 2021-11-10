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
using TownOfUs.CustomHats;
using TownOfUs.CustomOption;
using TownOfUs.Extensions;
using TownOfUs.RainbowMod;
using UnhollowerBaseLib;
using UnhollowerRuntimeLib;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TownOfUs
{
    [BepInPlugin(Id, "Town Of Moss", "0.403")]
    [BepInDependency(ReactorPlugin.Id)]
    public class TownOfUs : BasePlugin
    {
        public static string Version = "0.403";
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
        public static Sprite ShiftKill;
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
        public static Sprite PourSprite;

        private static DLoadImage _iCallLoadImage;


        private Harmony _harmony;

        public ConfigEntry<string> Ip { get; set; }

        public ConfigEntry<ushort> Port { get; set; }
        //public static Sprite BirthdayVoteSprite;


        public override void Load()
        {
            System.Console.WriteLine("000.000.000.000/000000000000000000");

            _harmony = new Harmony("jp.spiel.koke");

            Generate.GenerateAll();

            JanitorClean = CreateSprite("TownOfUs.Resources.Janitor.png");
            Inhale = CreateSprite("TownOfUs.Resources.Inhale.png");
            EngineerFix = CreateSprite("TownOfUs.Resources.Engineer.png");
            //EngineerArrow = CreateSprite("TownOfUs.Resources.EngineerArrow.png");
            SwapperSwitch = CreateSprite("TownOfUs.Resources.SwapperSwitch.png");
            SwapperSwitchDisabled = CreateSprite("TownOfUs.Resources.SwapperSwitchDisabled.png");
            Shift = CreateSprite("TownOfUs.Resources.Shift.png");
            Footprint = CreateSprite("TownOfUs.Resources.Footprint.png");
            Rewind = CreateSprite("TownOfUs.Resources.Rewind.png");
            NormalKill = CreateSprite("TownOfUs.Resources.NormalKill.png");
            ShiftKill = CreateSprite("TownOfUs.Resources.ShiftKill.png");
            MedicSprite = CreateSprite("TownOfUs.Resources.Medic.png");
            SeerSprite = CreateSprite("TownOfUs.Resources.Seer.png");
            SampleSprite = CreateSprite("TownOfUs.Resources.Sample.png");
            MorphSprite = CreateSprite("TownOfUs.Resources.Morph.png");
            PossessSprite = CreateSprite("TownOfUs.Resources.Possess.png");
            ReleaseSprite = CreateSprite("TownOfUs.Resources.Release.png");
            Camouflage = CreateSprite("TownOfUs.Resources.Camouflage.png");
            Arrow = CreateSprite("TownOfUs.Resources.Arrow.png");
            // Abstain = CreateSprite("TownOfUs.Resources.Abstain.png");
            // Extend = CreateSprite("TownOfUs.Resources.Extend.png");
            MineSprite = CreateSprite("TownOfUs.Resources.Mine.png");
            SwoopSprite = CreateSprite("TownOfUs.Resources.Swoop.png");
            DouseSprite = CreateSprite("TownOfUs.Resources.Douse.png");
            IgniteSprite = CreateSprite("TownOfUs.Resources.Ignite.png");
            ReviveSprite = CreateSprite("TownOfUs.Resources.Revive.png");
            ButtonSprite = CreateSprite("TownOfUs.Resources.Button.png");
            DragSprite = CreateSprite("TownOfUs.Resources.Drag.png");
            DropSprite = CreateSprite("TownOfUs.Resources.Drop.png");
            PolusSprite = CreateSprite("TownOfUs.Resources.polus.gg.png");
            TargetSprite = CreateSprite("TownOfUs.Resources.TargetIcon.png");
            HackSprite = CreateSprite("TownOfUs.Resources.Hack.png");
            WaxSprite = CreateSprite("TownOfUs.Resources.Wax.png");
            PaintSprite[0] = CreateSprite("TownOfUs.Resources.PaintRed.png");
            PaintSprite[1] = CreateSprite("TownOfUs.Resources.PaintBlue.png");
            PaintSprite[2] = CreateSprite("TownOfUs.Resources.PaintYellow.png");
            InkSprite = CreateSprite("TownOfUs.Resources.Ink.png");
            PourSprite = CreateSprite("TownOfUs.Resources.Pour.png");
            CloseVentButtonSprite = CreateSprite("TownOfUs.Resources.CloseVentButton.png");
            PlaceCameraSprite = CreateSprite("TownOfUs.Resources.PlaceCameraButton.png");
            AnimatedVentSprite = CreateSprite("TownOfUs.Resources.AnimatedVentSealed.png");
            StaticVentSprite = CreateSprite("TownOfUs.Resources.StaticVentSealed.png");

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

            // ServerManager.Instance.AddOrUpdateRegion(new StaticRegionInfo(
            // 	"Custom-Server", StringNames.NoTranslation, ip, new ServerInfo[]
            // 	{
            // 		new ServerInfo("Custom-Server", ip, Port.Value)
            // 	}
            // ).Cast<IRegionInfo>());

            ServerManager.DefaultRegions = defaultRegions.ToArray();

            SceneManager.add_sceneLoaded((Action<Scene, LoadSceneMode>) ((scene, loadSceneMode) =>
            {
                ModManager.Instance.ShowModStamp();
            }));

            _harmony.PatchAll();
            DirtyPatches.Initialize(_harmony);
        }

        public static Sprite CreateSprite(string name, bool hat = false)
        {
            var pixelsPerUnit = hat ? 225f : 100f;
            var pivot = hat ? new Vector2(0.5f, 0.8f) : new Vector2(0.5f, 0.5f);

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

        public static Sprite CreatePolusHat(string name)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var imageStream = assembly.GetManifestResourceStream(name);
            var img = imageStream.ReadFully();

            var tex = new Texture2D(128, 128, (TextureFormat) 1, false);
            LoadImage(tex, img, false);
            tex.DontDestroy();
            var sprite = Sprite.Create(tex, new Rect(0f, 0f, tex.width, tex.height), new Vector2(0.5f, 0.5f));
            sprite.DontDestroy();
            return sprite;
        }

        private static void LoadImage(Texture2D tex, byte[] data, bool markNonReadable)
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
