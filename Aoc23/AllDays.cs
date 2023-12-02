using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aoc23
{
    public abstract class AllDays
    {
        public string[] Lines;
        public AllDays(string path)
        {
            using (StreamReader sr = new StreamReader($"{path}/input.txt"))
            {
                Lines = sr.ReadToEnd().Split("\n");
            }
        }

        public abstract void ExecutePart1();
        public abstract void ExecutePart2();
    }
}
