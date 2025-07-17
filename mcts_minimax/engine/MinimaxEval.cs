using System.Runtime.Serialization.Formatters.Binary;

class MinimaxEval
{

    public int Depth { get; private set; }

    static double[,] KingTableW = {
    { -3.0, -4.0, -4.0, -5.0, -5.0, -4.0, -4.0, -3.0 },
    { -3.0, -4.0, -4.0, -5.0, -5.0, -4.0, -4.0, -3.0 },
    { -3.0, -4.0, -4.0, -5.0, -5.0, -4.0, -4.0, -3.0 },
    { -3.0, -4.0, -4.0, -5.0, -5.0, -4.0, -4.0, -3.0 },
    { -2.0, -3.0, -3.0, -4.0, -4.0, -3.0, -3.0, -2.0 },
    { -1.0, -2.0, -2.0, -2.0, -2.0, -2.0, -2.0, -1.0 },
    {  2.0,  2.0,  0.0,  0.0,  0.0,  0.0,  2.0,  2.0 },
    {  2.0,  3.0,  1.0,  0.0,  0.0,  1.0,  3.0,  2.0 }
};

    static double[,] QueenTableW = {
    { -2.0, -1.0, -1.0, -0.5, -0.5, -1.0, -1.0, -2.0 },
    { -1.0,  0.0,  0.0,  0.0,  0.0,  0.0,  0.0, -1.0 },
    { -1.0,  0.0,  0.5,  0.5,  0.5,  0.5,  0.0, -1.0 },
    { -0.5,  0.0,  0.5,  0.5,  0.5,  0.5,  0.0, -0.5 },
    {  0.0,  0.0,  0.5,  0.5,  0.5,  0.5,  0.0, -0.5 },
    { -1.0,  0.5,  0.5,  0.5,  0.5,  0.5,  0.0, -1.0 },
    { -1.0,  0.0,  0.5,  0.0,  0.0,  0.0,  0.0, -1.0 },
    { -2.0, -1.0, -1.0, -0.5, -0.5, -1.0, -1.0, -2.0 }
};

    static double[,] RookTableW = {
    { 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0 },
    { 0.5, 1.0, 1.0, 1.0, 1.0, 1.0, 1.0, 0.5 },
    { -0.5, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, -0.5 },
    { -0.5, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, -0.5 },
    { -0.5, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, -0.5 },
    { -0.5, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, -0.5 },
    { -0.5, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, -0.5 },
    { 0.0, 0.0, 0.0, 0.5, 0.5, 0.0, 0.0, 0.0 }
};

    static double[,] BishopTableW = {
    { -2.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -2.0 },
    { -1.0,  0.0,  0.0,  0.0,  0.0,  0.0,  0.0, -1.0 },
    { -1.0,  0.0,  0.5,  1.0,  1.0,  0.5,  0.0, -1.0 },
    { -1.0,  0.5,  0.5,  1.0,  1.0,  0.5,  0.5, -1.0 },
    { -1.0,  0.0,  1.0,  1.0,  1.0,  1.0,  0.0, -1.0 },
    { -1.0,  1.0,  1.0,  1.0,  1.0,  1.0,  1.0, -1.0 },
    { -1.0,  0.5,  0.0,  0.0,  0.0,  0.0,  0.5, -1.0 },
    { -2.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -2.0 }
};


    static double[,] KnightTableW = {
    { -5.0, -4.0, -3.0, -3.0, -3.0, -3.0, -4.0, -5.0 },
    { -4.0, -2.0,  0.0,  0.0,  0.0,  0.0, -2.0, -4.0 },
    { -3.0,  0.0,  1.0,  1.5,  1.5,  1.0,  0.0, -3.0 },
    { -3.0,  0.5,  1.5,  2.0,  2.0,  1.5,  0.5, -3.0 },
    { -3.0,  0.0,  1.5,  2.0,  2.0,  1.5,  0.0, -3.0 },
    { -3.0,  0.5,  1.0,  1.5,  1.5,  1.0,  0.5, -3.0 },
    { -4.0, -2.0,  0.0,  0.5,  0.5,  0.0, -2.0, -4.0 },
    { -5.0, -4.0, -3.0, -3.0, -3.0, -3.0, -4.0, -5.0 }
};


