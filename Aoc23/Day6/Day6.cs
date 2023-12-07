using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aoc23
{
    public class Day6 : AllDays
    {
        public Day6() : base("Day6")
        {
        }

        public override void ExecutePart1()
        {
            var raceTimes = Lines[0].Substring(5).Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray();
            var recordDistances = Lines[1].Substring(9).Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray();

            long totalWays = 1;
            for (int i = 0; i < raceTimes.Length; i++)
            {
                int waysToWin = CalculateWaysToWin(raceTimes[i], recordDistances[i]);
                totalWays *= waysToWin;
            }

            Console.WriteLine($"Total ways : {totalWays}");

        }

        private int CalculateWaysToWin(int raceTime, int recordDistance)
        {
            int ways = 0;
            for (int holdTime = 0; holdTime < raceTime; holdTime++)
            {
                int speed = holdTime;
                int travelTime = raceTime - holdTime;
                int distance = speed * travelTime;

                if (distance > recordDistance)
                {
                    ways++;
                }
            }
            return ways;
        }

        public override void ExecutePart2()
        {
            long raceTime = long.Parse(Lines[0].Substring(5).Replace(" ", ""));
            long recordDistance = long.Parse(Lines[1].Substring(9).Replace(" ", ""));

            long waysToWin = CalculateWaysToWinPart2(raceTime, recordDistance);

            Console.WriteLine($"Total ways : {waysToWin}");

        }

        private long CalculateWaysToWinPart2(long raceTime, long recordDistance)
        {
            long ways = 0;
            for (long holdTime = 0; holdTime < raceTime; holdTime++)
            {
                long speed = holdTime;
                long travelTime = raceTime - holdTime;
                long distance = speed * travelTime;

                if (distance > recordDistance)
                {
                    ways++;
                }
            }
            return ways;
        }
    }
}
