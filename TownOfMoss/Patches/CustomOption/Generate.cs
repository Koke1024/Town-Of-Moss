using System;

namespace TownOfUs.CustomOption
{
    public class Generate
    {
        
        public static CustomHeaderOption CustomGameSettings;
        public static CustomNumberOption MaxImpostorRoles;
        public static CustomToggleOption MadMateOn;
        public static CustomToggleOption GlitchOn;
        public static CustomNumberOption MaxNeutralRoles;
        public static CustomToggleOption KillCoolResetOnMeeting;
        public static CustomToggleOption ColourblindComms;
        public static CustomToggleOption MeetingColourblind;
        public static CustomToggleOption ImpostorSeeRoles;
        public static CustomToggleOption DeadSeeRoles;
        public static CustomToggleOption RoleUnderName;
        public static CustomNumberOption VanillaGame;
        public static CustomNumberOption PolusReactorTimeLimit;
        public static CustomNumberOption AirshipReactorTimeLimit;
        public static CustomStringOption PolusVitalMove;
        public static CustomNumberOption AdminTimeLimitTime;
        public static CustomToggleOption NoticeNeutral;
        public static CustomToggleOption GhostCantMove;
        #region Crewmate Roles
        public static CustomHeaderOption CrewmateRoles;
        public static CustomNumberOption MayorOn;
        public static CustomNumberOption BodyGuardOn;
        public static CustomNumberOption LoversOn;
        public static CustomNumberOption SheriffOn;
        public static CustomNumberOption EngineerOn;
        public static CustomNumberOption SwapperOn;
        public static CustomNumberOption InvestigatorOn;
        public static CustomNumberOption TimeLordOn;
        public static CustomNumberOption MedicOn;
        public static CustomNumberOption SeerOn;
        public static CustomNumberOption SecurityGuardOn;
        public static CustomNumberOption SnitchOn;
        public static CustomNumberOption NecromancerOn;
        public static CustomNumberOption ChargerOn;
        public static CustomNumberOption DruidOn;
        public static CustomNumberOption PainterOn;
        public static CustomNumberOption SnifferOn;
        #endregion
        #region Neutral Roles
        public static CustomHeaderOption NeutralRoles;
        public static CustomNumberOption ZombieOn;
        public static CustomNumberOption JesterOn;
        public static CustomNumberOption ShifterOn;
        public static CustomNumberOption ExecutionerOn;
        public static CustomNumberOption ArsonistOn;
        // public static CustomNumberOption PhantomOn;
        public static CustomNumberOption SniperOn;
        public static CustomNumberOption ScavengerOn;
        #endregion
        #region Impostor Roles
        public static CustomHeaderOption ImpostorRoles;
        public static CustomNumberOption JanitorOn;
        public static CustomNumberOption KirbyOn;
        public static CustomNumberOption MorphlingOn;
        public static CustomNumberOption CamouflagerOn;
        public static CustomNumberOption MinerOn;
        public static CustomNumberOption SwooperOn;
        public static CustomNumberOption UndertakerOn;
        public static CustomNumberOption AssassinOn;
        public static CustomNumberOption UnderdogOn;
        public static CustomNumberOption CrackerOn;
        public static CustomNumberOption MultiKillerOn;
        public static CustomNumberOption PuppeteerOn;
        public static CustomNumberOption DollMakerOn;
        #endregion

        public static CustomHeaderOption RoleOptions;
        public static CustomHeaderOption CrewmateRolesSetting;
        
//-------------------------------------CREWMATE-------------------------------------------------------

        public static CustomHeaderOption Mayor;
        public static CustomNumberOption MayorVoteBank;
        public static CustomToggleOption MayorMeetingOnDead;
        public static CustomToggleOption MayorAnonymous;
        public static CustomNumberOption MayorExtendTime;

        public static CustomHeaderOption Lovers;
        public static CustomToggleOption BothLoversDie;

        public static CustomHeaderOption Sheriff;
        // public static CustomToggleOption ShowSheriff;
        public static CustomToggleOption SheriffKillOther;
        public static CustomToggleOption SheriffKillsJester;
        public static CustomToggleOption SheriffKillsGlitch;
        public static CustomToggleOption SheriffKillsArsonist;
        public static CustomToggleOption SheriffKillsMadmate;
        public static CustomNumberOption SheriffKillCd;
        public static CustomToggleOption SheriffBodyReport;
        
        public static CustomHeaderOption Engineer;
        public static CustomStringOption EngineerPer;
        public static CustomToggleOption EngineerCanFixOnlyInVent;

        public static CustomHeaderOption Investigator;
        public static CustomNumberOption InvestigatorSeeColorRange;
        public static CustomNumberOption InvestigatorSeeRange;
        public static CustomNumberOption InvestigatorMapUpdate;

        public static CustomHeaderOption TimeLord;
        public static CustomToggleOption RewindRevive;
        public static CustomToggleOption RewindFlash;
        public static CustomNumberOption RewindDuration;
        public static CustomNumberOption RewindCooldown;

        public static CustomHeaderOption Medic;
        public static CustomNumberOption MedicReportDegradation;

        public static CustomHeaderOption Seer;
        public static CustomNumberOption SeerCooldown;
        public static CustomNumberOption SeerInvestigateTime;
        public static CustomStringOption SeerInfo;
        public static CustomStringOption SeeReveal;
        public static CustomToggleOption NeutralRed;
        
