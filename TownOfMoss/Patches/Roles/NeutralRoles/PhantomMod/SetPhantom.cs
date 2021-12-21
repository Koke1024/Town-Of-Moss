using System;
using System.Linq;
using HarmonyLib;
using Hazel;
using TownOfUs.Roles;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace TownOfUs.NeutralRoles.PhantomMod
{
    [HarmonyPatch(typeof(AirshipExileController), nameof(AirshipExileController.WrapUpAndSpawn))]
    public static class AirshipExileController_WrapUpAndSpawn
    {
        public static void Postfix(AirshipExileController __instance) => SetPhantom.ExileControllerPostfix(__instance);
    }
    
    [HarmonyPatch(typeof(ExileController), nameof(ExileController.WrapUp))]
    public class SetPhantom
    {
        public static PlayerControl WillBePhantom;
        public static Vector2 StartPosition;

        public static void ExileControllerPostfix(ExileController __instance)
        {
            var exiled = __instance.exiled?.Object;
            if (!PlayerControl.LocalPlayer.Data.IsDead && exiled != PlayerControl.LocalPlayer) return;
            if (exiled == PlayerControl.LocalPlayer && PlayerControl.LocalPlayer.Is(RoleEnum.Jester)) return;
            if (PlayerControl.LocalPlayer != WillBePhantom) return;

            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Phantom))
            {
                Role.RoleDictionary.Remove(PlayerControl.LocalPlayer.PlayerId);
                var role = new Phantom(PlayerControl.LocalPlayer);
                role.RegenTask();
                Lights.SetLights();

                RemoveTasks(PlayerControl.LocalPlayer);
                PlayerControl.LocalPlayer.MyPhysics.ResetMoveState();

                System.Console.WriteLine("Become Phantom - Phantom");

                PlayerControl.LocalPlayer.gameObject.layer = LayerMask.NameToLayer("Players");

                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                    (byte) CustomRPC.PhantomDied, SendOption.Reliable, -1);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
            }

            if (Role.GetRole<Phantom>(PlayerControl.LocalPlayer).Caught) return;
            Vent startingVent = null;
            if (ShipStatus.Instance.Type == ShipStatus.MapType.Pb) {
                startingVent =
                    ShipStatus.Instance.AllVents.Where(x => x.name != "AdminVent").ToArray()[
                        Random.RandomRangeInt(0, ShipStatus.Instance.AllVents.Count - 1)];
            }
            else {
                startingVent =
                    ShipStatus.Instance.AllVents[Random.RandomRangeInt(0, ShipStatus.Instance.AllVents.Count)];                
            }
            PlayerControl.LocalPlayer.NetTransform.RpcSnapTo(startingVent.transform.position);
            PlayerControl.LocalPlayer.MyPhysics.RpcEnterVent(startingVent.Id);
        }

        public static void Postfix(ExileController __instance) => ExileControllerPostfix(__instance);

        public static void RemoveTasks(PlayerControl player)
        {
            var totalTasks = PlayerControl.GameOptions.NumCommonTasks + PlayerControl.GameOptions.NumLongTasks +
                             PlayerControl.GameOptions.NumShortTasks;


            foreach (var task in player.myTasks)
                if (task.TryCast<NormalPlayerTask>() != null)
                {
                    var normalPlayerTask = task.Cast<NormalPlayerTask>();

                    var updateArrow = normalPlayerTask.taskStep > 0;
                    
                    normalPlayerTask.taskStep = 0;
                    normalPlayerTask.Initialize();
                    if (normalPlayerTask.TaskType == TaskTypes.PickUpTowels)
                        foreach (var console in Object.FindObjectsOfType<TowelTaskConsole>())
                            console.Image.color = Color.white;
                    normalPlayerTask.taskStep = 0;

                    if (updateArrow)
                        normalPlayerTask.UpdateArrow();
                    
                    var taskInfo = player.Data.FindTaskById(task.Id);
                    taskInfo.Complete = false;
                }
        }

        /*public static void ResetTowels(NormalPlayerTask task)
        {
            var towelTask = task.Cast<TowelTask>();
            var data = new byte[8];
            var array = Enumerable.Range(0, 14).ToList();
            array.Shuffle();
            var b3 = 0;
            while (b3 < data.Length)
            {
                data[b3] = (byte)array[b3];
                b3++;
            }

            towelTask.Data = data;
            return;
        }

        public static void ResetRecords(NormalPlayerTask task)
        {
            task.Data = new 
        }*/

        public static void AddCollider(Phantom role)
        {
            var player = role.Player;
            var collider2d = player.gameObject.AddComponent<BoxCollider2D>();
            collider2d.isTrigger = true;
            var button = player.gameObject.AddComponent<PassiveButton>();
            button.OnClick = new Button.ButtonClickedEvent();
            button.OnMouseOut = new Button.ButtonClickedEvent();
            button.OnMouseOver = new Button.ButtonClickedEvent();

            button.OnClick.AddListener((Action) (() =>
            {
                if (MeetingHud.Instance) return;
                if (PlayerControl.LocalPlayer.Data.IsDead) return;
                role.Caught = true;
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                    (byte) CustomRPC.CatchPhantom, SendOption.Reliable, -1);
                writer.Write(role.Player.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
            }));
        }
    }
}