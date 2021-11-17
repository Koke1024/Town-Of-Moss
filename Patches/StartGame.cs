using System;
using System.Linq;
using Epic.OnlineServices.AntiCheatCommon;
using HarmonyLib;
using TownOfUs.Extensions;
using TownOfUs.ImpostorRoles.MinerMod;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TownOfUs.OnStartGame
{
    public enum PolusVitalPosition{
        Default,
        Labo,
        Ship,
        O2,
    }

    //vent animation off
    // [HarmonyPatch(typeof(Vent), nameof(Vent.Start))]
    // public static class RewriteVent {
    //     public static void Prefix(Vent __instance) {
    //         __instance.EnterVentAnim = null;
    //         __instance.ExitVentAnim = null;
    //     }
    // }
    
    [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.Start))]
    public static class PolusVitalMoving
    {
        public static void Postfix(ShipStatus __instance)
        {
            // foreach (var obj in GameObject.FindObjectsOfType<MonoBehaviour>()) {
            //     AmongUsExtensions.Log($"{obj.name}: {obj.transform.position.x}, {obj.transform.position.y}");
            // }
            
            if (__instance.Type != ShipStatus.MapType.Pb || CustomGameOptions.PolusVitalMove == PolusVitalPosition.Default) {
                return;
            }
            var objects = GameObject.FindObjectsOfType<MonoBehaviour>().Where(x => x.name == "panel_vitals");

            bool addVentShip = false;
            switch (CustomGameOptions.PolusVitalMove) {
                case PolusVitalPosition.Labo:
                    foreach (var obj in objects) {
                        obj.transform.position =
                            new Vector3(34.94884f, -8.82f, obj.transform.position.z);                    
                    }
                    break;
                case PolusVitalPosition.Ship:
                    foreach (var obj in objects) {
                        obj.transform.position =
                            new Vector3(16.67f, 0.12f, obj.transform.position.z); 
                    }

                    addVentShip = true;
                    break;
                case PolusVitalPosition.O2:
                    foreach (var obj in objects) {
                        obj.transform.position =
                            new Vector3(1.527f, -18.57f, obj.transform.position.z);                    
                    }
                    break;
            }

            if (addVentShip) {
                var allVents = ShipStatus.Instance.AllVents.ToList();
                var ventPrefab = Object.FindObjectOfType<Vent>();
                var vent = Object.Instantiate(ventPrefab, ventPrefab.transform.parent);
                vent.Id = MinerPerformKill.GetAvailableId();
                vent.transform.position = new Vector3(16.67f, -3.89f, 0);

                vent.Left = allVents[10]; //ElectricalBuildingVent
                vent.Right = allVents[9]; //ScienceBuildingVent
                vent.Center = null;
                vent.name = "DropShipVent";
                vent.Left.Right = vent;
                vent.Right.Right = vent;

                allVents.Add(vent);
                ShipStatus.Instance.AllVents = allVents.ToArray();
            }
        }
    }
}