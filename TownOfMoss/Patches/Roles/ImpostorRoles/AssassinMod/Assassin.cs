using System.Collections.Generic;
using System.Linq;
using Il2CppSystem;
using Il2CppSystem.Globalization;
using Rewired;
using TMPro;
using TownOfUs.ImpostorRoles.AssassinMod;
using UnityEngine;

namespace TownOfUs.Roles
{
    public class Assassin : Role
    {
        public Dictionary<byte, GameObject> Buttons = new Dictionary<byte, GameObject>();


        public Dictionary<string, Color> ColorMapping = new Dictionary<string, Color>
        {
            { "Mayor", new Color(0.44f, 0.31f, 0.66f, 1f) },
            // { "Sheriff", Color.yellow },
            { "Sheriff", Color.yellow },
            { "Engineer", new Color(1f, 0.65f, 0.04f, 1f) },
            { "Swapper", new Color(0.4f, 0.9f, 0.4f, 1f) },
            { "Investigator", new Color(0f, 0.7f, 0.7f, 1f) },
            { "Time Lord", new Color(0f, 0f, 1f, 1f) },
            // { "Lover", new Color(1f, 0.4f, 0.8f, 1f) },
            { "Medic", new Color(0f, 0.4f, 0f, 1f) },
            { "Seer", new Color(1f, 0.8f, 0.5f, 1f) },
            { "SecurityGuard", new Color(0.67f, 0.67f, 1f)},
            // { "Spy", new Color(0.8f, 0.64f, 0.8f, 1f) },
            { "Snitch", new Color(0.83f, 0.69f, 0.22f, 1f) },
            { "Altruist", new Color(0.4f, 0f, 0f, 1f) },
            { "Charger", new Color(0.99f, 1f, 0.2f) },
            { "Druid", new Color(0.4f, 0f, 0.56f) },
            { "Painter", new Color(0.81f, 0.81f, 0.81f) },
            { "Sniffer", new Color(0.65f, 0f, 0.83f) }
        };

        public Dictionary<byte, string> Guesses = new Dictionary<byte, string>();


        public Assassin(PlayerControl player) : base(player)
        {
            Name = "Assassin";
            ImpostorText = () => "Kill during meetings if you can guess their roles";
            TaskText = () => "Guess the roles of the people and kill them mid-meeting";
            Color = Palette.ImpostorRed;
            RoleType = RoleEnum.Assassin;
            Faction = Faction.Impostors;

            if (CustomGameOptions.MadMateOn) {
                Name = "Madmate";
                Faction = Faction.Crewmates;
                Player.Data.Role.TeamType = RoleTeamTypes.Crewmate;
                ImpostorText = () => "Support Impostors and shoot crewmates";
                TaskText = () => "Support Impostors and shoot crewmates";
            }

            RemainingKills = CustomGameOptions.AssassinKills;

            if (CustomGameOptions.AssassinGuessNeutrals && CustomGameOptions.MaxNeutralRoles > 0)
            {
                ColorMapping.Add("Jester", new Color(1f, 0.75f, 0.8f, 1f));
                ColorMapping.Add("Shifter", new Color(0.6f, 0.6f, 0.6f, 1f));
                ColorMapping.Add("Executioner", new Color(0.55f, 0.25f, 0.02f, 1f));
                ColorMapping.Add("The Glitch", Color.green);
                ColorMapping.Add("Arsonist", new Color(1f, 0.3f, 0f));
                ColorMapping.Add("Sniper", new Color(0.33f, 0.33f, 0.35f));
                ColorMapping.Add("Zombie", new Color(0.47f, 0.22f, 0f));
            }

            if (CustomGameOptions.AssassinCrewmateGuess) ColorMapping.Add("Crewmate", Color.white);
            if (CustomGameOptions.AssassinGuessImpostors) ColorMapping.Add("Impostor", Color.red);

            CleanUpMapping();
        }

