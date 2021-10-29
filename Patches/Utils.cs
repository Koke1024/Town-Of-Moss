using HarmonyLib;
using Hazel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Il2CppSystem.Text;
using Reactor.Extensions;
using TMPro;
using TownOfUs.CrewmateRoles.MedicMod;
using TownOfUs.CustomHats;
using TownOfUs.CustomOption;
using TownOfUs.Extensions;
using TownOfUs.ImpostorRoles.CamouflageMod;
using TownOfUs.Roles;
using TownOfUs.Roles.Modifiers;
using UnhollowerBaseLib;
using UnityEngine;
using Object = UnityEngine.Object;
using PerformKill = TownOfUs.ImpostorRoles.UnderdogMod.PerformKill;

namespace TownOfUs
{
    [HarmonyPatch]
    public static class Utils
    {
        internal static bool ShowDeadBodies = false;

        public static Dictionary<byte, Color> oldColors = new Dictionary<byte, Color>();

        public static List<WinningPlayerData> potentialWinners = new List<WinningPlayerData>();
        public static Dictionary<byte, float> MeetingKillTimers = new Dictionary<byte, float>();
        public static string roleString = "";
        public static bool IsStreamMode = false;

        public static void SetSkin(PlayerControl Player, uint skin)
        {
            Player.MyPhysics.SetSkin(skin);
        }

        public static void Morph(PlayerControl Player, PlayerControl MorphedPlayer, bool resetAnim = false)
        {
            if (CamouflageUnCamouflage.IsCamoed) return;

            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Seer))
            {
                Player.nameText.text = MorphedPlayer.Data.PlayerName;
            }

            var targetAppearance = MorphedPlayer.GetDefaultAppearance();

            PlayerControl.SetPlayerMaterialColors(targetAppearance.ColorId, Player.myRend);
            Player.HatRenderer.SetHat(targetAppearance.HatId, targetAppearance.ColorId);
            Player.nameText.transform.localPosition = new Vector3(
                0f,
                Player.Data.HatId == 0U ? 1.5f :
                HatCreation.TallIds.Contains(Player.Data.HatId) ? 2.2f : 2.0f,
                -0.5f
            );

            if (Player.MyPhysics.Skin.skin.ProdId != DestroyableSingleton<HatManager>.Instance
                .AllSkins.ToArray()[(int)targetAppearance.SkinId].ProdId)
                SetSkin(Player, targetAppearance.SkinId);

            if (Player.CurrentPet == null || Player.CurrentPet.ProdId !=
                DestroyableSingleton<HatManager>.Instance.AllPets.ToArray()[(int)targetAppearance.PetId].ProdId)
            {
                if (Player.CurrentPet != null) Object.Destroy(Player.CurrentPet.gameObject);

                Player.CurrentPet =
                    Object.Instantiate(
                        DestroyableSingleton<HatManager>.Instance.AllPets.ToArray()[(int)targetAppearance.PetId]);
                Player.CurrentPet.transform.position = Player.transform.position;
                Player.CurrentPet.Source = Player;
                Player.CurrentPet.Visible = Player.Visible;
            }

