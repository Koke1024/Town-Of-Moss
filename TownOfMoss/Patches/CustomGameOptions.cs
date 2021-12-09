using TownOfUs.CrewmateRoles.EngineerMod;
using TownOfUs.CrewmateRoles.BodyGuardMod;
using TownOfUs.CrewmateRoles.SeerMod;
using TownOfUs.CustomOption;
using TownOfUs.ImpostorRoles.MorphlingMod;
using TownOfUs.NeutralRoles.ExecutionerMod;
using TownOfUs.NeutralRoles.ShifterMod;
using TownOfUs.OnStartGame;
using TownOfUs.Roles;

namespace TownOfUs
{
    public static class CustomGameOptions
    {
        public static int MayorOn => (int) Generate.MayorOn.Get();
        public static int BodyGuardOn => (int) Generate.BodyGuardOn.Get();
        public static int JesterOn => (int) Generate.JesterOn.Get();
        public static int LoversOn => 0;
        // public static int LoversOn => (int) Generate.LoversOn.Get();
        public static int SheriffOn => (int) Generate.SheriffOn.Get();
        public static int JanitorOn => (int) Generate.JanitorOn.Get();
        public static int KirbyOn => (int) Generate.KirbyOn.Get();
        public static int EngineerOn => (int) Generate.EngineerOn.Get();
        public static int SwapperOn => (int) Generate.SwapperOn.Get();
        public static int ShifterOn => 0;
        // public static int ShifterOn => (int) Generate.ShifterOn.Get();
        public static int InvestigatorOn => (int) Generate.InvestigatorOn.Get();
        public static int TimeLordOn => (int) Generate.TimeLordOn.Get();
        public static int MedicOn => (int) Generate.MedicOn.Get();
        public static int SeerOn => (int) Generate.SeerOn.Get();
        public static int SecurityGuardOn => (int) Generate.SecurityGuardOn.Get();
        public static int PainterOn => (int) Generate.PainterOn.Get();
        public static int SnifferOn => (int) Generate.SnifferOn.Get();
        public static bool GlitchOn => Generate.GlitchOn.Get();
        public static int MorphlingOn => (int) Generate.MorphlingOn.Get();
        public static int CamouflagerOn => (int) Generate.CamouflagerOn.Get();
        public static int ExecutionerOn => (int) Generate.ExecutionerOn.Get();
        // public static int SpyOn => (int) Generate.SpyOn.Get();
         public static int SpyOn => (int) 0;
        public static int SnitchOn => (int) Generate.SnitchOn.Get();
        public static int MinerOn => (int) Generate.MinerOn.Get();
        public static int SwooperOn => (int) Generate.SwooperOn.Get();
        public static int ArsonistOn => (int) Generate.ArsonistOn.Get();
        public static int SniperOn => (int) Generate.SniperOn.Get();
        public static int SniperWinCnt => (int) Generate.SniperWinCnt.Get();
        public static EngineerFixPer SniperPerGame => (EngineerFixPer) Generate.SniperPerGame.Get();
        // public static int ScavengerOn => (int) Generate.ScavengerOn.Get();
        public static int ScavengerOn => 0;
        public static int ScavengerWinCount => (int) Generate.ScavengerWinCnt.Get();
        public static int AltruistOn => (int) Generate.AltruistOn.Get();
        public static int ZombieOn => (int) Generate.ZombieOn.Get();
        public static int ChargerOn => (int) Generate.ChargerOn.Get();
        public static int DruidOn => (int) Generate.DruidOn.Get();
        public static int UndertakerOn => (int) Generate.UndertakerOn.Get();
        public static int PuppeteerOn => (int) Generate.PuppeteerOn.Get();
        public static int DollMakerOn => (int) Generate.DollMakerOn.Get();
        public static int AssassinOn => (int) Generate.AssassinOn.Get();
        public static int UnderdogOn => 0;
        // public static int UnderdogOn => (int) Generate.UnderdogOn.Get();
        public static int MultiKillerOn => (int) Generate.MultiKillerOn.Get();
        public static int CrackerOn => (int) Generate.CrackerOn.Get();
        public static int PhantomOn => 0;
        public static int TorchOn => 0;
        public static int DiseasedOn => 0;
        public static int FlashOn => (int) 0;
        public static int TiebreakerOn => 0;
        public static int DrunkOn => 0;
        public static int BigBoiOn => 0;
        public static int ButtonBarryOn => 0;
        public static int VanillaGame => 0;
        // public static int VanillaGame => (int) Generate.VanillaGame.Get();
        public static bool BothLoversDie => Generate.BothLoversDie.Get();
        // public static bool ShowSheriff => Generate.ShowSheriff.Get();
        public static bool ShowSheriff => false;
        public static bool SheriffKillOther => Generate.SheriffKillOther.Get();
        public static bool SheriffKillsMadmate => Generate.SheriffKillsMadmate.Get();
        public static float SheriffKillCd => Generate.SheriffKillCd.Get();
        public static int MayorVoteBank => 0;
        public static bool MayorMeetingOnDead => Generate.MayorMeetingOnDead.Get();
        public static bool MayorAnonymous => Generate.MayorAnonymous.Get();
        // public static int MayorVoteBank => (int) Generate.MayorVoteBank.Get();
        // public static bool MayorAnonymous => Generate.MayorAnonymous.Get();
        public static float ShifterCd => Generate.ShifterCd.Get();
        public static ShiftEnum WhoShifts => (ShiftEnum) Generate.WhoShifts.Get();
        public static float InvestigatorSeeColorRange => Generate.InvestigatorSeeColorRange.Get();
        public static float InvestigatorSeeRange => Generate.InvestigatorSeeRange.Get();
        public static float InvestigatorMapUpdate => Generate.InvestigatorMapUpdate.Get();

