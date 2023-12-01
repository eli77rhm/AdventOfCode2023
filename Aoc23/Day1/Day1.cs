using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Aoc23
{
    public class Day1 : AllDays
    {
        public Day1() : base("Day1")
        {
        }

        public void ExecutePart1()
        {
            int sum = 0;

            foreach (var line in Lines)
            {
                var digits = line.Where(char.IsDigit).ToArray();
                if (digits.Length >= 2)
                {
                    int firstDigit = int.Parse(digits.First().ToString());
                    int lastDigit = int.Parse(digits.Last().ToString());
                    int calibrationValue = firstDigit * 10 + lastDigit;
                    sum += calibrationValue;
                }
                else if (digits.Length == 1)
                {
                    int singleDigit = int.Parse(digits.First().ToString());
                    int calibrationValue = singleDigit * 10 + singleDigit;
                    sum += calibrationValue;
                }
            }

            Console.WriteLine("Total Calibration Value: " + sum);
        }

        public void ExecutePart2()
        {
            int sum = 0;

            foreach (var singleline in Lines)
            {
                var line = Converted(singleline);
                var digits = line.Where(char.IsDigit).ToArray();
                if (digits.Length >= 2)
                {
                    int firstDigit = int.Parse(digits.First().ToString());
                    int lastDigit = int.Parse(digits.Last().ToString());
                    int calibrationValue = firstDigit * 10 + lastDigit;
                    sum += calibrationValue;
                }
                else if (digits.Length == 1)
                {
                    int singleDigit = int.Parse(digits.First().ToString());
                    int calibrationValue = singleDigit * 10 + singleDigit;
                    sum += calibrationValue;
                }
            }

            Console.WriteLine("Total Calibration Value: " + sum);
        }

        string Converted(string str)
        {
            return str
                .Replace("zero", "zero0zero")
                .Replace("one", "one1one")
                .Replace("two", "two2two")
                .Replace("three", "three3three")
                .Replace("four", "four4four")
                .Replace("five", "five5five")
                .Replace("six", "six6six")
                .Replace("seven", "seven7seven")
                .Replace("eight", "eight8eight")
                .Replace("nine", "nine9nine");

        }
    }
}
