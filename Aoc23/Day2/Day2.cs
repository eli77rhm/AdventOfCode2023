using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aoc23
{
    class Game
    {
        public int Id { get; set; }
        public List<Set> Sets { get; set; } = new List<Set>();
    }

    class Set
    {
        public int Red { get; set; }
        public int Green { get; set; }
        public int Blue { get; set; }
    }
    public class Day2 : AllDays
    {
        private int possibleRed = 12;
        private int possibleGreen = 13;
        private int possibleBlue = 14;
        public Day2() : base("Day2")
        {
        }

        public override void ExecutePart1()
        {
            int sum = 0;
            foreach (var line in Lines)
            {
                var game = ParseGame(line);

                _ = IsPossibleGame(game) ? sum += game.Id : sum += 0;

            }

            Console.WriteLine("sum of the IDs of the games: " + sum);
        }

        private bool IsPossibleGame(Game game)
        {
            foreach (var set in game.Sets)
            {
                if (set.Red > possibleRed || set.Green > possibleGreen || set.Blue > possibleBlue)
                {
                    return false;
                }
            }
            return true;
        }

        public override void ExecutePart2()
        {
            uint sum = 0;
            foreach (var line in Lines)
            {
                var game = ParseGame(line);

                sum += CalculatePower(game);

            }

            Console.WriteLine("sum of the IDs of the games: " + sum);
        }

        private uint CalculatePower(Game game)
        {
            var reds = game.Sets.Max(Set => Set.Red);
            var greens = game.Sets.Max(Set => Set.Green);
            var blues = game.Sets.Max(Set => Set.Blue);

            return (uint)(reds * greens * blues);
        }

        private Game ParseGame(string line)
        {
            var parts = line.Split(":");

            int gameId = int.Parse(parts[0].Replace("Game" , " ").Trim());
            var game = new Game { Id = gameId };

            var sets = parts[1].Split(';');
            foreach (var set in sets)
            {
                var cubeParts = set.Trim().Split(',');
                var cubeSet = new Set();

                foreach (var cube in cubeParts)
                {
                    var colorCount = cube.Trim().Split(' ');

                    int count = int.Parse(colorCount[0]);
                    string color = colorCount[1];

                    switch (color)
                    {
                        case "red":
                            cubeSet.Red += count;
                            break;
                        case "green":
                            cubeSet.Green += count;
                            break;
                        case "blue":
                            cubeSet.Blue += count;
                            break;
                    }
                }

                game.Sets.Add(cubeSet);
            }
            return game;
        }
    }
}