        public static CustomHeaderOption SecurityGuard;
        public static CustomNumberOption SecurityGuardCooldown;
        public static CustomNumberOption SecurityGuardTotalScrews;
        public static CustomNumberOption SecurityGuardCamPrice;
        public static CustomNumberOption SecurityGuardVentPrice;

        public static CustomHeaderOption Snitch;
        public static CustomToggleOption SnitchOnLaunch;
        public static CustomToggleOption SnitchSeesNeutrals;
        public static CustomStringOption SnitchOpenDoorImmediately;
        public static CustomStringOption SnitchNoShotTiming;

        public static CustomHeaderOption Necromancer;
        public static CustomNumberOption ReviveDuration;
        public static CustomNumberOption NecroCoolDown;
        public static CustomToggleOption NecromancerTargetBody;
        public static CustomToggleOption NecromancerLendBody;
        
        public static CustomHeaderOption BodyGuard;
        public static CustomNumberOption GuardRange;
        public static CustomNumberOption GuardDuration;
        public static CustomNumberOption GuardCoolDown;
        public static CustomToggleOption DieOnGuard;
        // public static CustomStringOption ShowShielded;
        public static CustomStringOption WhoGetsNotification;

        public static CustomHeaderOption Charger;
        public static CustomNumberOption MaxChargeTime;
        public static CustomNumberOption ConsumeChargeTime;

        public static CustomHeaderOption Druid;
        public static CustomNumberOption DruidReviveRange;
        public static CustomStringOption DruidReviveLimit;

        public static CustomHeaderOption Painter;
        public static CustomNumberOption PaintColorMax;
        public static CustomNumberOption PaintCd;

        public static CustomHeaderOption Sniffer;
        public static CustomToggleOption SnifferCanReport;
        public static CustomNumberOption SnifferMaxRange;
        
//-------------------------------------IMPOSTOR ROLES-------------------------------------------------------
        public static CustomHeaderOption ImpostorRolesSetting;

        public static CustomHeaderOption Morphling;
        public static CustomNumberOption MorphlingCooldown;
        public static CustomNumberOption MorphlingDuration;
        public static CustomStringOption MorphCanVent;

        public static CustomHeaderOption Camouflager;
        public static CustomNumberOption CamouflagerCooldown;
        public static CustomNumberOption CamouflagerDuration;

        public static CustomHeaderOption Janitor;
        public static CustomNumberOption CleanCd;
        public static CustomNumberOption CleanDuration;

        public static CustomHeaderOption Miner;
        public static CustomNumberOption MineCooldown;
        public static CustomNumberOption MaxVentNum;

        public static CustomHeaderOption Swooper;
        public static CustomNumberOption SwoopCooldown;
        public static CustomNumberOption SwoopDuration;
        public static CustomStringOption SwooperCanVent;

        public static CustomHeaderOption Undertaker;
        public static CustomNumberOption DragCooldown;
        public static CustomNumberOption DragVelocity;
        public static CustomToggleOption VentWithBody;
        
        public static CustomHeaderOption Cracker;
        public static CustomNumberOption CrackCd;
        public static CustomNumberOption CrackDur;
        
        public static CustomHeaderOption MultiKiller;
        public static CustomNumberOption MultiKillerCdRate;
        public static CustomNumberOption MultiKillEnableTime;

        public static CustomHeaderOption Madmate;
        public static CustomToggleOption MadmateCanVent;

        public static CustomHeaderOption Assassin;
        public static CustomNumberOption AssassinKills;
        public static CustomToggleOption AssassinGuessNeutrals;
        public static CustomToggleOption AssassinCrewmateGuess;
        public static CustomToggleOption AssassinGuessImpostors;
        public static CustomToggleOption AssassinMultiKill;
        public static CustomToggleOption AssassinCanKillAfterVote;
        public static CustomToggleOption AllImpostorCanGuess;
        public static CustomToggleOption LastImpostorCanGuess;
        
        public static CustomHeaderOption Puppeteer;
        public static CustomNumberOption PossessTime;
        public static CustomNumberOption PossessCd;
        public static CustomNumberOption PossessMaxTime;
        public static CustomNumberOption ReleaseWaitTime;
        public static CustomToggleOption PossessBodyReport;

        public static CustomHeaderOption DollMaker;
        public static CustomNumberOption DollBreakTime;

        //-------------------------------------NEUTRAL ROLES-------------------------------------------------------
        public static CustomHeaderOption NeutralRolesSetting;

        public static CustomHeaderOption TheGlitch;
        public static CustomToggleOption GlitchAdmin;
        public static CustomNumberOption MimicCooldownOption;
        public static CustomNumberOption MimicDurationOption;
        public static CustomNumberOption HackCooldownOption;
        public static CustomNumberOption HackDurationOption;
        public static CustomNumberOption GlitchKillCooldownOption;
        public static CustomStringOption GlitchHackDistanceOption;

        public static CustomHeaderOption Jester;
        public static CustomToggleOption JesterUseVent;
        public static CustomToggleOption JesterDragBody;
        public static CustomToggleOption JesterCanMorph;
        
        public static CustomHeaderOption Executioner;
        public static CustomStringOption OnTargetDead;

