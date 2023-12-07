using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Aoc23
{
    public class Day7 : AllDays
    {
        public Day7() : base("Day7")
        {
        }

        public override void ExecutePart1()
        {

            var hands = Parse();

            var rankedHands = hands.OrderByDescending(h => h.Score).ToList();
            rankedHands.Sort((a, b) => CompareRankedHands(a, b, "23456789TJQKA"));

            long totalWinnings = 0;
            for (int i = 0; i < rankedHands.Count; i++)
            {
                var score = (long)rankedHands[i].Bid * (rankedHands.Count - i);
                totalWinnings += score;
                Console.WriteLine($"{rankedHands[i].SortedHand}, {score}");

            }

            Console.WriteLine($"Total winnings: {totalWinnings}");
        }

        private List<Card> Parse()
        {
            var list = new List<Card>();
            foreach (var line in Lines)
            {
                var card = new Card();
                var parts = line.Split(' ');
                card.Hand = parts[0];
                card.Bid = int.Parse(parts[1]);
                card.SortedHand = SortedHand(card.Hand);
                card = GetRank(card);
                list.Add(card);
            }
            return list;

        }

        private Card GetRank(Card card)
        {
            foreach (char hand in card.Hand)
            {
                if (!card.HandDetails.ContainsKey(hand))
                    card.HandDetails[hand] = 1;
                card.HandDetails[hand] *= 3;
            }

            card.Score = card.HandDetails.Where(x => x.Value > 1).Sum(x => x.Value);
            return card;
        }

        private string SortedHand(string hand)
        {
            string order = "AKQJT98765432";
            var sortedHand = string.Concat(hand.OrderBy(c => c));
            var groupedHand = sortedHand.GroupBy(c => c)
                .OrderByDescending(g => g.Count())
                .ThenBy(g => order.IndexOf(g.Key))
                .ToArray();

            var finalSortedHand = "";
            foreach (var group in groupedHand)
            {
                finalSortedHand += new string(group.Key, group.Count());
            }

            return finalSortedHand;
        }

        private int CompareRankedHands(Card a, Card b, string order)
        {
            int rankComparison = a.Score.CompareTo(b.Score);
            if (rankComparison != 0)
            {
                return rankComparison * -1;
            }

            return CompareHands(a.Hand, b.Hand, order);
        }

        private int CompareHands(string hand1, string hand2, string order)
        {
            for (int i = 0; i < hand1.Length; i++)
            {
                int index1 = order.IndexOf(hand1[i]);
                int index2 = order.IndexOf(hand2[i]);

                if (index1 != index2)
                {
                    return index2.CompareTo(index1);
                }
            }

            return hand2.Length.CompareTo(hand1.Length);
        }

        public override void ExecutePart2()
        {
            var hands = ParsePart2();

            var rankedHands = hands.OrderByDescending(h => h.Score).ToList();
            rankedHands.Sort((a, b) => CompareRankedHands(a, b, "J23456789TQKA"));

            long totalWinnings = 0;
            for (int i = 0; i < rankedHands.Count; i++)
            {
                var score = (long)rankedHands[i].Bid * (rankedHands.Count - i);
                totalWinnings += score;
                Console.WriteLine($"{rankedHands[i].SortedHand}, {rankedHands[i].Hand} , {rankedHands[i].Score}");

            }

            Console.WriteLine($"Total winnings: {totalWinnings}");
        }

        private List<Card> ParsePart2()
        {
            var list = new List<Card>();
            foreach (var line in Lines)
            {
                var card = new Card();
                var parts = line.Split(' ');
                card.Hand = parts[0];
                card.Bid = int.Parse(parts[1]);
                card.ModifiedHand = parts[0];
                card.GetRankPart2();
                card.SortedHand = SortedHandPart2(card.ModifiedHand);
                list.Add(card);
            }
            return list;

        }

        private string SortedHandPart2(string hand)
        {
            string order = "AKQT98765432J";
            var sortedHand = string.Concat(hand.OrderBy(c => c));
            var groupedHand = sortedHand.GroupBy(c => c)
                .OrderByDescending(g => g.Count())
                .ThenBy(g => order.IndexOf(g.Key))
                .ToArray();

            var finalSortedHand = "";
            foreach (var group in groupedHand)
            {
                finalSortedHand += new string(group.Key, group.Count());
            }

            return finalSortedHand;
        }

        public class Card
        {
            public string Hand { get; set; }
            public string SortedHand { get; set; }
            public string ModifiedHand { get; set; }
            public int Bid { get; set; }
            public int Score { get; set; }
            public Dictionary<char, int> HandDetails = new Dictionary<char, int>();

            public void GetRankPart2()
            {
                foreach (char hand in this.Hand)
                {
                    if (!this.HandDetails.ContainsKey(hand))
                        this.HandDetails[hand] = 1;
                    else
                        this.HandDetails[hand] *= 3;
                }
                string order = "AKQT98765432J";
                var sorted = this.HandDetails
                    .OrderByDescending(x => x.Value)
                    .ThenBy(x => order.IndexOf(x.Key))
                    .ToList();
                if (this.HandDetails.ContainsKey('J'))
                {
                    
                    if (sorted.Count() > 1)
                    {
                        var Jvalue = this.HandDetails['J'];
                        this.HandDetails.Remove('J');
                        if(sorted[0].Key == 'J')
                            sorted.RemoveAt(0);
                        var key = sorted[0].Key;
                        var value = sorted[0].Value;
                        this.HandDetails[key] = value * 3 * Jvalue;
                        this.ModifiedHand = this.ModifiedHand.Replace("J", key.ToString());
                    }
                }


                this.Score = this.HandDetails.Where(x => x.Value > 1).Sum(x => x.Value);
            }
        }
    }
}
