using System.Collections.Generic;

namespace GWTeamCalculator
{
    public class Team 
    {
        // todo
        public float CustomTeamModifer { get; set; }

        // todo
        public float TeamModifier { get; set; }

        public IEnumerable<Player> Players { get; set; }
        
        public TeamName TeamName { get; }

        public Team(TeamName teamName,IEnumerable<Player> players, float teamModifier = 1)
        {
            TeamName = teamName;
            Players = players;
            TeamModifier = teamModifier;
        }
    }
}