        public static CustomHeaderOption Arsonist;
        public static CustomNumberOption ArsonistDouseTime;
        public static CustomNumberOption DouseCooldown;
        public static CustomToggleOption ArsonistGameEnd;
        
        public static CustomHeaderOption Sniper;
        public static CustomNumberOption SniperWinCnt;
        public static CustomStringOption SniperPerGame;
        
        public static CustomHeaderOption Scavenger;
        public static CustomNumberOption ScavengerWinCnt;
        
        public static CustomHeaderOption Zombie;
        public static CustomNumberOption ZombieReviveTime;
        public static CustomToggleOption ZombieKilledBySeer;

        public static CustomHeaderOption Shifter;
        public static CustomNumberOption ShifterCd;
        public static CustomStringOption WhoShifts;
        
//-------------------------------------MODIFIER-------------------------------------------------------
        public static CustomHeaderOption Modifiers;
        public static CustomNumberOption TorchOn;
        public static CustomNumberOption DiseasedOn;
        public static CustomNumberOption FlashOn;
        public static CustomNumberOption TiebreakerOn;
        public static CustomNumberOption DrunkOn;
        public static CustomNumberOption ButtonBarryOn;
        public static CustomNumberOption BigBoiOn;
        public static Func<object, string> PercentFormat { get; } = value => $"{value:0}%";
        private static Func<object, string> CooldownFormat { get; } = value => $"{value:0.0#}s";
        private static Func<object, string> DistanceFormat { get; } = value => $"{value:0.0#}m";


