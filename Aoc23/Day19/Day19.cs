using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Aoc23
{
    public class Day19 : AllDays
    {
        public Day19() : base("Day19")
        {
        }

        public override void ExecutePart1()
        {
            var workflow = new Workflow();
            workflow.ParseInput(Lines);
            int total = workflow.ProcessParts();
            Console.WriteLine($"Total Sum: {total}");
        }

        public override void ExecutePart2()
        {
            var workflow = new Workflow();
            workflow.ParseInput(Lines);
            long total = workflow.ProcessRange();
            Console.WriteLine($"expect Sum: {167409079868000}");
            Console.WriteLine($"Total  Sum: {total}");
        }

        public class Part
        {
            public int X { get; set; }
            public int M { get; set; }
            public int A { get; set; }
            public int S { get; set; }

            public static Part Parse(string line)
            {
                var part = new Part();
                var matches = Regex.Matches(line, @"([xmas])=(\d+)");
                foreach (Match match in matches)
                {
                    int value = int.Parse(match.Groups[2].Value);
                    switch (match.Groups[1].Value)
                    {
                        case "x": part.X = value; break;
                        case "m": part.M = value; break;
                        case "a": part.A = value; break;
                        case "s": part.S = value; break;
                    }
                }
                return part;
            }

            public int GetValue(string part)
            {
                return part switch
                {
                    "x" => X,
                    "m" => M,
                    "a" => A,
                    "s" => S,
                };
            }

            public int SumValues()
            {
                return X + M + A + S;
            }
        }

        public class PartRange
        {
            public long minX { get; set; }
            public long minM { get; set; }
            public long minA { get; set; }
            public long minS { get; set; }
            public long maxX { get; set; }
            public long maxM { get; set; }
            public long maxA { get; set; }
            public long maxS { get; set; }

            public PartRange()
            {
                minX = minA = minM = minS = 1;
                maxX = maxA = maxM = maxS = 4000;
            }

            public PartRange DeepCopy()
            {
                var copy = new PartRange();
                copy.minA = this.minA;
                copy.minX = this.minX;
                copy.minS = this.minS;
                copy.minM = this.minM;
                copy.maxX = this.maxX;
                copy.maxM = this.maxM;
                copy.maxA = this.maxA;
                copy.maxS = this.maxS;

                return copy;
            }

            public void UpdateRange(string condition)
            {
                var match = Regex.Match(condition, @"([xmas])\s*([<>=])\s*(\d+)");
                var attribute = match.Groups[1].Value;
                var operation = match.Groups[2].Value;
                var threshold = int.Parse(match.Groups[3].Value);

                switch (attribute)
                {
                    case "x":
                        {
                            switch (operation)
                            {
                                case ">": minX = (minX < threshold ? threshold +1 : minX); break;
                                case "<": maxX = (maxX > threshold ? threshold -1 : maxX); break;
                                case "=": minX = maxX = threshold; break;
                            }
                            break;
                        }
                    case "m":
                        {
                            switch (operation)
                            {
                                case ">": minM = (minM < threshold ? threshold +1 : minM); break;
                                case "<": maxM = (maxM > threshold ? threshold -1 : maxM); break;
                                case "=": minM = maxM = threshold; break;
                            }
                            break;
                        }
                    case "a":
                        {
                            switch (operation)
                            {
                                case ">": minA = (minA < threshold ? threshold +1 : minA); break;
                                case "<": maxA = (maxA > threshold ? threshold -1 : maxA); break;
                                case "=": minA = maxA = threshold; break;
                            }
                            break;
                        }
                    case "s":
                        {
                            switch (operation)
                            {
                                case ">": minS = (minS < threshold ? threshold +1 : minS); break;
                                case "<": maxS = (maxS > threshold ? threshold -1 : maxS); break;
                                case "=": minS = maxS = threshold; break;
                            }
                            break;
                        }
                }

            }

            public void UpdateRangeReverse(string condition)
            {
                var match = Regex.Match(condition, @"([xmas])\s*([<>=])\s*(\d+)");
                var attribute = match.Groups[1].Value;
                var operation = match.Groups[2].Value;
                var threshold = int.Parse(match.Groups[3].Value);

                switch (attribute)
                {
                    case "x":
                        {
                            switch (operation)
                            {
                                case ">": maxX = (maxX > threshold ? threshold  : maxX ); break;
                                case "<": minX = (minX < threshold ? threshold  : minX); break;
                                case "=": minX = maxX = threshold; break;
                            }
                            break;
                        }
                    case "m":
                        {
                            switch (operation)
                            {
                                case ">": maxM = (maxM > threshold ? threshold  : maxM ); break;
                                case "<": minM = (minM < threshold ? threshold  : minM); break;
                                case "=": minM = maxM = threshold; break;
                            }
                            break;
                        }
                    case "a":
                        {
                            switch (operation)
                            {
                                case ">": maxA = (maxA > threshold ? threshold  : maxA); break;
                                case "<": minA = (minA < threshold ? threshold : minA); break;
                                case "=": minA = maxA = threshold; break;
                            }
                            break;
                        }
                    case "s":
                        {
                            switch (operation)
                            {
                                case ">": maxS = (maxS > threshold ? threshold  : maxS); break;
                                case "<": minS = (minS < threshold ? threshold  : minS); break;
                                case "=": minS = maxS = threshold; break;
                            }
                            break;
                        }
                }

            }

            public long Combinations()
            {
                return (maxX - minX + 1) *
                       (maxS - minS + 1) *
                       (maxM - minM + 1) *
                       (maxA - minA + 1);
            }

            public string Print()
            {
                return $"x:({minX},{maxX}),m:({minM},{maxM}) ,a:({minA},{maxA}) ,s:({minS},{maxS})  , com:{Combinations()}";
            }

        }

        public class Workflow
        {
            private Dictionary<string, List<Func<Part, string>>> _workflows;
            private Dictionary<string, List<Func<PartRange, string>>> _rangeWorkflows;
            private List<Part> _parts;

            public Workflow()
            {
                _workflows = new Dictionary<string, List<Func<Part, string>>>();
                _rangeWorkflows = new Dictionary<string, List<Func<PartRange, string>>>();
                _parts = new List<Part>();
            }

            public void ParseInput(string[] lines)
            {
                bool parsingWorkflows = true;

                foreach (var line in lines)
                {
                    if (string.IsNullOrWhiteSpace(line.Trim()))
                    {
                        parsingWorkflows = false;
                        continue;
                    }

                    if (parsingWorkflows)
                    {
                        ParseWorkflow(line.Trim());
                    }
                    else
                    {
                        _parts.Add(Part.Parse(line.Trim()));
                    }
                }
            }

            private void ParseWorkflow(string line)
            {
                var parts = line.Split('{');
                var workflowName = parts[0];
                var rules = parts[1].TrimEnd('}').Split(',');

                var workflowRules = new List<Func<Part, string>>();
                var workflowRanges = new List<Func<PartRange, string>>();

                var conditionPartsList = new List<string>();
                bool isEmptyCondition = false;
                string lastDestination = "";
                for(int i = 0; i < rules.Length; i++)
                {
                    var rule = rules[i];
                    if (rule == "A" || rule == "R")
                    {
                        workflowRules.Add(part => rule);
                        isEmptyCondition = true;
                        lastDestination = rule;
                        continue;
                    }

                    var conditionParts = rule.Split(':');
                    if (conditionParts.Length > 1)
                    {
                        var condition = ParseCondition(conditionParts[0]);
                        var destination = conditionParts[1];
                        workflowRules.Add(part => condition(part) ? destination : null);
                        workflowRanges.Add(partRange =>
                        {
                            partRange.UpdateRange(conditionParts[0]);
                            foreach (var condition in conditionPartsList)
                            {
                                partRange.UpdateRangeReverse(condition);
                            }
                            conditionPartsList.Add(conditionParts[0]);
                            return destination;
                        });
                        
                    }
                    else
                    {
                        var destination = conditionParts[0];
                        workflowRules.Add(parts => destination);
                        isEmptyCondition = true;
                        lastDestination = destination;
                        //workflowRanges.Add(partRange => destination);
                    }

                }

                if (isEmptyCondition)
                {
                    workflowRanges.Add(partRange =>
                    {
                        foreach (var condition in conditionPartsList)
                        {
                            partRange.UpdateRangeReverse(condition);
                        }

                        return lastDestination;
                    });
                }

                _workflows[workflowName] = workflowRules;
                _rangeWorkflows[workflowName] = workflowRanges;
            }

            private Func<Part, bool> ParseCondition(string condition)
            {
                var match = Regex.Match(condition, @"([xmas])\s*([<>=])\s*(\d+)");
                var attribute = match.Groups[1].Value;
                var operation = match.Groups[2].Value;
                var threshold = int.Parse(match.Groups[3].Value);

                return part =>
                {
                    int value = part.GetValue(attribute);
                    return operation switch
                    {
                        ">" => value > threshold,
                        "<" => value < threshold,
                        "=" => value == threshold,
                    };
                };
            }

            public int ProcessParts()
            {
                return _parts.Sum(part => ProcessPart(part, "in") ? part.SumValues() : 0);
            }

            private bool ProcessPart(Part part, string workflowName)
            {
                foreach (var rule in _workflows[workflowName])
                {
                    var result = rule(part);
                    if (result != null)
                    {
                        if (result == "A") return true;
                        if (result == "R") return false;
                        return ProcessPart(part, result);
                    }
                }

                return false;
            }

            public long ProcessRange()
            {
                var range = new PartRange();
                var results = new List<PartRange>();
                ProcessRange(range, "in", results);
                return results.Sum(r => Math.Abs(r.Combinations()));
            }

            private void ProcessRange(PartRange partRange, string workflowName, List<PartRange> results)
            {
                foreach (var rule in _rangeWorkflows[workflowName])
                {
                    var range = partRange.DeepCopy();
                    var result = rule(range);
                    Console.WriteLine(result);
                    if (result != null)
                    {
                        if (result == "A")
                        {
                            results.Add(range);
                            Console.WriteLine($"# , {range.Print()}");
                            continue;
                        }
                        if (result == "R") continue;
                        ProcessRange(range, result, results);
                    }
                }

                return;
            }
        }

    }
}
