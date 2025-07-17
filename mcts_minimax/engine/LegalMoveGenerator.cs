static class LegalMoveGenerator
{

    public static List<MoveDto> Generate(Board board, PieceColor targetColor)
    {
        List<MoveDto> legalMoves = new List<MoveDto>();

        // Generate legal moves for the specified color
        for (int i = 0; i < board.Height; i++)
        {
            for (int j = 0; j < board.Width; j++)
            {
                Piece? piece = board.Pieces[i, j];
                int[] from = new int[] { i, j };
                if (piece != null && piece.PieceColor == targetColor)
                {
                    // Generate moves for the piece
                    for (int row = 0; row < board.Height; row++)
                    {
                        for (int col = 0; col < board.Width; col++)
                        {
                            if (row == i && col == j) continue; // Skip the piece's current position
                            int[] to = new int[] { row, col };
                            bool flag = Validator.IsValidMove(from, to, board);
                            if (flag)
                            {
                                MoveDto move = new MoveDto();
                                move.from = from;
                                move.to = to;
                                Piece? capturedPiece = board.Pieces[row, col];
                                if (capturedPiece != null)
                                {

                                    move.capturedPiece = capturedPiece.Clone();
                                }
                                else
                                {
                                    move.capturedPiece = null!;
                                }
                                move.movedPiece = piece.Clone();


                                // for castle fill other from other to properties of rook coorerdinates
                                if (piece.Type == PieceType.King && Math.Abs(col - j) == 2)
                                {
                                    if (col < j) // left castle
                                    {
                                        move.other_from = new int[] { row, 0 };
                                        move.other_to = new int[] { row, col + 1 };

                                        Piece rook = board.Pieces[row, 0];
                                        move.movedOtherPiece = rook.Clone();
                                    }
                                    else // right castle
                                    {
                                        move.other_from = new int[] { row, 7 };
                                        move.other_to = new int[] { row, col + 1 };

                                        Piece rook = board.Pieces[row, 7];
                                        move.movedOtherPiece = rook.Clone();
                                    }
                                }
                                else
                                {
                                    move.other_from = null!;
                                    move.other_to = null!;
                                    move.movedOtherPiece = null!;
                                }


                                legalMoves.Add(move);
                            }

                        }
                    }
                }
            }

        }


        return legalMoves;
    }


    public static bool isWhiteWin(Board board)
    {
        List<MoveDto> legalMoves = Generate(board, PieceColor.Black);
        if (legalMoves.Count == 0)
        {


            // No legal moves for black, check if white can capture the king
            int[] kingPosition = Validator.FindKingPosition(board, PieceColor.Black);
            return !Validator.IsKingSafe(board, PieceColor.Black, kingPosition);
        }

        return false;
    }


    public static bool isBlackWin(Board board)
    {
        List<MoveDto> legalMoves = Generate(board, PieceColor.White);
        if (legalMoves.Count == 0)
        {


            // No legal moves for White, check if black can capture the king
            int[] kingPosition = Validator.FindKingPosition(board, PieceColor.White);
            return !Validator.IsKingSafe(board, PieceColor.White, kingPosition);
        }

        return false;
    }


    public static bool isDraw(Board board)
    {
        return Generate(board, PieceColor.White).Count == 0 &&
               Generate(board, PieceColor.Black).Count == 0 &&
               !isWhiteWin(board) &&
               !isBlackWin(board);
    }
}