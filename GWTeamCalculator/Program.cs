using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using static GWTeamCalculator.GWFileManager;

namespace GWTeamCalculator
{
    class Program
    {
        private static List<Player> EnemyPlayers = new List<Player>
        {
            new Player("vina", 9.3f),
            new Player("saori", 1.05f),
            new Player("tal", 0.81f),
            new Player("yun", 0.8f),
            new Player("kenen", 4.54f),
            new Player("yue lao", 2.02f),
            new Player("xie lian", 1.72f),
            new Player("ai", 1.91f),
            new Player("miaka", 2.05f),
            new Player("meizhuyu", 1.32f),
            new Player("kriss", 2.25f),
            new Player("laney", 2.35f),
            new Player("meihui", 0.84f),
            new Player("dovee", 0.81f),
            new Player("faithlyn", 1.22f),
            new Player("maestro2", 1.51f),
            new Player("seraph2", 1.16f),
            new Player("tal2", 4.39f),
            new Player("yun2", 3.88f),
            new Player("kenen2", 2.20f),
            new Player("yue lao2", 2.08f),
            new Player("xie lian2", 1.97f),
            new Player("ai2", 1.86f),
            new Player("miaka2", 3.19f),
            new Player("meizhuyu2", 1.22f),
            new Player("kriss2", 0.88f),
            new Player("laney2", 0.83f),
            new Player("meihu", 0.71f),
            new Player("dovee2", 0.73f),
            new Player("faithlyn2", 0.96f),
            new Player("maestro3", 1.85f),
            new Player("seraph3", 1.2F),
            new Player("tal3", 0.01f),
            new Player("yun3", 0.01f),
            new Player("kenen3", 0.01f),
            new Player("yue lao3", 0.01f),
            new Player("xie lian3", 0.01f),
            new Player("ai3", 0.01f),
            new Player("miaka3", 0.01f),
            new Player("meizhuyu3", 0.01f),
            new Player("kriss3", 0.01f),
            new Player("laney3", 0.01f),
            new Player("meihui3", 0.01f),
            new Player("dovee3", 0.01f),
            new Player("faithlyn3", 0.01f)
        };

        private static List<Player> OurPlayers = new List<Player>
        {
            new Player("syn", 4.6f),
            new Player("Ari", 4.3f),
            new Player("Ahsoka", 3.2f),
            new Player("Mando", 3.1f),
            new Player("Zen", 2.7f),
            new Player("Hagrid", 2.7f),
            new Player("Pooplace", 2.5f),
            new Player("Xing", 2.5f),
            new Player("Lamia", 2.5f),
            new Player("Tai", 2.4f),
            new Player("Calliste", 2.4f),
            new Player("Leia", 2.3f),
            new Player("Wendy", 2.1f),
            new Player("ChineseName", 2.1f),
            new Player("Jiodi", 2.03f),
            new Player("Li Fei Dei", 1.8f),
            new Player("lillo", 1.8f),
            new Player("Violet", 1.79f),
            new Player("ruri", 1.6f),
            new Player("Rosen", 1.6f),
            new Player("King", 1.6f),
            new Player("Keyani", 1.55f),
            new Player("Min Deji", 1.4f),
            new Player("Maestro", 1.38f),
            new Player("reckless", 2.28f),
            new Player("lilianna", 2.27f),
            new Player("lee lei", 2.24f),
            new Player("westley", 1.09f),
            new Player("boba fett", 0.93f),
            new Player("Flicker", 1.2f),
            new Player("lindseyrst", 0.8f),
            new Player("aisha", 0.79f),
            new Player("Eiza", 0.87f),
            new Player("jeong hyun", 1.12f),
            new Player("keli", 1.1f),
            new Player("huntey", 1.28f),
            new Player("zienna", 1.27f),
            new Player("keyani", 1.55f),
            new Player("jing", 1.66f),
            new Player("lin yuan", 1.2f),
            new Player("kairi", 1.35f),
            new Player("yue shen", 1.2f),
            new Player("syrinnia", 1.12f),
            new Player("SakuraKittie", 0.96f),
            new Player("Historia", 0.71f)
        };

        const int OurShuffleAttempts = 10000;
        const int TheirShuffleAttempts = 500000;

