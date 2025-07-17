using System.Drawing;
using Microsoft.Extensions.DependencyInjection;
class GamePanel : BasePanel
{
    private Board? board = null;

    private AppContext appContext = null!;

    private Drawer drawer = null!;

    private Task? keyboardTask = null;

    private int c_col = 3;
    private int c_row = 3;

    private MinimaxEval minimaxEval;


    private MCTSeval mctsEval;

    private RandomMoveGenerator randomMoveGenerator;

    public GamePanel()
    {
        int depth = 3;
        this.minimaxEval = new MinimaxEval(depth);
        this.mctsEval = new MCTSeval();
        this.randomMoveGenerator = new RandomMoveGenerator();
    }

    private CancellationTokenSource? keyboardTaskCts;

    private Piece selectedPiece = null!;
    public override void OnStartUp()
    {

        this.appContext = ServiceLocator.Instance.GetService<AppContext>()!;
        this.drawer = ServiceLocator.Instance.GetService<Drawer>()!;

        if (appContext.currentGameMode == GameMode.SinglePlayer)
        {
            appContext.CurrentPlayerColor = PieceColor.White;
            appContext.IsPlayerTurn = true;
        }

        drawer.Clear();
        // Logic to handle when the game panel starts up
        this.InitializeBoard();
        this.DrawBoard();
        drawer.SetCursorPosition(c_col, c_row);

        keyboardTaskCts = new CancellationTokenSource();

        keyboardTask = Task.Run(() => this.KeyboardController(keyboardTaskCts.Token));


        keyboardTask.Wait();
    }
    public override void OnClose()
    {
        // Logic to handle when the game panel closes
        keyboardTaskCts?.Cancel();
        keyboardTask?.Wait();
        keyboardTaskCts?.Dispose();
        keyboardTask?.Dispose();
    }
    public void StartGame()
    {
        // Logic to start the game
        Console.WriteLine("Game Started");
    }

    private void InitializeBoard()
    {
        board = new Board(8, 8);
        board.Pieces = new Piece[8, 8];

        // Place pawns
        for (int col = 0; col < 8; col++)
        {
            board.Pieces[1, col] = new Piece(PieceColor.Black, PieceType.Pawn, 1, col);
            board.Pieces[6, col] = new Piece(PieceColor.White, PieceType.Pawn, 6, col);
        }

        // Place Rooks
        board.Pieces[0, 0] = new Piece(PieceColor.Black, PieceType.Rook, 0, 0);
        board.Pieces[0, 7] = new Piece(PieceColor.Black, PieceType.Rook, 0, 7);
        board.Pieces[7, 0] = new Piece(PieceColor.White, PieceType.Rook, 7, 0);
        board.Pieces[7, 7] = new Piece(PieceColor.White, PieceType.Rook, 7, 7);

        // Place Knights
        board.Pieces[0, 1] = new Piece(PieceColor.Black, PieceType.Knight, 0, 1);
        board.Pieces[0, 6] = new Piece(PieceColor.Black, PieceType.Knight, 0, 6);
        board.Pieces[7, 1] = new Piece(PieceColor.White, PieceType.Knight, 7, 1);
        board.Pieces[7, 6] = new Piece(PieceColor.White, PieceType.Knight, 7, 6);

        // Place Bishops
        board.Pieces[0, 2] = new Piece(PieceColor.Black, PieceType.Bishop, 0, 2);
        board.Pieces[0, 5] = new Piece(PieceColor.Black, PieceType.Bishop, 0, 5);
        board.Pieces[7, 2] = new Piece(PieceColor.White, PieceType.Bishop, 7, 2);
        board.Pieces[7, 5] = new Piece(PieceColor.White, PieceType.Bishop, 7, 5);

        // Place Queens
        board.Pieces[0, 3] = new Piece(PieceColor.Black, PieceType.Queen, 0, 3);
        board.Pieces[7, 3] = new Piece(PieceColor.White, PieceType.Queen, 7, 3);

        // Place Kings
        board.Pieces[0, 4] = new Piece(PieceColor.Black, PieceType.King, 0, 4);
        board.Pieces[7, 4] = new Piece(PieceColor.White, PieceType.King, 7, 4);
    }

