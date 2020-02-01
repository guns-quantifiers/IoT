using Core.Constants;
using Strategies;
using StrategyTests;
using System.Collections.Generic;
using System.IO;
using TestCaseGeneratorConsole.ResultsExport;

namespace TestCaseGeneratorConsole
{
    static class Program
    {
        static void Main(string[] args)
        {
            TestCaseSettings settings = new TestCaseSettings(
                6,
                //new SetCountingStrategyModel()
                //{
                //    Strategy = CountingStrategy.UstonSS,
                //    DeckAmount = 6,
                //    UseTrueCounter = true
                //},
                new SetCountingStrategyModel()
                {
                    Strategy = CountingStrategy.Optimal,
                    DeckAmount = 6,
                    UseTrueCounter = true
                },
                10,
                250,
                10,
                0.75,
                200,
                //new Strategies.BetStrategy.LinearConfiguration() { A = 1, B = -1 },
                //new Strategies.BetStrategy.QuadraticConfiguration() { A = 0.15, B = 0.8, C = -1 },
                new Strategies.BetStrategy.LogisticFunctionConfiguration() { A = 0.01, L = 0.12, K = 1.2, X0 = -0.5 },
                23);
            TestCaseGeneratorV2 generator = new TestCaseGeneratorV2();
            List<PlayerDecision> testResults = generator.Generate(settings);
            ExcelResultExporter exporter = new ExcelResultExporter(Path.Combine(Directory.GetCurrentDirectory(), "ExcelResults"), true);
            exporter.SaveResultsToFile(testResults, settings);
        }
    }
}
