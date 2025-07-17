static class Validator
{
    public static bool IsValidMove(int[] from, int[] to, Board board)
    {
        if (!IsInsideBoard(from) || !IsInsideBoard(to))
            return false;

        Piece? piece = board.Pieces[from[0], from[1]];
        if (piece == null)
            return false;

        if (board.Pieces[to[0], to[1]]?.PieceColor == piece.PieceColor)
            return false;

        // check castling
        if (piece.Type == PieceType.King && Math.Abs(from[1] - to[1]) == 2)
        {
            return CheckCastle(from, to, piece, board);
        }

        // check en passant
        if (piece.Type == PieceType.Pawn && Math.Abs(from[1] - to[1]) == 1 && from[0] != to[0] && board.Pieces[to[0], to[1]] == null)
        {
            return CheckEnPassant(from, to, piece, board);
        }



        if (!CheckDirection(from, to, piece, board))
            return false;

        // Temporarily apply move
        Piece? captured = board.Pieces[to[0], to[1]];
        board.Pieces[to[0], to[1]] = piece;
        board.Pieces[from[0], from[1]] = null!;

        // Now find king's actual position *after move*
        int[] kingPosition = FindKingPosition(board, piece.PieceColor);

        // Check if king is safe
        bool kingSafe = IsKingSafe(board, piece.PieceColor, kingPosition);

        // Revert move
        board.Pieces[from[0], from[1]] = piece;
        board.Pieces[to[0], to[1]] = captured;

        return kingSafe;
    }

    private static bool IsInsideBoard(int[] pos)
    {
        return pos[0] >= 0 && pos[0] < 8 && pos[1] >= 0 && pos[1] < 8;
    }

    public static int[] FindKingPosition(Board board, PieceColor color)
    {
        for (int i = 0; i < board.Width; i++)
        {
            for (int j = 0; j < board.Height; j++)
            {
                var p = board.Pieces[i, j];
                if (p != null && p.Type == PieceType.King && p.PieceColor == color)
                    return new[] { i, j };
            }
        }

        throw new Exception("King not found on the board.");
    }

    private static bool CheckDirection(int[] from, int[] to, Piece piece, Board board)
    {
        int dx = to[1] - from[1];
        int dy = to[0] - from[0];

        switch (piece.Type)
        {
            case PieceType.Bishop:
                if (Math.Abs(dx) == Math.Abs(dy))
                    return IsPathClear(from, to, board);
                break;

            case PieceType.Rook:
                if (from[0] == to[0] || from[1] == to[1])
                    return IsPathClear(from, to, board);
                break;

            case PieceType.Queen:
                if (Math.Abs(dx) == Math.Abs(dy) || from[0] == to[0] || from[1] == to[1])
                    return IsPathClear(from, to, board);
                break;

            case PieceType.Knight:
                return (Math.Abs(dx) == 2 && Math.Abs(dy) == 1) || (Math.Abs(dx) == 1 && Math.Abs(dy) == 2);

            case PieceType.King:
                return Math.Abs(dx) <= 1 && Math.Abs(dy) <= 1;

            case PieceType.Pawn:
                int direction = piece.PieceColor == PieceColor.White ? -1 : 1;
                int startRow = piece.PieceColor == PieceColor.White ? 6 : 1;

                // Move forward
                if (dx == 0 && dy == direction && board.Pieces[to[0], to[1]] == null)
                    return true;

                // Initial double-step
                if (dx == 0 && dy == 2 * direction && from[0] == startRow &&
                    board.Pieces[from[0] + direction, from[1]] == null &&
                    board.Pieces[to[0], to[1]] == null)
                    return true;

                // Diagonal capture
                if (Math.Abs(dx) == 1 && dy == direction &&
                    board.Pieces[to[0], to[1]] != null &&
                    board.Pieces[to[0], to[1]]!.PieceColor != piece.PieceColor)
                    return true;

                break;
        }

        return false;
    }

    private static bool IsPathClear(int[] from, int[] to, Board board)
    {
        int dx = Math.Sign(to[1] - from[1]);
        int dy = Math.Sign(to[0] - from[0]);

        int x = from[1] + dx;
        int y = from[0] + dy;

        while (x != to[1] || y != to[0])
        {
            if (board.Pieces[y, x] != null)
                return false;

            x += dx;
            y += dy;
        }

        return true;
    }

    public static bool IsKingSafe(Board board, PieceColor kingColor, int[] kingPos)
    {
        for (int i = 0; i < board.Width; i++)
        {
            for (int j = 0; j < board.Height; j++)
            {
                Piece? attacker = board.Pieces[i, j];
                if (attacker == null || attacker.PieceColor == kingColor)
                    continue;

                int[] from = { i, j };

                switch (attacker.Type)
                {
                    case PieceType.Bishop:
                        if (Math.Abs(kingPos[0] - i) == Math.Abs(kingPos[1] - j) &&
                            IsPathClear(from, kingPos, board))
                            return false;
                        break;

                    case PieceType.Rook:
                        if ((i == kingPos[0] || j == kingPos[1]) &&
                            IsPathClear(from, kingPos, board))
                            return false;
                        break;

                    case PieceType.Queen:
                        if ((Math.Abs(kingPos[0] - i) == Math.Abs(kingPos[1] - j) ||
                            i == kingPos[0] || j == kingPos[1]) &&
                            IsPathClear(from, kingPos, board))
                            return false;
                        break;

                    case PieceType.Knight:
                        int dx = Math.Abs(j - kingPos[1]);
                        int dy = Math.Abs(i - kingPos[0]);
                        if ((dx == 2 && dy == 1) || (dx == 1 && dy == 2))
                            return false;
                        break;

                    case PieceType.Pawn:
                        int dir = attacker.PieceColor == PieceColor.White ? -1 : 1;
                        if (kingPos[0] == i + dir && Math.Abs(kingPos[1] - j) == 1)
                            return false;
                        break;

                    case PieceType.King:
                        if (Math.Abs(i - kingPos[0]) <= 1 && Math.Abs(j - kingPos[1]) <= 1)
                            return false;
                        break;
                }
            }
        }

        return true;
    }

    private static bool CheckCastle(int[] from, int[] to, Piece piece, Board board)
    {
        if (piece.Type != PieceType.King || piece.MoveCount != 0)
            return false;

        if (piece.PieceColor == PieceColor.White && from[0] != 7 || to[0] != 7)
            return false;
        if (piece.PieceColor == PieceColor.Black && from[0] != 0 || to[0] != 0)
            return false;

        int row = from[0];
        int colFrom = from[1];
        int colTo = to[1];
        int direction = colTo > colFrom ? 1 : -1;

        int rookCol = direction == 1 ? 7 : 0;
        int expectedKingDest = direction == 1 ? 6 : 2;
        int expectedRookDest = direction == 1 ? 5 : 3;

        // 1. Destination must match expected castling square
        if (colTo != expectedKingDest)
            return false;

        // 2. Check if squares between king and rook are empty
        for (int i = colFrom + direction; i != rookCol; i += direction)
        {
            if (board.Pieces[row, i] != null)
                return false;
        }

        // 3. Validate rook
        Piece? rook = board.Pieces[row, rookCol];
        if (rook == null || rook.Type != PieceType.Rook || rook.MoveCount != 0 || rook.PieceColor != piece.PieceColor)
            return false;

        // 4. Check that king is not in check nor crosses or ends in check
        if (!IsKingSafe(board, piece.PieceColor, new int[] { row, colFrom })) return false;
        if (!IsKingSafe(board, piece.PieceColor, new int[] { row, colFrom + direction })) return false;

        // 5. Simulate castling move
        board.Pieces[row, colTo] = piece; // move king
        board.Pieces[row, colFrom] = null!;

        board.Pieces[row, expectedRookDest] = rook; // move rook
        board.Pieces[row, rookCol] = null!;

        bool safeAfter = IsKingSafe(board, piece.PieceColor, new int[] { row, colTo });

        // 6. Revert castling move
        board.Pieces[row, colFrom] = piece;
        board.Pieces[row, colTo] = null!;

        board.Pieces[row, rookCol] = rook;
        board.Pieces[row, expectedRookDest] = null!;

        return safeAfter;
    }

    private static bool CheckEnPassant(int[] from, int[] to, Piece pawn, Board board)
    {
        if (pawn.Type != PieceType.Pawn) return false;
        if (board.EnPassantTargetSquare == null) return false;

        int direction = pawn.PieceColor == PieceColor.White ? -1 : 1;

        // Check if move is a diagonal step to the en passant target square
        if (to[0] == from[0] + direction &&
            Math.Abs(to[1] - from[1]) == 1 &&
            to[0] == board.EnPassantTargetSquare[0] &&
            to[1] == board.EnPassantTargetSquare[1])
        {
            // Verify there's an enemy pawn just behind the target square
            Piece? capturedPawn = board.Pieces[to[0] - direction, to[1]];
            if (capturedPawn != null &&
                capturedPawn.Type == PieceType.Pawn &&
                capturedPawn.PieceColor != pawn.PieceColor)
            {
                return true;
            }
        }

        return false;
    }

}
