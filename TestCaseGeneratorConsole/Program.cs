using Core.Constants;
using Core.Models;
using Strategies.BetStrategy;
using StrategyTests;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TestCaseGeneratorConsole
{
    static class Program
    {
        static void Main(string[] args)
        {
            TestCaseSettings settings = new TestCaseSettings(
                4,
                CountingStrategy.UstonSS,
                5,
                0.66,
                10,
                new LinearConfiguration() { A = 1, B = 1 },
                1);
            TestCaseGenerator generator = new TestCaseGenerator();
            List<PlayerDecision> testResults = generator.Generate(settings);

            //ConsoleWriteResults();

            Console.WriteLine($"====> Number of deals: {testResults.Select(d => d.GameSnapshot.History.Last()).GroupBy(d => d.Id.Value.Increment).Count()}");
            Console.WriteLine($"====> SUM: {testResults.Sum(d => d.Value)}");
            Console.WriteLine($"====> Max counter: {testResults.Max(d => d.Counter)}");
            Console.WriteLine($"====> Bet with max counter: {testResults.OrderBy(d => -d.Counter).FirstOrDefault(c => Math.Abs(c.Value) > 0.01)?.Value}");
            Console.WriteLine();
            Console.WriteLine($"====> Min counter: {testResults.Min(d => d.Counter)}");
            Console.WriteLine($"====> Bet with Min counter: {testResults.OrderBy(d => d.Counter).FirstOrDefault(c => Math.Abs(c.Value) > 0.01)?.Value}");
            Console.WriteLine();
            Console.WriteLine($"====> Biggest win: {testResults.Max(d => d.Value)}");
            Console.WriteLine($"====> Biggest loss: {testResults.Min(d => d.Value)}");

            void ConsoleWriteResults()
            {
                foreach (var playerDecision in testResults)
                {
                    Console.WriteLine($"Move: {playerDecision.Type} Value: {playerDecision.Value} for counter:  {playerDecision.Counter}");
                    Console.WriteLine($"{playerDecision.GameSnapshot.History.LastOrDefault()?.DealConsoleWrite()}");
                    Console.WriteLine($"<-------->{Environment.NewLine}");
                }
            }
        }

        public static string DealConsoleWrite(this Deal deal)
        {
            return $"Deal: {deal.Id.Value.Increment}{Environment.NewLine} +" +
                   $" Ended: {deal.IsEnded}{Environment.NewLine}" +
                   $" Croupier: {string.Join(", ", deal.CroupierHand.Cards)}{Environment.NewLine}" +
                   $" Player: {string.Join(", ", deal.PlayerHand.Cards)}{Environment.NewLine}";
        }
    }
}