        public static bool RewindRevive => Generate.RewindRevive.Get();
        public static bool RewindFlash => Generate.RewindFlash.Get();
        public static float RewindDuration => Generate.RewindDuration.Get();
        public static float RewindCooldown => Generate.RewindCooldown.Get();
        public static bool TimeLordVitals => false;
        // public static bool TimeLordVitals => Generate.TimeLordVitals.Get();
        public static ShieldOptions ShowShielded => (ShieldOptions) Generate.ShowShielded.Get();

        public static NotificationOptions NotificationShield =>
            (NotificationOptions) Generate.WhoGetsNotification.Get();
        public static float GuardRange => Generate.GuardRange.Get();
        public static float GuardDuration => Generate.GuardDuration.Get();
        public static float GuardCoolDown => Generate.GuardCoolDown.Get();
        public static bool ShieldBreaks => true;
        public static float MedicReportDegradation => Generate.MedicReportDegradation.Get();
        public static float SeerCd => Generate.SeerCooldown.Get();
        public static float MaxChargeTime => Generate.MaxChargeTime.Get();
        public static float DruidReviveRange => Generate.DruidReviveRange.Get();
        public static ReviveLimit DruidReviveLimit => (ReviveLimit)Generate.DruidReviveLimit.Get();
        public static float SecurityGuardCooldown => Generate.SecurityGuardCooldown.Get();
        public static int SecurityGuardTotalScrews => (int)Generate.SecurityGuardTotalScrews.Get();
        public static int SecurityGuardCamPrice => (int)Generate.SecurityGuardCamPrice.Get();
        public static int SecurityGuardVentPrice => (int)Generate.SecurityGuardVentPrice.Get();
        public static float ConsumeChargeTime => Generate.ConsumeChargeTime.Get();
        public static SeerInfo SeerInfo => (SeerInfo) Generate.SeerInfo.Get();
        public static SeeReveal SeeReveal => (SeeReveal) Generate.SeeReveal.Get();
        public static bool NeutralRed => Generate.NeutralRed.Get();
        public static bool GlitchAdmin => Generate.GlitchAdmin.Get();
        public static float MimicCooldown => Generate.MimicCooldownOption.Get();
        public static float MimicDuration => Generate.MimicDurationOption.Get();
        public static float HackCooldown => Generate.HackCooldownOption.Get();
        public static float HackDuration => Generate.HackDurationOption.Get();
        public static float GlitchKillCooldown => Generate.GlitchKillCooldownOption.Get();
        public static int GlitchHackDistance => Generate.GlitchHackDistanceOption.Get();
        public static float MorphlingCd => Generate.MorphlingCooldown.Get();
        public static MorphVentOptions SwooperCanVent => (MorphVentOptions)Generate.SwooperCanVent.Get();
        public static float MorphlingDuration => Generate.MorphlingDuration.Get();
        public static MorphVentOptions MorphCanVent => (MorphVentOptions)Generate.MorphCanVent.Get();
        public static float CamouflagerCd => Generate.CamouflagerCooldown.Get();
        public static float CamouflagerDuration => Generate.CamouflagerDuration.Get();
        public static bool KillCoolResetOnMeeting => Generate.KillCoolResetOnMeeting.Get();
        public static bool ColourblindComms => Generate.ColourblindComms.Get();
        public static bool MeetingColourblind => false;
        // public static bool MeetingColourblind => Generate.MeetingColourblind.Get();
        // public static OnTargetDead OnTargetDead => (OnTargetDead) Generate.OnTargetDead.Get();
        public static bool SnitchOnLaunch => true;
        // public static bool SnitchOnLaunch => Generate.SnitchOnLaunch.Get();
        public static bool SnitchSeesNeutrals => Generate.SnitchSeesNeutrals.Get();
        public static OpenDoorImmediate SnitchOpenDoorImmediately => (OpenDoorImmediate)Generate.SnitchOpenDoorImmediately.Get();
        public static SnitchNoShotTiming SnitchShotTiming => (SnitchNoShotTiming)Generate.SnitchNoShotTiming.Get();
        public static float MineCd => Generate.MineCooldown.Get();
        public static int MaxVentNum => (int)Generate.MaxVentNum.Get();
        public static float SwoopCd => Generate.SwoopCooldown.Get();
        public static float SwoopDuration => Generate.SwoopDuration.Get();
        public static float SeerInvestigateTime => Generate.SeerInvestigateTime.Get();
        public static float PossessTime => Generate.PossessTime.Get();
        public static float PossessCd => Generate.PossessCd.Get();
        public static bool PossessBodyReport => Generate.PossessBodyReport.Get();
        public static float PossessMaxTime => Generate.PossessMaxTime.Get();
        public static float ReleaseWaitTime => Generate.ReleaseWaitTime.Get();
        public static float DollBreakTime => Generate.DollBreakTime.Get();
        public static bool ImpostorSeeRoles => Generate.ImpostorSeeRoles.Get();
        public static bool DeadSeeRoles => Generate.DeadSeeRoles.Get();
        public static float ArsonistDouseTime => Generate.ArsonistDouseTime.Get();
        public static float DouseCd => Generate.DouseCooldown.Get();
        public static float ZombieReviveTime => Generate.ZombieReviveTime.Get();
        public static bool ZombieKilledBySeer => Generate.ZombieKilledBySeer.Get();
        public static bool ArsonistGameEnd => false;
        // public static bool ArsonistGameEnd => Generate.ArsonistGameEnd.Get();
        public static bool JesterUseVent => Generate.JesterUseVent.Get();
        public static bool JesterDragBody => Generate.JesterDragBody.Get();
        public static bool JesterCanMorph => Generate.JesterCanMorph.Get();
        public static int MaxImpostorRoles => 10;
        // public static int MaxImpostorRoles => (int) Generate.MaxImpostorRoles.Get();
        public static int MaxNeutralRoles => (int) Generate.MaxNeutralRoles.Get();
        public static int CrackCd => (int) Generate.CrackCd.Get();
        public static int CrackDur => (int) Generate.CrackDur.Get();
        public static int MultiKillerCdRate => (int) Generate.MultiKillerCdRate.Get();
        public static float MultiKillEnableTime => (float) Generate.MultiKillEnableTime.Get();
        public static int PolusReactorTimeLimit => (int) Generate.PolusReactorTimeLimit.Get();
        public static int AirshipReactorTimeLimit => (int) Generate.AirshipReactorTimeLimit.Get();
        public static PolusVitalPosition PolusVitalMove => (PolusVitalPosition)Generate.PolusVitalMove.Get();
        public static float AdminTimeLimitTime => (float) Generate.AdminTimeLimitTime.Get();
        public static bool RoleUnderName => Generate.RoleUnderName.Get();
        public static EngineerFixPer EngineerFixPer => (EngineerFixPer) Generate.EngineerPer.Get();
        public static bool EngineerCanFixOnlyInVent => Generate.EngineerCanFixOnlyInVent.Get();
        public static float ReviveDuration => Generate.ReviveDuration.Get();
        // public static int MayorExtendTime => (int) Generate.MayorExtendTime.Get();
        public static int MayorExtendTime => 0;
        public static bool AltruistTargetBody => Generate.AltruistTargetBody.Get();
        public static bool SheriffBodyReport => Generate.SheriffBodyReport.Get();
        public static float CleanCd => Generate.CleanCd.Get();
        public static float CleanDuration => Generate.CleanDuration.Get();
        public static float DragCd => Generate.DragCooldown.Get();
        public static float DragVel => Generate.DragVelocity.Get();
        public static bool VentWithBody => Generate.VentWithBody.Get();
        // public static bool AssassinGuessNeutrals => Generate.AssassinGuessNeutrals.Get();
        // public static bool AssassinCrewmateGuess => Generate.AssassinCrewmateGuess.Get();
        // public static bool AssassinGuessImpostors => Generate.AssassinGuessImpostors.Get();
        public static bool AssassinGuessNeutrals => true;
        public static bool AssassinCrewmateGuess => false;
        public static bool AssassinGuessImpostors => false;
        public static int AssassinKills => (int) Generate.AssassinKills.Get();
        public static bool AssassinMultiKill => Generate.AssassinMultiKill.Get();
        public static bool MadMateOn => Generate.MadMateOn.Get();
        public static bool AssassinCanKillAfterVote => false;
        public static bool AllImpCanGuess => false;
        public static bool LastImpCanGuess => Generate.LastImpostorCanGuess.Get();
        public static bool NoticeNeutral => Generate.NoticeNeutral.Get();
        public static bool GhostCantMove => Generate.GhostCantMove.Get();
        public static int PaintColorMax => (int)Generate.PaintColorMax.Get();
        public static float PaintCd => Generate.PaintCd.Get();
        public static bool SnifferCanReport => Generate.SnifferCanReport.Get();
        public static float SnifferMaxRange => Generate.SnifferMaxRange.Get();
    }
}