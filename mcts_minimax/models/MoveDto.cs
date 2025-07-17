class MoveDto
{
    public int[] from { get; set; } = new int[2];
    public int[] to { get; set; } = new int[2];

    public int[] other_from { get; set; } = new int[2];
    public int[] other_to { get; set; } = new int[2];
    public Piece capturedPiece { get; set; } = null!;
    public Piece movedPiece { get; set; } = null!;
    public Piece movedOtherPiece { get; set; } = null!;


    public bool IsSame(MoveDto other)
    {
        return from[0] == other.from[0] && from[1] == other.from[1] &&
               to[0] == other.to[0] && to[1] == other.to[1] &&
               (other_from == null || (other_from[0] == other.other_from[0] && other_from[1] == other.other_from[1])) &&
               (other_to == null || (other_to[0] == other.other_to[0] && other_to[1] == other.other_to[1]));
    }
}