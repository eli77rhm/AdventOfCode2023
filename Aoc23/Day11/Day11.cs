using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.VisualBasic.CompilerServices;

namespace Aoc23
{
    public class Day11 : AllDays
    {
        public Day11() : base("Day11")
        {
        }

        public override void ExecutePart1()
        {
            var map = Lines.Select(line => line.Trim().ToCharArray()).ToArray();

            ExpandUniverse(ref map);
            PrintMap(map);
            var galaxyPositions = FindGalaxyPositions(map);
            var shortestPathList = new List<int>();
            int sumOfShortestPaths = 0;

            for (int i = 0; i < galaxyPositions.Count - 1; i++)
            {
                for (int j = i + 1; j < galaxyPositions.Count; j++)
                {
                    var shortestPaths = Math.Abs(galaxyPositions[i].x - galaxyPositions[j].x) +
                                        Math.Abs(galaxyPositions[i].y - galaxyPositions[j].y);
                    //var shortestPaths = GetShortestPathDijestra(map, galaxyPositions[i], galaxyPositions[j], 2);
                    Console.WriteLine($"{i + 1}: {j + 1}: {shortestPaths}");
                    shortestPathList.Add(shortestPaths);
                }
            }

            sumOfShortestPaths = shortestPathList.Sum();
            Console.WriteLine($"Sum of shortest paths: {sumOfShortestPaths}");

        }

        private void PrintMap(char[][] map)
        {
            for (int y = 0; y < map.Length; y++)
            {
                for (int x = 0; x < map[y].Length; x++)
                {
                    Console.Write(map[y][x]);
                }
                Console.Write("\n");
            }
        }

        private void ExpandUniverse(ref char[][] map)
        {

            for (int y = 0; y < map.Length; y++)
            {
                if (map[y].All(c => c == '.'))
                {
                    for (int i = 0; i < map[y].Length; i++)
                    {
                        map[y][i] = 'F';
                    }
                }
            }
            for (int x = 0; x < map[0].Length; x++)
            {
                if (map.All(row => row[x] == '.' || row[x] == 'F'))
                {
                    for (int i = 0; i < map[0].Length; i++)
                    {
                        map[i][x] = 'F';
                    }
                }
            }
        }

        private List<(int x, int y)> FindGalaxyPositions(char[][] map)
        {
            var positions = new List<(int x, int y)>();
            for (int y = 0; y < map.Length; y++)
            {
                for (int x = 0; x < map[y].Length; x++)
                {
                    if (map[y][x] == '#')
                    {
                        positions.Add((x, y));
                    }
                }
            }
            return positions;
        }

        private int GetShortestPathDijestra(char[][] map, (int x, int y) start, (int x, int y) end, int factor)
        {
            var visited = new HashSet<(int x, int y)>();
            var queue = new Queue<((int x, int y) position, int distance)>();
            queue.Enqueue((start, 0));

            while (queue.Count > 0)
            {
                var (current, distance) = queue.Dequeue();
                if (current == end)
                {
                    return distance;
                }

                if (!visited.Add(current))
                    continue;

                foreach (var next in GetNeighbors(map, current))
                {
                    if (!visited.Contains(next))
                    {
                        if (map[next.y][next.x] == 'F')
                            queue.Enqueue((next, distance + factor));
                        else
                        {
                            queue.Enqueue((next, distance + 1));

                        }

                    }
                }
            }

            return -1;
        }

        private IEnumerable<(int x, int y)> GetNeighbors(char[][] map, (int x, int y) position)
        {
            var directions = new[] { (0, 1), (1, 0), (0, -1), (-1, 0) };
            foreach (var (dx, dy) in directions)
            {
                int newX = position.x + dx;
                int newY = position.y + dy;
                if (newX >= 0 && newX < map[0].Length
                              && newY >= 0 && newY < map.Length
                              && (map[newY][newX] == '.'
                                  || map[newY][newX] == '#'
                                  || map[newY][newX] == 'F'))
                {
                    yield return (newX, newY);
                }
            }
        }

        public override void ExecutePart2()
        {
            var map = Lines.Select(line => line.Trim().ToCharArray()).ToArray();
            ExpandUniverse(ref map);
            //PrintMap(map);
            var galaxyPositions = FindGalaxyPositions(map);
            var shortestPathList = new List<long>();

            for (int i = 0; i < galaxyPositions.Count - 1; i++)
            {
                for (int j = i + 1; j < galaxyPositions.Count; j++)
                {
                    var shortestPaths = GetShortestPath(map, galaxyPositions[i], galaxyPositions[j], 1000000);
                    //Console.WriteLine($"{i + 1}: {j + 1}: {shortestPaths}");
                    shortestPathList.Add(shortestPaths);
                }
            }

            long sumOfShortestPaths = shortestPathList.Sum();
            Console.WriteLine($"Sum of shortest paths: {sumOfShortestPaths}");

        }

        private int GetShortestPath(char[][] map, (int x, int y) Start, (int x, int y) End, int factor)
        {
            int distance = 0;
            var xdiff = End.x - Start.x;
            for (int i = Start.x; i != End.x; i+= (xdiff/Math.Abs(xdiff)))
            {
                if (map[Start.y][i] == 'F')
                    distance += factor;
                else
                    distance++;
            }
            var ydiff = End.y - Start.y;
            for (int j = Start.y; j != End.y; j+=(ydiff / Math.Abs(ydiff)))
            {
                if (map[j][End.x] == 'F')
                    distance += factor;
                else
                    distance++;
            }
            return distance;
        }
    }
}
