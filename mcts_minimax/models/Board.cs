class Board
{
    public int Width { get; set; }
    public int Height { get; set; }
    public Piece[,] Pieces { get; set; }

    public int[]? EnPassantTargetSquare { get; set; }

    public Board(int width, int height)
    {
        Width = width;
        Height = height;
        Pieces = new Piece[width, height];
    }

    public void PlacePiece(Piece piece, int x, int y)
    {
        if (x < 0 || x >= Width || y < 0 || y >= Height)
            throw new ArgumentOutOfRangeException("Position is out of bounds.");

        Pieces[x, y] = piece;
    }

    public Board Clone()
    {
        Board copy = new Board(Width, Height);

        // Deep copy the Pieces array
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                if (Pieces[x, y] != null)
                    copy.Pieces[x, y] = Pieces[x, y].Clone(); // requires Piece.Clone()
            }
        }

        // Copy EnPassantTargetSquare
        if (EnPassantTargetSquare != null)
            copy.EnPassantTargetSquare = (int[])EnPassantTargetSquare.Clone();

        return copy;
    }


    public bool IsSame(Board other)
    {
        if (other == null || Width != other.Width || Height != other.Height)
            return false;


        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                if ((Pieces[x, y] == null && other.Pieces[x, y] != null) ||
                    (Pieces[x, y] != null && other.Pieces[x, y] == null) ||
                    (Pieces[x, y] != null && !Pieces[x, y].IsSame(other.Pieces[x, y])))
                {
                    return false;
                }
            }
        }

        return true;
    }

}