            PlayerControl.SetPlayerMaterialColors(targetAppearance.ColorId, Player.CurrentPet.rend);
            /*if (resetAnim && !Player.inVent)
            {
                Player.MyPhysics.ResetAnim();
            }*/
        }

        public static void Unmorph(PlayerControl Player)
        {
            var appearance = Player.GetDefaultAppearance();

            Player.nameText.text = Player.Data.PlayerName;
            PlayerControl.SetPlayerMaterialColors(appearance.ColorId, Player.myRend);
            Player.HatRenderer.SetHat(appearance.HatId, appearance.ColorId);
            Player.nameText.transform.localPosition = new Vector3(
                0f,
                appearance.HatId == 0U ? 1.5f :
                HatCreation.TallIds.Contains(appearance.HatId) ? 2.2f : 2.0f,
                -0.5f
            );

            if (Player.MyPhysics.Skin.skin.ProdId != DestroyableSingleton<HatManager>.Instance
                .AllSkins.ToArray()[(int)appearance.SkinId].ProdId)
                SetSkin(Player, appearance.SkinId);

            if (Player.CurrentPet != null) Object.Destroy(Player.CurrentPet.gameObject);

            Player.CurrentPet =
                Object.Instantiate(
                    DestroyableSingleton<HatManager>.Instance.AllPets.ToArray()[(int)appearance.PetId]);
            Player.CurrentPet.transform.position = Player.transform.position;
            Player.CurrentPet.Source = Player;
            Player.CurrentPet.Visible = Player.Visible;

            PlayerControl.SetPlayerMaterialColors(appearance.ColorId, Player.CurrentPet.rend);

            /*if (!Player.inVent)
            {
                Player.MyPhysics.ResetAnim();
            }*/
        }

        public static void Camouflage()
        {
            foreach (var player in PlayerControl.AllPlayerControls)
            {
                player.nameText.text = "";
                PlayerControl.SetPlayerMaterialColors(Color.grey, player.myRend);
                player.HatRenderer.SetHat(0, 0);
                if (player.MyPhysics.Skin.skin.ProdId != DestroyableSingleton<HatManager>.Instance
                    .AllSkins.ToArray()[0].ProdId)
                    SetSkin(player, 0);

                if (player.CurrentPet != null) Object.Destroy(player.CurrentPet.gameObject);
                player.CurrentPet =
                    Object.Instantiate(
                        DestroyableSingleton<HatManager>.Instance.AllPets.ToArray()[0]);
                player.CurrentPet.transform.position = player.transform.position;
                player.CurrentPet.Source = player;
                player.CurrentPet.Visible = player.Visible;
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
            return player.Is(RoleEnum.Undertaker) || player.Is(RoleEnum.Druid) || 
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
                     PlayerControl.AllPlayerControls.ToArray().Count(x => (x.Data.IsImpostor || x.Is(RoleEnum.Assassin)) && !x.Data.IsDead) == 1);
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

            return player.Data.IsImpostor ? RoleEnum.Impostor : RoleEnum.Crewmate;
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
            return Role.GetRoles(RoleEnum.Medic).Any(role =>
            {
                var shieldedPlayer = ((Medic)role).ShieldedPlayer;
                return shieldedPlayer != null && player.PlayerId == shieldedPlayer.PlayerId;
            });
        }

        public static Medic getMedic(this PlayerControl player)
        {
            return Role.GetRoles(RoleEnum.Medic).FirstOrDefault(role =>
            {
                var shieldedPlayer = ((Medic)role).ShieldedPlayer;
                return shieldedPlayer != null && player.PlayerId == shieldedPlayer.PlayerId;
            }) as Medic;
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
            KillButtonManager button,
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
                        if (Utils.CanMorph(killer) || killer.Is(RoleEnum.Kirby)) {
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
                var deadBody = new DeadPlayer
                {
                    PlayerId = target.PlayerId,
                    KillerId = killer.PlayerId,
                    KillTime = DateTime.UtcNow
                };

                Murder.KilledPlayers.Add(deadBody);
                
                if (!killer.AmOwner) return;

                if (target.Is(ModifierEnum.Diseased) && killer.Is(RoleEnum.Glitch))
                {
                    var glitch = Role.GetRole<Glitch>(killer);
                    glitch.LastKill = DateTime.UtcNow.AddSeconds(2 * CustomGameOptions.GlitchKillCooldown);
                    glitch.Player.SetKillTimer(CustomGameOptions.GlitchKillCooldown * 3);
                    return;
                }

                if (target.Is(ModifierEnum.Diseased) && killer.Data.IsImpostor)
                {
                    killer.SetKillTimer(PlayerControl.GameOptions.KillCooldown * 3);
                    return;
                }

                if (killer.Is(RoleEnum.Underdog))
                {
                    killer.SetKillTimer(PlayerControl.GameOptions.KillCooldown * (PerformKill.LastImp() ? 0.5f : 1.5f));
                    return;
                }

                if (killer.Is(RoleEnum.MultiKiller)) {
                    MultiKiller mk = Role.GetRole<MultiKiller>(killer);
                    if (!mk.killedOnce) {
                        killer.SetKillTimer(0);
                        mk.firstKillTime = DateTime.UtcNow;
                    }
                    else {
                        killer.SetKillTimer(mk.MaxTimer());
                    }
                    mk.killedOnce = !mk.killedOnce;
                    return;
                }

                if (killer.Data.IsImpostor)
                {
                    killer.SetKillTimer(PlayerControl.GameOptions.KillCooldown);
                }
            }
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

        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.SetInfected))]
        public static class PlayerControl_SetInfected
        {
            public static void Postfix()
            {
                if (!RpcHandling.Check(20)) return;

                if (PlayerControl.LocalPlayer.name == "Sykkuno")
                {
                    var edison = PlayerControl.AllPlayerControls.ToArray()
                        .FirstOrDefault(x => x.name == "Edis0n" || x.name == "Edison");
                    if (edison != null)
                    {
                        edison.name = "babe";
                        edison.nameText.text = "babe";
                    }
                }

                if (PlayerControl.LocalPlayer.name == "fuslie PhD")
                {
                    var sykkuno = PlayerControl.AllPlayerControls.ToArray()
                        .FirstOrDefault(x => x.name == "Sykkuno");
                    if (sykkuno != null)
                    {
                        sykkuno.name = "babe's babe";
                        sykkuno.nameText.text = "babe's babe";
                    }
                }
            }
        }

        // [HarmonyPatch(typeof(OptionsMenuBehaviour), nameof(OptionsMenuBehaviour.Open))]
        // public static class SettingStringDisplayOpen
        // {
        //     public static void Prefix(OptionsMenuBehaviour __instance)
        //     {
        //         if (settingString != "") {
        //             GameObject.Find("setting string").SetActive(true);
        //             return;
        //         }
        //         settingText(ref settingString);
        //
        //         var position = Camera.main.ViewportToWorldPoint(new Vector3(0f, 1f, Camera.main.nearClipPlane));
        //         var roleSummary = new GameObject("setting string");
        //         roleSummary.AddComponent<TextMeshPro>();
        //         roleSummary.transform.position = new Vector3(position.x + 0.1f,
        //             position.y - 0.1f, -14f);
        //         roleSummary.transform.localScale = new Vector3(1f, 1f, 1f);
        //
        //         var roleSummaryText = new StringBuilder();
        //         roleSummaryText.AppendLine("Players and roles at the end of the game:");
        //
        //         TMPro.TMP_Text roleSummaryTextMesh = roleSummary.GetComponent<TMPro.TMP_Text>();
        //         roleSummaryTextMesh.alignment = TMPro.TextAlignmentOptions.TopLeft;
        //         roleSummaryTextMesh.color = Color.white;
        //         roleSummaryTextMesh.fontSizeMin = 1.5f;
        //         roleSummaryTextMesh.fontSizeMax = 1.5f;
        //         roleSummaryTextMesh.fontSize = 1.5f;
        //         var roleSummaryTextMeshRectTransform = roleSummaryTextMesh.GetComponent<RectTransform>();
        //         roleSummaryTextMeshRectTransform.anchoredPosition =
        //             new Vector2(position.x + 3.5f, position.y - 0.1f);
        //         roleSummaryTextMesh.text = Utils.settingString;
        //     }
        // }
        // [HarmonyPatch(typeof(OptionsMenuBehaviour), nameof(OptionsMenuBehaviour.Close))]
        // public static class SettingStringDisplayClose
        // {
        //     public static void Prefix(OptionsMenuBehaviour __instance) {
        //         GameObject.Find("setting string").SetActive(false);
        //     }
        // }
        
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
    
}
