using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Aoc23
{
    public class Day22 : AllDays
    {
        public Day22() : base("Day22")
        {
        }


        public override void ExecutePart1()
        {
            var sortedBricks = Parse();
            var SettledBricks = GetSettled(sortedBricks);

            int numberOfDestroyables = 0;
            var removedList = GetBricksWhichRemovedOne(SettledBricks);
            foreach (var brickfromRemovedList in removedList)
            {
                var newSettledBricks = GetSettled(brickfromRemovedList.ToList());
                if (newSettledBricks.All(kv => kv.Value.Z == SettledBricks[kv.Key].Z))
                    numberOfDestroyables++;
            }

            Console.WriteLine(numberOfDestroyables);
        }

        public override void ExecutePart2()
        {
            var sortedBricks = Parse();
            var SettledBricks = GetSettled(sortedBricks);

            int numberOfAffectedBrickes = 0;
            foreach (var brickfromRemovedList in GetBricksWhichRemovedOne(SettledBricks))
            {
                var newSettledBricks = GetSettled(brickfromRemovedList.ToList());
                numberOfAffectedBrickes += newSettledBricks.Count(kv => kv.Value.Z != SettledBricks[kv.Key].Z);
            }

            Console.WriteLine(numberOfAffectedBrickes);
        }


        public List<List<Brick>> GetBricksWhichRemovedOne(Dictionary<int, Brick> bricks)
        {
            List<List<Brick>> result = new List<List<Brick>>();

            foreach (var brick in bricks)
            {
                var newList = bricks.Values.ToList();
                newList.Remove(brick.Value);
                result.Add(newList);
            }

            return result;
        }

        public class Brick
        {
            public int Id { get; set; }
            public HashSet<(int, int, int)> Positions { get; set; }
            public int Z { get; set; }

            public Brick(int id, HashSet<(int, int, int)> positions, int z)
            {
                Id = id;
                Positions = positions;
                Z = z;
            }
        }

        List<Brick> Parse()
        {
            var bricks = new List<Brick>();
            int id = 0;
            foreach (var line in Lines)
            {
                var parts = line.Split('~');
                var start = parts[0].Split(',').Select(int.Parse).ToArray();
                var end = parts[1].Trim().Split("<-")[0].Trim().Split(',').Select(int.Parse).ToArray();
                //var name = parts[1].Trim().Split("<-")[1].Trim();

                var position = new HashSet<(int, int, int)>();
                int Z = int.MaxValue;
                for (int x = Math.Min(start[0], end[0]); x <= Math.Max(start[0], end[0]); x++)
                    for (int y = Math.Min(start[1], end[1]); y <= Math.Max(start[1], end[1]); y++)
                        for (int z = Math.Min(start[2], end[2]); z <= Math.Max(start[2], end[2]); z++)
                        {
                            position.Add((x, y, z));
                            Z = Math.Min(Z, z);
                        }

                bricks.Add(new Brick(id, position, Z));
                id++;
            }
            return bricks.OrderBy(b => b.Z).ToList();
        }

        private Dictionary<int, Brick> GetSettled(List<Brick> bricks)
        {
            var resultBricks = new Dictionary<int, Brick>();
            var occupied = new HashSet<(int, int, int)>();
            foreach (var b in bricks)
            {
                var current = b;

                while (current.Z > 1 && !current.Positions.Any(c => occupied.Contains((c.Item1, c.Item2, c.Item3 - 1))))
                {
                    current = new Brick(
                        current.Id,
                        new HashSet<(int, int, int)>(current.Positions.Select(c => (c.Item1, c.Item2, c.Item3 - 1))),
                        current.Z - 1);

                }

                occupied.UnionWith(current.Positions);
                resultBricks[current.Id] = current;
            }
            return resultBricks;
        }




    }
}