        protected void CleanUpMapping() {
            Dictionary<string, int> onList = new Dictionary<string, int> {
                { "Mayor", CustomGameOptions.MayorOn },
                // { "Sheriff", CustomGameOptions.SheriffOn },
                { "Sheriff", CustomGameOptions.SheriffOn },
                { "Engineer", CustomGameOptions.EngineerOn },
                { "Swapper", CustomGameOptions.SwapperOn },
                { "Investigator", CustomGameOptions.InvestigatorOn },
                { "Time Lord", CustomGameOptions.TimeLordOn },
                { "Lover", CustomGameOptions.LoversOn },
                { "Medic", CustomGameOptions.MedicOn },
                { "BodyGuard", CustomGameOptions.BodyGuardOn },
                { "Seer", CustomGameOptions.SeerOn },
                { "SecurityGuard", CustomGameOptions.SecurityGuardOn },
                { "Spy", CustomGameOptions.SpyOn },
                { "Snitch", CustomGameOptions.SnitchOn },
                { "Altruist", CustomGameOptions.AltruistOn },
                { "Charger", CustomGameOptions.ChargerOn },
                { "Druid", CustomGameOptions.DruidOn },
                { "Painter", CustomGameOptions.PainterOn },
                
                { "Jester", CustomGameOptions.JesterOn },
                { "Shifter", CustomGameOptions.ShifterOn },
                { "Executioner", CustomGameOptions.ExecutionerOn },
                { "The Glitch", CustomGameOptions.GlitchOn? 100: 0 },
                { "Arsonist", CustomGameOptions.ArsonistOn },
                { "Zombie", CustomGameOptions.ZombieOn },
                
                { "Assassin", CustomGameOptions.MadMateOn? 0: (CustomGameOptions.LastImpCanGuess? 0: CustomGameOptions.AssassinOn) },
                { "Janitor", CustomGameOptions.JanitorOn },
                { "Popopo", CustomGameOptions.KirbyOn },
                { "Morphling", CustomGameOptions.MorphlingOn },
                { "Camouflager", CustomGameOptions.CamouflagerOn },
                { "Miner", CustomGameOptions.MinerOn },
                { "Swooper", CustomGameOptions.SwooperOn },
                { "Undertaker", CustomGameOptions.UndertakerOn },
                { "Underdog", CustomGameOptions.UnderdogOn },
                { "Cracker", CustomGameOptions.CrackerOn },
                { "MultiKiller", CustomGameOptions.MultiKillerOn },
                { "Puppeteer", CustomGameOptions.PuppeteerOn },
                { "DollMaker", CustomGameOptions.DollMakerOn },
                { "Madmate", CustomGameOptions.MadMateOn? 100: 0 }
            };

            foreach (var row in onList) {
                if (row.Value == 0) {
                    if (ColorMapping.ContainsKey(row.Key)) {
                        ColorMapping.Remove(row.Key);
                    }
                }
            }

            if (Player.Is(RoleEnum.Sniper)) {
                ColorMapping.Remove("Sniper");
            }
        }
        protected override void IntroPrefix(IntroCutscene._CoBegin_d__18 __instance)
        {
            if (!CustomGameOptions.MadMateOn) {
                return;
            }

            if (Player.Is(RoleEnum.Assassin)) {
                var madMateTeam = new Il2CppSystem.Collections.Generic.List<PlayerControl>();
                madMateTeam.Add(PlayerControl.LocalPlayer);
                __instance.yourTeam = madMateTeam;
                return;
            }

            var assassin = GetRoles(RoleEnum.Assassin);
            if (!assassin.Any()) {
                return;
            }
            __instance.yourTeam.Remove(assassin.First().Player);
        }

        public bool GuessedThisMeeting { get; set; } = false;

        public int RemainingKills { get; set; }

        public List<string> PossibleGuesses => ColorMapping.Keys.ToList();
    }
}
