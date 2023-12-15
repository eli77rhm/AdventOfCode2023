using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Aoc23
{
    public class Day14 : AllDays
    {
        public Day14() : base("Day14")
        {
        }

        public override void ExecutePart1()
        {
            var columns = RowsToColumns(Lines);

            var sortedNorthColumns = columns.Select(SortNorthParts).ToList();

            int loads = 0;
            for (int i = 0; i < sortedNorthColumns[0].Length; i++)
            {
                var oCount = sortedNorthColumns.Count(col => col[i] == 'O');
                loads += oCount * (sortedNorthColumns[0].Length - i);
            }

            var rows = ColumnsToRows(sortedNorthColumns.ToArray());

            Console.WriteLine($"the total load on the north support beams: {loads}");
        }

        public override void ExecutePart2()
        {
            var loads = CalculateValueForIteration(1000000000);
            Console.WriteLine($"the total load on the north support beams: {loads}");
        }

        private List<string> RowsToColumns(string[] lines)
        {
            var columns = new List<string>();

            for (int col = 0; col < lines[0].Trim().Length; col++)
            {
                var columnChars = new char[lines.Length];
                for (int row = 0; row < lines.Length; row++)
                {
                    columnChars[row] = lines[row][col];
                }
                columns.Add(new string(columnChars));
            }

            return columns;
        }

        private List<string> ColumnsToRows(string[] lines)
        {
            var rows = new List<string>();

            for (int row = 0; row < lines[0].Trim().Length; row++)
            {
                var rowChars = new char[lines.Length];
                for (int col = 0; col < lines.Length; col++)
                {
                    rowChars[col] = lines[col][row];
                }
                rows.Add(new string(rowChars));
            }

            return rows;
        }

        private string SortNorthParts(string column)
        {
            var parts = column.Split('#');
            var sortedParts = new List<string>();
            foreach (var part in parts)
            {
                int oCount = part.Count(c => c == 'O');
                int dotCount = part.Count(c => c == '.');
                sortedParts.Add(new string('O', oCount) + new string('.', dotCount));
            }
            return string.Join("#", sortedParts);
        }

        private string SortSouthParts(string column)
        {
            var parts = column.Split('#');
            var sortedParts = new List<string>();
            foreach (var part in parts)
            {
                int oCount = part.Count(c => c == 'O');
                int dotCount = part.Count(c => c == '.');
                sortedParts.Add(new string('.', dotCount) + new string('O', oCount));
            }
            return string.Join("#", sortedParts);
        }

        private string SortEastParts(string row)
        {
            var parts = row.Split('#');
            var sortedParts = new List<string>();
            foreach (var part in parts)
            {
                int oCount = part.Count(c => c == 'O');
                int dotCount = part.Count(c => c == '.');
                sortedParts.Add(new string('.', dotCount) + new string('O', oCount));
            }
            return string.Join("#", sortedParts);
        }

        private string SortWestParts(string row)
        {
            var parts = row.Split('#');
            var sortedParts = new List<string>();
            foreach (var part in parts)
            {
                int oCount = part.Count(c => c == 'O');
                int dotCount = part.Count(c => c == '.');
                sortedParts.Add(new string('O', oCount) + new string('.', dotCount));
            }
            return string.Join("#", sortedParts);
        }

        private long CalculateValueForIteration(int totalIterations)
        {
            var seenValues = new Dictionary<(int, string) ,int >();
            var values = new Dictionary<int, int>();
            int iteration = 0;
            while (iteration < totalIterations)
            {
                int value = CalculateNextIterationValue(); 
               
                if (seenValues.ContainsKey((value ,LinesToString())))
                {
                    int loopStart = seenValues[(value, LinesToString())];
                    int loopLength = iteration - loopStart;

                    int remainingIterations = totalIterations - iteration;
                    int partialLoopIterations = remainingIterations % loopLength;

                    return values[(loopStart + partialLoopIterations - 1)];
                }
                else
                {
                    seenValues.Add((value, LinesToString()) , iteration);
                    values.Add(iteration, value);
                    iteration++;
                }
            }

            return 0;
        }

        private int CalculateNextIterationValue()
        {
            var Ncolumns = RowsToColumns(Lines);
            var sortedNorthColumns = Ncolumns.Select(SortNorthParts);

            var Wrows = ColumnsToRows(sortedNorthColumns.ToArray());
            var sortedWestRows = Wrows.Select(SortWestParts);

            var Scolumns = RowsToColumns(sortedWestRows.ToArray());
            var sortedSouthColumns = Scolumns.Select(SortSouthParts);

            var Erows = ColumnsToRows(sortedSouthColumns.ToArray());
            var sortedEastRows = Erows.Select(SortEastParts);

            Lines = sortedEastRows.ToArray();

            int loads = 0;
            for (int j = 0; j < Lines.Length; j++)
            {
                var oCount = Lines[j].Count(c => c == 'O');
                loads += oCount * (Lines.Length - j);
            }

            return loads;
        }

        private string LinesToString()
        {
            var str = "";
            foreach (var line in Lines)
            {
                str+=line+"\n";
            }
            return str;
        }
    }
}
