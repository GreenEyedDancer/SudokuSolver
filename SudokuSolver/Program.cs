using System;
using System.Collections.Generic;
using System.Linq;

namespace SudokuSolver
{
    class Cell
    {
        public int Column { get; set; }
        public int Row { get; set; }
        public string Value { get; set; }
        public int SquareZoneNumber { get; set; }
        public string PossibleResults { get; set; }
    }

    class Board
    {
        public List<Cell> Cells { get; set; }
        public int NumberOfCellChecks { get; set; }

        public Board()
        {
            Cells = new List<Cell>();
            FillCellsWithStartingValues();
            NumberOfCellChecks = 0;
        }

        private void FillCellsWithStartingValues()
        {
            FillLineWithNumbers(0, "100000000");
            FillLineWithNumbers(1, "005086000");
            FillLineWithNumbers(2, "026510090");
            FillLineWithNumbers(3, "003400050");
            FillLineWithNumbers(4, "710000048");
            FillLineWithNumbers(5, "080009300");
            FillLineWithNumbers(6, "070065820");
            FillLineWithNumbers(7, "000240500");
            FillLineWithNumbers(8, "000000007");
        }

        private void FillLineWithNumbers(int row, string numbersLine)
        {
            for (int column = 0; column < 9; column++)
            {
                Cells.Add(new Cell
                {
                    Column = column,
                    Row = row,
                    Value = numbersLine[column].ToString(),
                    SquareZoneNumber = CalculateHorizontalOffset(row) + CalculateVerticalOffset(column),
                    PossibleResults = numbersLine[column] == '0' ? "123456789" : ""
                });
            }
        }

        private int CalculateHorizontalOffset(int row)
        {
            switch(row)
            {
                case 0: case 1: case 2:
                    return 1;
                case 3: case 4: case 5:
                    return 4;
                default:
                    return 7;
            }
        }

        private int CalculateVerticalOffset(int column)
        {
            return column / 3;
        }

        public void ExcludingMethodToSolve()
        {
            foreach(var cell in Cells.Where(c => c.Value == "0"))
                ConsiderPossibleNumbersInCell(cell);
        }

        public void NumberOneByOneMethodToSolve()
        {
            for (int i = 1; i <= 9; i++)
                ConsiderSingleNumber(i.ToString());
        }

        private void ConsiderSingleNumber(string number)
        {
            var cellsWithValueEqualNumber = Cells.Where(c => c.Value == number).ToList();
            for (int squareZoneIndex = 1; squareZoneIndex <= 9; squareZoneIndex++)
            {
                if (cellsWithValueEqualNumber.Any(c => c.SquareZoneNumber == squareZoneIndex))
                    continue;

                var possibleResults = Cells.Where(c => c.SquareZoneNumber == squareZoneIndex
                    && c.Value == "0" && c.PossibleResults.Contains(number)).ToList();
                foreach (var cell in possibleResults)
                {
                    if (cellsWithValueEqualNumber.Any(c => c.Row == cell.Row)
                     || cellsWithValueEqualNumber.Any(c => c.Column == cell.Column))
                    {
                        cell.PossibleResults = cell.PossibleResults.Replace(number, "");
                        NumberOfCellChecks++;
                    }
                }

                if (possibleResults.Where(c => c.PossibleResults.Contains(number)).Count() == 1)
                {
                    var cell = possibleResults.FirstOrDefault(c => c.PossibleResults.Contains(number));
                    cell.Value = number;
                    cell.PossibleResults = string.Empty;
                }
            }
        }

        private void ConsiderPossibleNumbersInCell(Cell cell)
        {
            if (cell.Value == "0")
            {
                NumberOfCellChecks++;
                ConsiderNumbersInRow(cell);
                if (cell.Value == "0")
                {
                    ConsiderNumbersInColumn(cell);
                    if (cell.Value == "0")
                        ConsiderNumberInSquareZone(cell);
                }
            }
        }

        private void ConsiderNumbersInRow(Cell cell)
        {
            foreach (var cellToCheck in Cells.Where(c => c.Column == cell.Column && cell.PossibleResults.Contains(c.Value)))
                ExcludePossibleNumber(cell, cellToCheck.Value);
        }

        private void ConsiderNumbersInColumn(Cell cell)
        {
            foreach (var cellToCheck in Cells.Where(c => c.Row == cell.Row && cell.PossibleResults.Contains(c.Value)))
                ExcludePossibleNumber(cell, cellToCheck.Value);
        }

        private void ConsiderNumberInSquareZone(Cell cell)
        {
            foreach (var cellToCheck in Cells.Where(c => c.SquareZoneNumber == cell.SquareZoneNumber && cell.PossibleResults.Contains(c.Value)))
                ExcludePossibleNumber(cell, cellToCheck.Value);
        }

        private void ExcludePossibleNumber(Cell cell, string numberValue)
        {
            cell.PossibleResults = cell.PossibleResults.Replace(numberValue, "");
            SetValueIfOnePossibleResultLeft(cell);
        }

        private void SetValueIfOnePossibleResultLeft(Cell cell)
        {
            if (cell.PossibleResults.Length == 1)
            {
                cell.Value = cell.PossibleResults;
                cell.PossibleResults = string.Empty;
            }
        }
    }

    class Program
    {
        static void Main()
        {
            var Board = new Board();
            int i = 0;
            while (Board.Cells.Any(c => c.Value == "0"))
            {
                DrawBoard(Board);
                Board.ExcludingMethodToSolve();
                Board.NumberOneByOneMethodToSolve();
                i++;
            }
            Console.WriteLine($"Finished in {i} iterations!");
            Console.WriteLine($"In total of {Board.NumberOfCellChecks} checks");
            DrawBoard(Board);
        }

        public static void DrawBoard(Board Board)
        {
            for (int y = 0; y < 9; y++)
            {
                foreach (var cell in Board.Cells.Where(c => c.Row == y))
                    Console.Write(cell.Value);

                Console.WriteLine();

            }

            Console.ReadLine();
            Console.Clear();
        }
    }
}
