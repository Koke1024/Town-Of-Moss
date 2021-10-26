using HarmonyLib;
using TownOfUs.CustomHats;
using TownOfUs.Extensions;
using UnityEngine;

namespace TownOfUs
{
    //[HarmonyPriority(Priority.VeryHigh)] // to show this message first, or be overrided if any plugins do
    [HarmonyPatch(typeof(PingTracker), nameof(PingTracker.Update))]
    public static class PingTracker_Update
    {
        private static string GenerateHatText()
        {
            HatCreation.HatData data;
            if (HatCreation.IdToData.ContainsKey(PlayerControl.LocalPlayer.Data.HatId))
                data = HatCreation.IdToData[PlayerControl.LocalPlayer.Data.HatId];
            else return "";
            return data.author == "" ? $"\n{data.name} hat" : $"\n{data.name} hat by {data.author}";
        }

        [HarmonyPrefix]
        public static void Prefix(PingTracker __instance)
        {
            // if (!__instance.GetComponentInChildren<SpriteRenderer>())
            // {
            //     var spriteObject = new GameObject("Polus Sprite");
            //     spriteObject.AddComponent<SpriteRenderer>().sprite = TownOfUs.PolusSprite;
            //     spriteObject.transform.parent = __instance.transform;
            //     spriteObject.transform.localPosition = new Vector3(-1f, -0.3f, -1);
            //     spriteObject.transform.localScale *= 0.72f;
            // }
        }

        [HarmonyPostfix]
        public static void Postfix(PingTracker __instance)
        {
            var position = __instance.GetComponent<AspectPosition>();
            position.DistanceFromEdge = new Vector3(3.2f, 0.1f, 0);
            position.AdjustPosition();

            __instance.text.text =
                "<color=#00FF00FF>Town of Moss v"+TownOfUs.Version+"</color>"+$" {AmongUsClient.Instance.Ping}ms";
        }
    }
}