    static double[,] PawnTableW = {
    { 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0 },
    { 5.0, 5.0, 5.0, 5.0, 5.0, 5.0, 5.0, 5.0 },
    { 1.0, 1.0, 2.0, 3.0, 3.0, 2.0, 1.0, 1.0 },
    { 0.5, 0.5, 1.0, 2.5, 2.5, 1.0, 0.5, 0.5 },
    { 0.0, 0.0, 0.0, 2.0, 2.0, 0.0, 0.0, 0.0 },
    { 0.5, -0.5, -1.0, 0.0, 0.0, -1.0, -0.5, 0.5 },
    { 0.5, 1.0, 1.0, -2.0, -2.0, 1.0, 1.0, 0.5 },
    { 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0 }
};


    static double[,] KingTableB = FlipVertical(KingTableW);
    static double[,] QueenTableB = FlipVertical(QueenTableW);
    static double[,] RookTableB = FlipVertical(RookTableW);
    static double[,] BishopTableB = FlipVertical(BishopTableW);
    static double[,] KnightTableB = FlipVertical(KnightTableW);
    static double[,] PawnTableB = FlipVertical(PawnTableW);



    public MinimaxEval(int depth)
    {
        Depth = depth;
    }


    public MoveDto? FindBestMove(Board board, PieceColor aiColor)
    {
        bool isMaximizingPlayer = aiColor == PieceColor.White;
        var legalMoves = LegalMoveGenerator.Generate(board, aiColor);
        if (legalMoves.Count == 0) return null;

        double bestScore = isMaximizingPlayer ? double.MinValue : double.MaxValue;
        MoveDto? bestMove = null;

        foreach (var move in legalMoves)
        {
            ApplyMove(board, move);
            double score = Minimax(board, 1, !isMaximizingPlayer);  // Start at depth 1
            UndoMove(board, move);

            if ((isMaximizingPlayer && score > bestScore) || (!isMaximizingPlayer && score < bestScore))
            {
                bestScore = score;
                bestMove = move;
            }
        }

        return bestMove;
    }

    private double Minimax(Board board, int depth, bool maximizingPlayer)
    {
        if (depth == 4) return EvaluateBoard(board);

        var legalMoves = LegalMoveGenerator.Generate(board, maximizingPlayer ? PieceColor.White : PieceColor.Black);
        if (legalMoves.Count == 0)
        {
            if (LegalMoveGenerator.isWhiteWin(board)) return maximizingPlayer ? 99999 : -99999;
            if (LegalMoveGenerator.isBlackWin(board)) return maximizingPlayer ? -99999 : 99999;
            return 0;
        }

        double bestScore = maximizingPlayer ? double.MinValue : double.MaxValue;

        foreach (var move in legalMoves)
        {
            ApplyMove(board, move);
            double score = Minimax(board, depth + 1, !maximizingPlayer);
            UndoMove(board, move);

            if (maximizingPlayer)
                bestScore = Math.Max(bestScore, score);
            else
                bestScore = Math.Min(bestScore, score);
        }

        return bestScore;
    }


    private void ApplyMove(Board board, MoveDto move)
    {

        board.Pieces[move.to[0], move.to[1]] = board.Pieces[move.from[0], move.from[1]]!;
        board.Pieces[move.from[0], move.from[1]] = null!;
        board.Pieces[move.to[0], move.to[1]].MoveCount += 1;

        if (move.other_from != null)
        {
            board.Pieces[move.other_to[0], move.other_to[1]] = board.Pieces[move.other_from[0], move.other_from[1]];
            board.Pieces[move.other_from[0], move.other_from[1]] = null!;
            board.Pieces[move.other_to[0], move.other_to[1]].MoveCount += 1;
        }

        for (int i = 0; i < board.Width; i++)
        {
            if (board.Pieces[0, i] != null && board.Pieces[0, i]!.Type == PieceType.Pawn && board.Pieces[0, i]!.PieceColor == PieceColor.White)
            {
                board.Pieces[0, i]!.Type = PieceType.Queen; // Promote to Queen
            }
            if (board.Pieces[7, i] != null && board.Pieces[7, i]!.Type == PieceType.Pawn && board.Pieces[7, i]!.PieceColor == PieceColor.Black)
            {
                board.Pieces[7, i]!.Type = PieceType.Queen; // Promote to Queen
            }
        }
    }

