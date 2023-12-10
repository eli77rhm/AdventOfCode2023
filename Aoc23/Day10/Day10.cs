using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Aoc23
{
    public class Day10 : AllDays
    {
        public Day10() : base("Day10")
        {
        }

        public override void ExecutePart1()
        {
            var map = Lines.Select(line => line.Trim().ToCharArray()).ToArray();
            var start = FindStart(map);

            var maxDistance = FindMaxDistance(map, start);

            Console.WriteLine($"max distance: {maxDistance}");
        }

        public override void ExecutePart2()
        {
            var map = Lines.Select(line => line.Trim().ToCharArray()).ToArray();
            var start = FindStart(map);

            var loopTiles = FindLoop(map, start);
            var loopMap = CreateLoopMap(map, loopTiles);
            var enclosedCount = Enclosed(loopMap, loopTiles, start);

            Console.WriteLine($"enclosed Count: {enclosedCount}");
        }

        private char[][] CreateLoopMap(char[][] map, HashSet<(int x, int y)> loopTiles)
        {
            var newMap = map.Select(l => l.Select(k => '.').ToArray()).ToArray();
            foreach (var tile in loopTiles)
            {
                newMap[tile.y][tile.x] = map[tile.y][tile.x];
            }
            return newMap;
        }

        private (int x, int y) FindStart(char[][] map)
        {
            for (int y = 0; y < map.Length; y++)
            {
                for (int x = 0; x < map[y].Length; x++)
                {
                    if (map[y][x] == 'S')
                        return (x, y);
                }
            }
            return (-1, -1); 
        }

        private int FindMaxDistance(char[][] grid, (int x, int y) start)
        {
            var visited = new HashSet<(int x, int y)>();
            var queue = new Queue<((int x, int y) position, int distance)>();
            queue.Enqueue((start, 0));
            int maxDistance = 0;

            while (queue.Count > 0)
            {
                var (position, distance) = queue.Dequeue();
                if (visited.Contains(position)) continue;
                visited.Add(position);

                maxDistance = Math.Max(maxDistance, distance);

                foreach (var neighbor in GetNeighbors(grid, position))
                {
                    if (!visited.Contains(neighbor))
                    {
                        queue.Enqueue((neighbor, distance + 1));
                    }
                }
            }

            return maxDistance;
        }

        private IEnumerable<(int x, int y)> GetNeighbors(char[][] grid, (int x, int y) position)
        {
            var directions = new ((int dx, int dy), string dName)[] { ((0, 1), "D"), ((1, 0), "R"), ((0, -1),"U"), ((-1, 0),"L") };
            foreach (var ((dx, dy),dName) in directions)
            {
                int newX = position.x + dx;
                int newY = position.y + dy;
                if (newX >= 0 && newX < grid[0].Length && newY >= 0 && newY < grid.Length && IsPipe(dName, grid[newY][newX]))
                {

                    yield return (newX, newY);
                }
            }
        }

        private bool IsPipe(string dName, char c)
        {
            //S ->: -,J,7
            //S <-:  -,L, F
            //S ^: | ,F,7
            //S v : |, L, J
            switch (dName)
            {
                case "U": return "|F7".Contains(c); break;
                case "R": return "-J7".Contains(c); break;
                case "L": return "-LF".Contains(c); break;
                case "D": return "|LJ".Contains(c); break;
            }
            return false;
        }


        private HashSet<(int x, int y)> FindLoop(char[][] map, (int x, int y) start)
        {
            var visited = new HashSet<(int x, int y)>();
            var queue = new Queue<(int x, int y)>();
            queue.Enqueue(start);

            while (queue.Count > 0)
            {
                var (x, y) = queue.Dequeue();
                if (!visited.Add((x, y))) continue;

                foreach (var next in GetNextPositions(map, x, y))
                {
                    if (!visited.Contains(next) && IsValidPosition(map, next))
                    {
                        queue.Enqueue(next);
                    }
                }
            }

            return visited;
        }

        private IEnumerable<(int x, int y)> GetNextPositions(char[][] map, int x, int y)
        {
            switch (map[y][x])
            {
                case '|': return new[] { (x, y - 1), (x, y + 1) };
                case '-': return new[] { (x - 1, y), (x + 1, y) };
                case 'L': return new[] { (x, y - 1), (x + 1, y) }; 
                case 'J': return new[] { (x, y - 1), (x - 1, y) }; 
                case '7': return new[] { (x, y + 1), (x - 1, y) }; 
                case 'F': return new[] { (x, y + 1), (x + 1, y) }; 
                case 'S': return new[] { (x, y - 1), (x + 1, y), (x - 1, y), (x, y + 1) }; 
                default: return Enumerable.Empty<(int, int)>();
            }
        }

        private bool IsValidPosition(char[][] grid, (int x, int y) position)
        {
            int x = position.x;
            int y = position.y;
            return x >= 0 && x < grid[0].Length && y >= 0 && y < grid.Length && ".|LJ7F-S".Contains(grid[y][x]);
        }

        private int Enclosed(char[][] map, HashSet<(int x, int y)> loopTiles, (int x, int y) start)
        {
            map[start.y][start.x] = '-';
            int enclisedCount = 0;
            for (int i = 0; i < map.Length; i++)
            {
                bool enclosed = false;
                for (int j = 0; j < map[0].Length; j++)
                {
                    char tile = map[i][j];
                    //var maxCol = loopTiles.Where(t => t.x == i).Max(t => t.y);
                    //var minCol = loopTiles.Where(t => t.x == i).Min(t => t.y);
                    if ("|F7".Contains(tile))
                    {
                        enclosed = !enclosed;
                    }
                    else if (tile == '.')
                    {
                        if (enclosed)
                            enclisedCount++;
                    }
                }
            }
            return enclisedCount;
        }


    }
}
