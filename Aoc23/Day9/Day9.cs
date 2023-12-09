using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aoc23
{
    public class Day9 : AllDays
    {
        public Day9() : base("Day9")
        {
        }

        public override void ExecutePart1()
        {
            var histories = Lines.Select(line => line.Split(' ').Select(int.Parse).ToList()).ToList();

            int sumOfNextValues = histories.Sum(history => FindNextValue(history));

            Console.WriteLine($"Sum of extrapolated values: {sumOfNextValues}");

        }

        public override void ExecutePart2()
        {
            var histories = Lines.Select(line => line.Split(' ').Select(int.Parse).ToList()).ToList();

            int sumOfNextValues = histories.Sum(history => FindFirstValue(history));

            Console.WriteLine($"Sum of extrapolated values: {sumOfNextValues}");
        }

        private int FindNextValue(List<int> history)
        {
            List<List<int>> sequences = new List<List<int>> { new List<int>(history) };

            while (sequences.Last().Distinct().Count() != 1 || sequences.Last().First() != 0)
            {
                List<int> nextSequence = new List<int>();
                for (int i = 0; i < sequences.Last().Count - 1; i++)
                {
                    nextSequence.Add(sequences.Last()[i + 1] - sequences.Last()[i]);
                }
                sequences.Add(nextSequence);
            }

            for (int level = sequences.Count - 2; level >= 0; level--)
            {
                sequences[level].Add(sequences[level].Last() + sequences[level + 1].Last());
            }

            return sequences[0].Last();
        }

        private int FindFirstValue(List<int> history)
        {
            List<List<int>> sequences = new List<List<int>> { new List<int>(history) };

            while (sequences.Last().Distinct().Count() != 1 || sequences.Last().First() != 0)
            {
                List<int> nextSequence = new List<int>();
                for (int i = 0; i < sequences.Last().Count - 1; i++)
                {
                    nextSequence.Add(sequences.Last()[i + 1] - sequences.Last()[i]);
                }
                sequences.Add(nextSequence);
            }

            for (int level = sequences.Count - 1; level > 0; level--)
            {
                sequences[level - 1].Insert(0, sequences[level - 1][0] - sequences[level][0]);
            }

            return sequences[0][0];
        }

    }
}
