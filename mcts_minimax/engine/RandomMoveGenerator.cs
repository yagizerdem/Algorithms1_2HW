class RandomMoveGenerator
{

    public MoveDto GenerateRandomMove(Board board, PieceColor pieceColor)
    {
        List<MoveDto> legalMoves = LegalMoveGenerator.Generate(board, pieceColor);
        if (legalMoves.Count == 0)
        {
            return null!; // No legal moves available
        }

        Random random = new Random();
        int randomIndex = random.Next(legalMoves.Count);
        return legalMoves[randomIndex];
    }
}