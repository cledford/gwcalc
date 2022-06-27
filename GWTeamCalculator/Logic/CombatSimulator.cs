using System;
using System.Linq;

namespace GWTeamCalculator
{
    public class CombatSimulator
    {
        public static bool DoWeWinCombat(TeamWeightCalibration calibratorUs, Team us, 
            TeamWeightCalibration calibratorThem, Team enemy, bool isCalibration = false)
        {
            int fatigueCounterUs = 0;
            int fatigueCounterThem = 0;

            int currentIndexUs = 0;
            int currentIndexThem = 0;

            var usPlayers = us.Players.ToArray();
            var enemyPlayers = enemy.Players.ToArray();

            bool isBattleComplete = false;

            // Apply modifiers for teams
            // TODO: implement custom modifiers
            // default is always 1
            var usModifiers = 1 * us.TeamModifier;
            var enemyModifiers = 1 * enemy.TeamModifier;

            while(!isBattleComplete)
            {
                if (currentIndexUs == 15)
                {
                    Console.WriteLine("Bummer! We ran out of dudes. We lose.");
                    // Us loses, they have reached the end of the line.
                    return false; 
                }

                if(currentIndexThem == 15)
                {
                    Console.WriteLine("All their base and stuff. We win!");
                    // Enemy loses, they have reached the end of the line.
                    return true;
                }

                float ourGuysMight = usPlayers[currentIndexUs].Might * usModifiers;
                float theirGuysMight = enemyPlayers[currentIndexThem].Might * enemyModifiers;

                // If Us might is higher than Enemy might, index Enemy team and increment fatigue counter for Us player
                if (ourGuysMight > theirGuysMight)
                {
                    Console.WriteLine($"Player at index {currentIndexUs} has defeated enemy at {currentIndexThem}");
                    currentIndexThem++;
                    fatigueCounterThem = 0;
                    fatigueCounterUs++;

                    if (isCalibration)
                    {
                        calibratorThem.AddResult(enemy.TeamName, currentIndexThem, false, theirGuysMight * 0.01f);
                        calibratorUs.AddResult(us.TeamName, currentIndexUs + 1, true, ourGuysMight * 0.01f);
                    }
                }
                else
                {
                    Console.WriteLine($"Enemy at index {currentIndexThem} has defeated our player at {currentIndexUs}");
                    // Otherwise, do the opposite, resetting our own fatigue counter in the process.
                    currentIndexUs++;
                    fatigueCounterUs = 0;
                    fatigueCounterThem++;

                    if (isCalibration)
                    {
                        calibratorThem.AddResult(enemy.TeamName, currentIndexThem + 1, true, theirGuysMight * 0.01f);
                        calibratorUs.AddResult(us.TeamName, currentIndexUs, false, ourGuysMight * 0.01f);
                    }
                }

                if (fatigueCounterThem == 5)
                {
                    Console.WriteLine("Enemy player has reached full fatigue, moving to next enemy...");
                    // Enemy player has reached full fatigue
                    // reset fatigue counter and increment team index by 1
                    fatigueCounterThem = 0;
                    currentIndexThem++;
                }
                else if (fatigueCounterUs == 5)
                {
                    Console.WriteLine("Our player has reached full fatigue, moving to next player...");
                    // Us player has reached full fatigue
                    // reset fatigue counter and increment team index by 1
                    fatigueCounterUs = 0;
                    currentIndexUs++;
                }
            }

            // idk how we got here just call it whoever has the lower index the winner.
            return currentIndexUs < currentIndexThem;
        }
    }
}