    private void UndoMove(Board board, MoveDto move)
    {
        board.Pieces[move.to[0], move.to[1]] = null!;
        board.Pieces[move.from[0], move.from[1]] = move.movedPiece.Clone();
        // move.movedPiece.Row = move.from[0];
        // move.movedPiece.Col = move.from[1];

        if (move.movedOtherPiece != null)
        {
            board.Pieces[move.other_to[0], move.other_to[1]] = null!;
            board.Pieces[move.other_from![0], move.other_from![1]] = move.movedOtherPiece.Clone();
            // move.movedOtherPiece.Row = move.other_from[0];
            // move.movedOtherPiece.Col = move.other_from[1];
        }

        if (move.capturedPiece != null)
        {
            board.Pieces[move.to[0], move.to[1]] = move.capturedPiece;
        }

    }


    public double EvaluateBoard(Board board)
    {
        double score = 0.0;

        for (int row = 0; row < board.Height; row++)
        {
            for (int col = 0; col < board.Width; col++)
            {
                Piece? piece = board.Pieces[row, col];
                if (piece != null)
                {
                    double pieceValue = GetPieceValue(piece);
                    score += pieceValue;
                }
            }
        }

        return score;
    }


    private double GetPieceValue(Piece piece)
    {
        double value = piece.Type switch
        {
            PieceType.Pawn => 1.0,
            PieceType.Rook => 5.0,
            PieceType.Knight => 3.0,
            PieceType.Bishop => 3.0,
            PieceType.Queen => 9.0,
            PieceType.King => 100.0,
            _ => 0.0
        };


        // Add positional value based on piece type and color
        value += piece.PieceColor == PieceColor.White ? GetWhitePiecePositionValue(piece) : GetBlackPiecePositionValue(piece);

        if (piece.PieceColor == PieceColor.Black)
        {
            value = -value; // Negate value for black pieces
        }

        return value;
    }


    private double GetWhitePiecePositionValue(Piece piece)
    {
        return piece.Type switch
        {
            PieceType.Pawn => PawnTableW[piece.Row, piece.Col],
            PieceType.Rook => RookTableW[piece.Row, piece.Col],
            PieceType.Knight => KnightTableW[piece.Row, piece.Col],
            PieceType.Bishop => BishopTableW[piece.Row, piece.Col],
            PieceType.Queen => QueenTableW[piece.Row, piece.Col],
            PieceType.King => KingTableW[piece.Row, piece.Col],
            _ => 0.0
        };
    }

    private double GetBlackPiecePositionValue(Piece piece)
    {
        return piece.Type switch
        {
            PieceType.Pawn => PawnTableB[piece.Row, piece.Col],
            PieceType.Rook => RookTableB[piece.Row, piece.Col],
            PieceType.Knight => KnightTableB[piece.Row, piece.Col],
            PieceType.Bishop => BishopTableB[piece.Row, piece.Col],
            PieceType.Queen => QueenTableB[piece.Row, piece.Col],
            PieceType.King => KingTableB[piece.Row, piece.Col],
            _ => 0.0
        };
    }

    static double[,] FlipVertical(double[,] table)
    {
        int rows = table.GetLength(0);
        int cols = table.GetLength(1);
        double[,] flipped = new double[rows, cols];

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                flipped[i, j] = table[rows - 1 - i, j];
            }
        }


        return flipped;
    }


}