    // keyboard related
    public void KeyboardController(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            if (Console.KeyAvailable)
            {
                // Read one key
                ConsoleKeyInfo keyInfo = Console.ReadKey(true);

                // Handle the key
                switch (keyInfo.Key)
                {
                    case ConsoleKey.UpArrow:
                        c_row = Math.Max(3, c_row - 1);
                        break;
                    case ConsoleKey.DownArrow:
                        c_row = Math.Min(10, c_row + 1);
                        break;
                    case ConsoleKey.LeftArrow:
                        c_col = Math.Max(3, c_col - 2);
                        break;
                    case ConsoleKey.RightArrow:
                        c_col = Math.Min(17, c_col + 2);
                        break;

                    case ConsoleKey.Enter:
                        HandlePressEnter();
                        break;
                }

                drawer.SetCursorPosition(c_col, c_row);

                while (Console.KeyAvailable)
                    Console.ReadKey(true);
            }

            Thread.Sleep(50);
        }
    }

    // ui related
    private void DrawBoard()
    {
        if (board == null) return;

        string boardui =
        @"
   a b c d e f g h
  -----------------
8| . . . . . . . . |
7| . . . . . . . . |
6| . . . . . . . . |
5| . . . . . . . . |
4| . . . . . . . . |
3| . . . . . . . . |
2| . . . . . . . . |
1| . . . . . . . . |
  -----------------
";

        drawer.Print(0, 0, boardui);

        for (int row = 0; row < board.Width; row++)
        {
            for (int col = 0; col < board.Height; col++)
            {
                if (board.Pieces[row, col] != null)
                {
                    drawer.Print(col * 2 + 3, row + 3, board.Pieces[row, col].Symbol, consoleColor: board.Pieces[row, col] != null && board.Pieces[row, col]?.PieceColor == PieceColor.White ? ConsoleColor.White : ConsoleColor.DarkGray);
                }
            }
        }
    }

    // logic related
    private void HandlePressEnter()
    {
        if (board == null) return;

        int row = c_row - 3;
        int col = (c_col - 3) / 2;

        if (row < 0 || row >= board.Width || col < 0 || col >= board.Height)
            return;

        Piece? piece = board.Pieces[row, col];


        if (piece != null && appContext.CurrentPlayerColor == piece.PieceColor)
        {
            SelectPiece();
        }
        else if (selectedPiece != null && (piece == null || selectedPiece.PieceColor != piece.PieceColor))
        {
            MovePiece();
        }
    }

    private void SelectPiece()
    {
        if (board == null) return;
        if (!appContext.IsPlayerTurn) return;

        int row = c_row - 3;
        int col = (c_col - 3) / 2;

        if (row < 0 || row >= board.Width || col < 0 || col >= board.Height)
            return;

        Piece? piece = board.Pieces[row, col];

        if (piece != null && piece.PieceColor == appContext.CurrentPlayerColor)
        {
            // If the piece is already selected, deselect it
            if (selectedPiece != null)
            {
                DropPiece();
            }


            selectedPiece = piece;
            drawer.Print(selectedPiece.Col * 2 + 3, selectedPiece.Row + 3, selectedPiece.Symbol, consoleColor: ConsoleColor.DarkYellow);
        }
    }

    private void MovePiece()
    {
        if (board == null || selectedPiece == null) return;
        if (!appContext.IsPlayerTurn) return;

        int row = c_row - 3;
        int col = (c_col - 3) / 2;

        bool flag = Validator.IsValidMove(new int[] { selectedPiece.Row, selectedPiece.Col }, new int[] { row, col }, board);

        if (!flag)
        {
            DropPiece();
            return;
        }

        // hanlde rook if castling
        if (selectedPiece.Type == PieceType.King && Math.Abs(selectedPiece.Col - col) == 2)
        {
            // Handle castling
            if (col < selectedPiece.Col) // Left castling
            {
                board.Pieces[row, col + 1] = board.Pieces[row, 0]; // Move rook to the right
                board.Pieces[row, 0] = null!;
            }
            else // Right castling
            {
                board.Pieces[row, col - 1] = board.Pieces[row, 7]; // Move rook to the left
                board.Pieces[row, 7] = null!;
            }
        }

        // hanlde en passant 
        if (selectedPiece.Type == PieceType.Pawn && Math.Abs(selectedPiece.Col - col) == 1 && selectedPiece.Row != row && board.Pieces[row, col] == null)
        {
            // Handle en passant
            if (selectedPiece.PieceColor == PieceColor.White && row == 2)
            {
                board.Pieces[row + 1, col] = null!; // Remove the captured pawn
            }
            else if (selectedPiece.PieceColor == PieceColor.Black && row == 5)
            {
                board.Pieces[row - 1, col] = null!; // Remove the captured pawn
            }
        }


        if (selectedPiece.Type == PieceType.Pawn && Math.Abs(row - selectedPiece.Row) == 2)
            board.EnPassantTargetSquare = new[] { (selectedPiece.Row + row) / 2, selectedPiece.Col };
        else
            board.EnPassantTargetSquare = null;


        board.Pieces[row, col] = selectedPiece;
        board.Pieces[selectedPiece.Row, selectedPiece.Col] = null!;
        selectedPiece.Row = row;
        selectedPiece.Col = col;
        selectedPiece.MoveCount += 1;




        DropPiece();
        CheckPromote();
        DrawBoard(); //  sync ui 


        if (appContext.currentGameMode == GameMode.Minimax)
        {
            appContext.IsPlayerTurn = false;
            MoveDto? dto = minimaxEval.FindBestMove(board, PieceColor.Black);

            // MoveDto? dto = randomMoveGenerator.GenerateRandomMove(board, PieceColor.Black);

            if (dto != null)
            {
                board.Pieces[dto.to[0], dto.to[1]] = dto.movedPiece; // Move the piece to the new position
                board.Pieces[dto.from[0], dto.from[1]] = null!; // Remove the piece from the old position
            }

            CheckPromote();
            DrawBoard(); //  sync ui 
            appContext.IsPlayerTurn = true;

        }



        if (appContext.currentGameMode == GameMode.MCTS)
        {
            appContext.IsPlayerTurn = false;
            MoveDto? dto = mctsEval.FindBesteMove(board, PieceColor.Black);

            if (dto != null)
            {
                board.Pieces[dto.to[0], dto.to[1]] = dto.movedPiece; // Move the piece to the new position
                board.Pieces[dto.from[0], dto.from[1]] = null!; // Remove the piece from the old position
            }

            CheckPromote();
            DrawBoard(); //  sync ui 
            appContext.IsPlayerTurn = true;

        }





        if (appContext.currentGameMode == GameMode.SinglePlayer)
        {
            appContext.CurrentPlayerColor = appContext.CurrentPlayerColor == PieceColor.White ? PieceColor.Black : PieceColor.White;
        }


        (bool isEnd, PieceColor? color) = CheckGameEnd();
        if (isEnd && color == PieceColor.White)
        {
            Console.WriteLine("White wins!");
        }
        else if (isEnd && color == PieceColor.Black)
        {
            Console.WriteLine("Black wins!");
        }
        else if (isEnd && color == null)
        {
            Console.WriteLine("It's a draw!");
        }

    }

    private void DropPiece()
    {
        if (board == null || selectedPiece == null) return;

        int row = c_row - 3;
        int col = (c_col - 3) / 2;

        if (row < 0 || row >= board.Width || col < 0 || col >= board.Height)
            return;

        // Reset the selected piece
        drawer.Print(selectedPiece.Col * 2 + 3, selectedPiece.Row + 3, selectedPiece.Symbol, consoleColor: selectedPiece.PieceColor == PieceColor.White ? ConsoleColor.White : ConsoleColor.DarkGray);
        selectedPiece = null!;
    }

    private void CheckPromote()
    {
        for (int i = 0; i < board!.Width; i++)
        {
            if (board.Pieces[0, i] != null && board.Pieces[0, i].PieceColor == PieceColor.White && board.Pieces[0, i].Type == PieceType.Pawn)
            {
                // Promote the pawn to a queen
                board.Pieces[0, i] = new Piece(PieceColor.White, PieceType.Queen, 0, i);
            }

            if (board.Pieces[7, i] != null && board.Pieces[7, i].PieceColor == PieceColor.Black && board.Pieces[7, i].Type == PieceType.Pawn)
            {
                // Promote the pawn to a queen
                board.Pieces[7, i] = new Piece(PieceColor.Black, PieceType.Queen, 7, i);
            }
        }
    }


    private (bool, PieceColor?) CheckGameEnd()
    {
        if (LegalMoveGenerator.isWhiteWin(board!))
        {
            return (true, PieceColor.White);
        }
        else if (LegalMoveGenerator.isBlackWin(board!))
        {
            return (true, PieceColor.Black);
        }
        else if (LegalMoveGenerator.isDraw(board!))
        {
            return (true, null);
        }

        return (false, null);
    }

}