        static void Main(string[] args)
        {
            var ourPeople = LoadTeamFromFile("ours");
            var theirPeople = LoadTeamFromFile("theirs");

            Guild ourGuild = new Guild()
            {
                Name = "TestGuild",
                Players = ourPeople
            };

            Guild enemyGuild = new Guild()
            {
                Name = "EnemyGuild",
                Players = theirPeople
            };

            Console.WriteLine($"{ourGuild.Name} vs {enemyGuild.Name}");

            var averageMightUs = ourGuild.Players.Average(p => p.Might);
            var averageMightThem = enemyGuild.Players.Average(p => p.Might);

            Console.WriteLine($"Stats: {ourGuild.Name}");
            Console.WriteLine($"Average Might: {averageMightUs}");

            Console.WriteLine($"Stats: {enemyGuild.Name}");
            Console.WriteLine($"Average Might: {averageMightThem}");

            Console.WriteLine("Let's set some stuff up...");

            //Console.WriteLine("Shall we use our team optimizer? [y/n]");
            TeamOptimizer ourTeamOptimizer = null;
            bool useTeamOurTeamOptimiser = true; //Console.ReadKey().KeyChar.Equals('y');
            if (useTeamOurTeamOptimiser)
            {
                ourTeamOptimizer = SetUpOptimizer(ourGuild, 1f);
                ourTeamOptimizer.MaxShuffleAttempts = OurShuffleAttempts;
            }

            //Console.WriteLine("Shall we use the enemy team optimizer? [y/n]");
            TeamOptimizer theirTeamOptimizer = null;
            bool useTheirTeamOptimizer = true; // Console.ReadKey().KeyChar.Equals('y');
            if (useTheirTeamOptimizer)
            {
                theirTeamOptimizer = SetUpOptimizer(enemyGuild,1f);
                theirTeamOptimizer.MaxShuffleAttempts = TheirShuffleAttempts;
            }

            CombatLearning.CalibrateWeights(
                ourTeamOptimizer, 
                theirTeamOptimizer, 10000000);


            bool didWeWin = false;
            int iteration = 0;
            int currentVictoryRecord = 0;
            int newPotentialRecord = 0;
            GuildTeams winningTeam = ourGuild.GuildTeams;
            bool done = false;
            while (!done)
            {
                if (iteration > theirTeamOptimizer.MaxShuffleAttempts * ourTeamOptimizer.MaxShuffleAttempts)
                {
                    
                    Console.WriteLine("Something weird happened and we just did a lot of attempts");
                    Console.WriteLine($"Current team:");
                    PrintResultsToFil(ourTeamOptimizer,theirTeamOptimizer,ourGuild,enemyGuild,currentVictoryRecord,winningTeam,didWeWin);
                    done = true;
                }
                iteration++;

                Console.WriteLine("Time for battle! Who will win?");

                Console.WriteLine("Team Sun!");

                var winnerSun = CombatSimulator.DoWeWinCombat(
                    ourTeamOptimizer.WeightCalibration, 
                    ourGuild.GuildTeams.Sun,
                    theirTeamOptimizer.WeightCalibration, 
                    enemyGuild.GuildTeams.Sun)
                        ? ourGuild.Name 
                        : enemyGuild.Name;

                Console.WriteLine($"And the winner of Sun battle is: {winnerSun}");

                var winnerMoon = CombatSimulator.DoWeWinCombat(
                    ourTeamOptimizer.WeightCalibration, 
                    ourGuild.GuildTeams.Moon,
                    theirTeamOptimizer.WeightCalibration, 
                    enemyGuild.GuildTeams.Moon)
                        ? ourGuild.Name 
                        : enemyGuild.Name;

                Console.WriteLine($"And the winner of Moon battle is: {winnerMoon}");

                var winnerStar = CombatSimulator.DoWeWinCombat(
                    ourTeamOptimizer.WeightCalibration,
                    ourGuild.GuildTeams.Star,
                    theirTeamOptimizer.WeightCalibration,
                    enemyGuild.GuildTeams.Star)
                        ? ourGuild.Name 
                        : enemyGuild.Name;

                Console.WriteLine($"And the winner of Star battle is: {winnerStar}");

                string finalWinner = new[] { winnerSun, winnerMoon, winnerStar }
                    .Where(w => w.Equals(ourGuild.Name))
                    .Count() > 1
                    ? ourGuild.Name
                    : enemyGuild.Name;

                Console.WriteLine("and the final winner issssss...");
                Console.WriteLine(finalWinner);

                if(finalWinner == ourGuild.Name)
                {
                    Console.WriteLine("We win! here's the lineup!");
                    PrintTeams(ourGuild);

                    newPotentialRecord++;

                    if(newPotentialRecord >= currentVictoryRecord)
                    {
                        currentVictoryRecord = newPotentialRecord;
                        winningTeam = ourGuild.GuildTeams;
                    }

                    if(newPotentialRecord == 100)
                    {
                        Console.WriteLine($"Seems good, 100-win milestone in a row! Current wincount: {newPotentialRecord}");
                        //Console.WriteLine("Super streak team is:");
                        //PrintTeams(ourGuild);
                    }

                    Console.WriteLine("Enemy team needs to change up!");

                    if (theirTeamOptimizer.ShuffleAttempt >= theirTeamOptimizer.MaxShuffleAttempts)
                    {
                        if (ourTeamOptimizer.ShuffleAttempt >= ourTeamOptimizer.MaxShuffleAttempts)
                        {
                            Console.WriteLine($"Max attempts done, I tested {theirTeamOptimizer.MaxShuffleAttempts} times!!!");
                            didWeWin = true;
                            done = true;
                            break;
                        }

                        // let them try again
                        theirTeamOptimizer.ResetShuffles();

                        // try a new team
                        ourTeamOptimizer.ShuffleTeams();
                    }

                    Console.WriteLine("They lose, trying new team configuration");
                    theirTeamOptimizer.ShuffleTeams();
                    //continue;
                }
                else
                {
                    newPotentialRecord = 0;

                    if (ourTeamOptimizer.ShuffleAttempt >= ourTeamOptimizer.MaxShuffleAttempts)
                    {
                        Console.WriteLine($"Max attempts done, I tested {ourTeamOptimizer.MaxShuffleAttempts} times!!!");
                        didWeWin = false;

                        done = true;
                        break;
                    }

                    Console.WriteLine("We lose, trying new team configuration");
                    ourTeamOptimizer.ShuffleTeams();
                    //    continue;
                }

                Console.WriteLine($"Did we win? {didWeWin}");
            }

            PrintResultsToFil(ourTeamOptimizer, theirTeamOptimizer, ourGuild, enemyGuild, 
                currentVictoryRecord, winningTeam, true);

            Console.WriteLine("Press any key to continue..");
            Console.ReadKey();
        }

