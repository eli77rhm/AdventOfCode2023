using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Aoc23
{
    public class Day15 : AllDays
    {
        public Day15() : base("Day15")
        {
        }

        public override void ExecutePart1()
        {
            var steps = Lines[0].Split(',');

            int sumOfResults = steps.Sum(step => HASHAlgorithm(step));
            Console.WriteLine($"Sum of the results: {sumOfResults}");
        }

        private int HASHAlgorithm(string str)
        {
            int currentValue = 0;
            foreach (char c in str)
            {
                currentValue += (int)c;
                currentValue *= 17;
                currentValue %= 256;
            }
            return currentValue;
        }

        public override void ExecutePart2()
        {
            var boxes = new List<Box>();
            bool isAddOperation = false;
            var steps = Lines[0].Split(',');
            foreach (var step in steps)
            {
                var parts = step.Split(new char[] { '=', '-' });
                var label = parts[0];
                var focal = parts[1];

                isAddOperation = (step.Contains("=") ? true : false);


                var boxIndex = HASHAlgorithm(label);
               
                if (boxes.Any(b => b.Index == boxIndex))
                {
                    var box = boxes.Single(b => b.Index == boxIndex);
                    if (box.List.Any(l => l.Item1 == label))
                    {
                        var listIndex = box.List.FindIndex(item => item.Item1 == label);
                        if (isAddOperation)
                        {
                            var focalLength = int.Parse(focal);
                            box.List[listIndex] = (label, focalLength);
                        }
                        else
                        {
                            box.List.RemoveAt(listIndex);
                        }
                       
                    }
                    else
                    {
                        if (isAddOperation)
                        {
                            var focalLength = int.Parse(focal);
                            box.List.Add((label, focalLength));
                        }
                        
                    }
                }
                else
                {
                    if (isAddOperation)
                    {
                        var focalLength = int.Parse(focal);
                        var box = new Box();
                        box.Index = boxIndex;
                        box.List = new List<(string, int)>();
                        box.List.Add((label, focalLength));
                        boxes.Add(box);
                    }

                }
            }

            var score = 0;
            foreach (var box in boxes)
            {
                for (int i = 0; i < box.List.Count; i++)
                {
                    score += ((box.Index + 1) * (i + 1) * box.List[i].Item2);
                }
            }

            Console.WriteLine($"Sum of the results: {score}");
        }

        public class Box
        {
            public int Index { get; set; }
            public List<(string, int)> List { get; set; }
        }


    }
}
