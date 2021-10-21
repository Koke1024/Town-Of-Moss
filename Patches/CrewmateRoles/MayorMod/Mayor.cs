using System.Collections.Generic;
using UnityEngine;

namespace TownOfUs.Roles
{
    public class Mayor : Role
    {
        public List<byte> ExtraVotes = new List<byte>();
        public KillButtonManager ButtonButton;
        public bool ButtonUsed;

        public float reportDelay;
        // public bool extended = false;

        public Mayor(PlayerControl player) : base(player)
        {
            Name = "Mayor";
            ImpostorText = () => "Tiebreaker and have extra emergency button";
            TaskText = () => "You are Tiebreaker and have extra emergency button";
            Color = new Color(0.44f, 0.31f, 0.66f, 1f);
            RoleType = RoleEnum.Mayor;
            // VoteBank = CustomGameOptions.MayorVoteBank;
            ExtraVotes = new List<byte>();
        }

        public int VoteBank { get; set; }
        public bool SelfVote { get; set; }

        public bool VotedOnce { get; set; }

        public PlayerVoteArea Abstain { get; set; }
        // public PlayerVoteArea Extend { get; set; }

        public bool CanVote => VoteBank > 0 && !SelfVote;
    }
}