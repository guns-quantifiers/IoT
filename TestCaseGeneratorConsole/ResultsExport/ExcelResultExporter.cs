using ClosedXML.Excel;
using StrategyTests;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

namespace TestCaseGeneratorConsole.ResultsExport
{
    internal class ExcelResultExporter
    {
        private readonly string _directorPath;
        private readonly bool _shouldGenerateDecisionExcel;

        public ExcelResultExporter(string directoryPath, bool shouldGenerateDecisionExcel)
        {
            Directory.CreateDirectory(directoryPath);
            _directorPath = directoryPath;
            _shouldGenerateDecisionExcel = shouldGenerateDecisionExcel;
        }

        public void SaveResultsToStream(List<PlayerDecision> decisionsHistory, TestCaseSettings settings, Stream targetStream)
        {
            GenerateWorkbookResults(decisionsHistory, settings)
                .SaveAs(targetStream);
        }

        public void SaveResultsToFile(List<PlayerDecision> decisionsHistory, TestCaseSettings settings)
        {
            GenerateWorkbookResults(decisionsHistory, settings)
                .SaveAs(Path.Combine(_directorPath, GetSaveFileName($"Results_{DateTime.Now.ToLongTimeString()}.xlsx")));
        }

        public XLWorkbook GenerateWorkbookResults(List<PlayerDecision> decisionsHistory, TestCaseSettings settings)
        {
            XLWorkbook workbook = new XLWorkbook();
            double? maxResult = null, minResult = null;
            if (_shouldGenerateDecisionExcel)
            {
                var decisionsWorksheet = workbook.Worksheets.Add(GenerateTableForDecisions(decisionsHistory, out maxResult, out minResult), "Decisions").SetTabColor(XLColor.ArmyGreen);
                decisionsWorksheet.ColumnWidth = 14;
            }
            var summaryWorksheet = workbook.Worksheets.Add(GenerateExcelSummary(decisionsHistory, maxResult, minResult), "Summary").SetTabColor(XLColor.Amber);
            summaryWorksheet.ColumnWidth = 14;
            var settingsWorksheet = workbook.Worksheets.Add(GetTableForSettings(settings), "Generation settings").SetTabColor(XLColor.Amethyst);
            settingsWorksheet.ColumnWidth = 14;
            return workbook;
        }

        private DataTable GenerateTableForDecisions(List<PlayerDecision> decisionsHistory, out double? maxResult, out double? minResult)
        {
            DataTable table = new DataTable();
            table.Columns.Add("Decision type");
            table.Columns.Add("Decision impact");
            table.Columns.Add("Counter for bet");
            table.Columns.Add("Bet multiplier for bet");
            table.Columns.Add("Current money result");
            double moneyResult = 0;
            maxResult = 0;
            minResult = 0;
            for (int i = 0; i < decisionsHistory.Count; i++)
            {
                moneyResult += decisionsHistory[i].Value;
                table.Rows.Add(
                    decisionsHistory[i].Type,
                    decisionsHistory[i].Value,
                    decisionsHistory[i].Counter.ToString("F2"),
                    decisionsHistory[i].BetMultiplier.ToString("F2"),
                    moneyResult
                );
                if (maxResult < moneyResult)
                {
                    maxResult = moneyResult;
                }
                else if (minResult > moneyResult)
                {
                    minResult = moneyResult;
                }
            }
            return table;
        }

        private DataTable GenerateExcelSummary(List<PlayerDecision> decisionsHistory, double? maxResult, double? minResult)
        {
            Summary summary = new Summary(decisionsHistory);
            DataTable table = new DataTable();
            table.Columns.Add("Deals played");
            table.Columns.Add("Final money result");
            table.Columns.Add("Maximum counter");
            table.Columns.Add("Minimum counter");
            table.Columns.Add("Bet for max counter");
            table.Columns.Add("Bet for min counter");
            table.Columns.Add("Biggest win");
            table.Columns.Add("Biggest loss");
            var rowValues = new List<object>
            {
                summary.NumberOfDeals.ToString(),
                summary.MoneyResult.ToString("F2"),
                summary.MaxCounter.ToString("F2"),
                summary.MinCounter.ToString("F2"),
                summary.BetForMaxCounter.ToString("F2"),
                summary.BetForMinCounter.ToString("F2"),
                summary.BiggestWin.ToString("F2"),
                summary.BiggestLoss.ToString("F2")
            };

            if (maxResult != null && minResult != null)
            {
                table.Columns.Add("Maximum running money result");
                table.Columns.Add("Minimum running money result");

                rowValues.Add(maxResult);
                rowValues.Add(minResult);
            }

            table.Rows.Add(rowValues.ToArray());
            return table;
        }

        private DataTable GetTableForSettings(TestCaseSettings settings)
        {
            DataTable table = new DataTable();
            table.Columns.Add("Generated games");
            table.Columns.Add("Seed");
            table.Columns.Add("Betting strategy type");
            table.Columns.Add("Betting strategy equation");
            table.Columns.Add("Number of decks");
            table.Columns.Add("Minimum bet");
            table.Columns.Add("Maximum bet");
            table.Columns.Add("Deck penetration");
            table.Columns.Add("Counting strategy type");
            table.Rows.Add(
                settings.GamesToGenerate.ToString(),
                settings.Seed.ToString("F2"),
                settings.CalculatorConfiguration.Type,
                settings.CalculatorConfiguration.Equation,
                settings.NumberOfDecks,
                settings.MinimumBet,
                settings.MaximumBet,
                settings.BasicBet,
                settings.DeckPenetration.ToString("F2")
            );
            return table;
        }

        private string GetSaveFileName(string fileName) =>
            Path.GetInvalidFileNameChars().Aggregate(
                fileName,
                (file, invalidChar) => file.Replace(invalidChar, '_'));
    }

    internal class Summary
    {
        public Summary(List<PlayerDecision> decisionsHistory)
        {
            NumberOfDeals = decisionsHistory.Select(d => d.GameSnapshot.History.Last()).GroupBy(d => d.Id.Value.Increment).Count();
            MoneyResult = decisionsHistory.Sum(d => d.Value);
            MaxCounter = decisionsHistory.Max(d => d.Counter);
            MinCounter = decisionsHistory.Min(d => d.Counter);
            BetForMaxCounter = Math.Abs(decisionsHistory.OrderByDescending(d => d.Counter).FirstOrDefault(c => (c.Value) > 0.01)?.Value ?? 0);
            BetForMinCounter = Math.Abs(decisionsHistory.OrderBy(d => d.Counter).FirstOrDefault(c => Math.Abs(c.Value) > 0.01)?.Value ?? 0);
            BiggestWin = decisionsHistory.Max(d => d.Value);
            BiggestLoss = decisionsHistory.Min(d => d.Value);
        }

        public int NumberOfDeals { get; }
        public double MoneyResult { get; }
        public double MaxCounter { get; }
        public double MinCounter { get; }
        public double BetForMaxCounter { get; }
        public double BetForMinCounter { get; }
        public double BiggestWin { get; }
        public double BiggestLoss { get; }
    }
}
