using Microsoft.Extensions.DependencyInjection;

class Program
{
    static void Main(string[] args)
    {
        ConfigServices.Configure();
        AppContext context = ServiceLocator.Instance.GetService<AppContext>()!;

        context.ChangePanel(new MainPanel());


        // Test();
    }


    // private static void Test()
    // {
    //     Board board = new Board(8, 8);
    //     board.Pieces = new Piece[8, 8];

    //     // Place pawns
    //     for (int col = 0; col < 8; col++)
    //     {
    //         board.Pieces[1, col] = new Piece(PieceColor.Black, PieceType.Pawn, 1, col);
    //         board.Pieces[6, col] = new Piece(PieceColor.White, PieceType.Pawn, 6, col);
    //     }

    //     // Place Rooks
    //     board.Pieces[0, 0] = new Piece(PieceColor.Black, PieceType.Rook, 0, 0);
    //     board.Pieces[0, 7] = new Piece(PieceColor.Black, PieceType.Rook, 0, 7);
    //     board.Pieces[7, 0] = new Piece(PieceColor.White, PieceType.Rook, 7, 0);
    //     board.Pieces[7, 7] = new Piece(PieceColor.White, PieceType.Rook, 7, 7);

    //     // Place Knights
    //     board.Pieces[0, 1] = new Piece(PieceColor.Black, PieceType.Knight, 0, 1);
    //     board.Pieces[0, 6] = new Piece(PieceColor.Black, PieceType.Knight, 0, 6);
    //     board.Pieces[7, 1] = new Piece(PieceColor.White, PieceType.Knight, 7, 1);
    //     board.Pieces[7, 6] = new Piece(PieceColor.White, PieceType.Knight, 7, 6);

    //     // Place Bishops
    //     board.Pieces[0, 2] = new Piece(PieceColor.Black, PieceType.Bishop, 0, 2);
    //     board.Pieces[0, 5] = new Piece(PieceColor.Black, PieceType.Bishop, 0, 5);
    //     board.Pieces[7, 2] = new Piece(PieceColor.White, PieceType.Bishop, 7, 2);
    //     board.Pieces[7, 5] = new Piece(PieceColor.White, PieceType.Bishop, 7, 5);

    //     // Place Queens
    //     board.Pieces[0, 3] = new Piece(PieceColor.Black, PieceType.Queen, 0, 3);
    //     board.Pieces[7, 3] = new Piece(PieceColor.White, PieceType.Queen, 7, 3);

    //     // Place Kings
    //     board.Pieces[0, 4] = new Piece(PieceColor.Black, PieceType.King, 0, 4);
    //     board.Pieces[7, 4] = new Piece(PieceColor.White, PieceType.King, 7, 4);


    //     board.Pieces[0, 6] = null!;
    //     board.Pieces[0, 5] = null!;


    //     var moves = LegalMoveGenerator.Generate(board, PieceColor.Black);
    //     ;
    // }




}