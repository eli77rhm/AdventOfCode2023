using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.VisualBasic.CompilerServices;

namespace Aoc23
{
    public class Day12 : AllDays
    {
        public Day12() : base("Day12")
        {
        }

        public override void ExecutePart1()
        {
            long totalArrangements = 0;
            foreach (var line in Lines)
            {
                var parts = line.Split();
                var springConditions = parts[0] + ".";
                var groupSizes = parts[1].Trim().Split(",").Select(int.Parse).ToArray();
                int numberOfGroups = groupSizes.Length;
                groupSizes = groupSizes.Append(springConditions.Length + 1).ToArray();

                totalArrangements += TotalArrangements(springConditions, groupSizes, numberOfGroups);
            }

            Console.WriteLine($"Sum of Arrangements: {totalArrangements}");

        }

        private long TotalArrangements(string springConditions, int[] groupSizes, int numberOfGroups)
        {
            var arrangements = CreateEmptyArrangements(springConditions.Length, numberOfGroups);

            arrangements[0][0][0] = 1;
            for (int index = 0; index < springConditions.Length; index++)
                for (int groupIndex = 0; groupIndex <= numberOfGroups + 1; groupIndex++)
                    for (int groupSize = 0; groupSize <= springConditions.Length + 1; groupSize++)
                    {
                        long currentCount = arrangements[index][groupIndex][groupSize];
                        if (currentCount == 0)
                            continue;

                        if (springConditions[index] == '.' || springConditions[index] == '?')
                            if (groupSize == 0 || (groupIndex > 0 && groupSize == groupSizes[groupIndex - 1]))
                                 arrangements[index + 1][groupIndex][0] += currentCount;

                        if (springConditions[index] == '#' || springConditions[index] == '?')
                            arrangements[index + 1][groupIndex + (groupSize == 0 ? 1 : 0)][groupSize + 1] += currentCount;
                    }

            return arrangements[springConditions.Length][numberOfGroups][0];
        }

        private long[][][] CreateEmptyArrangements(int totalLength, int numberOfGroups)
        {
            long[][][] arrangements = new long[totalLength + 1][][];
            for (int i = 0; i <= totalLength; i++)
            {
                arrangements[i] = new long[numberOfGroups + 2][];
                for (int j = 0; j <= numberOfGroups + 1; j++)
                    arrangements[i][j] = new long[totalLength + 2];
            }

            return arrangements;
        }


        public override void ExecutePart2()
        {
            long totalArrangements = 0;
            foreach (var line in Lines)
            {
                var unfoldedLine = UnfoldRow(line);//only diff with part1

                var parts = unfoldedLine.Split();
                var springConditions = parts[0] + ".";
                var groupSizes = parts[1].Trim().Split(",").Select(int.Parse).ToArray();
                int numberOfGroups = groupSizes.Length;
                groupSizes = groupSizes.Append(springConditions.Length + 1).ToArray();

                totalArrangements += TotalArrangements(springConditions, groupSizes, numberOfGroups);
            }

            Console.WriteLine($"Sum of Arrangements: {totalArrangements}");
        }

        private string UnfoldRow(string row)
        {
            var parts = row.Split();
            string springConditions = parts[0];
            string groupSizes = parts[1];

            string unfoldedSpringConditions = string.Join("?", Enumerable.Repeat(springConditions, 5));
            string unfoldedGroupSizes = string.Join(",", Enumerable.Repeat(groupSizes, 5));

            return $"{unfoldedSpringConditions} {unfoldedGroupSizes}";
        }


    }
}
