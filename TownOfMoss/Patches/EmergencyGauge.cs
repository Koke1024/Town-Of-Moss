using HarmonyLib;
using System.Collections;
using BepInEx.IL2CPP.Utils;
using TMPro;
using TownOfUs.Extensions;
using UnityEngine;

namespace TownOfUs {
    [HarmonyPatch(typeof(ProgressTracker), nameof(ProgressTracker.FixedUpdate))]
    public static class EmergencyGaugePatch {
        public static ProgressTracker tracker; 
        public static TextMeshPro text = null; 
        public static bool Prefix(ProgressTracker __instance) {
            if (text == null) {
                text = __instance.GetComponentInChildren<TextMeshPro>();
            }
            var system = ShipStatus.Instance.Systems;
            float countDown = 0;

            if (system.ContainsKey(SystemTypes.Reactor))
            {
                var reactorSystemType = system[SystemTypes.Reactor].Cast<ICriticalSabotage>();
                if (reactorSystemType.IsActive) {
                    countDown = reactorSystemType.Countdown;
                }
            }
            
            if (system.ContainsKey(SystemTypes.LifeSupp))
            {
                var lifeSuppSystemType = system[SystemTypes.LifeSupp].Cast<LifeSuppSystemType>();
                if (lifeSuppSystemType.IsActive) {
                    countDown = lifeSuppSystemType.Countdown;                    
                }
            }

            if (system.ContainsKey(SystemTypes.Laboratory))
            {
                var reactorSystemType = system[SystemTypes.Laboratory].Cast<ReactorSystemType>();
                if (reactorSystemType.IsActive) {
                    countDown = reactorSystemType.Countdown;                    
                }
            }

            if (countDown != 0) {
                float curValue = countDown / 60.0f;
                __instance.TileParent.material.SetFloat("_Buckets", 1);
                __instance.TileParent.material.SetFloat("_FullBuckets", Mathf.Clamp(curValue, 0, 1));
                text.gameObject.SetActive(false);
                return false;
            }
            text.gameObject.SetActive(true);
            return true;
        }
        
        private static IEnumerator CoEmergencyGauge(this HudManager hudManager)
        {
            hudManager.TaskCompleteOverlay.gameObject.SetActive(true);
            yield return Effects.Slide2D(hudManager.TaskCompleteOverlay, new Vector2(0f, -8f), Vector2.zero, 0.25f);
            for (float time = 0f; time < 0.75f; time += Time.deltaTime)
            {
                yield return null;
            }
            yield return Effects.Slide2D(hudManager.TaskCompleteOverlay, Vector2.zero, new Vector2(0f, 8f), 0.25f);
            hudManager.TaskCompleteOverlay.gameObject.SetActive(false);
            yield break;
        }
    }
}