        public static void GenerateAll()
        {
            var num = 0;

            Patches.ExportButton = new Export(num++);
            Patches.ImportButton = new Import(num++);

#region Custom Game Setting
            CustomGameSettings = new CustomHeaderOption(num++, "Custom Game Settings");
            // MaxImpostorRoles =
            //      new CustomNumberOption(num++, "<color=#FF0000FF>Max Impostor Roles</color>", 3f, 1f, 3f, 1f);
            MadMateOn = new CustomToggleOption(num++, "<color=#FF0000FF>Add Madmate</color>", false);
            GlitchOn = new CustomToggleOption(num++, "<color=#00FF00FF>Add Glitch</color>", false);
            MaxNeutralRoles =
                new CustomNumberOption(num++, "<color=#FF00FFFF>Number of Neutral Roles</color>", 0, 0, 3, 1);
            KillCoolResetOnMeeting = new CustomToggleOption(num++, "Kill Cooldown Reset on Meeting", false);
            ColourblindComms = new CustomToggleOption(num++, "Camouflaged Comms", true);
            LastImpostorCanGuess = new CustomToggleOption(num++, "Last Impostor Can Snipe", true);
            // MeetingColourblind = new CustomToggleOption(num++, "Camouflaged Meetings", false);
            ImpostorSeeRoles = new CustomToggleOption(num++, "Impostors See Teammate's Role", true);
            
            PolusVitalMove = 
                new CustomStringOption(num++, "Polus Vital Move", new[] {"Default", "Labo", "Ship", "O2"});
            PolusReactorTimeLimit = new CustomNumberOption(num++, "Polus Reactor Time Limit", 45.0f, 30f, 60f, 5f, CooldownFormat);
            AirshipReactorTimeLimit = new CustomNumberOption(num++, "AirShip Reactor Time Limit", 75.0f, 45f, 90f, 5f, CooldownFormat);
            AdminTimeLimitTime = new CustomNumberOption(num++, "Admin and Security Camera Usable Time", 0, 0f, 40f, 5f, CooldownFormat);

            DeadSeeRoles =
                new CustomToggleOption(num++, "Dead can see everyone's roles", true);

            RoleUnderName = new CustomToggleOption(num++, "Role Appears Under Name");
            NoticeNeutral = new CustomToggleOption(num++, "Notice Assigned Neutral Roles", true);
            GhostCantMove = new CustomToggleOption(num++, "Dead Player Can't Move Before Report", false);
            // VanillaGame = new CustomNumberOption(num++, "Probability of a completely vanilla game", 0f, 0f, 100f, 5f,
            //     PercentFormat);
#endregion
#region Crewmate Roles
            CrewmateRoles = new CustomHeaderOption(num++, "<color=#00FF00FF>Crewmate Roles</color>");
            BodyGuardOn = new CustomNumberOption(true, num++, "<color=#00773AFF>BodyGuard</color>", 90.0f, 0f, 100f, 10f,
                PercentFormat);
            ChargerOn = new CustomNumberOption(true, num++, "<color=#FCFF33FF>Charger</color>", 90.0f, 0f, 100f, 10f,
                PercentFormat);
            DruidOn = new CustomNumberOption(true, num++, "<color=#66008EFF>Druid</color>", 90.0f, 0f, 100f, 10f,
                PercentFormat);
            EngineerOn = new CustomNumberOption(true, num++, "<color=#FFA60AFF>Engineer</color>", 90.0f, 0f, 100f, 10f,
                PercentFormat);
            InvestigatorOn = new CustomNumberOption(true, num++, "<color=#00B3B3FF>Investigator</color>", 90.0f, 0f, 100f,
                10f, PercentFormat);
            MayorOn = new CustomNumberOption(true, num++, "<color=#704FA8FF>Mayor</color>", 90.0f, 0f, 100f, 10f,
                PercentFormat);
            MedicOn = new CustomNumberOption(true, num++, "<color=#006600FF>Medic</color>", 90.0f, 0f, 100f, 10f,
                PercentFormat);
            NecromancerOn = new CustomNumberOption(true, num++, "<color=#660000FF>Necromancer</color>", 90.0f, 0f, 100f, 10f,
                PercentFormat);
            PainterOn = new CustomNumberOption(true, num++, "<color=#CECECEFF>Painter</color>", 0.0f, 0f, 100f, 10f,
                PercentFormat);
            SecurityGuardOn = new CustomNumberOption(true, num++, "<color=#AAAAFFFF>SecurityGuard</color>", 90.0f, 0f, 100f, 10f,
                PercentFormat);
            SeerOn = new CustomNumberOption(true, num++, "<color=#FFCC80FF>Seer</color>", 90.0f, 0f, 100f, 10f,
                PercentFormat);
            SheriffOn = new CustomNumberOption(true, num++, "<color=#FFFF00FF>Sheriff</color>", 90.0f, 0f, 100f, 10f,
                PercentFormat);
            SnifferOn = new CustomNumberOption(true, num++, "<color=#A500D3FF>Sniffer</color>", 90.0f, 0f, 100f, 10f,
                PercentFormat);
            SnitchOn = new CustomNumberOption(true, num++, "<color=#D4AF37FF>Snitch</color>", 90.0f, 0f, 100f, 10f,
                PercentFormat);
            SwapperOn = new CustomNumberOption(true, num++, "<color=#66E666FF>Swapper</color>", 90.0f, 0f, 100f, 10f,
                PercentFormat);
            TimeLordOn = new CustomNumberOption(true, num++, "<color=#0000FFFF>Time Lord</color>", 90.0f, 0f, 100f, 10f,
                PercentFormat);
            // SpyOn = new CustomNumberOption(true, num++, "<color=#CCA3CCFF>Spy</color>", 90.0f, 0f, 100f, 10f,
            //     PercentFormat);
#endregion
#region Neutral Roles
            NeutralRoles = new CustomHeaderOption(num++, "<color=#FF00FFFF>Neutral Roles</color>");
            ArsonistOn = new CustomNumberOption(true, num++, "<color=#FF4D00FF>Arsonist</color>", 90.0f, 0f, 100f, 10f,
                PercentFormat);
            ExecutionerOn = new CustomNumberOption(true, num++, "<color=#8C4005FF>Executioner</color>", 90.0f, 0f, 100f,
                10f, PercentFormat);
            JesterOn = new CustomNumberOption(true, num++, "<color=#FFBFCCFF>Jester</color>", 90.0f, 0f, 100f, 10f,
                PercentFormat);
            // ShifterOn = new CustomNumberOption(true, num++, "<color=#999999FF>Shifter</color>", 0.0f, 0f, 100f, 10f,
            //     PercentFormat);
            // PhantomOn = new CustomNumberOption(true, num++, "<color=#662962>Phantom</color>", 90.0f, 0f, 100f, 10f,
            //     PercentFormat);
            SniperOn = new CustomNumberOption(true, num++, "<color=#545459>Sniper</color>", 90.0f, 0f, 100f, 10f,
                PercentFormat);
            ZombieOn = new CustomNumberOption(true, num++, "<color=#773800>Zombie</color>", 90.0f, 0f, 100f, 10f,
                PercentFormat);
            // ScavengerOn = new CustomNumberOption(true, num++, "<color=#773800>Scavenger</color>", 90.0f, 0f, 100f, 10f,
            //     PercentFormat);
#endregion
#region Impostor Roles
            ImpostorRoles = new CustomHeaderOption(num++, "<color=#FF0000FF>Impostor Roles</color>");
            AssassinOn = new CustomNumberOption(true, num++, "<color=#FF0000FF>Assassin</color>", 90.0f, 0f, 100f, 10f,
                PercentFormat);
            CamouflagerOn = new CustomNumberOption(true, num++, "<color=#FF0000FF>Camouflager</color>", 90.0f, 0f, 100f,
                10f, PercentFormat);
            CrackerOn = new CustomNumberOption(true, num++, "<color=#FF0000FF>Cracker</color>", 90.0f, 0f, 100f, 10f,
                PercentFormat);
            DollMakerOn = new CustomNumberOption(true, num++, "<color=#FF0000FF>DollMaker</color>", 90.0f, 0f, 100f, 10f,
                PercentFormat);
            JanitorOn = new CustomNumberOption(true, num++, "<color=#FF0000FF>Janitor</color>", 90.0f, 0f, 100f, 10f,
                PercentFormat);
            MinerOn = new CustomNumberOption(true, num++, "<color=#FF0000FF>Miner</color>", 90.0f, 0f, 100f, 10f,
                PercentFormat);
            MorphlingOn = new CustomNumberOption(true, num++, "<color=#FF0000FF>Morphling</color>", 90.0f, 0f, 100f, 10f,
                PercentFormat);
            MultiKillerOn = new CustomNumberOption(true, num++, "<color=#FF0000FF>MultiKiller</color>", 90.0f, 0f, 100f, 10f,
                PercentFormat);
            KirbyOn = new CustomNumberOption(true, num++, "<color=#A86BA3FF>Popopo</color>", 90.0f, 0f, 100f, 10f,
                PercentFormat);
            PuppeteerOn = new CustomNumberOption(true, num++, "<color=#FF0000FF>Puppeteer</color>", 90.0f, 0f, 100f, 10f,
                PercentFormat);
            SwooperOn = new CustomNumberOption(true, num++, "<color=#FF0000FF>Swooper</color>", 90.0f, 0f, 100f, 10f,
                PercentFormat);
            UndertakerOn = new CustomNumberOption(true, num++, "<color=#FF0000FF>Undertaker</color>", 90.0f, 0f, 100f, 10f,
                PercentFormat);

#endregion
#region Role Options
            RoleOptions = new CustomHeaderOption(num++, "Custom Role Settings");
            CrewmateRolesSetting = new CustomHeaderOption(num++, "<color=#00FF00FF>Crewmate Roles</color>");
            
            BodyGuard = new CustomHeaderOption(num++, "<color=#00773AFF>BodyGuard</color>");
            GuardCoolDown =
                new CustomNumberOption(num++, "Guard Cooldown", 15, 10, 30, 1f, CooldownFormat);
            GuardDuration =
                new CustomNumberOption(num++, "Guard Duration", 15, 10, 60, 1f, CooldownFormat);
            GuardRange =
                new CustomNumberOption(num++, "Guard Range", 15, 10, 60, 1f, DistanceFormat);
            DieOnGuard =
                new CustomToggleOption(num++, "Dies on Guard", true);
            // ShowShielded =
            //     new CustomStringOption(num++, "Show Shielded Player",
            //         new[] {"BodyGuard", "Self", "Self+Medic", "Everyone"});
            WhoGetsNotification =
                new CustomStringOption(num++, "Who gets murder attempt indicator",
                    new[] {"Everyone", "Shielded", "BodyGuard", "Nobody"});
            
            Charger =
                new CustomHeaderOption(num++, "<color=#FCFF33FF>Charger</color>");
            MaxChargeTime = new CustomNumberOption(num++, "Time to Charge Maximum", 2f, 1f, 5f, 0.5f, CooldownFormat);
            ConsumeChargeTime = new CustomNumberOption(num++, "Time to Consume", 30f, 10f, 100f, 5f, CooldownFormat);
            
            Druid =
                new CustomHeaderOption(num++, "<color=#66008EFF>Druid</color>");
            DruidReviveRange = new CustomNumberOption(num++, "Distance to revive dead", 20f, 2f, 40f, 1f, DistanceFormat);
            DruidReviveLimit =
                new CustomStringOption(num++, "Revive Limit Per Round", new[] {"No Limit", "1", "2", "3"});
            
            Engineer =
                new CustomHeaderOption(num++, "<color=#FFA60AFF>Engineer</color>");
            EngineerPer =
                new CustomStringOption(num++, "Engineer Fix Per", new[] {"Round", "Game"});
            EngineerCanFixOnlyInVent =
                new CustomToggleOption(num++, "Fix Only In Vent", true);

            Investigator =
                new CustomHeaderOption(num++, "<color=#00B3B3FF>Investigator</color>");
            // FootprintSize = new CustomNumberOption(num++, "Footprint Size", 4f, 1f, 10f, 1f);
            // FootprintInterval =
            //     new CustomNumberOption(num++, "Footprint Interval", 1f, 0.25f, 5f, 0.25f, CooldownFormat);
            // FootprintDuration = new CustomNumberOption(num++, "Footprint Duration", 10f, 1f, 10f, 0.5f, CooldownFormat);
            // AnonymousFootPrint = new CustomToggleOption(num++, "Anonymous Footprint", false);
            // VentFootprintVisible = new CustomToggleOption(num++, "Footprint Vent Visible", false);
            InvestigatorSeeRange = new CustomNumberOption(num++, "See Someone Range", 8f, 3f, 30f, 1f, DistanceFormat);
            InvestigatorSeeColorRange = new CustomNumberOption(num++, "See Color Range", 50f, 0f, 100f, 100f, PercentFormat);
            InvestigatorMapUpdate = new CustomNumberOption(num++, "Map Update Interval", 1f, 0f, 10f, 0.5f, CooldownFormat);
            
            Mayor =
                new CustomHeaderOption(num++, "<color=#704FA8FF>Mayor</color>");
            
            MayorMeetingOnDead =
                new CustomToggleOption(num++, "Call Meeting On Dead", true);
            
            Medic =
                new CustomHeaderOption(num++, "<color=#006600FF>Medic</color>");
            MedicReportDegradation =
                new CustomNumberOption(num++, "Report Degradation Duration", 5, 3, 20, 1f,
                    CooldownFormat);
            
            Necromancer = new CustomHeaderOption(num++, "<color=#660000FF>Necromancer</color>");
            ReviveDuration =
                new CustomNumberOption(num++, "Revive Duration", 1, 1, 30, 1f, CooldownFormat);
            NecroCoolDown = new CustomNumberOption(num++, "Necromancy Cooldown", 30f, 10f, 60f, 2.5f, CooldownFormat);
            // NecromancerTargetBody =
            //     new CustomToggleOption(num++, "Target's body disappears", false);
            // NecromancerLendBody =
            //     new CustomToggleOption(num++, "Lend Body to Victim", false);

            // ShieldBreaks = new CustomToggleOption(num++, "Shield breaks on murder attempt", true);

            // MayorVoteBank =
            //     new CustomNumberOption(num++, "Initial Mayor Vote Bank", 1, 1, 5, 1);
            //
            // MayorAnonymous =
            //     new CustomToggleOption(num++, "Votes Show Anonymous", true);
            //
            // MayorExtendTime =
            //     new CustomNumberOption(num++, "Time Mayor Extends Meeting", 30f, 15f, 60f, 5f, CooldownFormat);

            // Lovers =
            //     new CustomHeaderOption(num++, "<color=#FF66CCFF>Lovers</color>");
            // BothLoversDie = new CustomToggleOption(num++, "Both Lovers Die");
            
            Painter =
                new CustomHeaderOption(num++, "<color=#CECECEFF>Painter</color>");
            PaintColorMax = new CustomNumberOption(num++, "Paint Color Num", 2f, 1f, 3f, 1f);
            PaintCd = new CustomNumberOption(num++, "Paint Cooldown", 25f, 5f, 40f, 2.5f, CooldownFormat);

            SecurityGuard =
                new CustomHeaderOption(num++, "<color=#AAAAFFFF>SecurityGuard</color>");
            SecurityGuardCooldown = new CustomNumberOption(num++, "Security Guard Cooldown", 30f, 10f, 60f, 2.5f);
            SecurityGuardTotalScrews = new CustomNumberOption(num++, "Number Of Screws", 7f, 1f, 15f, 1f);
            SecurityGuardCamPrice = new CustomNumberOption(num++, "Number Of Screws Per Cam", 2f, 1f, 15f, 1f);
            SecurityGuardVentPrice = new CustomNumberOption(num++, "Number Of Screws Per Vent", 1f, 1f, 15f, 1f);
            
            Seer =
                new CustomHeaderOption(num++, "<color=#FFCC80FF>Seer</color>");

            SeerCooldown =
                new CustomNumberOption(num++, "Seer Cooldown", 30f, 5f, 60f, 5f, CooldownFormat);

            SeerInvestigateTime =
                new CustomNumberOption(num++, "Seer Investigating Time", 3f, 1f, 10f, 0.5f, CooldownFormat);

            SeerInfo =
                new CustomStringOption(num++, "Info that Seer sees", new[] {"Team", "Role"});

            SeeReveal =
                new CustomStringOption(num++, "Who Sees That They Are Revealed",
                    new[] {"Nobody", "Imps+Neut", "Crew", "All"});
            NeutralRed =
                new CustomToggleOption(num++, "Neutrals show up as Impostors", true);
            
            Sheriff =
                new CustomHeaderOption(num++, "<color=#FFFF00FF>Sheriff</color>");
            // ShowSheriff = new CustomToggleOption(num++, "Show Sheriff", false);

            SheriffKillOther =
                new CustomToggleOption(num++, "Miskill Kills Crewmate", false);
            SheriffKillsMadmate =
                new CustomToggleOption(num++, "Can Kills Madmate", false);

            SheriffKillCd =
                new CustomNumberOption(num++, "Sheriff Kill Cooldown", 25f, 10f, 40f, 2.5f, CooldownFormat);
            SheriffBodyReport = new CustomToggleOption(num++, "Sheriff can report they killed");
            
            Sniffer =
                new CustomHeaderOption(num++, "<color=#A500D3FF>Sniffer</color>");
            SnifferCanReport = new CustomToggleOption(num++, "Sniffer Can Report", false);
            SnifferMaxRange = new CustomNumberOption(num++, "Sniff Max Range", 10f, 5f, 50f, 5f, DistanceFormat);
            
            Snitch = new CustomHeaderOption(num++, "<color=#D4AF37FF>Snitch</color>");
            // SnitchOnLaunch =
            //     new CustomToggleOption(num++, "Knows who they are on Game Start", true);
            SnitchSeesNeutrals = new CustomToggleOption(num++, "sees neutral roles", false);
            SnitchOpenDoorImmediately = new CustomStringOption(num++, "Open door immediately", new[] {"Always", "One Task Left", "None"});
            SnitchNoShotTiming = new CustomStringOption(num++, "Conditions for not being shot", new[] {"Revealed", "Completed"});

            TimeLord =
                new CustomHeaderOption(num++, "<color=#0000FFFF>Time Lord</color>");
            RewindRevive = new CustomToggleOption(num++, "Revive During Rewind", true);
            RewindFlash = new CustomToggleOption(num++, "Has Blue Outline on Rewind", true);
            RewindDuration = new CustomNumberOption(num++, "Rewind Duration", 3f, 1f, 15f, 0.5f, CooldownFormat);
            RewindCooldown = new CustomNumberOption(num++, "Rewind Cooldown", 35f, 20f, 50f, 2.5f, CooldownFormat);

            // TimeLordVitals =
            //     new CustomToggleOption(num++, "Time Lord can use Vitals", false);

            NeutralRolesSetting = new CustomHeaderOption(num++, "<color=#FF00FFFF>Neutral Roles</color>");
            
            TheGlitch =
                new CustomHeaderOption(num++, "<color=#00FF00FF>The Glitch</color>");
            GlitchAdmin = new CustomToggleOption(num++, "Glitch Has Portable Admin");
            MimicCooldownOption = new CustomNumberOption(num++, "Mimic Cooldown", 10, 10, 120, 2.5f, CooldownFormat);
            MimicDurationOption = new CustomNumberOption(num++, "Mimic Duration", 15, 1, 30, 1f, CooldownFormat);
            HackCooldownOption = new CustomNumberOption(num++, "Hack Cooldown", 10, 10, 120, 2.5f, CooldownFormat);
            HackDurationOption = new CustomNumberOption(num++, "Hack Duration", 15, 1, 30, 1f, CooldownFormat);
            GlitchKillCooldownOption =
                new CustomNumberOption(num++, "Kill Cooldown", 30, 10, 120, 2.5f, CooldownFormat);
            GlitchHackDistanceOption =
                new CustomStringOption(num++, "Hack Distance", new[] {"Short", "Normal", "Long"});
            
            // Shifter =
            //     new CustomHeaderOption(num++, "<color=#999999FF>Shifter</color>");
            // ShifterCd =
            //     new CustomNumberOption(num++, "Shifter Cooldown", 30f, 10f, 60f, 2.5f, CooldownFormat);
            // WhoShifts = new CustomStringOption(num++,
            //     "Who gets the Shifter role on Shift", new[] {"NoImps", "RegCrew", "Nobody"});

            
            
            // Executioner =
            //     new CustomHeaderOption(num++, "<color=#8C4005FF>Executioner</color>");

            Arsonist = new CustomHeaderOption(num++, "<color=#FF4D00FF>Arsonist</color>");

            ArsonistDouseTime =
                new CustomNumberOption(num++, "Douse Duration", 3, 1, 5, 0.5f, CooldownFormat);

            DouseCooldown =
                new CustomNumberOption(num++, "Douse Cooldown", 15, 10, 40, 2.5f, CooldownFormat);

            // ArsonistGameEnd = new CustomToggleOption(num++, "Game keeps going so long as Arsonist is alive", false);

            Jester = new CustomHeaderOption(num++, "<color=#FFFFBFCC>Jester</color>");

            JesterUseVent =
                new CustomToggleOption(num++, "Jester Can Use Vents", true);
            JesterDragBody = new CustomToggleOption(num++, "Jester Can Drag Body", true);
            JesterCanMorph = new CustomToggleOption(num++, "Jester Can Morph", true);
            
            Sniper = new CustomHeaderOption(num++, "<color=#545459>Sniper</color>");

            SniperWinCnt =
                new CustomNumberOption(num++, "Number of Sniper Kills to Win", 2, 1, 4, 1);
            SniperPerGame = new CustomStringOption(num++, "Kills to Win Per", new[] {"Round", "Game"});
            
            Zombie = new CustomHeaderOption(num++, "<color=#773800>Zombie</color>");

            ZombieReviveTime =
                new CustomNumberOption(num++, "Zombie Revive Time", 15, 2.5f, 60, 2.5f, CooldownFormat);
            ZombieKilledBySeer = new CustomToggleOption(num++, "Killed By Seer", true);

            // Scavenger = new CustomHeaderOption(num++, "<color=#545459>Scavenger</color>");
            //
            // ScavengerWinCnt =
            //     new CustomNumberOption(num++, "Number of Scavenger Eats to Win", 4, 1, 6, 1);
            //
            
            // ImpostorRolesSetting = new CustomHeaderOption(num++, "<color=#FF0000FF>Impostor Roles</color>");

            ImpostorRolesSetting = new CustomHeaderOption(num++, "<color=#FF0000FF>Impostor Roles</color>");
            
            Madmate = new CustomHeaderOption(num++, "<color=#FF0000FF>Madmate</color>");
            MadmateCanVent = new CustomToggleOption(num++, "Madmate Can Vent", false);
            
            Assassin = new CustomHeaderOption(num++, "<color=#FF0000FF>Assassin</color>");
            AssassinKills = new CustomNumberOption(num++, "Number of Assassin Kills", 10, 1, 10, 1);
            AssassinMultiKill = new CustomToggleOption(num++, "Assassin Can Kill Continuous");
            // AssassinCrewmateGuess = new CustomToggleOption(num++, "Assassin can Guess \"Crewmate\"", false);
            // AssassinGuessNeutrals = new CustomToggleOption(num++, "Assassin can Guess Neutral roles", true);
            // AssassinGuessImpostors = new CustomToggleOption(num++, "Assassin can Snipe Impostor", true);
            // AllImpostorCanGuess = new CustomToggleOption(num++, "All of Impostors can snipe while meeting", false);

            Camouflager =
                new CustomHeaderOption(num++, "<color=#FF0000FF>Camouflager</color>");
            CamouflagerCooldown =
                new CustomNumberOption(num++, "Camouflager Cooldown", 25, 10, 40, 2.5f, CooldownFormat);
            CamouflagerDuration =
                new CustomNumberOption(num++, "Camouflager Duration", 5, 3, 15, 1f, CooldownFormat);

            Cracker = new CustomHeaderOption(num++, "<color=#FF0000FF>Cracker</color>");
            CrackCd = new CustomNumberOption(num++, "Crack Cooldown", 25f, 10f, 50f, 5f, CooldownFormat);
            CrackDur = new CustomNumberOption(num++, "Crack Duration", 10f, 5, 40, 2.5f, CooldownFormat);
            
            
            DollMaker = new CustomHeaderOption(num++, "<color=#FF0000FF>DollMaker</color>");
            DollBreakTime = new CustomNumberOption(num++, "Doll Self Broken Time", 20.0f, 5f, 60f, 5f, CooldownFormat);
            
            Janitor = new CustomHeaderOption(num++, "<color=#FF0000FF>Janitor</color>");
            CleanCd =
                new CustomNumberOption(num++, "Clean Cooldown", 45, 15, 60, 5f, CooldownFormat);
            CleanDuration =
                new CustomNumberOption(num++, "Clean Duration", 3.0f, 0.5f, 5f, 0.5f, CooldownFormat);
            
            Miner = new CustomHeaderOption(num++, "<color=#FF0000FF>Miner</color>");
            MineCooldown =
                new CustomNumberOption(num++, "Mine Cooldown", 20, 10, 40, 2.5f, CooldownFormat);
            MaxVentNum =
                new CustomNumberOption(num++, "Max Mine Num", 5, 3, 10, 1);
            
            Morphling =
                new CustomHeaderOption(num++, "<color=#FF0000FF>Morphling</color>");
            MorphlingCooldown =
                new CustomNumberOption(num++, "Morphling Cooldown", 15, 10, 40, 2.5f, CooldownFormat);
            MorphlingDuration =
                new CustomNumberOption(num++, "Morphling Duration", 10, 5, 15, 1f, CooldownFormat);
            MorphCanVent =
                new CustomStringOption(num++, "Morphling Can Use Vent", new[] {"On not Morphing", "Always", "None"});

            MultiKiller = new CustomHeaderOption(num++, "<color=#FF0000FF>MultiKiller</color>");
            MultiKillerCdRate = new CustomNumberOption(num++, "MultiKiller Cooldown Rate", 200f, 100f, 250f, 10f, PercentFormat);
            MultiKillEnableTime = new CustomNumberOption(num++, "MultiKill Enable Time", 5.0f, 1f, 20f, 1f, CooldownFormat);

            
            
            Puppeteer = new CustomHeaderOption(num++, "<color=#FF0000FF>Puppeteer</color>");
            PossessTime = new CustomNumberOption(num++, "Possess Time", 3.0f, 1f, 5f, 0.5f, CooldownFormat);
            PossessMaxTime = new CustomNumberOption(num++, "Possess Max Time", 15.0f, 5f, 30f, 2.5f, CooldownFormat);
            ReleaseWaitTime = new CustomNumberOption(num++, "Wait Time After Release", 3.0f, 1f, 10f, 0.5f, CooldownFormat);
            PossessBodyReport = new CustomToggleOption(num++, "Perpetrator Can Report", false);
            Swooper = new CustomHeaderOption(num++, "<color=#FF0000FF>Swooper</color>");

            SwoopCooldown =
                new CustomNumberOption(num++, "Swoop Cooldown", 30, 10, 40, 2.5f, CooldownFormat);
            SwoopDuration =
                new CustomNumberOption(num++, "Swoop Duration", 5, 5, 15, 1f, CooldownFormat);
            SwooperCanVent =
                new CustomStringOption(num++, "Swooper Can Use Vent", new[] {"On not Swooping", "Always", "None"});

            Undertaker = new CustomHeaderOption(num++, "<color=#FF0000FF>Undertaker</color>");
            DragCooldown = new CustomNumberOption(num++, "Drag Cooldown", 1f, 1f, 40, 1f, CooldownFormat);
            DragVelocity = new CustomNumberOption(num++, "Drag Velocity", 100.0f, 50.0f, 150.0f, 25.0f, PercentFormat);
            VentWithBody = new CustomToggleOption(num++, "Can Vent on Dragging", false);
#endregion

            // Modifiers = new CustomHeaderOption(num++, "Modifiers");
            // TorchOn = new CustomNumberOption(true, num++, "<color=#FFFF99FF>Torch</color>", 0f, 0f, 100f, 10f,
            //     PercentFormat);
            // DiseasedOn =
            //     new CustomNumberOption(true, num++, "<color=#808080FF>Diseased</color>", 0f, 0f, 100f, 10f,
            //         PercentFormat);
            // FlashOn = new CustomNumberOption(true, num++, "<color=#FF8080FF>Flash</color>", 0f, 0f, 100f, 10f,
            //     PercentFormat);
            // TiebreakerOn = new CustomNumberOption(true, num++, "<color=#99E699FF>Tiebreaker</color>", 0f, 0f, 100f, 10f,
            //     PercentFormat);
            // DrunkOn = new CustomNumberOption(true, num++, "<color=#758000FF>Drunk</color>", 0f, 0f, 100f, 10f,
            //     PercentFormat);
            // BigBoiOn = new CustomNumberOption(true, num++, "<color=#FF8080FF>Giant</color>", 0f, 0f, 100f, 10f,
            //     PercentFormat);
            // ButtonBarryOn =
            //     new CustomNumberOption(true, num++, "<color=#E600FFFF>Button Barry</color>", 0f, 0f, 100f, 10f,
            //         PercentFormat);
        }
    }
}