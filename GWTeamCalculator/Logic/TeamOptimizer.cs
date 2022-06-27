using System;
using System.Collections.Generic;
using System.Linq;

namespace GWTeamCalculator
{
    public class TeamOptimizer
    {
        const int TotalPlayersAllowed = 45;
        const int PlayersPerTeam = 15;


        public float CustomEnemyTeamModifier = 1f;
        public int MaxShuffleAttempts = 1000;

        private float floorMight = 0; // fill teams with these 
                                      // higher median/mean teams should also have a higher floor  

        private readonly Guild _ourGuild;
        private int _shuffleAttempt = 0;

        private float _teamModifier;

        public Guild Guild => _ourGuild;
        public TeamWeightCalibration WeightCalibration { get; }
        public int ShuffleAttempt => _shuffleAttempt;

        public TeamOptimizer(Guild guild, float teamModifier = 1)
        {
            _ourGuild = guild;
            WeightCalibration = new TeamWeightCalibration();
            _teamModifier = teamModifier;
        }

        public GuildTeams CreateTeams()
        {
            _ = new List<Player>(45);
            var teamSun = new Team(TeamName.Sun, null, _teamModifier);
            var teamMoon = new Team(TeamName.Moon, null, _teamModifier);
            var teamStar = new Team(TeamName.Star, null, _teamModifier);
            List<Player> playerPool;
            if (_ourGuild.Players.Count() < TotalPlayersAllowed)
            {
                playerPool = FillTeamDefaults().ToList();
                _ourGuild.Players = playerPool;
            }
            else
            {
                playerPool = _ourGuild.Players.ToList();
            }

            DefaultSortTeams(playerPool, teamSun, teamMoon, teamStar);

            return _ourGuild.GuildTeams;
        }

        private IDictionary<int, Player> 
            CreateWeightSlotsDefault(IDictionary<int,float> weights)
        {
            return new Dictionary<int, Player>(
                weights.OrderByDescending(w => w.Value)
                    .Select(w =>
                        new KeyValuePair<int, Player>(w.Key, 
                            new Player("placeholder", w.Value)))); // we will use weight value but replace with actual might
        }

        public void ShuffleTeams(bool randomize = false)
        {
            if (randomize)
            {
                ShuffleByRandom();
            }
            else
            {
                ShuffleByWeight();
            }

            _shuffleAttempt++;
        }

        private void ShuffleByRandom()
        {
            var newPool = new List<Player>(_ourGuild.Players);

            var sun = new Player[15];
            var moon = new Player[15];
            var star = new Player[15];

            int sunCount = 0;
            int moonCount = 0;
            int starCount = 0;

            var sunUsedIndices = new List<int>();
            var moonUsedIndices = new List<int>();
            var starUsedIndices = new List<int>();

            var random = new Random();

            foreach (var player in newPool)
            {
                int teamNumber = random.Next(1, 3);
                int slot;

                // sun
                if (teamNumber == 0 && sunCount < 15)
                {
                    // get random slot number
                    slot = GetRandomSlotNumber(random,sunUsedIndices);
                    sun[slot] = player;
                    sunCount++;

                } // moon
                else if (teamNumber == 1 && moonCount < 15)
                {
                    slot = GetRandomSlotNumber(random,moonUsedIndices);
                    moon[slot] = player;
                    moonCount++;

                } // star
                else if (teamNumber == 2 && starCount < 15)
                {
                    slot = GetRandomSlotNumber(random,starUsedIndices);
                    star[slot] = player;
                    starCount++;
                }
                else
                { // if we get here that means a team number was chosen that 
                  // was already full - find then next nonfull team and add the 
                  // player there

                    if(starCount < 15)
                    {
                        slot = GetRandomSlotNumber(random,starUsedIndices);
                        star[slot] = player;
                        starCount++;
                    }
                    else if(moonCount < 15)
                    {
                        slot = GetRandomSlotNumber(random,moonUsedIndices);
                        moon[slot] = player;
                        moonCount++;
                    }
                    else if (sunCount < 15)
                    {
                        slot = GetRandomSlotNumber(random,sunUsedIndices);
                        sun[slot] = player;
                        sunCount++;
                    }
                    else
                    {
                        FillNextEmpty(sun, moon, star, ref sunCount, ref moonCount, ref starCount, player);
                    }
                }
            }

            _ourGuild.GuildTeams.Sun.Players = sun;
            _ourGuild.GuildTeams.Moon.Players = moon;
            _ourGuild.GuildTeams.Star.Players = star;
        }

        private static void FillNextEmpty(Player[] sun, Player[] moon, Player[] star, 
            ref int sunCount, ref int moonCount, ref int starCount, Player player)
        {
            for (int i = 0; i < 15; i++)
            {
                if (sun[i] == null)
                {
                    sun[i] = player;
                    sunCount++;
                }
                else if (moon[i] == null)
                {
                    moon[i] = player;
                    moonCount++;
                }
                else if (star[i] == null)
                {
                    star[i] = player;
                    starCount++;
                }
                else
                {
                    //Debugger.Break();
                }
            }
        }

