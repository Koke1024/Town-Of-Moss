using HarmonyLib;
using Hazel;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Il2CppSystem;
using Il2CppSystem.Text;
using Reactor.Extensions;
using TownOfUs.CrewmateRoles.MedicMod;
using TownOfUs.CustomOption;
using TownOfUs.Extensions;
using TownOfUs.ImpostorRoles.CamouflageMod;
using TownOfUs.NeutralRoles.ZombieMod;
using TownOfUs.Roles;
using TownOfUs.Roles.Modifiers;
using UnhollowerBaseLib;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TownOfUs
{
    [HarmonyPatch]
    public static class Utils
    {
        internal static bool ShowDeadBodies = false;
        public static DeadBody myBody = null;

        public static Dictionary<byte, Color> oldColors = new Dictionary<byte, Color>();

        public static List<WinningPlayerData> potentialWinners = new List<WinningPlayerData>();
        public static Dictionary<byte, float> MeetingKillTimers = new Dictionary<byte, float>();
        public static string roleString = "";
        public static bool IsStreamMode = false;
        public static string crewRateString = "";
        public static string impRateString = "";
        public static string neutralRateString = "";
        public static readonly Dictionary<byte, DeadPlayer> KilledPlayers = new Dictionary<byte, DeadPlayer>();

        public static void Morph(PlayerControl player, PlayerControl MorphedPlayer, bool resetAnim = false)
        {
            if (CamouflageUnCamouflage.IsCamoed) return;
            if (player.GetCustomOutfitType() != CustomPlayerOutfitType.Morph)
                player.SetOutfit(CustomPlayerOutfitType.Morph, MorphedPlayer.Data.DefaultOutfit);
            
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Seer)) {
                player.nameText.text = MorphedPlayer.Data.PlayerName;
            }
        }

        public static void Unmorph(PlayerControl player)
        {
           player.SetOutfit(CustomPlayerOutfitType.Default);
        }

        public static void Camouflage()
        {
            foreach (var player in PlayerControl.AllPlayerControls)
            {
                if (player.GetCustomOutfitType() != CustomPlayerOutfitType.Camouflage)
                {
                    player.SetOutfit(CustomPlayerOutfitType.Camouflage, new GameData.PlayerOutfit()
                    {
                        ColorId = player.GetDefaultOutfit().ColorId,
                        HatId = "",
                        SkinId = "",
                        VisorId = "",
                        _playerName = " "
                    });
                    //player.nameText.text = "";
                PlayerControl.SetPlayerMaterialColors(Color.grey, player.myRend);
                }
            }
        }

        public static void UnCamouflage()
        {
            foreach (var player in PlayerControl.AllPlayerControls) Unmorph(player);
        }

        public static bool IsCrewmate(this PlayerControl player)
        {
            return GetRole(player) == RoleEnum.Crewmate;
        }

        public static void AddUnique<T>(this Il2CppSystem.Collections.Generic.List<T> self, T item)
            where T : IDisconnectHandler
        {
            if (!self.Contains(item)) self.Add(item);
        }

        public static bool isLover(this PlayerControl player)
        {
            return player.Is(RoleEnum.Lover) || player.Is(RoleEnum.LoverImpostor);
        }

        public static bool isHitWall(this PlayerControl player)
        {
            var hits = Physics2D.OverlapBoxAll(player.transform.position, new Vector2(0.6f, 0.5f), 0);
            hits = hits.ToArray().Where(c =>
                    (c.name.Contains("Vent") || !c.isTrigger) && c.gameObject.layer != 8 && c.gameObject.layer != 5)
                .ToArray();
            return hits.Count != 0;
        }

        public static bool Is(this PlayerControl player, RoleEnum roleType)
        {
            return Role.GetRole(player)?.RoleType == roleType;
        }

        public static bool Is(this PlayerControl player, ModifierEnum modifierType)
        {
            return Modifier.GetModifier(player)?.ModifierType == modifierType;
        }

        public static bool Is(this PlayerControl player, Faction faction)
        {
            return Role.GetRole(player)?.Faction == faction;
        }
        public static bool CanDrag(this PlayerControl player)
        {
            return player.Is(RoleEnum.Undertaker) || 
                   (player.Is(RoleEnum.Druid) && Role.GetRole<Druid>(player).CanRevive())
                   || 
                    (CustomGameOptions.JesterDragBody && player.Is(RoleEnum.Jester));
        }
        public static bool CanMorph(this PlayerControl player)
        {
            return player.Is(RoleEnum.Morphling) ||
                    (CustomGameOptions.JesterCanMorph && player.Is(RoleEnum.Jester));
        }
        public static bool CanSnipe(this PlayerControl player)
        {
            return player.Is(RoleEnum.Assassin) || player.Is(RoleEnum.Sniper) ||
                    (CustomGameOptions.AllImpCanGuess && player.Is(Faction.Impostors)) ||
                    (CustomGameOptions.LastImpCanGuess && player.Is(Faction.Impostors) &&
                     PlayerControl.AllPlayerControls.ToArray().Count(x => (x.Data.IsImpostor() || x.Is(RoleEnum.Assassin)) && !x.Data.IsDead) == 1);
        }

        public static List<PlayerControl> GetCrewmates(List<PlayerControl> impostors)
        {
            return PlayerControl.AllPlayerControls.ToArray().Where(
                player => !impostors.Any(imp => imp.PlayerId == player.PlayerId)
            ).ToList();
        }

        public static List<PlayerControl> GetImpostors(
            List<GameData.PlayerInfo> infected)
        {
            var impostors = new List<PlayerControl>();
            foreach (var impData in infected)
                impostors.Add(impData.Object);

            return impostors;
        }

        public static RoleEnum GetRole(PlayerControl player)
        {
            if (player == null) return RoleEnum.None;
            if (player.Data == null) return RoleEnum.None;

            var role = Role.GetRole(player);
            if (role != null) return role.RoleType;

            return player.Data.IsImpostor() ? RoleEnum.Impostor : RoleEnum.Crewmate;
        }

        public static PlayerControl PlayerById(byte id)
        {
            foreach (var player in PlayerControl.AllPlayerControls)
                if (player.PlayerId == id)
                    return player;

            return null;
        }

        public static bool isShielded(this PlayerControl player)
        {
            return Role.GetRoles(RoleEnum.BodyGuard).Any(role =>
            {
                var shieldedPlayer = ((BodyGuard)role).ShieldedPlayer;
                if (shieldedPlayer == null || player.PlayerId != shieldedPlayer.PlayerId) {
                    return false;
                }

                if (Vector2.Distance(player.GetTruePosition(), role.Player.GetTruePosition()) <=
                    CustomGameOptions.GuardRange / 2.0f) {
                    return true;
                }
                return false;
            });
        }

        public static BodyGuard getBodyGuard(this PlayerControl player)
        {
            return Role.GetRoles(RoleEnum.BodyGuard).FirstOrDefault(role =>
            {
                var shieldedPlayer = ((BodyGuard)role).ShieldedPlayer;
                return shieldedPlayer != null && player.PlayerId == shieldedPlayer.PlayerId;
            }) as BodyGuard;
        }

        public static PlayerControl getClosestPlayer(PlayerControl refPlayer, List<PlayerControl> AllPlayers)
        {
            var num = double.MaxValue;
            var refPosition = refPlayer.GetTruePosition();
            PlayerControl result = null;
            foreach (var player in AllPlayers)
            {
                if (player.Data.IsDead || player.PlayerId == refPlayer.PlayerId || !player.Collider.enabled) continue;
                var playerPosition = player.GetTruePosition();
                var distBetweenPlayers = Vector2.Distance(refPosition, playerPosition);
                var isClosest = distBetweenPlayers < num;
                if (!isClosest) continue;
                var vector = playerPosition - refPosition;
                if (PhysicsHelpers.AnyNonTriggersBetween(
                    refPosition, vector.normalized, vector.magnitude, Constants.ShipAndObjectsMask
                )) continue;
                num = distBetweenPlayers;
                result = player;
            }

            return result;
        }

        public static PlayerControl getClosestPlayer(PlayerControl refplayer)
        {
            return getClosestPlayer(refplayer, PlayerControl.AllPlayerControls.ToArray().ToList());
        }
        public static void SetTarget(
            ref PlayerControl closestPlayer,
            KillButton button,
            float maxDistance = float.NaN,
            List<PlayerControl> targets = null
        )
        {
            if (!button.isActiveAndEnabled) return;

            button.SetTarget(
                SetClosestPlayer(ref closestPlayer, maxDistance, targets)
            );
        }

        public static PlayerControl SetClosestPlayer(
            ref PlayerControl closestPlayer,
            float maxDistance = float.NaN,
            List<PlayerControl> targets = null
        )
        {
            if (float.IsNaN(maxDistance))
                maxDistance = GameOptionsData.KillDistances[PlayerControl.GameOptions.KillDistance];
            var player = getClosestPlayer(
                PlayerControl.LocalPlayer,
                targets ?? PlayerControl.AllPlayerControls.ToArray().ToList()
            );
            var closeEnough = player == null || (
                getDistBetweenPlayers(PlayerControl.LocalPlayer, player) < maxDistance
            );
            return closestPlayer = closeEnough ? player : null;
        }

        public static PlayerControl SetClosestPlayerToPlayer(
            PlayerControl fromPlayer,
            ref PlayerControl closestPlayer,
            float maxDistance = float.NaN,
            List<PlayerControl> targets = null
        )
        {
            if (float.IsNaN(maxDistance))
                maxDistance = GameOptionsData.KillDistances[PlayerControl.GameOptions.KillDistance];
            var player = getClosestPlayer(
                fromPlayer,
                targets ?? PlayerControl.AllPlayerControls.ToArray().ToList()
            );
            var closeEnough = player == null || (
                getDistBetweenPlayers(fromPlayer, player) < maxDistance
            );
            return closestPlayer = closeEnough ? player : null;
        }

        public static double getDistBetweenPlayers(PlayerControl player, PlayerControl refplayer)
        {
            var truePosition = refplayer.GetTruePosition();
            var truePosition2 = player.GetTruePosition();
            return Vector2.Distance(truePosition, truePosition2);
        }

        public static void RpcMurderPlayer(PlayerControl killer, PlayerControl target)
        {
            MurderPlayer(killer, target);
            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                (byte)CustomRPC.BypassKill, SendOption.Reliable, -1);
            writer.Write(killer.PlayerId);
            writer.Write(target.PlayerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
        }

        public static Vector3 GetNameTextPosition(this PlayerControl player) {
            if (player == PlayerControl.LocalPlayer) {
                return new Vector3(
                    0f, player.CurrentOutfit.HatId == "hat_NoHat" ? 1.5f : 2.0f,
                        -0.5f
                    );
            }
            return new Vector3(0f, -1.0f, -0.5f);
        }

        public static void MurderPlayer(PlayerControl killer, PlayerControl target)
        {
            var data = target.Data;
            if (data != null && !data.IsDead)
            {
                if (killer == PlayerControl.LocalPlayer)
                    SoundManager.Instance.PlaySound(PlayerControl.LocalPlayer.KillSfx, false, 0.8f);

                target.gameObject.layer = LayerMask.NameToLayer("Ghost");
                if (target.AmOwner)
                {
                    try
                    {
                        if (Minigame.Instance)
                        {
                            Minigame.Instance.Close();
                            Minigame.Instance.Close();
                        }

                        if (MapBehaviour.Instance)
                        {
                            MapBehaviour.Instance.Close();
                            MapBehaviour.Instance.Close();
                        }
                    }
                    catch
                    {
                    }

                    var killerData = killer.Data;
                    var isSwooped = false;
                    if (killer == target) {
                        var dollMaker = Role.GetRoles(RoleEnum.DollMaker).FirstOrDefault(r => ((DollMaker)r).DollList.ContainsKey(target.PlayerId));
                        if (dollMaker is not null && !dollMaker.Equals(default(Role))) {
                            killerData = dollMaker.Player.Data;
                            killer = dollMaker.Player;
                        }
                    }
                    if (killerData._object.Is(RoleEnum.Swooper)) {
                        var swooper = Role.GetRole<Swooper>(killer);
                        if (swooper.IsSwooped) {
                            isSwooped = true;
                        }
                    }
                    if (CamouflageUnCamouflage.IsCamoed || isSwooped) {
                        killerData = data;
                    }
                    else {
                        if (killer.CanMorph() || killer.Is(RoleEnum.Kirby)) {
                            var morph = Role.GetRole<Morphling>(killer);
                            if (morph.MorphedPlayer != null) {
                                killerData = morph.MorphedPlayer.Data;
                            }
                        }
                        else if (killer.Is(RoleEnum.Glitch)) {
                            var glitch = Role.GetRole<Glitch>(killer);
                            if (glitch.IsUsingMimic && glitch.MimicTarget != null) {
                                killerData = glitch.MimicTarget.Data;
                            }
                        }
                    }

                    DestroyableSingleton<HudManager>.Instance.KillOverlay.ShowKillAnimation(killerData, data);
                    DestroyableSingleton<HudManager>.Instance.ShadowQuad.gameObject.SetActive(false);
                    target.nameText.GetComponent<MeshRenderer>().material.SetInt("_Mask", 0);
                    target.RpcSetScanner(false);
                    var importantTextTask = new GameObject("_Player").AddComponent<ImportantTextTask>();
                    importantTextTask.transform.SetParent(AmongUsClient.Instance.transform, false);
                    if (!PlayerControl.GameOptions.GhostsDoTasks)
                    {
                        for (var i = 0; i < target.myTasks.Count; i++)
                        {
                            var playerTask = target.myTasks.ToArray()[i];
                            playerTask.OnRemove();
                            Object.Destroy(playerTask.gameObject);
                        }

                        target.myTasks.Clear();
                        importantTextTask.Text = DestroyableSingleton<TranslationController>.Instance.GetString(
                            StringNames.GhostIgnoreTasks,
                            new Il2CppReferenceArray<Il2CppSystem.Object>(0));
                    }
                    else
                    {
                        importantTextTask.Text = DestroyableSingleton<TranslationController>.Instance.GetString(
                            StringNames.GhostDoTasks,
                            new Il2CppReferenceArray<Il2CppSystem.Object>(0));
                    }

                    target.myTasks.Insert(0, importantTextTask);
                }

                killer.MyPhysics.StartCoroutine(killer.KillAnimations.Random().CoPerformKill(killer, target));
                AddDeadBody(killer, target);
                
                if (!killer.AmOwner) return;
                
                if (killer.Data.IsImpostor())
                {
                    killer.SetKillTimer(PlayerControl.GameOptions.KillCooldown);
                }
                Role.GetRole(killer).PostKill(target);

                if (target.Is(RoleEnum.Zombie)) {
                    target.GetRole<Zombie>().deadTime = DateTime.UtcNow;                    
                }

                if (target.Is(ModifierEnum.Diseased) && killer.Data.IsImpostor())
                {
                    killer.SetKillTimer(PlayerControl.GameOptions.KillCooldown * 3);
                }
            }
        }

        public static void AddDeadBody(PlayerControl killer, PlayerControl target)
        {
            var body = Object.FindObjectsOfType<DeadBody>().ToArray().FirstOrDefault(x => x.ParentId == target.PlayerId);
            //System.Console.WriteLine("FOURF");
            var deadBody = new DeadPlayer
            {
                PlayerId = target.PlayerId,
                KillerId = killer.PlayerId,
                KillTime = DateTime.UtcNow,
                Body = body
            };

            KilledPlayers.Add(target.PlayerId, deadBody);
        }
        
        public static void RpcOverrideDeadBodyInformation(byte victimId, byte killerId) {
            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                (byte)CustomRPC.SetKillerId, SendOption.Reliable, -1);
            writer.Write(victimId);
            writer.Write(killerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            OverrideDeadBodyInformation(victimId, killerId);
        }
        
        public static void OverrideDeadBodyInformation(byte victimId, byte killerId) {
            KilledPlayers[victimId].KillerId = killerId;
        }

        public static void StartFlash(Color color, float alpha = 0.3f) {
            color.a = alpha;
            if (HudManager.InstanceExists && HudManager.Instance.FullScreen)
            {
                var fullscreen = DestroyableSingleton<HudManager>.Instance.FullScreen;
                var oldcolour = fullscreen.color;
                fullscreen.enabled = true;
                fullscreen.color = color;
            }
        }
        public static void EndFlash() {
            if (HudManager.InstanceExists && HudManager.Instance.FullScreen)
            {
                var fullscreen = DestroyableSingleton<HudManager>.Instance.FullScreen;
                fullscreen.enabled = false;
            }
        }

        public static IEnumerator FlashCoroutine(Color color, float waitfor = 1f, float alpha = 0.3f)
        {
            color.a = alpha;
            if (HudManager.InstanceExists && HudManager.Instance.FullScreen)
            {
                var fullscreen = DestroyableSingleton<HudManager>.Instance.FullScreen;
                var oldcolour = fullscreen.color;
                fullscreen.enabled = true;
                fullscreen.color = color;
            }

            yield return new WaitForSeconds(waitfor);

            if (HudManager.InstanceExists && HudManager.Instance.FullScreen)
            {
                var fullscreen = DestroyableSingleton<HudManager>.Instance.FullScreen;
                fullscreen.enabled = false;
                fullscreen.color = new Color(1f, 0f, 0f, 0.37254903f);
            }
        }

        public static IEnumerator ShadowQuadFlashCoroutine(Color color, float waitfor = 1f) {
            float alpha;
            Color c = Color.black;
            if (DestroyableSingleton<HudManager>.Instance.ShadowQuad)
            {
                alpha = DestroyableSingleton<HudManager>.Instance.ShadowQuad.material.color.a;
                c = DestroyableSingleton<HudManager>.Instance.ShadowQuad.material.color;
                color.a = alpha;
                DestroyableSingleton<HudManager>.Instance.ShadowQuad.material.color = color;
            }

            yield return new WaitForSeconds(waitfor);

            if (DestroyableSingleton<HudManager>.Instance.ShadowQuad)
            {
                DestroyableSingleton<HudManager>.Instance.ShadowQuad.material.color = c;
            }
        }

        public static IEnumerable<(T1, T2)> Zip<T1, T2>(List<T1> first, List<T2> second)
        {
            return first.Zip(second, (x, y) => (x, y));
        }

        public static void DestroyAll(this IEnumerable<Component> listie)
        {
            foreach (var item in listie)
            {
                if (item == null) continue;
                Object.Destroy(item);
                if (item.gameObject == null) return;
                Object.Destroy(item.gameObject);
            }
        }

        public static void EndGame(GameOverReason reason = GameOverReason.ImpostorByVote, bool showAds = false)
        {
            ShipStatus.RpcEndGame(reason, showAds);
        }
        
        private static void settingText(ref string __result)
        {
            var builder = new StringBuilder(GameSettings.AllOptions ? __result : "");

            foreach (var option in CustomOption.CustomOption.AllOptions)
            {
                if (option.Name == "Custom Role Settings" && !GameSettings.AllOptions) break;
                if (option.Type == CustomOptionType.Button) continue;
                if (option.Type == CustomOptionType.Header) builder.AppendLine($"\n{option.Name}");
                else if (option.Indent) builder.AppendLine($"     {option.Name}: {option}");
                else builder.AppendLine($"{option.Name}: {option}");
            }


            __result = builder.ToString();


            if (CustomOption.CustomOption.LobbyTextScroller && __result.Count(c => c == '\n') > 38)
                __result = __result.Insert(__result.IndexOf('\n'), " (Scroll for more)");
            else __result = __result.Insert(__result.IndexOf('\n'), "Press Tab to see All Options");


            __result = $"<size=1.25>{__result}</size>";
        }
        
        public static void AirKill(PlayerControl player, PlayerControl target){
            Vector3 vector = target.transform.position;
            vector.z = vector.y / 1000f;
            player.transform.position = vector;
            player.NetTransform.SnapTo(vector);
        }
    
        public static float getZfromY(float y){
            return y / 1000f;
        }
        
        public static bool ExistBody(byte id) {
            return Utils.KilledPlayers.TryGetValue(id, out var body);
        }

        public static DeadBody GetBody(byte id) {
            if (!Utils.KilledPlayers.ContainsKey(id)) {
                return null;
            }
            return Utils.KilledPlayers[id].Body;
        }
    }
    
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
    public class MeetingHudStart
    {
        public static void Prefix(MeetingHud __instance)
        {
            if (CustomGameOptions.KillCoolResetOnMeeting) {
                return;
            }
            Utils.MeetingKillTimers.Clear();
            foreach (Role role in Role.GetImpRoles()) {
                Utils.MeetingKillTimers.Add(role.Player.PlayerId, role.Player.killTimer);
            }
        }
    }
    [HarmonyPriority(Priority.VeryLow)]
    [HarmonyPatch(typeof(Object), nameof(Object.Destroy), typeof(Object))]
    public static class HUDClose
    {
        public static void Postfix(Object obj)
        {
            if (CustomGameOptions.KillCoolResetOnMeeting) {
                return;
            }
            if (ExileController.Instance == null || obj != ExileController.Instance.gameObject) return;
            foreach (Role role in Role.GetImpRoles()) {
                if (!Utils.MeetingKillTimers.ContainsKey(role.Player.PlayerId)) {
                    continue;
                }
                Utils.MeetingKillTimers[role.Player.PlayerId] = Mathf.Clamp(Utils.MeetingKillTimers[role.Player.PlayerId], 5.0f, Utils.MeetingKillTimers[role.Player.PlayerId]);
                role.Player.SetKillTimer(Utils.MeetingKillTimers[role.Player.PlayerId]);
            }
        }
    }

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.Revive))]
    public static class DeadBodyRemove {
        public static void Postfix(PlayerControl __instance) {
            Utils.KilledPlayers.Remove(__instance.PlayerId);
            if (Utils.myBody) {
                if (Utils.myBody.ParentId == __instance.PlayerId) {
                    Utils.myBody = null;
                }
            }
        }
    }

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.FixedUpdate))]
    public static class PlayerUpdate {
        public static bool Prefix(PlayerControl __instance) {
            if (LobbyBehaviour.Instance) {
                return true;
            }
            if (PlayerControl.AllPlayerControls.Count <= 1) return true;
            if (__instance.GetRole() == null) {
                return true;
            }
            if (__instance.AmOwner) {
                if (!__instance.GetRole().PreFixedUpdateLocal()) {
                    return false;
                }
            }
            return __instance.GetRole().PreFixedUpdate();
        }
        public static void Postfix(PlayerControl __instance) {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (__instance.GetRole() == null) {
                return;
            }
            if (__instance.AmOwner) {
                __instance.GetRole().PostFixedUpdateLocal();
            }
            __instance.GetRole().PostFixedUpdate();
        }
    }

    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class HudUpdate {
        public static bool Prefix(HudManager __instance) {
            if (PlayerControl.AllPlayerControls.Count <= 1 || ShipStatus.Instance == null ||
                PlayerControl.LocalPlayer == null || PlayerControl.LocalPlayer.Data == null) {
                return true;
            }
            if (PlayerControl.LocalPlayer.GetRole() == null) {
                return true;
            }
            
            return PlayerControl.LocalPlayer.GetRole().PreHudUpdate(__instance);
        }
        public static void Postfix(HudManager __instance) {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer.GetRole() == null) {
                return;
            }
            PlayerControl.LocalPlayer.GetRole().PostHudUpdate(__instance);
        }
    }

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.FixedUpdate))]
    public static class Initialize {
        public static void Prefix(PlayerControl __instance) {
            var role = __instance.GetRole();
            if (role == null) return;
            if (HudManager._instance.isIntroDisplayed) {
                role.firstInitialize = false;
            }
            if (role.firstInitialize) {
                return;
            }
            role.Initialize();
            if (role.Player.AmOwner) {
                role.InitializeLocal();                
            }
            role.firstInitialize = true;
        }
    }
    
}
