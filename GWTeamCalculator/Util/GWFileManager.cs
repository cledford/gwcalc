using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GWTeamCalculator
{
    public class GWFileManager
    {

        public static IEnumerable<Player> LoadTeamFromFile(string fileName)
        {
            using TextFieldParser parser = new(
                @$"{Directory.GetCurrentDirectory()}\teamInput\{fileName}.csv");

            parser.TextFieldType = FieldType.Delimited;
            parser.SetDelimiters(",");

            var players = new List<Player>();

            while (!parser.EndOfData)
            {
                //Processing row
                string[] fields = parser.ReadFields();
                foreach (string field in fields)
                {
                    string name = fields[0];
                    string mightAsString = fields[1];

                    // force smallest might if try fails
                    float might = float.TryParse(mightAsString, out float tryMight) ? tryMight : 0.01f;

                    players.Add(new Player(name, might));
                }
            }

            return players;
        }

        public static void PrintResultsToFil(TeamOptimizer ourTeamOptimizer,
            TeamOptimizer theirTeamOptimizer, Guild ourGuild, Guild enemyGuild,
            int currentVictoryRecord, GuildTeams winningTeam, bool didWeWin)
        {
            string filePath = @$"{Directory.GetCurrentDirectory()}\data";

            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }

            string fileName = $"{ourGuild.Name}-vs-{enemyGuild.Name}-{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}.txt";

            using StreamWriter outputFile = new StreamWriter($"{filePath}\\{fileName}");
            outputFile.WriteLine($"Testing Report for {ourGuild.Name} vs {enemyGuild.Name}");
            outputFile.WriteLine();
            outputFile.WriteLine(); // i knwo this isnt teh best way to do it i just wanna visualize the spacing
            outputFile.WriteLine();
            outputFile.WriteLine($"Attempted {ourTeamOptimizer.ShuffleAttempt} different teams {Environment.NewLine} each facing {theirTeamOptimizer.MaxShuffleAttempts} iterations of enemy teams");

            string winText = didWeWin ? "We won!" : "We lost...";

            outputFile.WriteLine();
            outputFile.WriteLine($"Overall, we {winText}");
            outputFile.WriteLine();
            outputFile.WriteLine();
            outputFile.WriteLine($"Our best record was {currentVictoryRecord}");
            outputFile.WriteLine();
            outputFile.WriteLine();

            outputFile.WriteLine("Our best team was: ");

            WriteTeamToFile(winningTeam.Sun, outputFile);
            WriteTeamToFile(winningTeam.Moon, outputFile);
            WriteTeamToFile(winningTeam.Star, outputFile);

            outputFile.WriteLine("Our team weights are:");
            var weights = ourTeamOptimizer.WeightCalibration.GetWeights();

            WriteWeightsToFile(TeamName.Sun, weights.SunWeights, outputFile);
            WriteWeightsToFile(TeamName.Moon, weights.MoonWeights, outputFile);
            WriteWeightsToFile(TeamName.Star, weights.StarWeights, outputFile);

            outputFile.WriteLine("Hope you enjoyed using the simulator!");

            outputFile.Flush();
        }

        public static void PrintTeams(Guild ourGuild)
        {
            Console.WriteLine("Team sun!");
            foreach (var player in ourGuild.GuildTeams.Sun.Players)
            {
                Console.WriteLine($"Name: {player.Name}, Might: {player.Might}");
            }

            Console.WriteLine("Team moon!");
            foreach (var player in ourGuild.GuildTeams.Moon.Players)
            {
                Console.WriteLine($"Name: {player.Name}, Might: {player.Might}");
            }

            Console.WriteLine("Team star!");
            foreach (var player in ourGuild.GuildTeams.Star.Players)
            {
                Console.WriteLine($"Name: {player.Name}, Might: {player.Might}");
            }
        }

        public static void WriteTeamToFile(Team winningTeam, StreamWriter outputFile)
        {
            outputFile.WriteLine($"-----------------{winningTeam.TeamName}----------------------");
            foreach (var player in winningTeam.Players.Select(s => s))
            {
                outputFile.WriteLine();
                outputFile.WriteLine($"Player: {player.Name}");
                outputFile.WriteLine($"Might: {player.Might}");
            }
            outputFile.WriteLine("---------------------------------------");
        }

        public static void WriteWeightsToFile(TeamName teamName, IDictionary<int, float> teamSlotWeights,
            StreamWriter outputFile)
        {
            outputFile.WriteLine();
            outputFile.WriteLine($"Team {teamName} slot weights:");
            outputFile.WriteLine();
            outputFile.WriteLine();
            foreach (var kvp in teamSlotWeights)
            {
                outputFile.WriteLine($"Slot: {kvp.Key} Weight: {kvp.Value}");
            }
            outputFile.WriteLine();
            outputFile.WriteLine();
        }
    }
}