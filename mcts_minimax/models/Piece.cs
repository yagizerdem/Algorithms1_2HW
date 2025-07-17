class Piece
{
    public PieceColor PieceColor { get; set; }
    public PieceType Type { get; set; }
    public int Row { get; set; }
    public int Col { get; set; }
    public int MoveCount { get; set; } = 0;
    public bool IsFirstMove => this.MoveCount == 0;

    public Piece(PieceColor pieceColor, PieceType type, int row, int col)
    {
        PieceColor = pieceColor;
        Type = type;
        Row = row;
        Col = col;
        MoveCount = 0;
    }

    public string Symbol
    {
        get
        {
            return Type switch
            {
                PieceType.Pawn => PieceColor == PieceColor.White ? "P" : "p",
                PieceType.Rook => PieceColor == PieceColor.White ? "R" : "r",
                PieceType.Knight => PieceColor == PieceColor.White ? "N" : "n",
                PieceType.Bishop => PieceColor == PieceColor.White ? "B" : "b",
                PieceType.Queen => PieceColor == PieceColor.White ? "Q" : "q",
                PieceType.King => PieceColor == PieceColor.White ? "K" : "k",
                _ => "."
            };
        }
    }


    public Piece Clone()
    {
        return new Piece(this.PieceColor, this.Type, this.Row, this.Col)
        {
            MoveCount = this.MoveCount
        };
    }


    public bool IsSame(Piece other)
    {
        if (other == null) return false;
        return PieceColor == other.PieceColor && Type == other.Type && Row == other.Row && Col == other.Col && MoveCount == other.MoveCount;
    }
}