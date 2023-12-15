using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Aoc23
{
    public class Day13 : AllDays
    {
        Dictionary<int, (int num, bool isRow)> initialMirrors = new();
        public Day13() : base("Day13")
        {
        }

        public override void ExecutePart1()
        {
            var content = string.Join("\n", Lines);
            var patterns = content.Split(new string[] { "\n\r\n" }, StringSplitOptions.RemoveEmptyEntries);

            int summaryValue = 0;

            for (int i = 0; i < patterns.Length; i++)
            {
                var prows = patterns[i].Trim().Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
                var rows = prows.Select(r => r.Trim()).ToArray();
                var result = FindReflections(rows, i);
                if(result != -1)
                    summaryValue += result;

            }

            Console.WriteLine($"Summary value: {summaryValue}");
        }

        private int FindReflections(string[] rows, int patternIndex)
        {
            for (int i = 1; i < rows.Length; i++)
            {
                bool all = true;
                var tuple = rows.Take(i).Reverse().Zip(rows.Skip(i));
                foreach (var x in tuple)
                {
                    if (x.First != x.Second)
                    {
                        all = false;
                        break;
                    }
                }

                if (all)
                {
                    if (initialMirrors.TryGetValue(patternIndex, out (int num, bool isRow) x))
                    {
                        if (x.isRow && x.num == i) continue;
                    }
                    initialMirrors[patternIndex] = (i, true);
                    return i * 100;
                }
            }

            int columnsLength = rows[0].Length;
            var columns = new string[columnsLength];
            for (int i = 0; i < columnsLength; i++)
            {
                StringBuilder sb = new();
                foreach (var row in rows)
                {
                    sb.Append(row[i]);
                }
                columns[i] = sb.ToString();
            }

            for (int j = 1; j < columns.Length; j++)
            {
                bool all = true;
                var tuple = columns.Take(j).Reverse().Zip(columns.Skip(j));
                foreach (var x in tuple)
                {
                    if (x.First != x.Second)
                    {
                        all = false;
                        break;
                    }
                }

                if (all)
                {
                    if (initialMirrors.TryGetValue(patternIndex, out (int num, bool isRow) x))
                    {
                        if (!x.isRow && x.num == j) continue;

                    }
                    initialMirrors[patternIndex] = (j, false);
                    return  j;
                }
            }

            return -1;
        }

        public string[] GetColumns(string[] rows)
        {
            int columnsLength = rows[0].Length;
            var columns = new string[columnsLength];
            for (int i = 0; i < columnsLength; i++)
            {
                StringBuilder sb = new();
                foreach (var row in rows)
                {
                    sb.Append(row[i]);
                }
                columns[i] = sb.ToString();
            }
            return columns;
        }

        public override void ExecutePart2()
        {
            ExecutePart1(); // to fill dictionary with part1 results
            var content = string.Join("\n", Lines);
            var patterns = content.Split(new string[] { "\n\r\n" }, StringSplitOptions.RemoveEmptyEntries);

            int summaryValue = 0;

            for (int i = 0; i < patterns.Length; i++)
            {
                var pattern = patterns[i];
                for (int j = 0; j < pattern.Length; j++)
                {
                    if (pattern[j] != '#' && pattern[j] != '.')
                        continue;

                    StringBuilder newPattern = new StringBuilder(pattern);
                    newPattern[j] = (pattern[j] == '#' ? '.' : '#');


                    var prows = newPattern.ToString().Trim().Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
                    var rows = prows.Select(r => r.Trim()).ToArray();

                    var result = FindReflections(rows, i);
                    if (result != -1)
                    {
                        summaryValue += result;
                        break;
                    }
                    
                }


            }

            Console.WriteLine($"Summary value: {summaryValue}");
        }


    }
}
