using System;
using System.Collections.Generic;

namespace GWTeamCalculator
{
    public class TeamWeightCalibration
    {
        private List<Tuple<int, bool, float>> SunWeights;
        private List<Tuple<int, bool, float>> MoonWeights;
        private List<Tuple<int, bool, float>> StarWeights;

        public TeamWeightCalibration()
        {
            SunWeights = new List<Tuple<int, bool, float>>();
            MoonWeights = new List<Tuple<int, bool, float>>();
            StarWeights = new List<Tuple<int, bool, float>>();
            _totalWeights = new Dictionary<int, float>();
        }

        public TeamWeights GetWeights()
        {
            var calcedWeights = new TeamWeights(
                      CalculateWeights(SunWeights),
                      CalculateWeights(MoonWeights),
                      CalculateWeights(StarWeights)
                  );


            ResetWeights();

            return calcedWeights;
        }

        private void ResetWeights()
        {
            SunWeights = new List<Tuple<int, bool, float>>();
            MoonWeights = new List<Tuple<int, bool, float>>();
            StarWeights = new List<Tuple<int, bool, float>>();
        }

        private readonly Dictionary<int, float> _totalWeights;

        private IDictionary<int, float> CalculateWeights(
            List<Tuple<int, bool, float>> dataSet)
        {
            foreach(var tuple in dataSet)
            {
                int rowNumber = tuple.Item1;
                bool didWin = tuple.Item2;
                float might = tuple.Item3;

                if (!_totalWeights.ContainsKey(tuple.Item1))
                {
                    _totalWeights.Add(rowNumber, 10);
                }

                float currentWeight = _totalWeights[rowNumber];

                if(didWin)
                {
                    currentWeight += (might * 0.01f);
                }
                else
                {
                    currentWeight -= (might * 0.01f);
                }

                _totalWeights[rowNumber] = currentWeight;
            }

            for(int i = 1; i <= 15; i++)
            {
                if (!_totalWeights.ContainsKey(i))
                {
                    // won without being needed
                    _totalWeights.Add(i, 1);
                }
            }

            return _totalWeights;
        }

        public void AddResult(TeamName teamName, int slotNumber, 
            bool didWin, float playerMight)
        {
            switch(teamName)
            {
                case TeamName.Sun:
                    SunWeights.Add(
                        new Tuple<int, bool, float>(slotNumber, didWin, playerMight));
                    break;
                case TeamName.Moon:
                    MoonWeights.Add(
                        new Tuple<int, bool, float>(slotNumber, didWin, playerMight));
                    break;
                case TeamName.Star:
                    StarWeights.Add(
                        new Tuple<int, bool, float>(slotNumber, didWin, playerMight));
                    break;
            }
        }
    }
}
