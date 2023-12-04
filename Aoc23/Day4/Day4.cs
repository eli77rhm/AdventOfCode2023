using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aoc23
{
    public class Day4 : AllDays
    {
        public Day4() : base("Day4")
        {
        }

        public override void ExecutePart1()
        {
            var cards = Lines.Select(line => ParseCardLine(line)).ToList();

            int totalPoints = cards.Sum(card => CalculateCardPoints(card.Item1, card.Item2));

            Console.WriteLine($"Total points: {totalPoints}");
        }

        public override void ExecutePart2()
        {
            var cards = Lines.Select(line => ParseCardLine(line)).ToList();

            int totalCards = 0;
            for (int i = 0; i < cards.Count; i++)
            {
                ProcessCards(cards, i, 1, ref totalCards);
            }

            Console.WriteLine($"Total scratchcards: {totalCards}");
        }

        public Tuple<List<int>, List<int>> ParseCardLine(string line)
        {
            var parts = line.Split('|');
            var winningNumbers = parts[0].Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                .Skip(2)
                .Select(int.Parse)
                .ToList();
            var myNumbers = parts[1].Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(int.Parse)
                .ToList();

            return Tuple.Create(winningNumbers,myNumbers);
        }

        public int CalculateCardPoints(List<int> winningNumbers, List<int> myNumbers)
        {
            int matches = myNumbers.Count(num => winningNumbers.Contains(num));
            return matches > 0 ? 1 << (matches - 1) : 0; 
        }

        public void ProcessCards(List<Tuple<List<int>, List<int>>> cards, int index, int copies, ref int totalCards)
        {
            totalCards += copies;
            int matches = cards[index].Item2.Count(num => cards[index].Item1.Contains(num));


            for (int i = 1; i <= matches && index + i < cards.Count; i++)
            {
                ProcessCards(cards, index + i, copies, ref totalCards);
            }
        }
    }
}
