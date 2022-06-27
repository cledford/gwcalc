using System.Collections.Generic;

namespace GWTeamCalculator
{
    public class Guild
    {
        public string Name { get; set; }

        public GuildTeams GuildTeams { get; set; }

        public IEnumerable<Player> Players { get; set; }
    }
}
