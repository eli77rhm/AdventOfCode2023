using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Aoc23
{
    public class Day18 : AllDays
    {
        public Day18() : base("Day18")
        {
        }

        public Dictionary<string, (int, int)> directions = new Dictionary<string, (int, int)> {
            { "U", (0, -1) },
            { "D", (0, 1) },
            { "L", (-1, 0) },
            { "R", (1, 0) }
        };

        public override void ExecutePart1()
        {
            var input = Lines.Select(line => line.Trim().Split(" ")).ToArray();
            var edges = ColorCubes(input);

            int minX = int.MaxValue, maxX = int.MinValue, minY = int.MaxValue, maxY = int.MinValue;
            foreach (var (x, y) in edges.Keys)
            {
                minX = Math.Min(minX, x);
                maxX = Math.Max(maxX, x);
                minY = Math.Min(minY, y);
                maxY = Math.Max(maxY, y);
            }

            int area = Enclosed(edges, minX, maxX, minY, maxY);
            Console.WriteLine("The lagoon can hold " + (area + edges.Count));
        }

        private Dictionary<(int, int), string> ColorCubes(string[][] input)
        {
            var map = new Dictionary<(int, int), string>();
            (int x, int y) currentPosition = (0, 0);

            for (int i = 0; i < input.Length; i++)
            {
                string direction = input[i][0];
                int distance = int.Parse(input[i][1]);
                var (dx, dy) = directions[direction];

                for (int j = 0; j < distance; j++)
                {
                    currentPosition.x += dx;
                    currentPosition.y += dy;
                    map.Add(currentPosition, input[i][2]);
                }
            }

            return map;
        }

        private int Enclosed(Dictionary<(int, int), string> map, int minX, int maxX, int minY, int maxY)
        {
            bool[,] visited = new bool[maxX - minX + 1, maxY - minY + 1];

            // Flood fill from the edges
            for (int i = minX; i <= maxX; i++)
            {
                FloodFill(map, visited, i, minY, minX, maxX, minY, maxY);
                FloodFill(map, visited, i, maxY, minX, maxX, minY, maxY);
            }
            for (int j = minY; j <= maxY; j++)
            {
                FloodFill(map, visited, minX, j, minX, maxX, minY, maxY);
                FloodFill(map, visited, maxX, j, minX, maxX, minY, maxY);
            }

            // Count the enclosed items
            int enclosedCount = 0;
            for (int i = minX; i <= maxX; i++)
            {
                for (int j = minY; j <= maxY; j++)
                {
                    if (!visited[i - minX, j - minY] && !map.ContainsKey((i, j)))
                    {
                        enclosedCount++;
                    }
                }
            }

            return enclosedCount;
        }

        private void FloodFill(Dictionary<(int, int), string> map, bool[,] visited, int x, int y, int minX, int maxX, int minY, int maxY)
        {
            Queue<(int x, int y)> queue = new Queue<(int x, int y)>();
            queue.Enqueue((x, y));

            while (queue.Count > 0)
            {
                (int currX, int currY) = queue.Dequeue();

                if (currX < minX || currX > maxX || currY < minY || currY > maxY || visited[currX - minX, currY - minY])
                {
                    continue;
                }

                visited[currX - minX, currY - minY] = true;

                if (map.ContainsKey((currX, currY)))
                {
                    continue;
                }

                // Enqueue adjacent cells
                queue.Enqueue((currX + 1, currY));
                queue.Enqueue((currX - 1, currY));
                queue.Enqueue((currX, currY + 1));
                queue.Enqueue((currX, currY - 1));
            }
        }
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        public Dictionary<char, (int, int)> Ndirections = new Dictionary<char, (int, int)> {
            { '3', (0, 1) },
            { '1', (0, -1) },
            { '2', (-1, 0) },
            { '0', (1, 0) }
        };

        public Map _map = new Map();
        public override void ExecutePart2()
        {
            var input = Lines.Select(line => line.Trim().Split(" ")).ToArray();
            EdgePoints(input);
            //var edge = edgeArea();
            var area = polygonArea2();
            Console.WriteLine("The lagoon can hold " + (area));
        }

        private void EdgePoints(string[][] input)
        {
            (int x, int y) currentPosition = (0, 0);
            for (int i = 0; i < input.Length; i++)
            {
                var hex = input[i][2].ToCharArray().Skip(2).SkipLast(1).ToArray();
                char direction = hex[hex.Length - 1];
                var hexstring = new string(hex.SkipLast(1).ToArray());

                int distance = Int32.Parse(hexstring, System.Globalization.NumberStyles.HexNumber);
                var (dx, dy) = Ndirections[direction];

                _map.WallSize += distance;

                currentPosition.x += (distance * dx);
                currentPosition.y += (distance * dy);
                _map.Points.Add(new Points() { X = currentPosition.x, Y = currentPosition.y });
            }

        }

        public long polygonArea()
        {
            long res1 = 0;
            long res2 = 0;
            var pointsCount = _map.Points.Count;
            for (int i = 0; i < pointsCount; i++)
            {
                res1 += (_map.Points[i].X * _map.Points[(i + 1) % pointsCount].Y);
                res2 += (_map.Points[i].Y * _map.Points[(i + 1) % pointsCount].X);
            }

            return (_map.WallSize / 2) + Math.Abs((res1 - res2) / 2) + 1;
        }

        public long polygonArea2()
        {
            long res = 0;

            var shiftedPoints = _map.Points.Skip(1).Concat(_map.Points.Take(1)).ToArray();

            var shoelaces = _map.Points.Zip(shiftedPoints).ToArray();

            var listRes = new List<long>();
            for (int i = 0; i < shoelaces.Length; i++)
            {
                var p1 = shoelaces[i].First;
                var p2 = shoelaces[i].Second;
                res = (p1.X * p2.Y) - (p1.Y * p2.X);
                listRes.Add(res);
            }

            var area = Math.Abs(listRes.Sum()) / 2;
            return (_map.WallSize / 2) + area + 1;

        }

        public class Map
        {
            public Map()
            {
                Points = new List<Points>();
            }
            public List<Points> Points { get; set; }

            public int WallSize { get; set; }
        }

        public class Points
        {
            public long X { get; set; }
            public long Y { get; set; }
        }

    }
}
