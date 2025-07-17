using System.Collections;
using System.Text;

namespace SudokuSolver
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.Write(
@"
- Move cursor with arrow keys
- Press NumPad keys to enter numbers
- Press Enter to solve the Sudoku"
            );
            Console.Write("\n\n\n\n");
            Console.WriteLine("Press any key to start...");
            Console.ReadKey(true); // Wait for a key press before clearing the screen
            Console.Clear();

            // Initialize a 9x9 Sudoku board with -1 to represent empty cells
            int[,] board = new int[9, 9];
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    board[i, j] = -1;
                }
            }


            int cx = 0;
            int cy = 0;

            PrintBoard(board);
            PrintCursor(cx, cy);

            while (true)
            {
                ConsoleKeyInfo keyInfo = Console.ReadKey(true);
                if (keyInfo.Key == ConsoleKey.Escape)
                {
                    Environment.Exit(0); // Exit the program
                }
                if (keyInfo.Key == ConsoleKey.DownArrow)
                    cx += 1;
                if (keyInfo.Key == ConsoleKey.UpArrow)
                    cx -= 1;
                if (keyInfo.Key == ConsoleKey.LeftArrow)
                    cy -= 1;
                if (keyInfo.Key == ConsoleKey.RightArrow)
                    cy += 1;
                if (keyInfo.Key == ConsoleKey.Enter) break;

                cx = Math.Clamp(cx, 0, 8);
                cy = Math.Clamp(cy, 0, 8);

                if (new[] {
                    ConsoleKey.DownArrow, ConsoleKey.UpArrow,
                    ConsoleKey.LeftArrow, ConsoleKey.RightArrow
                }.Contains(keyInfo.Key))
                {
                    PrintCursor(cx, cy);
                }


                if (keyInfo.Key == ConsoleKey.NumPad1)
                    board[cx, cy] = 1;
                if (keyInfo.Key == ConsoleKey.NumPad2)
                    board[cx, cy] = 2;
                if (keyInfo.Key == ConsoleKey.NumPad3)
                    board[cx, cy] = 3;
                if (keyInfo.Key == ConsoleKey.NumPad4)
                    board[cx, cy] = 4;
                if (keyInfo.Key == ConsoleKey.NumPad5)
                    board[cx, cy] = 5;
                if (keyInfo.Key == ConsoleKey.NumPad6)
                    board[cx, cy] = 6;
                if (keyInfo.Key == ConsoleKey.NumPad7)
                    board[cx, cy] = 7;
                if (keyInfo.Key == ConsoleKey.NumPad8)
                    board[cx, cy] = 8;
                if (keyInfo.Key == ConsoleKey.NumPad9)
                    board[cx, cy] = 9;




                if (new[] {
                    ConsoleKey.NumPad1, ConsoleKey.NumPad2, ConsoleKey.NumPad3,
                    ConsoleKey.NumPad4, ConsoleKey.NumPad5, ConsoleKey.NumPad6,
                    ConsoleKey.NumPad7, ConsoleKey.NumPad8, ConsoleKey.NumPad9
                    }.Contains(keyInfo.Key))
                {
                    Console.Clear();
                    // Clear the cell if a number is entered
                    PrintBoard(board);

                }


            }

            Console.Clear();
            int k = 20;
            List<int[,]> kSolutions = new List<int[,]>();
            Solver(board, kSolutions, 20);
            int solutionIndex = 0;

            if (kSolutions.Count == 0)
            {
                Console.WriteLine("No solution found.");
            }
            else
            {
                Console.WriteLine($"Found {kSolutions.Count} solutions. Showing the first {k} solutions:");
                Console.WriteLine("Press Right Arrow to see next solution, Left Arrow to see previous solution, Escape to exit.");
                int[,] boardToPrint = kSolutions[solutionIndex];
                Console.SetCursorPosition(0, 5);
                PrintBoard(boardToPrint);
                while (true)
                {
                    ConsoleKeyInfo keyInfo = Console.ReadKey(true);
                    if (keyInfo.Key == ConsoleKey.Escape)
                    {
                        Environment.Exit(0); // Exit the program
                    }
                    if (keyInfo.Key == ConsoleKey.RightArrow)
                    {
                        solutionIndex = (solutionIndex + 1) % kSolutions.Count;
                        Console.SetCursorPosition(0, 5);
                        boardToPrint = kSolutions[solutionIndex];
                        PrintBoard(boardToPrint);
                    }
                    if (keyInfo.Key == ConsoleKey.LeftArrow)
                    {
                        solutionIndex = (solutionIndex - 1 + kSolutions.Count) % kSolutions.Count;
                        Console.SetCursorPosition(0, 5);
                        boardToPrint = kSolutions[solutionIndex];
                        PrintBoard(boardToPrint);
                    }



                }
            }

            Console.ReadKey();

        }

        static bool Solver(int[,] board, List<int[,]> topKRecords, int k = 5)
        {
            if (topKRecords.Count >= k)
            {
                return true; // If we already have k solutions, stop searching
            }
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (board[i, j] == -1)
                    {
                        for (int num = 1; num <= 9; num++)
                        {
                            // Check if the number can be placed in the cell
                            if (!CheckNumExistsInRow(board, num, i) &&
                                !CheckNumExistsInCol(board, num, j) &&
                                !CheckNumExistsInBox(board, num, i, j))
                            {
                                board[i, j] = num;

                                if (Solver(board, topKRecords, k))
                                {
                                    if (topKRecords.Count < k)
                                    {
                                        // If we haven't reached the limit of k solutions, add the current board
                                        int[,] solution = new int[9, 9];
                                        Array.Copy(board, solution, board.Length);
                                        topKRecords.Add(solution);
                                    }
                                    else
                                    {
                                        // If we have k solutions, we can stop searching further
                                        return true;
                                    }
                                }

                                board[i, j] = -1; // Reset the cell if it doesn't lead to a solution
                            }

                        }

                        return false;
                    }
                }
            }


            return true;
        }
        static bool CheckNumExistsInRow(int[,] board, int num, int rowIndex)
        {
            for (int i = 0; i < 9; i++)
            {
                if (board[rowIndex, i] == num)
                {
                    return true;
                }
            }
            return false;
        }
        static bool CheckNumExistsInCol(int[,] board, int num, int colIndex)
        {
            for (int i = 0; i < 9; i++)
            {
                if (board[i, colIndex] == num)
                {
                    return true;
                }
            }
            return false;
        }
        static bool CheckNumExistsInBox(int[,] board, int num, int rowIndex, int colIndex)
        {
            int rowStartIndex = (rowIndex / 3) * 3;
            int colStartIndex = (colIndex / 3) * 3;

            for (int i = rowStartIndex; i < rowStartIndex + 3; i++)
            {
                for (int j = colStartIndex; j < colStartIndex + 3; j++)
                {
                    if (board[i, j] == num)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        static void PrintBoard(int[,] board)
        {

            string template_1 = "+-------------------------------+";
            string template_2 = "<------------------------------->";
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(template_1);
            for (int i = 0; i < 9; i++)
            {
                if (i != 0 && i % 3 == 0)
                {
                    sb.AppendLine(template_2);
                }
                string row = "|";
                for (int j = 0; j < 9; j++)
                {
                    if (j != 0 && j % 3 == 0)
                    {
                        row += "| ";
                    }
                    row += $" {(board[i, j] != -1 ? board[i, j].ToString() : ".")} ";
                }
                sb.AppendLine(row + "|");
            }
            sb.AppendLine(template_1);

            Console.WriteLine(sb.ToString());
        }

        static void PrintCursor(int cx, int cy)
        {
            int x = cx switch
            {
                0 => 1,
                1 => 2,
                2 => 3,
                3 => 5,
                4 => 6,
                5 => 7,
                6 => 9,
                7 => 10,
                8 => 11,
                _ => throw new ArgumentOutOfRangeException(nameof(cx), "Invalid row index")
            };

            int y = cy switch
            {
                0 => 2,
                1 => 5,
                2 => 8,
                3 => 13,
                4 => 16,
                5 => 19,
                6 => 24,
                7 => 27,
                8 => 30,
                _ => throw new ArgumentOutOfRangeException(nameof(cy), "Invalid row index")
            };

            Console.SetCursorPosition(y, x);
        }
    }
}


