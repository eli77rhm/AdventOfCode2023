using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aoc23
{
    public class Day8 : AllDays
    {
        public Day8() : base("Day8")
        {
        }

        public override void ExecutePart1()
        {
            string instruction = Lines[0].Trim('\r');
            var nodes = ParseNodes(Lines.Skip(2));

            int steps = StepsToZZZ("AAA", instruction, nodes);
            Console.WriteLine($"Number of steps to ZZZ: {steps}");

        }

        Dictionary<string, (string Left, string Right)> ParseNodes(IEnumerable<string> lines)
        {
            var nodes = new Dictionary<string, (string Left, string Right)>();
            foreach (var line in lines)
            {
                var parts = line.Split(" = ", StringSplitOptions.RemoveEmptyEntries);
                var neighbors = parts[1].Trim('(', ')','\r').Split(", ");
                nodes[parts[0]] = (neighbors[0], neighbors[1]);
            }
            return nodes;
        }

        private int StepsToZZZ(string firstNode, string instuction, Dictionary<string, (string Left, string Right)> nodes)
        {
            int steps = 0;
            string currentNode = firstNode;
            int instuctionIndex = 0;

            while (currentNode != "ZZZ")
            {
                currentNode = instuction[instuctionIndex] == 'L' ? nodes[currentNode].Left : nodes[currentNode].Right;
                steps++;
                instuctionIndex = (instuctionIndex + 1) % instuction.Length;
            }

            return steps;
        }

        public override void ExecutePart2()
        {
            string instruction = Lines[0].Trim('\r');
            var nodes = ParseNodes(Lines.Skip(2));
            var firstNodes = nodes.Where(n => n.Key[2] == 'A');
            var stepslist = new List<long>();
            foreach (var firstNode in firstNodes)
            {
                int steps = StepsToZ(firstNode.Key, instruction, nodes);
                stepslist.Add(steps);
            }
            // https://en.wikipedia.org/wiki/Least_common_multiple
            var result = CalculateLCM(stepslist);
            Console.WriteLine($"Number of steps to Z: {result}");
        }


        private int StepsToZ(string firstNode, string instuction, Dictionary<string, (string Left, string Right)> nodes)
        {
            int steps = 0;
            string currentNode = firstNode;
            int instuctionIndex = 0;

            while (currentNode[2] != 'Z')
            {
                currentNode = instuction[instuctionIndex] == 'L' ? nodes[currentNode].Left : nodes[currentNode].Right;
                steps++;
                instuctionIndex = (instuctionIndex + 1) % instuction.Length;
            }

            return steps;
        }

        private long CalculateLCM(List<long> inputs)
        {
            if (inputs.Count >= 2)
            {
                return LCM(inputs[0], CalculateLCM(inputs.GetRange(1, inputs.Count-1)));
            }

            return inputs[0];
        }

        private long LCM(long a, long b)
        {
            return a * b / GCD(a, b);
        }

        private long GCD(long a, long b)
        {
            while (b != 0)
            {
                var temp = b;
                b = a % b;
                a = temp;
            }
            return a;
        }

    }
}
