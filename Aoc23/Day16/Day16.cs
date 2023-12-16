using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Aoc23
{
    public class Day16 : AllDays
    {
        public Day16() : base("Day16")
        {
        }

        public override void ExecutePart1()
        {
            var grid = Lines.Select(line => line.Trim().ToCharArray()).ToArray();
            int energizedTiles = CountEnergizedTiles(grid, (-1, 0, '>'));
            Console.WriteLine($"Number of tiles energized: {energizedTiles}");
        }

        public override void ExecutePart2()
        {
            var grid = Lines.Select(line => line.Trim().ToCharArray()).ToArray();
            var energizedTilesList = new List<int>();
            for (int i = 0; i < grid.Length; i++)
            {
                int energizedTiles = 0;
                energizedTiles = CountEnergizedTiles(grid, (-1, i, '>'));
                energizedTilesList.Add(energizedTiles);
                energizedTiles = CountEnergizedTiles(grid, (grid[i].Length, i, '<'));
                energizedTilesList.Add(energizedTiles);
            }
            for (int i = 0; i < grid[0].Length; i++)
            {
                int energizedTiles = 0;
                energizedTiles = CountEnergizedTiles(grid, (i, -1, 'v'));
                energizedTilesList.Add(energizedTiles);
                energizedTiles = CountEnergizedTiles(grid, (i, grid.Length, '^'));
                energizedTilesList.Add(energizedTiles);
            }

            var maxCount = energizedTilesList.Max();
            Console.WriteLine($"Max Number of tiles energized: {maxCount}");
        }

        private int CountEnergizedTiles(char[][] grid, (int x, int y, char direction) startPoint)
        {
            var energized = new HashSet<(int, int)>();
            var visited = new HashSet<(int, int, char)>();
            var beams = new List<(int x, int y, char direction)>() { (startPoint.x, startPoint.y, startPoint.direction) };

            while (beams.Count > 0)
            {
                var newBeams = new List<(int x, int y, char direction)>();
                foreach (var (x, y, direction) in beams)
                {
                    int dx = 0, dy = 0;
                    switch (direction)
                    {
                        case '>': dx = 1; break;
                        case '<': dx = -1; break;
                        case '^': dy = -1; break;
                        case 'v': dy = 1; break;
                    }

                    int nx = x + dx, ny = y + dy;
                    if (nx < 0 || nx >= grid[0].Length || ny < 0 || ny >= grid.Length)
                        continue;

                    if (!visited.Add((nx, ny, direction)))
                        continue;

                    char tile = grid[ny][nx];
                    energized.Add((nx, ny));

                    switch (tile)
                    {
                        case '.':
                            newBeams.Add((nx, ny, direction));
                            break;
                        case '/':
                            newBeams.Add((nx, ny, direction == '>' ? '^' : direction == '<' ? 'v' : direction == '^' ? '>' : '<'));
                            break;
                        case '\\':
                            newBeams.Add((nx, ny, direction == '>' ? 'v' : direction == '<' ? '^' : direction == '^' ? '<' : '>'));
                            break;
                        case '|':
                            if (direction == '>' || direction == '<')
                            {
                                newBeams.Add((nx, ny, '^'));
                                newBeams.Add((nx, ny, 'v'));
                            }
                            else
                            {
                                newBeams.Add((nx, ny, direction));
                            }
                            break;
                        case '-':
                            if (direction == '^' || direction == 'v')
                            {
                                newBeams.Add((nx, ny, '>'));
                                newBeams.Add((nx, ny, '<'));
                            }
                            else
                            {
                                newBeams.Add((nx, ny, direction));
                            }
                            break;
                    }
                }
                beams = newBeams;
            }

            //PrintMap(grid, energized);

            return energized.Count;
        }

        private void PrintMap(char[][] map , HashSet<(int, int)> energized )
        {
            for (int y = 0; y < map.Length; y++)
            {
                for (int x = 0; x < map[y].Length; x++)
                {
                    if(energized.Contains((x,y)))
                            Console.Write("#");
                    else
                        Console.Write(".");
                }
                Console.Write("\n");
            }
        }
    }
}