        private static TeamOptimizer SetUpOptimizer(Guild ourGuild, float teamModifier = 1)
        {
            TeamOptimizer ourTeamOptimizer;
            //Console.WriteLine($"Cool, so we are going to use the team optimizer. Go with defaults (y) or configure paramters (n)? [y/n]");
            ourTeamOptimizer = new TeamOptimizer(ourGuild, teamModifier);
            ourTeamOptimizer.CreateTeams();

            bool useDefaults = true;// Console.ReadKey().KeyChar.Equals('y');

            if (!useDefaults)
            {
                Console.WriteLine($"what market battle buff does the team have?");
                Console.WriteLine($"'0' for none");
                Console.WriteLine($"'1' for bronze");
                Console.WriteLine($"'2' for silver");
                Console.WriteLine($"'3' for gold");

                float totalModifier = 1;

                int mbBuff = 0;
                MarketBattleBuff buff;

                if (!int.TryParse(Console.ReadKey().KeyChar.ToString(), out mbBuff))
                {
                    Console.WriteLine("Wtf was that? You're getting a 0...");
                }
                else
                {
                    switch (mbBuff)
                    {
                        case 3:
                            buff = MarketBattleBuff.Gold;
                            break;
                        case 2:
                            buff = MarketBattleBuff.Silver;
                            break;
                        case 1:
                            buff = MarketBattleBuff.Bronze;
                            break;
                        case 0:
                        default:
                            buff = MarketBattleBuff.None + 1;
                            break;
                    }

                    totalModifier *= (float)buff;

                    Console.WriteLine("Add a multiplier number if you'd like to make the enemy team stronger for tighter calibrations");
                    int extraEnemyModifier = 1;

                    if (!int.TryParse(Console.ReadKey().KeyChar.ToString(), out extraEnemyModifier))
                    {
                        Console.WriteLine("Wtf was that? You're getting a 1...");
                    }
                    else
                    {
                        totalModifier *= extraEnemyModifier;
                    }
                }

                ourTeamOptimizer.CustomEnemyTeamModifier = 1 * totalModifier;

                Console.WriteLine("How many attempts?");
                int attempts = int.TryParse(Console.ReadKey().ToString(), out int times)
                    ? times
                    : 10000;

                ourTeamOptimizer.MaxShuffleAttempts = attempts;
            }

            return ourTeamOptimizer;
        }
    }
}
