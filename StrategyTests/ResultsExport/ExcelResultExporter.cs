﻿using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

namespace StrategyTests.ResultsExport
{
    public class ExcelResultExporter
    {
        private readonly string _directorPath = "";
        private readonly bool _shouldGenerateDecisionExcel;

        public ExcelResultExporter(string directoryPath, bool shouldGenerateDecisionExcel)
        {
            Directory.CreateDirectory(directoryPath);
            _directorPath = directoryPath;
            _shouldGenerateDecisionExcel = shouldGenerateDecisionExcel;
        }

        public ExcelResultExporter(bool shouldGenerateDecisionExcel)
        {
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
                .SaveAs(Path.Combine(_directorPath, GetSafeFileName()));
        }

        public void SaveSummaryToFile(Summary summary, TestCaseSettings settings)
        {
            XLWorkbook workbook = new XLWorkbook();
            var summaryWorksheet = workbook.Worksheets.Add(GenerateExcelSummary(summary, null, null), "Summary").SetTabColor(XLColor.Amber);
            summaryWorksheet.ColumnWidth = 14;
            var settingsWorksheet = workbook.Worksheets.Add(GetTableForSettings(settings), "Generation settings").SetTabColor(XLColor.Amethyst);
            settingsWorksheet.ColumnWidth = 14;
            workbook.SaveAs(Path.Combine(_directorPath, GetSafeFileName()));
        }

        private XLWorkbook GenerateWorkbookResults(List<PlayerDecision> decisionsHistory, TestCaseSettings settings)
        {
            XLWorkbook workbook = new XLWorkbook();
            double? maxResult = null, minResult = null;
            if (_shouldGenerateDecisionExcel)
            {
                var decisionsWorksheet = workbook.Worksheets.Add(GenerateTableForDecisions(decisionsHistory, out maxResult, out minResult), "Decisions").SetTabColor(XLColor.ArmyGreen);
                decisionsWorksheet.ColumnWidth = 14;
            }
            var summaryWorksheet = workbook.Worksheets.Add(GenerateExcelSummary(new Summary(decisionsHistory), maxResult, minResult), "Summary").SetTabColor(XLColor.Amber);
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
                    decisionsHistory[i].Value.ToString("F2"),
                    decisionsHistory[i].Counter.ToString("F2"),
                    decisionsHistory[i].BetMultiplier.ToString("F2"),
                    moneyResult.ToString("F2")
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

        private DataTable GenerateExcelSummary(Summary summary, double? maxResult, double? minResult)
        {
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

                rowValues.Add(maxResult.Value.ToString("F2"));
                rowValues.Add(minResult.Value.ToString("F2"));
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

        public string GetSafeFileName() =>
            Path.GetInvalidFileNameChars().Aggregate(
                $"Results_{DateTime.Now.ToLongTimeString()}.xlsx",
                (file, invalidChar) => file.Replace(invalidChar, '_'));
    }

    public class Summary
    {
        public Summary(List<PlayerDecision> decisionsHistory)
        {
            InitializeProperties(decisionsHistory);
        }

        public Summary(List<PlayerDecision> decisionsHistory, Summary summaryToMerge)
        {
            InitializeProperties(decisionsHistory);
            NumberOfDeals += summaryToMerge.NumberOfDeals;
            MoneyResult += summaryToMerge.MoneyResult;
            if (MaxCounter < summaryToMerge.MaxCounter)
            {
                MaxCounter = summaryToMerge.MaxCounter;
                BetForMaxCounter = summaryToMerge.BetForMaxCounter;
            }

            if (MinCounter > summaryToMerge.MinCounter)
            {
                MinCounter = summaryToMerge.MinCounter;
                BetForMinCounter = summaryToMerge.BetForMinCounter;
            }

            BiggestWin = Math.Max(BiggestWin, summaryToMerge.BiggestWin);
            BiggestLoss = Math.Max(BiggestLoss, summaryToMerge.BiggestLoss);
        }

        private void InitializeProperties(List<PlayerDecision> decisionsHistory)
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

        public int NumberOfDeals { get; private set; }
        public double MoneyResult { get; private set; }
        public double MaxCounter { get; private set; }
        public double MinCounter { get; private set; }
        public double BetForMaxCounter { get; private set; }
        public double BetForMinCounter { get; private set; }
        public double BiggestWin { get; private set; }
        public double BiggestLoss { get; private set; }
    }
}