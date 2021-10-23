using System.Collections.Generic;
using UnityEngine;

namespace TownOfUs.Roles
{
    public class Sniper : Assassin {
        public int KilledCount = 0;
        public Sniper(PlayerControl player) : base(player)
        {
            Name = "Sniper";
            ImpostorText = () => "Kill during meetings if you can guess their roles";
            TaskText = () => "Guess the roles and kill double at one meeting";
            Color = new Color(0.33f, 0.33f, 0.35f);
            RoleType = RoleEnum.Sniper;
            Faction = Faction.Neutral;

            RemainingKills = 100;

            ColorMapping = new Dictionary<string, Color> {
                { "Mayor", new Color(0.44f, 0.31f, 0.66f, 1f) },
                { "Sheriff", Color.yellow },
                { "Engineer", new Color(1f, 0.65f, 0.04f, 1f) },
                { "Swapper", new Color(0.4f, 0.9f, 0.4f, 1f) },
                { "Investigator", new Color(0f, 0.7f, 0.7f, 1f) },
                { "Time Lord", new Color(0f, 0f, 1f, 1f) },
                { "Lover", new Color(1f, 0.4f, 0.8f, 1f) },
                { "Medic", new Color(0f, 0.4f, 0f, 1f) },
                { "Seer", new Color(1f, 0.8f, 0.5f, 1f) },
                { "SecurityGuard", new Color(0.67f, 0.67f, 1f)},
                // { "Spy", new Color(0.8f, 0.64f, 0.8f, 1f) },
                { "Snitch", new Color(0.83f, 0.69f, 0.22f, 1f) },
                { "Altruist", new Color(0.4f, 0f, 0f, 1f) },
                { "Charger", new Color(0.99f, 1f, 0.2f) },
                { "Druid", new Color(0.4f, 0f, 0.56f) },
                
                { "The Glitch", Color.green},
                { "Jester", new Color(1f, 0.75f, 0.8f, 1f)},
                { "Shifter", new Color(0.6f, 0.6f, 0.6f, 1f)},
                { "Executioner", new Color(0.55f, 0.25f, 0.02f, 1f)},
                { "Arsonist", new Color(1f, 0.3f, 0f)},
                { "Zombie", new Color(0.47f, 0.22f, 0f)},

                { "Assassin", Color.red},
                { "Janitor", Color.red},
                { "Morphling", Color.red},
                { "Camouflager", Color.red},
                { "Miner", Color.red},
                { "Swooper", Color.red},
                { "Undertaker", Color.red},
                // { "Underdog", Color.red},
                { "Kirby", new Color(0.66f, 0.42f, 0.64f)},
                { "Cracker", Color.red},
                { "MultiKiller", Color.red},
                { "Puppeteer", Color.red},
                { "DollMaker", Color.red},
            };
            CleanUpMapping();
        }
        
        internal override bool EABBNOODFGL(ShipStatus __instance)
        {
            if (KilledCount < CustomGameOptions.SniperWinCnt) {
                return true;
            }
            Utils.EndGame();
            return false;
        }

        protected override void IntroPrefix(IntroCutscene._CoBegin_d__14 __instance)
        {
            var sniperTeam = new Il2CppSystem.Collections.Generic.List<PlayerControl>();
            sniperTeam.Add(PlayerControl.LocalPlayer);
            __instance.yourTeam = sniperTeam;
        }

        public void Wins() {
            KilledCount = CustomGameOptions.SniperWinCnt;
        }

        public void Loses()
        {
            Player.Data.IsImpostor = true;
        }
    }
}
