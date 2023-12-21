using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Aoc23
{
    public class Day21 : AllDays
    {
        public Day21() : base("Day21")
        {
        }
        private static char[,] garden;
        private static Dictionary<((int x, int y), int), bool> memo;

        public override void ExecutePart1()
        {
            (int x, int y) start = (0,0);
            garden = new char[Lines.Length, Lines[0].Trim().Length];

            for (int i = 0; i < Lines.Length; i++)
            {
                for (int j = 0; j < Lines[i].Trim().Length; j++)
                {
                    garden[i, j] = Lines[i][j];
                    if (garden[i, j] == 'S')
                        start = (j, i);
                }
            }

            memo = new Dictionary<((int x, int y), int), bool>();
            HashSet<(int x, int y)> reachable = new HashSet<(int x, int y)>();
            Explore(start.x, start.y, 64, reachable);

            Console.WriteLine($"Garden plots reachable in exactly 64 steps: {reachable.Count}");
        }

        private static void Explore(int x, int y, int steps, HashSet<(int x, int y)> reachable)
        {
            int[] Dx = { 0, 0, 1, -1 };
            int[] Dy = { 1, -1, 0, 0 };
            if (steps == 0)
            {
                reachable.Add((x, y));
                return;
            }

            if (memo.ContainsKey(((x, y), steps)))
                return;

            memo[((x, y), steps)] = true;

            for (int i = 0; i < 4; i++)
            {
                int nx = x + Dx[i];
                int ny = y + Dy[i];

                if (IsInGarden(ny, nx) && (garden[ny, nx] == '.' || garden[ny, nx] == 'S'))
                {
                    Explore(nx, ny, steps - 1, reachable);
                }
            }
        }

        private static bool IsInGarden(int y, int x)
        {
            return y >= 0 && y < garden.GetLength(0) && x >= 0 && x < garden.GetLength(1);
        }

        public override void ExecutePart2()
        {
            var start = FindStart(Lines.ToList());
            var steps = 26501365;

            //need 3 sequence
            var sequence = CalculateSequence(Lines, start, steps);

            var coefficients = SolveQuadratic(sequence);

            var grids = steps / Lines.Length;
            Console.WriteLine(Coefficients(grids, coefficients));
        }

        private (int , int) FindStart(List<string> input)
        {
            return Enumerable.Range(0, input.Count)
                .SelectMany(i => Enumerable.Range(0,input.Count)
                    .Where(j => input[i][j] == 'S')
                    .Select(j => (i, j)))
                .Single();
        }

        private List<int> CalculateSequence(string[] input, (int x , int y) start, int targetSteps)
        {
            int[] Dx = { 0, 0, 1, -1 };
            int[] Dy = { 1, -1, 0, 0 };
            var rem = targetSteps % input.Length;
            var sequence = new List<int>();
            var currentPosition = new HashSet<(int, int)> { start };
            var newPosition = new HashSet<(int, int)>();
            var steps = 0;

            for (var n = 0; n < 3; n++)
            {
                while (steps < n * input.Length + rem)
                {
                    foreach ((int x, int y) point in currentPosition)
                    {
                        for (int i = 0; i < 4; i++)
                        {
                            int nx = point.x + Dx[i];
                            int ny = point.y + Dy[i];
                            int wrappedX = ((nx % input.Length) + input.Length) % input.Length;
                            int wrappedY = ((ny % input.Length) + input.Length) % input.Length;

                            if (input[wrappedX][wrappedY] != '#')
                            {
                                newPosition.Add((nx, ny));
                            }
                        }
                    }

                   // currentPosition = new HashSet<(int, int)>(newPosition);
                    currentPosition = new HashSet<(int, int)>();
                    currentPosition.UnionWith(newPosition);
                    newPosition.Clear();
                    steps++;
                }

                sequence.Add(currentPosition.Count);
            }

            return sequence;
        }

        private (long a, long b, long c) SolveQuadratic(List<int> sequence)
        {
            var c = sequence[0];
            var a = ((sequence[2] - c) - (2 * (sequence[1] - c))) / 2;
            var b = (sequence[1] - c) - a;
            return (a, b, c);
        }

        private long Coefficients(long n , (long a, long b, long c) coefficients)
        {
            return coefficients.a * (n * n) + coefficients.b * n + coefficients.c;
        }


    }
}