        private int GetRandomSlotNumber(Random random, List<int> usedIndices)
        {
            int index = random.Next(0, 14);

            while(usedIndices.Count != 15)
            {
                if(!usedIndices.Contains(index))
                {
                    usedIndices.Add(index);
                }
                else if (index < 14)
                {
                    index++;
                }
                else if(index == 14)
                {
                    index = 0;
                }
            }

            return index;
        }

        private void ShuffleByWeight()
        {
            var weights = WeightCalibration.GetWeights();

            var newPool = new List<Player>(
                _ourGuild.Players.OrderByDescending(n =>
                    n.Might));

            var slotsByWeightSun = CreateWeightSlotsDefault(
                weights.SunWeights);

            var slotsByWeightMoon = CreateWeightSlotsDefault(
                weights.MoonWeights);

            var slotsByWeightStar = CreateWeightSlotsDefault(
                weights.StarWeights);

            var sun = new Player[15];
            var moon = new Player[15];
            var star = new Player[15];

            var random = new Random(DateTime.Now.Millisecond);

            int sunIndex = random.Next(0, 14);
            int moonIndex = random.Next(0, 14);
            int starIndex = random.Next(0, 14);

            foreach (var player in newPool)
            {
                var sunHighest = slotsByWeightSun.Skip(sunIndex).FirstOrDefault();
                var moonHighest = slotsByWeightMoon.Skip(moonIndex).FirstOrDefault();
                var starHighest = slotsByWeightStar.Skip(starIndex).FirstOrDefault();

                if (sunIndex < 15 && sunHighest.Value?.Might > moonHighest.Value?.Might
                    && sunHighest.Value?.Might > starHighest.Value?.Might) // sun
                {
                    sun[EnsureFloor(sunHighest.Key)] = player;
                    sunIndex++;
                }
                else if (moonIndex < 15
                    && moonHighest.Value?.Might > starHighest.Value?.Might) // moon
                {
                    moon[EnsureFloor(moonHighest.Key)] = player;
                    moonIndex++;
                }
                else if (starIndex < 15) // star
                {
                    star[EnsureFloor(starHighest.Key)] = player;
                    starIndex++;
                }
                else
                {
                    // need to make sure we are filling up the last of the slots!
                    if (sunIndex < 15)
                    {
                        sun[EnsureFloor(sunHighest.Key)] = player;
                        sunIndex++;
                    }
                    else if (moonIndex < 15)
                    {
                        moon[EnsureFloor(moonHighest.Key)] = player;
                        moonIndex++;
                    }
                    else if (starIndex < 15)
                    {
                        star[EnsureFloor(starHighest.Key)] = player;
                        starIndex++;
                    }
                    else
                    {
                        FillNextEmpty(sun, moon, star, ref sunIndex, ref moonIndex, ref starIndex, player);
                    }
                }
            }

            _ourGuild.GuildTeams.Sun.Players = sun;
            _ourGuild.GuildTeams.Moon.Players = moon;
            _ourGuild.GuildTeams.Star.Players = star;
        }

        private int EnsureFloor(int key)
            => key < 0 
            ? 0
            : key - 1;
        

        private void DefaultSortTeams(List<Player> playerPool, Team teamSun, 
            Team teamMoon, Team teamStar, bool isOrdered = true)
        {
            int sortIndex = 0;
            const int SortMaxIndex = 2;

            var sunPlayers = new List<Player>(15);
            var moonPlayers = new List<Player>(15);
            var starPlayers = new List<Player>(15);

            var playerPoolOrdered = isOrdered 
                ? playerPool 
                : playerPool.OrderByDescending(d => d.Might).ToList();

            foreach (var player in playerPoolOrdered)
            {
                switch (sortIndex)
                {
                    case 0:
                        sunPlayers.Add(player);
                         break;
                    case 1:
                        moonPlayers.Add(player);
                        break;
                    case 2:
                        starPlayers.Add(player);
                        break;
                    default:
                        sortIndex = 0;
                        break;
                }

                if (sortIndex == SortMaxIndex)
                {
                    sortIndex = 0;
                }
                else 
                {
                    sortIndex++;
                }
            }

            teamSun.Players = sunPlayers;
            teamMoon.Players = moonPlayers;
            teamStar.Players = starPlayers;

            _ourGuild.GuildTeams = new GuildTeams(new[] 
                { 
                    teamSun, 
                    teamMoon, 
                    teamStar 
                });
        }

        private IEnumerable<Player> FillTeamDefaults()
        {
            var meanMight = _ourGuild.Players
                .Where(p => p.Might > 0f)
                .Average(p => p.Might);

            if(meanMight < 3)
            {
                floorMight = 1;
            }
            else if(meanMight < 5)
            {
                floorMight = 2;
            }
            else if(meanMight < 7)
            {
                floorMight = 3;
            }
            else
            {
                floorMight = 4;
            }

            var filledPlayersList = new List<Player>(_ourGuild.Players);

            var currentPlayerCount = filledPlayersList.Count;

            Console.WriteLine($"Filling in all missing players with assumed might of {floorMight}");

            for(int i = 0; i < (TotalPlayersAllowed - currentPlayerCount); i++)
            {
                filledPlayersList.Add(new Player($"poopyface{i}", floorMight));
            }

            return filledPlayersList;
        }

        internal void ResetShuffles()
        {
            _shuffleAttempt = 0;
        }
    }
}
