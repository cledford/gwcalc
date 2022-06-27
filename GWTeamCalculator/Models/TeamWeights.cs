using System.Collections.Generic;

namespace GWTeamCalculator
{
    public class TeamWeights
    {
        public IDictionary<int, float> SunWeights { get; }
        public IDictionary<int, float> MoonWeights { get; }
        public IDictionary<int, float> StarWeights { get; }

        public TeamWeights(
            IDictionary<int, float> sun, 
            IDictionary<int, float> moon, 
            IDictionary<int, float> star)
        {
            SunWeights = sun;
            MoonWeights = moon;
            StarWeights = star;
        }
    }
}
