using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Aoc23
{
    public class Day17 : AllDays
    {
        public Day17() : base("Day17")
        {
        }

        public override void ExecutePart1()
        {
            var map = Lines.Select(line => line.Trim().Select(c => int.Parse(c.ToString())).ToArray()).ToArray();
            var minHeatLoss = GetShortestPathDijestra(map,0,3);
            Console.WriteLine("Minimum Heat Loss: " + minHeatLoss);
        }

        public override void ExecutePart2()
        {
            var map = Lines.Select(line => line.Trim().Select(c => int.Parse(c.ToString())).ToArray()).ToArray();
            var minHeatLoss = GetShortestPathDijestra(map, 4, 10);
            Console.WriteLine("Minimum Heat Loss: " + minHeatLoss);

        }

        private int GetShortestPathDijestra(int[][] map, int minStep, int maxStep)
        {
            var rows = map.Length;
            var cols = map[0].Length;

            var visited = new HashSet<((int x, int y), (int x, int y), int)>();
            var queue = new PriorityQueue<(int HeatLoss, (int x, int y) position, (int x, int y) direction, int step), int>();
            queue.Enqueue((0, (0, 0), (1, 0), 0), 0);
            queue.Enqueue((0, (0, 0), (0, 1), 0), 0);

            while (queue.Count > 0)
            {
                var (heatLoss, (x, y), direction, steps) = queue.Dequeue();

                if (x == rows - 1 && y == cols - 1 && steps >= minStep && steps <= maxStep)
                    return heatLoss;

                if (!visited.Add(((x, y), direction, steps)))
                    continue;

                foreach (var next in GetNeighbors(map, (x, y), direction, steps, minStep, maxStep))
                {
                    var newHeatLoss = heatLoss + map[next.y][next.x];
                    queue.Enqueue((newHeatLoss, (next.x, next.y), next.direction, next.step), newHeatLoss);
                }
            }

            return -1;
        }

        private List<(int x, int y, (int x, int y) direction, int step)> GetNeighbors(int[][] map, (int x, int y) position, (int x, int y) preDirection, int step, int minStep, int maxStep)
        {
            var neighbors = new List<(int x, int y, (int x, int y) direction, int step)>();
            if (step < maxStep)
            {
                int newX = position.x + preDirection.x;
                int newY = position.y + preDirection.y;
                if (newX >= 0 && newX < map[0].Length
                              && newY >= 0 && newY < map.Length)
                {
                    neighbors.Add((newX, newY, preDirection, step + 1));
                }

            }
            if (step >= minStep)
            {
                var directions = new List<(int, int)>() { (0, 1), (1, 0), (0, -1), (-1, 0) };
                directions.Remove(preDirection);
                directions.Remove((preDirection.x * -1, preDirection.y * -1));
                foreach (var (dx, dy) in directions)
                {

                    int newX = position.x + dx;
                    int newY = position.y + dy;

                    if (newX >= 0 && newX < map[0].Length
                                  && newY >= 0 && newY < map.Length)
                    {
                        neighbors.Add((newX, newY, (dx, dy), 1));
                    }
                }
            }


            return neighbors;
        }

        private void PrintMap(int[][] map, (int x, int y) direction)
        {
            for (int y = 0; y < map.Length; y++)
            {
                for (int x = 0; x < map[y].Length; x++)
                {
                    if (x == direction.x && y == direction.y)
                        Console.Write("#");
                    else
                        Console.Write(map[x][y]);
                }
                Console.Write("\n");
            }
            Console.Write("\n****************\n");
        }
    }
}
