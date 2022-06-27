using System.Linq;

namespace GWTeamCalculator
{
    public class CombatLearning
    {
        public static void CalibrateWeights(TeamOptimizer ours, 
            TeamOptimizer theirs, int maxAttempts)
        {
            int totalAttempts = 0;

            while(totalAttempts <= maxAttempts)
            {
                bool sunWin = CombatSimulator.DoWeWinCombat(
                    ours.WeightCalibration,
                    ours.Guild.GuildTeams.Sun, 
                    theirs.WeightCalibration,
                    theirs.Guild.GuildTeams.Sun,
                    true);

                bool moonWin = CombatSimulator.DoWeWinCombat(
                    ours.WeightCalibration,
                    ours.Guild.GuildTeams.Moon,
                    theirs.WeightCalibration,
                    theirs.Guild.GuildTeams.Moon,
                    true);

                bool starWin = CombatSimulator.DoWeWinCombat(
                    ours.WeightCalibration,
                    ours.Guild.GuildTeams.Star,
                    theirs.WeightCalibration,
                    theirs.Guild.GuildTeams.Star,
                    true);

                if (new[] { sunWin, moonWin, starWin }.Count(w => w) > 1)
                {
                    ours.ShuffleTeams(true);
                }
                else
                {
                    theirs.ShuffleTeams(true);
                }

                totalAttempts++;
            }
        }
    }
}
