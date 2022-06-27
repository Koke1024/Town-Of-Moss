using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using BepInEx.Logging;
using Reactor;
using Reactor.Extensions;
using UnityEngine;
using System.Text.Json;

namespace TownOfUs.Patches.CustomHats
{
    internal static class HatLoader
    {
        private const string HAT_RESOURCE_NAMESPACE = "TownOfMoss.Resources.Hats";
        private const string HAT_METADATA_JSON = "metadata.json";
        private const int HAT_ORDER_BASELINE = 99;

        private static ManualLogSource Log => PluginSingleton<TownOfUs>.Instance.Log;
        private static Assembly Assembly => typeof(TownOfUs).Assembly;

        private static bool LoadedHats = false;

        internal static void LoadHatsRoutine()
        {
            if (LoadedHats || !DestroyableSingleton<HatManager>.InstanceExists || DestroyableSingleton<HatManager>.Instance.allHats.Count == 0)
                return;
            LoadedHats = true;
            Coroutines.Start(LoadHats());
        }

        internal static IEnumerator LoadHats() {
            try
            {
                var hatJson = LoadJson();
                var hatData = DiscoverHatData(hatJson);

                DestroyableSingleton<HatManager>.Instance.allHats.ForEach(
                    (Action<HatData>)(x => x.StoreName = "Vanilla")
                );
                var originalCount = DestroyableSingleton<HatManager>.Instance.allHats.Count;
                for (var i = 0; i < hatData.Count; i++)
                {
                    hatData[i].displayOrder = originalCount + i;
                    HatManager.Instance.allHats.Add(hatData[i]);
                }
            }
            catch (Exception e)
            {
                Log.LogError($"Error while loading hats: {e.Message}\nStack: {e.StackTrace}");
            }
            yield return null;
        }

        private static HatMetadataJson LoadJson() {
            var stream = Assembly.GetManifestResourceStream($"{HAT_RESOURCE_NAMESPACE}.{HAT_METADATA_JSON}");
            var str = Encoding.UTF8.GetString(stream.ReadFully());
            string trimmed = String.Concat(str.Where(c => !Char.IsWhiteSpace(c)));
            var obj = JsonSerializer.Deserialize<HatMetadataJson>(trimmed);
            return obj;
        }

        private static List<HatData> DiscoverHatData(HatMetadataJson metadata)
        {
            var hatData = new List<HatData>();

            foreach (var hatCredit in metadata.Credits)
            {
                try
                {
                    var stream = Assembly.GetManifestResourceStream($"{HAT_RESOURCE_NAMESPACE}.{hatCredit.Id}.png");
                    if (stream != null)
                    {
                        var HatData = GenerateHatData(stream.ReadFully());
                        HatData.StoreName = hatCredit.Artist;
                        HatData.ProductId = hatCredit.Id;
                        HatData.name = hatCredit.Name;
                        HatData.Free = true;
                        hatData.Add(HatData);
                    }
                }
                catch (Exception e)
                {
                   Log.LogError(
                        $"Error loading hat {hatCredit.Id} in metadata file ({HAT_METADATA_JSON})");
                     Log.LogError($"{e.Message}\nStack:{e.StackTrace}");
                }
            }
            return hatData;
        }

        private static HatData GenerateHatData(byte[] mainImg)
        {
            
            //TODO: Move to Graphics Utils class
            var tex2D = new Texture2D(1, 1, TextureFormat.ARGB32, false);
            TownOfUs.LoadImage(tex2D, mainImg, false);
            var sprite = Sprite.Create(tex2D, new Rect(0.0f, 0.0f, tex2D.width, tex2D.height), new Vector2(0.5f, 0.5f), 100);
            
            
            var hat = ScriptableObject.CreateInstance<HatData>();
            hat.hatViewData.viewData.MainImage = sprite;
            hat.ChipOffset = new Vector2(-0.1f, 0.35f);

            hat.InFront = true;
            hat.NoBounce = true;

            return hat;
        }
    }
}

