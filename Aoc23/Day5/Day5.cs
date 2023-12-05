using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aoc23
{
    public class Day5 : AllDays
    {
        public Day5() : base("Day5")
        {
        }

        public override void ExecutePart1()
        {
            var seeds = new List<long>();
            var mappingSections = new List<List<(long, long, long)>>(); 
            List<(long, long, long)> currentSection = null;

            foreach (var line in Lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                if (line.StartsWith("seeds:"))
                {
                    seeds = line.Substring(6).Trim().Split(' ').Select(long.Parse).ToList();
                }
                else if (line.Contains("map:"))
                {
                    if (currentSection != null)
                    {
                        mappingSections.Add(currentSection);
                    }
                    currentSection = new List<(long, long, long)>();
                }
                else if (currentSection != null)
                {
                    var parts = line.Split(' ').Select(long.Parse).ToArray();
                    currentSection.Add((parts[0], parts[1], parts[2])); 
                }
            }
            if (currentSection != null)
            {
                mappingSections.Add(currentSection);
            }

            var lowestLocation = seeds.Select(seed => ConvertMapping(seed, mappingSections)).Min();
            Console.WriteLine($"Lowest location number: {lowestLocation}");

        }

        public override void ExecutePart2()
        {
            var seedRanges = new List<(long start, long length)>();
            var mappingSections = new List<List<(long, long, long)>>();
            List<(long, long, long)> currentSection = null;

            foreach (var line in Lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                if (line.StartsWith("seeds:"))
                {
                    var seedParts = line.Substring(6).Trim().Split(' ').Select(long.Parse).ToArray();
                    for (int i = 0; i < seedParts.Length; i += 2)
                    {
                        seedRanges.Add((seedParts[i], seedParts[i + 1]));
                    }
                }
                else if (line.Contains("map:"))
                {
                    if (currentSection != null)
                    {
                        mappingSections.Add(currentSection);
                    }
                    currentSection = new List<(long, long, long)>();
                }
                else if (currentSection != null)
                {
                    var parts = line.Split(' ').Select(long.Parse).ToArray();
                    currentSection.Add((parts[0], parts[1], parts[2]));
                }
            }
            if (currentSection != null)
            {
                mappingSections.Add(currentSection);
            }

            long lowestLocation = long.MaxValue;
            var minRanges = new List<long>();
            foreach (var (start, length) in seedRanges)
            {
                var end = start + length - 1;
                var range = (start, end);
                ConvertMappingPart2(range, mappingSections , mappingSections.Count , minRanges);
            }

            lowestLocation = minRanges.Min();

            Console.WriteLine($"Lowest location number: {lowestLocation}");
        }

        private long ConvertMapping(long number, List<List<(long, long, long)>> mappingSections)
        {
            foreach (var section in mappingSections)
            {
                number = ConvertSection(number, section);
            }
            return number;
        }


        private long ConvertSection(long number, List<(long destinationStart, long sourceStart, long rangeLength)> section)
        {
            foreach (var (destinationStart, sourceStart, rangeLength) in section)
            {
                if (number >= sourceStart && number < sourceStart + rangeLength)
                {
                    return destinationStart + (number - sourceStart);
                }
            }
            return number;
        }


        private void ConvertMappingPart2((long start, long end) range, List<List<(long, long, long)>> sections,
            int level, List<long> minRanges)
        {
            if (level == 0)
            {
                minRanges.Add(range.start);
            }
            else
            {
                bool InRange = false;
                foreach (var (destinationStart, sourceStart, rangeLength) in sections[sections.Count - level])
                {
                    if (range.start <= sourceStart + rangeLength - 1 && range.end >= sourceStart)
                    {
                        var newStart =  destinationStart + Math.Max(0, range.start - sourceStart);
                        var newEnd =  destinationStart + Math.Min(rangeLength - 1, range.end - sourceStart);
                        InRange = true;
                        ConvertMappingPart2((newStart, newEnd) , sections, level-1, minRanges);
                    }
                }
                if(!InRange)
                     ConvertMappingPart2((range.start, range.end), sections, level - 1, minRanges);
            }
            
        }
    }
}
