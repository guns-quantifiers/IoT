using Core.Constants;
using Strategies;
using StrategyTests;
using StrategyTests.ResultsExport;
using System;
using System.Collections.Generic;
using System.IO;

namespace TestCaseGeneratorConsole
{
    static class Program
    {
        private const int MaxGamesInBatch = 2000;
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
                    Strategy = CountingStrategy.Knockout,
                    DeckAmount = 6,
                    UseTrueCounter = true
                },
                10,
                350,
                10,
                0.8,
                1,
                new Strategies.BetStrategy.LinearConfiguration() { A = 1, B = -1 },
                //new Strategies.BetStrategy.QuadraticConfiguration() { A = 0.15, B = 0.8, C = -1 },
                //new Strategies.BetStrategy.LogisticFunctionConfiguration() { A = 0.01, L = 0.12, K = 1.2, X0 = -0.5 },
                //new Strategies.BetStrategy.LogisticFunctionConfiguration() { A = 0.02, L = 0.5, K = 0.9, X0 = 0.5 },
                23);
            bool shouldGenerateDecisionsWorksheet = true;
            TestCaseGeneratorV2 generator = new TestCaseGeneratorV2();
            if (settings.GamesToGenerate > MaxGamesInBatch)
            {
                Console.WriteLine($"Games to generate is bigger than {MaxGamesInBatch}, running generation in batch mode...");
                TestCaseSettings limitedSettings = new TestCaseSettings(settings, MaxGamesInBatch);
                int gamesCounter = MaxGamesInBatch;
                Summary summary = new Summary(generator.Generate(limitedSettings));
                while (gamesCounter < settings.GamesToGenerate)
                {
                    summary = new Summary(generator.Generate(limitedSettings), summary);
                    gamesCounter += MaxGamesInBatch;
                    Console.WriteLine($"End of generation batch, current games counter: {gamesCounter}");
                }

                ExcelResultExporter exporter = new ExcelResultExporter(Path.Combine(Directory.GetCurrentDirectory(), "ExcelResults"), false);
                exporter.SaveSummaryToFile(summary, settings);
            }
            else
            {
                List<PlayerDecision> testResults = generator.Generate(settings);
                ExcelResultExporter exporter = new ExcelResultExporter(Path.Combine(Directory.GetCurrentDirectory(), "ExcelResults"), shouldGenerateDecisionsWorksheet);
                exporter.SaveResultsToFile(testResults, settings);
            }
        }
    }
}
