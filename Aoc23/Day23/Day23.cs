using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Aoc23
{
    public class Day23 : AllDays
    {
        private Position Start;
        private Position End;
        private List<int> MaxList = new List<int>();
        public Day23() : base("Day23")
        {

        }

        public override void ExecutePart1()
        {
            var map = Parse();
            Start = FindPosition(map, 'S');
            var MaxList = new List<int>();

            var pathLength = LongestPath(map, new HashSet<(int, int)>(), (Start.X, Start.Y), 0, MaxList, false);
            Console.WriteLine($"The longest hike is {pathLength.Item2.Max()} steps long.");

        }

        public override void ExecutePart2()
        {
            var map = Parse();
            Start = FindPosition(map, 'S');
            End = FindPosition(map, 'E');

            var WeightedEdgesListFromAtoB = GetWeightedEdges(map);

            var visited = new HashSet<Position>();
            

            LongestPathPart2(map, visited, Start, WeightedEdgesListFromAtoB, 0);

            Console.WriteLine($"The longest hike is {MaxList.Max()} steps long.");
        }

        public char[,] Parse()
        {
            var rows = Lines.Length;
            var cols = Lines[0].Trim().Length;
            var map = new char[rows, cols];

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    map[i, j] = Lines[i][j];
                }
            }

            return map;
        }

        private Position FindPosition(char[,] map, char character)
        {
            var point = new Position(0, 0);
            for (int y = 0; y < map.GetLength(0); y++)
                for (int x = 0; x < map.GetLength(1); x++)
                    if (map[y, x] == character)
                        point = new Position(x, y);
            return point;
        }

        private (int, List<int>) LongestPath(char[,] map, HashSet<(int, int)> visited, (int x, int y) position, int length, List<int> maxList, bool part2)
        {
            var maxLength = length;

            foreach (Position neighbor in GetNeighbors(position.x, position.y, map, part2))
            {
                if (!visited.Contains((neighbor.X, neighbor.Y)))
                {
                    if (!visited.Add((neighbor.X, neighbor.Y)))
                    {
                        continue;
                    }
                    var (nlength, mlist) = LongestPath(map, new HashSet<(int, int)>(visited), (neighbor.X, neighbor.Y), length + 1, maxList, part2);
                    //Console.WriteLine($"{maxLength}<{nlength}, {visited.Count}, ({neighbor.x},{neighbor.y}):{map[neighbor.y, neighbor.x]}");
                    if (maxLength < nlength && map[neighbor.Y, neighbor.X] == 'E')
                    {
                        maxLength = nlength;
                        maxList.Add(maxLength);

                    }
                    visited.Remove((neighbor.X, neighbor.Y));

                }

            }

            return (maxLength, maxList);
        }


        //private (int, List<int>) LongestPath2(char[,] map, HashSet<Position> visited, Position position, int length, List<int> maxList, bool part2)
        //{
        //    var maxLength = 0;

        //    var stack = new Stack<(int position_x, int position_y, int length, HashSet<Position> visited, List<int> maxList)>();
        //    stack.Push((position.X, position.Y, length, new HashSet<Position>(visited), maxList));

        //    while (stack.Count > 0)
        //    {
        //        var (currentPositionX, currentPositionY, currentLength, currentVisited, currentMaxList) = stack.Pop();

        //        var neighbors = GetNeighbors(currentPositionX, currentPositionY, map, part2);
        //        var stackSize = stack.Count;
        //        foreach (Position neighbor in neighbors)
        //        {
        //            if (!currentVisited.Any(p => p.X == neighbor.X && p.Y == neighbor.Y))
        //            {
        //                // PrintMap(map, currentVisited, new Position(currentPositionX, currentPositionY));
        //                currentVisited.Add(neighbor);
        //                var newLength = currentLength + 1;
        //                //var newMaxList = new List<int>(currentMaxList);

        //                // Console.WriteLine($"{maxLength}<{newLength}, {currentVisited.Count}, ({neighbor.X},{neighbor.Y}):{map[neighbor.Y, neighbor.X]}");
        //                if (map[neighbor.Y, neighbor.X] == 'E' && maxLength < newLength)
        //                {
        //                    maxLength = newLength;
        //                    currentMaxList.Add(maxLength);
        //                }

        //                stack.Push((neighbor.X, neighbor.Y, newLength, new HashSet<Position>(currentVisited), currentMaxList));

        //                //currentVisited.RemoveWhere(p => p.X == neighbor.X && p.Y == neighbor.Y);
        //            }
        //        }

        //        if (stack.Count == stackSize && map[currentPositionX, currentPositionY] != 'E')
        //        {
        //            PrintMap(map, currentVisited, new Position(currentPositionX, currentPositionY));
        //        }
        //    }

        //    return (maxLength, maxList);
        //}


        private List<Position> GetNeighbors(int x, int y, char[,] map, bool part2)
        {
            var neighbors = new List<Position>();
            var directions = new Position[] { new(0, -1), new(1, 0), new(0, 1), new(-1, 0) }; // N, E, S, W
            var slopes = new Dictionary<char, (int, int)> { { '^', (0, -1) }, { '>', (1, 0) }, { 'v', (0, 1) }, { '<', (-1, 0) } };

            if (!part2 && slopes.ContainsKey(map[y, x]))
            {
                var (dx, dy) = slopes[map[y, x]];
                var nx = x + dx;
                var ny = y + dy;
                neighbors.Add(new Position(nx, ny));
            }
            else
            {
                foreach (var dir in directions)
                {
                    var nx = x + dir.X;
                    var ny = y + dir.Y;
                    if (nx >= 0 && nx < map.GetLength(1) && ny >= 0 && ny < map.GetLength(0))
                    {
                        var nposition = map[ny, nx];
                        //if (nposition == '.' || nposition == 'S' || nposition == 'E' || slopes.ContainsKey(nposition))
                        if (nposition != '#')
                        {
                            //if (slopes.ContainsKey(nposition) && slopes[nposition] != (dx, dy)) continue; // Move in slope direction only
                            neighbors.Add(new Position(nx, ny));
                        }
                    }
                }
            }

            return neighbors;

        }

        private void PrintMap(char[,] map, HashSet<Position> visited, Position currentPosition)
        {
            for (int y = 0; y < map.GetLength(0); y++)
            {
                for (int x = 0; x < map.GetLength(1); x++)
                {
                    if (currentPosition.X == x && currentPosition.Y == y)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    if (visited.Any(p => p.X == x && p.Y == y))
                    {
                        Console.Write('O');
                    }
                    else
                    {
                        Console.Write(map[y, x]);
                    }
                }

                Console.Write("\n");
            }
            Console.Write("\n ~~~~~~~~~~~~~~~~ \n");

            // Console.Clear();
        }

        private List<(int, int)> GetMoreThan2Neighbors(char[,] map)
        {
            var moreThan2NeighborsList = new List<(int, int)>();
            for (int y = 0; y < map.GetLength(0); y++)
            {
                for (int x = 0; x < map.GetLength(1); x++)
                {
                    var mneighbors = GetNeighbors(x, y, map, true);
                    if ((map[y, x] != '#' && mneighbors.Count > 2) || map[y, x] == 'S' || map[y, x] == 'E')
                    {
                        moreThan2NeighborsList.Add((x, y));
                    }
                }
            }


            return moreThan2NeighborsList;
        }

        private Dictionary<(int, int), List<((int, int), int)>> GetWeightedEdges(char[,] map)
        {
            var moreThan2NeighborList = GetMoreThan2Neighbors(map);

            Dictionary<(int, int), List<((int, int), int)>> WeightedEdgesListFromAtoB = new Dictionary<(int, int), List<((int, int), int)>>();

            foreach ((int X, int Y) position in moreThan2NeighborList)
            {
                var queue = new Queue<(int, int, int)>();
                WeightedEdgesListFromAtoB[(position.X, position.Y)] = new List<((int, int), int)>();
                queue.Enqueue((position.X, position.Y, 0));

                var visited = new HashSet<(int, int)>();
                while (queue.Count > 0)
                {
                    var (currentX, currentY, weight) = queue.Dequeue();
                    if (visited.Contains((currentX, currentY))) continue;
                    visited.Add((currentX, currentY));


                    if (moreThan2NeighborList.Contains((currentX, currentY)) /*moreThan2NeighborList.Any(p => p.X == currentX && p.Y == currentY)*/
                        && (currentX, currentY) != (position.X, position.Y))
                    {
                        WeightedEdgesListFromAtoB[(position.X, position.Y)].Add(((currentX, currentY), weight));
                        continue;
                    }


                    foreach (var neighbor in GetNeighbors(currentX, currentY, map, true))
                    {
                        queue.Enqueue((neighbor.X, neighbor.Y, weight + 1));
                    }

                }
            }

            return WeightedEdgesListFromAtoB;
        }

        private void LongestPathPart2(char[,] map, HashSet<Position> visited, Position start, Dictionary<(int, int), List<((int, int), int)>> weightedEdges, int length)
        {

            if (visited.Any(p => p.X == start.X && p.Y == start.Y)) return;

            visited.Add(start);

            if (start.Y == End.Y && start.X == End.X)
            {
                MaxList.Add(length);
            }
            foreach (var (nextNode, nextLength) in weightedEdges[(start.X, start.Y)])
            {
                 LongestPathPart2(map, new HashSet<Position>(visited), new Position(nextNode.Item1, nextNode.Item2), weightedEdges, length + nextLength);
            }

            visited.RemoveWhere(p => p.X == start.X && p.Y == start.Y);

        }

        public class Position
        {
            public Position(int x, int y)
            {
                X = x;
                Y = y;
            }

            public int X { get; set; }
            public int Y { get; set; }
        }
    }


}
