using System;
using System.Collections.Generic;
using System.Linq;
using GameCustomize;
using HarmonyLib;
using Hazel;
using Reactor;
using Reactor.Extensions;
using TownOfUs.CrewmateRoles.AltruistMod;
using TownOfUs.CrewmateRoles.ChargerMod;
using TownOfUs.CrewmateRoles.MedicMod;
using TownOfUs.CrewmateRoles.SwapperMod;
using TownOfUs.CrewmateRoles.TimeLordMod;
using TownOfUs.CustomOption;
using TownOfUs.Extensions;
using TownOfUs.ImpostorRoles.AssassinMod;
using TownOfUs.ImpostorRoles.MinerMod;
using TownOfUs.NeutralRoles.ExecutionerMod;
using TownOfUs.NeutralRoles.PhantomMod;
using TownOfUs.NeutralRoles.ArsonistMod;
using TownOfUs.ImpostorRoles.KirbyMod;
using TownOfUs.Patches.CrewmateRoles.SecurityGuardMod;
using TownOfUs.Patches.NeutralRoles.ZombieMod;
using TownOfUs.Roles;
using TownOfUs.Roles.Modifiers;
using UnhollowerBaseLib;
using UnityEngine;
using Coroutine = TownOfUs.ImpostorRoles.JanitorMod.Coroutine;
using Object = UnityEngine.Object;
using PerformKillButton = TownOfUs.NeutralRoles.ShifterMod.PerformKillButton;
using Random = UnityEngine.Random; //using Il2CppSystem;

namespace TownOfUs
{
    public static class RpcHandling
    {
        private static readonly List<(Type, CustomRPC, int)> CrewmateRoles = new List<(Type, CustomRPC, int)>();
        private static readonly List<(Type, CustomRPC, int)> GlitchRoles = new List<(Type, CustomRPC, int)>();
        private static readonly List<(Type, CustomRPC, int)> NeutralRoles = new List<(Type, CustomRPC, int)>();
        private static readonly List<(Type, CustomRPC, int)> ImpostorRoles = new List<(Type, CustomRPC, int)>();
        private static readonly List<(Type, CustomRPC, int)> CrewmateModifiers = new List<(Type, CustomRPC, int)>();
        private static readonly List<(Type, CustomRPC, int)> GlobalModifiers = new List<(Type, CustomRPC, int)>();
        private static bool LoversOn;
        private static bool PhantomOn;

        internal static bool Check(int probability)
        {
            if (probability == 0) return false;
            if (probability == 100) return true;
            var num = Random.RandomRangeInt(1, 101);
            return num <= probability;
        }

        /*
        private static void GenExe(List<GameData.PlayerInfo> infected, List<PlayerControl> crewmates)
        {
            PlayerControl pc;
            var targets = Utils.getCrewmates(infected).Where(x =>
            {
                var role = Role.GetRole(x);
                if (role == null) return true;
                return role.Faction == Faction.Crewmates;
            }).ToList();
            if (targets.Count > 1)
            {
                var rand = Random.RandomRangeInt(0, targets.Count);
                pc = targets[rand];
                var role = Role.Gen(typeof(Executioner), crewmates.Where(x => x.PlayerId != pc.PlayerId).ToList(),
                    CustomRPC.SetExecutioner);
                if (role != null)
                {
                    crewmates.Remove(role.Player);
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                        (byte) CustomRPC.SetTarget, SendOption.Reliable, -1);
                    writer.Write(role.Player.PlayerId);
                    writer.Write(pc.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    ((Executioner) role).target = pc;
                }
            }
        }*/

        private static void SortRoles(List<(Type, CustomRPC, int)> roles, int max = int.MaxValue)
        {
            roles.Shuffle();
            roles.Sort((a, b) =>
            {
                var a_ = a.Item3 == 100 ? 0 : 100;
                var b_ = b.Item3 == 100 ? 0 : 100;
                return a_.CompareTo(b_);
            });
            if (roles.Count > max)
                while (roles.Count > max)
                    roles.RemoveAt(roles.Count - 1);
        }

