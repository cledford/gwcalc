using System.Collections.Generic;

namespace GWTeamCalculator
{
    public class GuildTeams
    {
        public Team Sun { get; set; }
        public Team Moon { get; set; }
        public Team Star { get; set; }

        public GuildTeams(IEnumerable<Team> teams)
        {
            foreach(var team in teams)
            {
                switch(team.TeamName)
                {
                    case TeamName.Sun:
                        Sun = team;
                        break;
                    case TeamName.Moon:
                        Moon = team;
                        break;
                    case TeamName.Star:
                        Star = team;
                        break;
                }
            }
        }
    }
}
