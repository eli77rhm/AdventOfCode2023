using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Aoc23
{
    public class Number
    {
        public int Value { get; set; }
        public int Length { get; set; }
        public bool Adjacent { get; set; }
    }

    public class Gear
    {
        public int Row { get; set; }
        public int Col { get; set; }
        public List<int> Numbers { get; set; } = new List<int>();

        public int Value()
        {
            return Numbers.Aggregate(1, (acc, val) => acc * val);
        } 
    }

    public class Day3 : AllDays
    {
        public List<Number> Numbers = new List<Number>();
        public List<Gear> Gears = new List<Gear>();
        public Day3() : base("Day3")
        {
        }

        public override void ExecutePart1()
        {
            for (int i = 0; i < Lines.Length; i++)
            {
                for (int j = 0; j < Lines[i].Length; j++)
                {
                    if (char.IsDigit(Lines[i][j]))
                    {
                        var number = CreateNumber(i, j);
                        Numbers.Add(number);

                        j+=number.Length;

                    }
                }

            }
            var sum = Numbers.Where(x=>x.Adjacent).Sum(x => x.Value);
            Console.WriteLine("the sum of all of the part numbers in the engine schematic: " + sum);

        }

        public override void ExecutePart2()
        {
            for (int i = 0; i < Lines.Length; i++)
            {
                for (int j = 0; j < Lines[i].Length; j++)
                {
                    if (char.IsDigit(Lines[i][j]))
                    {
                        var number = CreateNumber3(i, j);
                        Numbers.Add(number);

                        j += number.Length;

                    }
                }

            }

            var sum = Gears.Where(x => x.Numbers.Count == 2).Sum(x => x.Value());
            Console.WriteLine("the sum of all of the part numbers in the engine schematic: " + sum);
        }

        private bool IsAdjacent(int row, int col)
        {
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    int newRow = row + i;
                    int newCol = col + j;

                    if (newRow >= 0 && newRow < Lines.Length && newCol >= 0 && newCol < Lines[newRow].Length)
                    {
                        if (!char.IsDigit(Lines[newRow][newCol]) && Lines[newRow][newCol] != '.' && Lines[newRow][newCol] != '\r')
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        private List<Gear> GetAdjacent(int row, int col)
        {
            var gears = new List<Gear>();
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    int newRow = row + i;
                    int newCol = col + j;

                    if (newRow >= 0 && newRow < Lines.Length && newCol >= 0 && newCol < Lines[newRow].Length)
                    {
                        if (!char.IsDigit(Lines[newRow][newCol]) && Lines[newRow][newCol] != '.' && Lines[newRow][newCol] != '\r')
                        {
                            var gear = new Gear()
                            {
                                Col = newCol,
                                Row = newRow,
                            };
                            gears.Add(gear);
                        }
                    }
                }
            }

            return gears;
        }

        private Number CreateNumber(int i, int j)
        {
            var number = new Number();
            string numberStr = "";
            while (j < Lines[i].Length && char.IsDigit(Lines[i][j]))
            {
                numberStr += Lines[i][j];
                if (IsAdjacent(i, j))
                    number.Adjacent = true;
                j++;
            }

            number.Value = int.Parse(numberStr);
            number.Length = numberStr.Length;
            return number;

        }

        private Number CreateNumber3(int i, int j)
        {
            var number = new Number();
            var gears = new List<Gear>();
            string numberStr = "";
            while (j < Lines[i].Length && char.IsDigit(Lines[i][j]))
            {
                numberStr += Lines[i][j];
                var adjastenceGears = GetAdjacent(i,j);
                if (adjastenceGears != null)
                {
                    number.Adjacent = true;
                    gears.AddRange(adjastenceGears);
                }
                j++;
            }

            number.Value = int.Parse(numberStr);
            number.Length = numberStr.Length;

            foreach (var gear in gears)
            {
                if (!Gears.Exists(x => x.Col == gear.Col && x.Row == gear.Row))
                {
                    gear.Numbers.Add(number.Value);
                    Gears.Add(gear);
                }
                else
                {
                    var g = Gears.Single(x => x.Col == gear.Col && x.Row == gear.Row);
                    if(!g.Numbers.Contains(number.Value))
                        g.Numbers.Add(number.Value);
                }
            }

            return number;
            
        }
    }
}