        private static void GenEachRole(List<GameData.PlayerInfo> infected)
        {
            var impostors = Utils.GetImpostors(infected);
            var crewmates = Utils.GetCrewmates(impostors);
            crewmates.Shuffle();
            impostors.Shuffle();
            
            SortRoles(CrewmateRoles);
            var maxNeutral = Il2CppSystem.Math.Min(PlayerControl.AllPlayerControls.Count - impostors.Count - 1, CustomGameOptions.MaxNeutralRoles);
            SortRoles(NeutralRoles, maxNeutral);
            for (; impostors.Count - ImpostorRoles.Count > 0;) {
                ImpostorRoles.Add((typeof(Impostor), CustomRPC.SetImpostor, 100));
            }
            int impostorNum = Math.Min(impostors.Count,
                CustomGameOptions.MadMateOn
                    ? CustomGameOptions.MaxImpostorRoles + 1
                    : CustomGameOptions.MaxImpostorRoles);
            SortRoles(ImpostorRoles, impostorNum);
            SortRoles(CrewmateModifiers, crewmates.Count);
            SortRoles(GlobalModifiers, crewmates.Count + impostors.Count);

            var crewAndNeutralRoles = new List<(Type, CustomRPC, int)>();
            crewAndNeutralRoles.AddRange(CrewmateRoles);
            SortRoles(crewAndNeutralRoles, crewmates.Count);
            if (CustomGameOptions.MadMateOn) {
                crewAndNeutralRoles.Insert(0, (typeof(Assassin), CustomRPC.SetAssassin, 100));
            }

            
            if (GlitchRoles.Any()) {
                crewAndNeutralRoles.InsertRange(0, GlitchRoles);
            }

            if (NeutralRoles.Any()) {
                crewAndNeutralRoles.InsertRange(0, NeutralRoles);
            }

            if (Check(CustomGameOptions.VanillaGame))
            {
                CrewmateRoles.Clear();
                NeutralRoles.Clear();
                GlitchRoles.Clear();
                CrewmateModifiers.Clear();
                GlobalModifiers.Clear();
                ImpostorRoles.Clear();
                LoversOn = false;
                PhantomOn = false;
            }

            PlayerControl executioner = null;
            var maxCrewAndNeutralNum = GameData.Instance.PlayerCount - impostors.Count;
            while (crewAndNeutralRoles.Count > maxCrewAndNeutralNum) {
                crewAndNeutralRoles.RemoveAt(crewAndNeutralRoles.Count - 1);
            }

            AmongUsExtensions.Log($"crewmates.Count:{crewmates.Count}");
            foreach (var (type, rpc, _) in crewAndNeutralRoles)
            {
                if (rpc == CustomRPC.SetExecutioner)
                {
                    executioner = crewmates[Random.RandomRangeInt(0, crewmates.Count)];
                    crewmates.Remove(executioner);
                    continue;
                }
                    
                Role.Gen<Role>(type, crewmates, rpc);
            }

            if (LoversOn)
                Lover.Gen(crewmates, impostors);

            while (impostors.Count > 0)
            {
                var (type, rpc, _) = ImpostorRoles.TakeFirst();
                if (type == null) break;
                Role.Gen<Role>(type, impostors.TakeFirst(), rpc);
            }

            foreach (var crewmate in crewmates) {
                Role.Gen<Role>(typeof(Crewmate), crewmate, CustomRPC.SetCrewmate);
            }

            foreach (var impostor in impostors) {
                Role.Gen<Role>(typeof(Impostor), impostor, CustomRPC.SetImpostor);
            }

            if (executioner != null)
            {
                var targets = Utils.GetCrewmates(impostors).Where(
                    crewmate => Role.GetRole(crewmate)?.Faction == Faction.Crewmates
                ).ToList();
                if (targets.Count > 0)
                {
                    var exec = Role.Gen<Executioner>(
                        typeof(Executioner),
                        executioner,
                        CustomRPC.SetExecutioner
                    );
                    var target = exec.target = targets[Random.RandomRangeInt(0, targets.Count)];

                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                        (byte)CustomRPC.SetTarget, SendOption.Reliable, -1);
                    writer.Write(executioner.PlayerId);
                    writer.Write(target.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                }
                else
                    Role.Gen<Role>(typeof(Crewmate), executioner, CustomRPC.SetExecutioner);
            }

            var canHaveModifier = PlayerControl.AllPlayerControls.ToArray().ToList();
            canHaveModifier.Shuffle();

            foreach (var (type, rpc, _) in GlobalModifiers)
                Role.Gen<Modifier>(type, canHaveModifier, rpc);

            canHaveModifier.RemoveAll(player => !player.Data.IsImpostor);
            canHaveModifier.Shuffle();

            while (canHaveModifier.Count > 0)
            {
                var (type, rpc, _) = CrewmateModifiers.TakeFirst();
                Role.Gen<Modifier>(type, canHaveModifier.TakeFirst(), rpc);
            }

            if (PhantomOn)
            {
                var vanilla = PlayerControl.AllPlayerControls.ToArray().Where(x => x.Is(RoleEnum.Crewmate)).ToList();
                var toChooseFrom = crewmates.Count > 0
                    ? crewmates
                    : PlayerControl.AllPlayerControls.ToArray().Where(x => x.Is(Faction.Crewmates) && !x.isLover())
                        .ToList();
                var rand = Random.RandomRangeInt(0, toChooseFrom.Count);
                var pc = toChooseFrom[rand];

                SetPhantom.WillBePhantom = pc;

                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                    (byte)CustomRPC.SetPhantom, SendOption.Reliable, -1);
                writer.Write(pc.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
            }
            else
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                    (byte)CustomRPC.SetPhantom, SendOption.Reliable, -1);
                writer.Write(byte.MaxValue);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
            }
        }


        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.HandleRpc))]
        public static class HandleRpc
        {
            public static void Postfix([HarmonyArgument(0)] byte callId, [HarmonyArgument(1)] MessageReader reader)
            {
                //if (callId >= 43) //System.Console.WriteLine("Received " + callId);
                byte readByte, readByte1, readByte2;
                sbyte readSByte, readSByte2;
                switch ((CustomRPC) callId)
                {
                    case CustomRPC.SetMayor:
                        readByte = reader.ReadByte();
                        new Mayor(Utils.PlayerById(readByte));
                        break;

                    case CustomRPC.SetJester:
                        readByte = reader.ReadByte();
                        new Jester(Utils.PlayerById(readByte));
                        break;

                    case CustomRPC.SetSheriff:
                        readByte = reader.ReadByte();
                        new Sheriff(Utils.PlayerById(readByte));
                        break;

                    case CustomRPC.SetEngineer:
                        readByte = reader.ReadByte();
                        new Engineer(Utils.PlayerById(readByte));
                        break;

                    case CustomRPC.SetJanitor:
                        new Janitor(Utils.PlayerById(reader.ReadByte()));
                        break;

                    case CustomRPC.SetKirby:
                        new Kirby(Utils.PlayerById(reader.ReadByte()));
                        break;

                    case CustomRPC.SetSwapper:
                        readByte = reader.ReadByte();
                        new Swapper(Utils.PlayerById(readByte));
                        break;

                    case CustomRPC.SetShifter:
                        readByte = reader.ReadByte();
                        new Shifter(Utils.PlayerById(readByte));
                        break;

                    case CustomRPC.SetInvestigator:
                        readByte = reader.ReadByte();
                        new Investigator(Utils.PlayerById(readByte));
                        break;

                    case CustomRPC.SetTimeLord:
                        readByte = reader.ReadByte();
                        new TimeLord(Utils.PlayerById(readByte));
                        break;

                    case CustomRPC.SetTorch:
                        readByte = reader.ReadByte();
                        new Torch(Utils.PlayerById(readByte));
                        break;
                    case CustomRPC.SetDiseased:
                        readByte = reader.ReadByte();
                        new Diseased(Utils.PlayerById(readByte));
                        break;
                    case CustomRPC.SetFlash:
                        readByte = reader.ReadByte();
                        new Flash(Utils.PlayerById(readByte));
                        break;

                    case CustomRPC.SetMedic:
                        readByte = reader.ReadByte();
                        new Medic(Utils.PlayerById(readByte));
                        break;
                    case CustomRPC.SetMorphling:
                        readByte = reader.ReadByte();
                        new Morphling(Utils.PlayerById(readByte));
                        break;
                    case CustomRPC.SetDollMaker:
                        readByte = reader.ReadByte();
                        new DollMaker(Utils.PlayerById(readByte));
                        break;

                    case CustomRPC.LoveWin:
                        var winnerlover = Utils.PlayerById(reader.ReadByte());
                        Role.GetRole<Lover>(winnerlover).Win();
                        break;


                    case CustomRPC.JesterLose:
                        foreach (var role in Role.AllRoles)
                            if (role.RoleType == RoleEnum.Jester)
                                ((Jester) role).Loses();

                        break;
                    case CustomRPC.PhantomLose:
                        foreach (var role in Role.AllRoles)
                            if (role.RoleType == RoleEnum.Phantom)
                                ((Phantom) role).Loses();

                        break;
                    case CustomRPC.GlitchLose:
                        foreach (var role in Role.AllRoles)
                            if (role.RoleType == RoleEnum.Glitch)
                                ((Glitch) role).Loses();

                        break;

                    case CustomRPC.ShifterLose:
                        foreach (var role in Role.AllRoles)
                            if (role.RoleType == RoleEnum.Shifter)
                                ((Shifter) role).Loses();

                        break;

                    case CustomRPC.ExecutionerLose:
                        foreach (var role in Role.AllRoles)
                            if (role.RoleType == RoleEnum.Executioner)
                                ((Executioner) role).Loses();

                        break;

                    case CustomRPC.NobodyWins:
                        Role.NobodyWinsFunc();
                        break;

                    case CustomRPC.SetCouple:
                        var id = reader.ReadByte();
                        var id2 = reader.ReadByte();
                        var b1 = reader.ReadByte();
                        var lover1 = Utils.PlayerById(id);
                        var lover2 = Utils.PlayerById(id2);

                        var roleLover1 = new Lover(lover1, 1, b1 == 0);
                        var roleLover2 = new Lover(lover2, 2, b1 == 0);

                        roleLover1.OtherLover = roleLover2;
                        roleLover2.OtherLover = roleLover1;

                        break;

                    case CustomRPC.Start:
                        /*
                        EngineerMod.PerformKill.UsedThisRound = false;
                        EngineerMod.PerformKill.SabotageTime = DateTime.UtcNow.AddSeconds(-100);
                        */
                        Utils.ShowDeadBodies = false;
                        Murder.KilledPlayers.Clear();
                        Role.NobodyWins = false;
                        RecordRewind.points.Clear();
                        AltruistKillButtonTarget.DontRevive = byte.MaxValue;
                        break;

                    case CustomRPC.JanitorClean:
                        readByte1 = reader.ReadByte();
                        var janitorPlayer = Utils.PlayerById(readByte1);
                        var janitorRole = Role.GetRole<Janitor>(janitorPlayer);
                        readByte = reader.ReadByte();
                        var deadBodies = Object.FindObjectsOfType<DeadBody>();
                        foreach (var body in deadBodies)
                            if (body.ParentId == readByte)
                                Coroutines.Start(Coroutine.CleanCoroutine(body, janitorRole));

                        break;
                    case CustomRPC.Wax:
                        readByte = reader.ReadByte();
                        readByte1 = reader.ReadByte();
                        var dollMaker = Role.GetRole<DollMaker>(Utils.PlayerById(readByte));
                        dollMaker.DollList.Add(readByte1, 0);
                        if (readByte1 == PlayerControl.LocalPlayer.PlayerId) {
                            Utils.PlayerById(readByte1).NetTransform.Halt();                            
                        }
                        break;
                    case CustomRPC.Inhale:
                        readByte1 = reader.ReadByte();
                        var kirbyPlayer = Utils.PlayerById(readByte1);
                        var kirbyRole = Role.GetRole<Kirby>(kirbyPlayer);
                        readByte = reader.ReadByte();
                        var deadBodies2 = Object.FindObjectsOfType<DeadBody>();
                        foreach (var body in deadBodies2) {
                            if (body.ParentId == readByte) {
                                Coroutines.Start(KirbyCoroutine.inhaleCoroutine(body, kirbyRole));
                            }
                        }
                        foreach (var p in PlayerControl.AllPlayerControls.ToArray().Where(x => x.CanDrag())) {
                            var taker = Role.GetRole<Undertaker>(p);
                            if (taker.CurrentlyDragging != null &&
                                taker.CurrentlyDragging.ParentId == readByte) {
                                taker.CurrentlyDragging = null;
                                taker.LastDragged = DateTime.UtcNow;
                            }

                            if (taker.Player == PlayerControl.LocalPlayer) {
                                taker._dragDropButton.renderer.sprite = TownOfUs.DragSprite;
                            }
                        }

                        break;
                    case CustomRPC.Spit:
                        var kirby = Role.GetRole<Kirby>(Utils.PlayerById(reader.ReadByte()));
                        kirby.TimeRemaining = 0;
                        kirby.Spit();
                        break;
                    case CustomRPC.EngineerFix:
                        var engineer = Utils.PlayerById(reader.ReadByte());
                        Role.GetRole<Engineer>(engineer).UsedThisRound = true;
                        break;


                    case CustomRPC.FixLights:
                        var lights = ShipStatus.Instance.Systems[SystemTypes.Electrical].Cast<SwitchSystem>();
                        lights.ActualSwitches = lights.ExpectedSwitches;
                        break;

                    case CustomRPC.SetExtraVotes:
                        var mayor = Utils.PlayerById(reader.ReadByte());
                        var mayorRole = Role.GetRole<Mayor>(mayor);
                        mayorRole.ExtraVotes = reader.ReadBytesAndSize().ToList();
                        if (!mayor.Is(RoleEnum.Mayor) && !mayor.Is(RoleEnum.Executioner)) {
                            mayorRole.VoteBank -= mayorRole.ExtraVotes.Count;
                        }

                        break;

                    case CustomRPC.SetSwaps:
                        readSByte = reader.ReadSByte();
                        SwapVotes.Swap1 =
                            MeetingHud.Instance.playerStates.FirstOrDefault(x => x.TargetPlayerId == readSByte);
                        readSByte2 = reader.ReadSByte();
                        SwapVotes.Swap2 =
                            MeetingHud.Instance.playerStates.FirstOrDefault(x => x.TargetPlayerId == readSByte2);
                        break;

                    case CustomRPC.Shift:
                        readByte1 = reader.ReadByte();
                        readByte2 = reader.ReadByte();
                        var shifter = Utils.PlayerById(readByte1);
                        var other = Utils.PlayerById(readByte2);
                        PerformKillButton.Shift(Role.GetRole<Shifter>(shifter), other);
                        break;
                    case CustomRPC.Rewind:
                        readByte = reader.ReadByte();
                        var TimeLordPlayer = Utils.PlayerById(readByte);
                        var TimeLordRole = Role.GetRole<TimeLord>(TimeLordPlayer);
                        StartStop.StartRewind(TimeLordRole);
                        break;
                    case CustomRPC.Protect:
                        readByte1 = reader.ReadByte();
                        readByte2 = reader.ReadByte();

                        var medic = Utils.PlayerById(readByte1);
                        var shield = Utils.PlayerById(readByte2);
                        Role.GetRole<Medic>(medic).ShieldedPlayer = shield;
                        Role.GetRole<Medic>(medic).UsedAbility = true;
                        break;
                    case CustomRPC.RewindRevive:
                        readByte = reader.ReadByte();
                        RecordRewind.ReviveBody(Utils.PlayerById(readByte));
                        break;
                    case CustomRPC.ZombieRevive:
                        readByte = reader.ReadByte();
                        ReviveSelf.ReviveBody(Utils.PlayerById(readByte));
                        break;
                    case CustomRPC.AttemptSound:
                        var medicId = reader.ReadByte();
                        readByte = reader.ReadByte();
                        StopKill.BreakShield(medicId, readByte, CustomGameOptions.ShieldBreaks);
                        break;
                    case CustomRPC.SetGlitch:
                        var GlitchId = reader.ReadByte();
                        var GlitchPlayer = Utils.PlayerById(GlitchId);
                        new Glitch(GlitchPlayer);
                        break;
                    case CustomRPC.BypassKill:
                        var killer = Utils.PlayerById(reader.ReadByte());
                        var target = Utils.PlayerById(reader.ReadByte());

                        Utils.MurderPlayer(killer, target);
                        break;
                    case CustomRPC.AssassinKill:
                        var toDie = Utils.PlayerById(reader.ReadByte());
                        AssassinKill.MurderPlayer(toDie);
                        break;
                    case CustomRPC.SetMimic:
                        var glitchPlayer = Utils.PlayerById(reader.ReadByte());
                        var mimicPlayer = Utils.PlayerById(reader.ReadByte());
                        var glitchRole = Role.GetRole<Glitch>(glitchPlayer);
                        glitchRole.MimicTarget = mimicPlayer;
                        glitchRole.IsUsingMimic = true;
                        Utils.Morph(glitchPlayer, mimicPlayer);
                        break;
                    case CustomRPC.RpcResetAnim:
                        var animPlayer = Utils.PlayerById(reader.ReadByte());
                        var theGlitchRole = Role.GetRole<Glitch>(animPlayer);
                        theGlitchRole.MimicTarget = null;
                        theGlitchRole.IsUsingMimic = false;
                        Utils.Unmorph(theGlitchRole.Player);
                        break;
                    case CustomRPC.GlitchWin:
                        var theGlitch = Role.AllRoles.FirstOrDefault(x => x.RoleType == RoleEnum.Glitch);
                        ((Glitch) theGlitch)?.Wins();
                        break;
                    case CustomRPC.SetHacked:
                        var hackPlayer = Utils.PlayerById(reader.ReadByte());
                        if (hackPlayer.PlayerId == PlayerControl.LocalPlayer.PlayerId)
                        {
                            var glitch = Role.AllRoles.FirstOrDefault(x => x.RoleType == RoleEnum.Glitch);
                            ((Glitch) glitch)?.SetHacked(hackPlayer);
                        }

                        break;
                    case CustomRPC.Investigate:
                        var seer = Utils.PlayerById(reader.ReadByte());
                        var otherPlayer = Utils.PlayerById(reader.ReadByte());
                        Role.GetRole<Seer>(seer).Investigated.Add(otherPlayer.PlayerId);
                        Role.GetRole<Seer>(seer).LastInvestigated = DateTime.UtcNow;
                        if (otherPlayer.Is(RoleEnum.Zombie)) {
                            Role.GetRole<Zombie>(otherPlayer).KilledBySeer = true;
                        }
                        break;
                    case CustomRPC.SetSeer:
                        new Seer(Utils.PlayerById(reader.ReadByte()));
                        break;
                    case CustomRPC.Morph:
                        var morphling = Utils.PlayerById(reader.ReadByte());
                        var morphTarget = Utils.PlayerById(reader.ReadByte());
                        var morphRole = Role.GetRole<Morphling>(morphling);
                        morphRole.TimeRemaining = CustomGameOptions.MorphlingDuration;
                        morphRole.MorphedPlayer = morphTarget;
                        break;
                    case CustomRPC.Unmorph:
                        var morphling2 = Utils.PlayerById(reader.ReadByte());
                        var morphRole2 = Role.GetRole<Morphling>(morphling2);
                        morphRole2.TimeRemaining = 0;
                        break;
                    case CustomRPC.SetExecutioner:
                        new Executioner(Utils.PlayerById(reader.ReadByte()));
                        break;
                    case CustomRPC.SetTarget:
                        var executioner = Utils.PlayerById(reader.ReadByte());
                        var exeTarget = Utils.PlayerById(reader.ReadByte());
                        var exeRole = Role.GetRole<Executioner>(executioner);
                        exeRole.target = exeTarget;
                        break;
                    case CustomRPC.SetCamouflager:
                        new Camouflager(Utils.PlayerById(reader.ReadByte()));
                        break;
                    case CustomRPC.Camouflage:
                        var camouflager = Utils.PlayerById(reader.ReadByte());
                        var camouflagerRole = Role.GetRole<Camouflager>(camouflager);
                        camouflagerRole.TimeRemaining = CustomGameOptions.CamouflagerDuration;
                        Utils.Camouflage();
                        break;
                    case CustomRPC.SetSpy:
                        new Spy(Utils.PlayerById(reader.ReadByte()));
                        break;
                    case CustomRPC.ExecutionerToJester:
                        TargetColor.ExeToJes(Utils.PlayerById(reader.ReadByte()));
                        break;
                    case CustomRPC.SetSnitch:
                        new Snitch(Utils.PlayerById(reader.ReadByte()));
                        break;
                    case CustomRPC.SetMiner:
                        new Miner(Utils.PlayerById(reader.ReadByte()));
                        break;
                    case CustomRPC.Mine:
                        var ventId = reader.ReadInt32();
                        var miner = Utils.PlayerById(reader.ReadByte());
                        var minerRole = Role.GetRole<Miner>(miner);
                        var pos = reader.ReadVector2();
                        var zAxis = reader.ReadSingle();
                        MinerPerformKill.SpawnVent(ventId, minerRole, pos, zAxis);
                        break;
                    case CustomRPC.SetSwooper:
                        new Swooper(Utils.PlayerById(reader.ReadByte()));
                        break;
                    case CustomRPC.Swoop:
                        var swooper = Utils.PlayerById(reader.ReadByte());
                        var swooperRole = Role.GetRole<Swooper>(swooper);
                        swooperRole.TimeRemaining = CustomGameOptions.SwoopDuration;
                        swooperRole.Swoop();
                        break;
                    case CustomRPC.Unswoop:
                        var swooper2 = Utils.PlayerById(reader.ReadByte());
                        var swooperRole2 = Role.GetRole<Swooper>(swooper2);
                        swooperRole2.TimeRemaining = 0;
                        break;
                    case CustomRPC.CrackRoom:
                        var cracker = Utils.PlayerById(reader.ReadByte());
                        var crackerRole = Role.GetRole<Cracker>(cracker);
                        var targetRoom = reader.ReadByte();
                        crackerRole.CrackRoom((SystemTypes)targetRoom);
                        break;
                    case CustomRPC.DetectCrackRoom:
                        var roomId = (SystemTypes)reader.ReadByte();
                        var cracker2 = Utils.PlayerById(reader.ReadByte());
                        Coroutines.Start(Cracker.HackRoomCoroutine(roomId, Role.GetRole<Cracker>(cracker2)));
                        if (PlayerControl.LocalPlayer.Is(RoleEnum.Cracker)) {
                            Coroutines.Start(Utils.FlashCoroutine(new Color(0f, 0f, 0f, 1f), CustomGameOptions.CrackDur));
                        }
                        break;
                    case CustomRPC.SetArsonist:
                        new Arsonist(Utils.PlayerById(reader.ReadByte()));
                        break;
                    case CustomRPC.SetSniper:
                        new Sniper(Utils.PlayerById(reader.ReadByte()));
                        break;
                    case CustomRPC.SetZombie:
                        new Zombie(Utils.PlayerById(reader.ReadByte()));
                        break;
                    case CustomRPC.Douse:
                        var arsonist = Utils.PlayerById(reader.ReadByte());
                        var douseTarget = Utils.PlayerById(reader.ReadByte());
                        var arsonistRole = Role.GetRole<Arsonist>(arsonist);
                        arsonistRole.DousedPlayers.Add(douseTarget.PlayerId);
                        arsonistRole.LastDoused = DateTime.UtcNow;

                        break;
                    case CustomRPC.Ignite:
                        var theArsonist = Utils.PlayerById(reader.ReadByte());
                        var theArsonistRole = Role.GetRole<Arsonist>(theArsonist);
                        Coroutines.Start(ArsonistCoroutine.Ignite(theArsonistRole));
                        break;

                    case CustomRPC.ArsonistWin:
                        var theArsonistTheRole = Role.AllRoles.FirstOrDefault(x => x.RoleType == RoleEnum.Arsonist);
                        ((Arsonist) theArsonistTheRole)?.Wins();
                        break;
                    case CustomRPC.ArsonistLose:
                        foreach (var role in Role.AllRoles)
                            if (role.RoleType == RoleEnum.Arsonist)
                                ((Arsonist) role).Loses();
                        break;
                    case CustomRPC.ZombieLose:
                        foreach (var role in Role.AllRoles)
                            if (role.RoleType == RoleEnum.Zombie)
                                ((Zombie) role).Loses();
                        break;
                    case CustomRPC.ZombieWin:
                        foreach (var role in Role.AllRoles)
                            if (role.RoleType == RoleEnum.Zombie)
                                ((Zombie) role).Wins();
                        break;
                    case CustomRPC.SniperWin:
                        var sniper = Role.AllRoles.FirstOrDefault(x => x.RoleType == RoleEnum.Sniper);
                        ((Sniper) sniper)?.Wins();
                        break;
                    case CustomRPC.SniperLose:
                        foreach (var role in Role.AllRoles)
                            if (role.RoleType == RoleEnum.Sniper)
                                ((Sniper) role).Loses();
                        break;
                    case CustomRPC.SetImpostor:
                        new Impostor(Utils.PlayerById(reader.ReadByte()));
                        break;
                    case CustomRPC.SetCrewmate:
                        new Crewmate(Utils.PlayerById(reader.ReadByte()));
                        break;
                    case CustomRPC.SyncCustomSettings:
                        Rpc.ReceiveRpc(reader);
                        break;
                    case CustomRPC.PassHost:
                        break;
                    case CustomRPC.SetAltruist:
                        new Altruist(Utils.PlayerById(reader.ReadByte()));
                        break;
                    case CustomRPC.SetCharger:
                        new Charger(Utils.PlayerById(reader.ReadByte()));
                        break;
                    case CustomRPC.SetDruid:
                        new Druid(Utils.PlayerById(reader.ReadByte()));
                        break;
                    case CustomRPC.SetSecurityGuard:
                        new SecurityGuard(Utils.PlayerById(reader.ReadByte()));
                        break;
                    case CustomRPC.SetBigBoi:
                        new BigBoiModifier(Utils.PlayerById(reader.ReadByte()));
                        break;
                    case CustomRPC.AltruistRevive:
                        readByte1 = reader.ReadByte();
                        var altruistPlayer = Utils.PlayerById(readByte1);
                        var altruistRole = Role.GetRole<Altruist>(altruistPlayer);
                        readByte = reader.ReadByte();
                        var theDeadBodies = Object.FindObjectsOfType<DeadBody>();
                        foreach (var body in theDeadBodies)
                            if (body.ParentId == readByte)
                            {
                                if (body.ParentId == PlayerControl.LocalPlayer.PlayerId)
                                    Coroutines.Start(Utils.FlashCoroutine(altruistRole.Color,
                                        CustomGameOptions.ReviveDuration, 0.5f));

                                Coroutines.Start(
                                    global::TownOfUs.CrewmateRoles.AltruistMod.Coroutine.AltruistRevive(body,
                                        altruistRole));
                            }

                        break;
                    case CustomRPC.DruidRevive:
                        readByte1 = reader.ReadByte();
                        var druidPlayer = Utils.PlayerById(readByte1);
                        var druidRole = Role.GetRole<Druid>(druidPlayer);
                        readByte = reader.ReadByte();
                        var bodies = Object.FindObjectsOfType<DeadBody>();
                        foreach (var body in bodies)
                            if (body.ParentId == readByte)
                            {
                                if (body.ParentId == PlayerControl.LocalPlayer.PlayerId)
                                    Coroutines.Start(Utils.FlashCoroutine(druidRole.Color,
                                        CustomGameOptions.ReviveDuration, 0.5f));

                                    druidRole.DruidRevive(body, druidRole);
                            }

                        break;
                    case CustomRPC.FixAnimation:
                        var player = Utils.PlayerById(reader.ReadByte());
                        player.MyPhysics.ResetMoveState();
                        player.Collider.enabled = true;
                        player.moveable = true;
                        player.NetTransform.enabled = true;
                        break;
                    case CustomRPC.SetButtonBarry:
                        new ButtonBarry(Utils.PlayerById(reader.ReadByte()));
                        break;
                    case CustomRPC.BarryButton:
                        var buttonBarry = Utils.PlayerById(reader.ReadByte());
                        if (AmongUsClient.Instance.AmHost)
                        {
                            MeetingRoomManager.Instance.reporter = buttonBarry;
                            MeetingRoomManager.Instance.target = null;
                            AmongUsClient.Instance.DisconnectHandlers.AddUnique(MeetingRoomManager.Instance
                                .Cast<IDisconnectHandler>());
                            if (ShipStatus.Instance.CheckTaskCompletion()) return;

                            DestroyableSingleton<HudManager>.Instance.OpenMeetingRoom(buttonBarry);
                            buttonBarry.RpcStartMeeting(null);
                        }

                        break;

                    case CustomRPC.SetUndertaker:
                        new Undertaker(Utils.PlayerById(reader.ReadByte()));
                        break;
                    case CustomRPC.Drag:
                        readByte1 = reader.ReadByte();
                        var dienerPlayer = Utils.PlayerById(readByte1);
                        var dienerRole = Role.GetRole<Undertaker>(dienerPlayer);
                        readByte = reader.ReadByte();
                        var dienerBodies = Object.FindObjectsOfType<DeadBody>();
                        foreach (var body in dienerBodies) {
                            if (body.ParentId == readByte) {
                                dienerRole.CurrentlyDragging = body;
                                break;
                            }
                        }

                        break;
                    case CustomRPC.Drop:
                        readByte1 = reader.ReadByte();
                        var v2 = reader.ReadVector2();
                        var v2z = reader.ReadSingle();
                        var dienerPlayer2 = Utils.PlayerById(readByte1);
                        var dienerRole2 = Role.GetRole<Undertaker>(dienerPlayer2);
                        var body2 = dienerRole2.CurrentlyDragging;
                        dienerRole2.CurrentlyDragging = null;

                        body2.transform.position = new Vector3(v2.x, v2.y, v2z);
                        break;
                    case CustomRPC.SetAssassin:
                        new Assassin(Utils.PlayerById(reader.ReadByte()));
                        break;
                    case CustomRPC.SetUnderdog:
                        new Underdog(Utils.PlayerById(reader.ReadByte()));
                        break;
                    case CustomRPC.SetCracker:
                        new Cracker(Utils.PlayerById(reader.ReadByte()));
                        break;
                    case CustomRPC.SetMultiKiller:
                        new MultiKiller(Utils.PlayerById(reader.ReadByte()));
                        break;
                    case CustomRPC.SetPuppeteer:
                        new Puppeteer(Utils.PlayerById(reader.ReadByte()));
                        break;
                    case CustomRPC.SetPhantom:
                        readByte = reader.ReadByte();
                        SetPhantom.WillBePhantom = readByte == byte.MaxValue ? null : Utils.PlayerById(readByte);
                        break;
                    case CustomRPC.PhantomDied:
                        var phantom = SetPhantom.WillBePhantom;
                        Role.RoleDictionary.Remove(phantom.PlayerId);
                        var phantomRole = new Phantom(phantom);
                        phantomRole.RegenTask();
                        phantom.gameObject.layer = LayerMask.NameToLayer("Players");
                        SetPhantom.RemoveTasks(phantom);
                        SetPhantom.AddCollider(phantomRole);
                        PlayerControl.LocalPlayer.MyPhysics.ResetMoveState();
                        System.Console.WriteLine("Become Phantom - Users");
                        break;
                    case CustomRPC.CatchPhantom:
                        var phantomPlayer = Utils.PlayerById(reader.ReadByte());
                        Role.GetRole<Phantom>(phantomPlayer).Caught = true;
                        break;
                    case CustomRPC.Possess:
                        var puppeteer = Utils.PlayerById(reader.ReadByte());
                        Role.GetRole<Puppeteer>(puppeteer).PossessPlayer = Utils.PlayerById(reader.ReadByte());
                        break;
                    case CustomRPC.UnPossess:
                        var puppeteer3 = Utils.PlayerById(reader.ReadByte());
                        Role.GetRole<Puppeteer>(puppeteer3).UnPossess();
                        break;
                    case CustomRPC.PossessKill:
                        var puppeteer2 = Utils.PlayerById(reader.ReadByte());
                        Role.GetRole<Puppeteer>(puppeteer2).KillUnPossess();
                        break;
                    case CustomRPC.ExtendMeeting:
                        MeetingHud.Instance.discussionTimer -= (int)CustomGameOptions.MayorExtendTime;
                        break;
                    case CustomRPC.PhantomWin:
                        Role.GetRole<Phantom>(Utils.PlayerById(reader.ReadByte())).CompletePhantomTasks = true;
                        break;
                    
                    case CustomRPC.AddMayorVoteBank:
                        Role.GetRole<Mayor>(Utils.PlayerById(reader.ReadByte())).VoteBank += reader.ReadInt32();
                        break;
                    case CustomRPC.OpenDoor:
                        int doorId = (int)reader.ReadByte();
                        PlainDoor openDoor = ShipStatus.Instance.AllDoors.ToArray().First(x => x.Id == doorId);
                        
                        openDoor.SetDoorway(true);
                        break;
                    case CustomRPC.PlaceCamera:
                        SGAction.placeCamera(reader.ReadBytesAndSize());
                        break;
                    case CustomRPC.SealVent:
                        SGAction.sealVent(reader.ReadPackedInt32());
                        break;
                    case CustomRPC.StartWatchAdmin:
                        AdminTimeLimit.AdminWatcher.Add(reader.ReadByte());
                        break;
                    case CustomRPC.EndWatchAdmin:
                        AdminTimeLimit.AdminWatcher.Remove(reader.ReadByte());
                        break;
                }
            }
        }

        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.RpcSetInfected))]
        public static class RpcSetInfected
        {
            public static void Prefix([HarmonyArgument(0)] ref Il2CppReferenceArray<GameData.PlayerInfo> infected)
            {
                Utils.ShowDeadBodies = false;
                Role.NobodyWins = false;
                CrewmateRoles.Clear();
                GlitchRoles.Clear();
                NeutralRoles.Clear();
                ImpostorRoles.Clear();
                CrewmateModifiers.Clear();
                GlobalModifiers.Clear();
                TheOtherRoles.MapOptions.clearAndReloadMapOptions();

                RecordRewind.points.Clear();
                Murder.KilledPlayers.Clear();
                AltruistKillButtonTarget.DontRevive = byte.MaxValue;

                var startWriter = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                    (byte) CustomRPC.Start, SendOption.Reliable, -1);
                AmongUsClient.Instance.FinishRpcImmediately(startWriter);


                LoversOn = Check(CustomGameOptions.LoversOn);
                PhantomOn = Check(CustomGameOptions.PhantomOn);

                #region Crewmate Roles
                
                if (Check(CustomGameOptions.MayorOn))
                    CrewmateRoles.Add((typeof(Mayor), CustomRPC.SetMayor, CustomGameOptions.MayorOn));

                if (Check(CustomGameOptions.SheriffOn))
                    CrewmateRoles.Add((typeof(Sheriff), CustomRPC.SetSheriff, CustomGameOptions.SheriffOn));

                if (Check(CustomGameOptions.EngineerOn))
                    CrewmateRoles.Add((typeof(Engineer), CustomRPC.SetEngineer, CustomGameOptions.EngineerOn));

                if (Check(CustomGameOptions.SwapperOn))
                    CrewmateRoles.Add((typeof(Swapper), CustomRPC.SetSwapper, CustomGameOptions.SwapperOn));

                if (Check(CustomGameOptions.InvestigatorOn))
                    CrewmateRoles.Add((typeof(Investigator), CustomRPC.SetInvestigator, CustomGameOptions.InvestigatorOn));

                if (Check(CustomGameOptions.TimeLordOn))
                    CrewmateRoles.Add((typeof(TimeLord), CustomRPC.SetTimeLord, CustomGameOptions.TimeLordOn));

                if (Check(CustomGameOptions.MedicOn))
                    CrewmateRoles.Add((typeof(Medic), CustomRPC.SetMedic, CustomGameOptions.MedicOn));

                if (Check(CustomGameOptions.SeerOn))
                    CrewmateRoles.Add((typeof(Seer), CustomRPC.SetSeer, CustomGameOptions.SeerOn));

                if (Check(CustomGameOptions.SpyOn))
                    CrewmateRoles.Add((typeof(Spy), CustomRPC.SetSpy, CustomGameOptions.SpyOn));

                if (Check(CustomGameOptions.SnitchOn))
                    CrewmateRoles.Add((typeof(Snitch), CustomRPC.SetSnitch, CustomGameOptions.SnitchOn));

                if (Check(CustomGameOptions.AltruistOn))
                    CrewmateRoles.Add((typeof(Altruist), CustomRPC.SetAltruist, CustomGameOptions.AltruistOn));

                if (Check(CustomGameOptions.ChargerOn))
                    CrewmateRoles.Add((typeof(Charger), CustomRPC.SetCharger, CustomGameOptions.ChargerOn));

                if (Check(CustomGameOptions.DruidOn))
                    CrewmateRoles.Add((typeof(Druid), CustomRPC.SetDruid, CustomGameOptions.DruidOn));

                if (Check(CustomGameOptions.SecurityGuardOn))
                    CrewmateRoles.Add((typeof(SecurityGuard), CustomRPC.SetSecurityGuard, CustomGameOptions.SecurityGuardOn));
                #endregion
                #region Neutral Roles
                if (CustomGameOptions.GlitchOn)
                    GlitchRoles.Add((typeof(Glitch), CustomRPC.SetGlitch, 100));
                
                if (Check(CustomGameOptions.JesterOn))
                    NeutralRoles.Add((typeof(Jester), CustomRPC.SetJester, CustomGameOptions.JesterOn));

                if (Check(CustomGameOptions.ShifterOn))
                    NeutralRoles.Add((typeof(Shifter), CustomRPC.SetShifter, CustomGameOptions.ShifterOn));

                if (Check(CustomGameOptions.ArsonistOn))
                    NeutralRoles.Add((typeof(Arsonist), CustomRPC.SetArsonist, CustomGameOptions.ArsonistOn));
                
                if (Check(CustomGameOptions.SniperOn))
                    NeutralRoles.Add((typeof(Sniper), CustomRPC.SetSniper, CustomGameOptions.SniperOn));
                
                if (Check(CustomGameOptions.ZombieOn))
                    NeutralRoles.Add((typeof(Zombie), CustomRPC.SetZombie, CustomGameOptions.ZombieOn));

                if (Check(CustomGameOptions.ExecutionerOn))
                    NeutralRoles.Add((typeof(Executioner), CustomRPC.SetExecutioner, CustomGameOptions.ExecutionerOn));
                #endregion
                #region Impostor Roles
                if (Check(CustomGameOptions.UndertakerOn))
                    ImpostorRoles.Add((typeof(Undertaker), CustomRPC.SetUndertaker, CustomGameOptions.UndertakerOn));

                if (!CustomGameOptions.MadMateOn && Check(CustomGameOptions.AssassinOn))
                    ImpostorRoles.Add((typeof(Assassin), CustomRPC.SetAssassin, CustomGameOptions.AssassinOn));

                if (Check(CustomGameOptions.UnderdogOn))
                    ImpostorRoles.Add((typeof(Underdog), CustomRPC.SetUnderdog, CustomGameOptions.UnderdogOn));

                if (Check(CustomGameOptions.CrackerOn))
                    ImpostorRoles.Add((typeof(Cracker), CustomRPC.SetCracker, CustomGameOptions.CrackerOn));

                if (Check(CustomGameOptions.MultiKillerOn))
                    ImpostorRoles.Add((typeof(MultiKiller), CustomRPC.SetMultiKiller, CustomGameOptions.MultiKillerOn));

                if (Check(CustomGameOptions.PuppeteerOn))
                    ImpostorRoles.Add((typeof(Puppeteer), CustomRPC.SetPuppeteer, CustomGameOptions.PuppeteerOn));

                if (Check(CustomGameOptions.MorphlingOn))
                    ImpostorRoles.Add((typeof(Morphling), CustomRPC.SetMorphling, CustomGameOptions.MorphlingOn));

                if (Check(CustomGameOptions.DollMakerOn))
                    ImpostorRoles.Add((typeof(DollMaker), CustomRPC.SetDollMaker, CustomGameOptions.DollMakerOn));

                if (Check(CustomGameOptions.CamouflagerOn))
                    ImpostorRoles.Add((typeof(Camouflager), CustomRPC.SetCamouflager, CustomGameOptions.CamouflagerOn));

                if (Check(CustomGameOptions.MinerOn))
                    ImpostorRoles.Add((typeof(Miner), CustomRPC.SetMiner, CustomGameOptions.MinerOn));

                if (Check(CustomGameOptions.SwooperOn))
                    ImpostorRoles.Add((typeof(Swooper), CustomRPC.SetSwooper, CustomGameOptions.SwooperOn));

                if (Check(CustomGameOptions.JanitorOn))
                    ImpostorRoles.Add((typeof(Janitor), CustomRPC.SetJanitor, CustomGameOptions.JanitorOn));

                if (Check(CustomGameOptions.KirbyOn))
                    ImpostorRoles.Add((typeof(Kirby), CustomRPC.SetKirby, CustomGameOptions.KirbyOn));
                #endregion
                #region Crewmate Modifiers
                if (Check(CustomGameOptions.TorchOn))
                    CrewmateModifiers.Add((typeof(Torch), CustomRPC.SetTorch, CustomGameOptions.TorchOn));

                if (Check(CustomGameOptions.DiseasedOn))
                    CrewmateModifiers.Add((typeof(Diseased), CustomRPC.SetDiseased, CustomGameOptions.DiseasedOn));
                #endregion
                #region Global Modifiers

                if (Check(CustomGameOptions.FlashOn))
                    GlobalModifiers.Add((typeof(Flash), CustomRPC.SetFlash, CustomGameOptions.FlashOn));

                if (Check(CustomGameOptions.BigBoiOn))
                    GlobalModifiers.Add((typeof(BigBoiModifier), CustomRPC.SetBigBoi, CustomGameOptions.BigBoiOn));

                if (Check(CustomGameOptions.ButtonBarryOn))
                    GlobalModifiers.Add(
                        (typeof(ButtonBarry), CustomRPC.SetButtonBarry, CustomGameOptions.ButtonBarryOn));
                #endregion

                GenEachRole(infected.ToList());

                
            }
        }
    }
}
