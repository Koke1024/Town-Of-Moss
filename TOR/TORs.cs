using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using TownOfUs.Roles;
using UnityEngine;

namespace TheOtherRoles {
    static class MapOptions {
        public static List<SurvCamera> camerasToAdd = new List<SurvCamera>();
        public static List<Vent> ventsToSeal = new List<Vent>();

        public static void clearAndReloadMapOptions() {
            camerasToAdd = new List<SurvCamera>();
            ventsToSeal = new List<Vent>();
        }
    }

    [HarmonyPatch(typeof(ExileController), "Begin")]
    class ExileControllerBeginPatch {
        public static void Prefix(ExileController __instance, [HarmonyArgument(0)] ref GameData.PlayerInfo exiled,
            [HarmonyArgument(1)] bool tie) {

            if (ShipStatus.Instance == null || ShipStatus.Instance.AllCameras == null || MapOptions.camerasToAdd == null) {
                return;
            }

            var allCameras = ShipStatus.Instance.AllCameras.ToList();
            MapOptions.camerasToAdd.ForEach(camera => {
                camera.gameObject.SetActive(true);
                camera.gameObject.GetComponent<SpriteRenderer>().color = Color.white;
                allCameras.Add(camera);
            });
            ShipStatus.Instance.AllCameras = allCameras.ToArray();
            MapOptions.camerasToAdd = new List<SurvCamera>();
            
            if (MapOptions.ventsToSeal == null) {
                return;
            }
            foreach (Vent vent in MapOptions.ventsToSeal) {
                if (vent == null) {
                    return;
                }
                PowerTools.SpriteAnim animator = vent.GetComponent<PowerTools.SpriteAnim>();
                if (animator == null) {
                    return;
                }
                animator?.Stop();
                vent.EnterVentAnim = vent.ExitVentAnim = null;
                vent.myRend.sprite = animator == null ? SecurityGuard.getStaticVentSealedSprite() : SecurityGuard.getAnimatedVentSealedSprite();
                vent.myRend.color = Color.white;
                vent.name = "SealedVent_" + vent.name;
            }
            MapOptions.ventsToSeal = new List<Vent>();
        }
    }
}