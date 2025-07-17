using System.Diagnostics.SymbolStore;
using System.IO.Compression;

class MCTSeval
{


    public MoveDto? FindBesteMove(Board board, PieceColor aiColor)
    {
        List<MoveDto> possibleMoves = LegalMoveGenerator.Generate(board, aiColor);
        if (possibleMoves.Count == 0)
        {
            return null; // No legal moves available
        }
        MCTSNode root = new MCTSNode(board.Clone(), aiColor, aiColor) { Parent = null };

        int iterCount = 0;
        int maxIterations = 100; // Set a limit for iterations to prevent infinite loops
        while (iterCount < maxIterations)
        {
            MCTSNode leaf = FindLeafNode(root)!;
            if (leaf.VisitCount == 0)
            {
                double result = Rollout(leaf.BoardState, leaf.CurrentPlayerColor);
                Backpropagate(leaf, result);
                iterCount++;
            }
            else
            {
                // Expansion phase
                List<MoveDto> legalMoves = LegalMoveGenerator.Generate(leaf.BoardState, leaf.CurrentPlayerColor);
                MoveDto selectedMove;


                if (legalMoves.Count == 0)
                {
                    // If no legal moves, backpropagate the result
                    double result = LegalMoveGenerator.isWhiteWin(leaf.BoardState) ? 1 : LegalMoveGenerator.isBlackWin(leaf.BoardState) ? -1 : 0;
                    Backpropagate(leaf, result);
                    iterCount++;
                    continue; // Go to the next iteration
                }

                while (true)
                {
                    selectedMove = legalMoves[new Random().Next(legalMoves.Count)];
                    bool flag = true;
                    foreach (MoveDto move in leaf.PossibleMoves)
                    {
                        if (move.IsSame(selectedMove))
                        {
                            flag = false;
                            break;
                        }
                    }
                    if (flag)
                    {
                        leaf.PossibleMoves.Add(selectedMove);
                        break;
                    }
                }
                Board clonedBoard = leaf.BoardState.Clone();
                ApplyMove(clonedBoard, selectedMove);



                MCTSNode childNode = new MCTSNode(clonedBoard, aiColor, leaf.CurrentPlayerColor == PieceColor.White ? PieceColor.Black : PieceColor.White)
                {
                    Parent = leaf,
                };

                leaf.Children.Add(childNode);
                childNode.Parent = leaf;
                iterCount++;
            }


        }


        MCTSNode? bestChild = SelectBestChild(root);
        if (bestChild == null)
        {
            return null; // No best child found
        }

        MoveDto? bestMove = null;
        Board board_ = root.BoardState.Clone();
        foreach (MoveDto move in root.PossibleMoves)
        {
            ApplyMove(board_, move);
            if (board_.IsSame(bestChild.BoardState))
            {
                bestMove = move;
                UndoMove(board_, move);
                break;
            }
            UndoMove(board_, move);
        }

        return bestMove;
    }


    public float GetUTCValue(MCTSNode node, float explorationConstant = 1.41f)
    {
        if (node.VisitCount == 0)
        {
            return float.MaxValue; // Unvisited nodes are prioritized
        }

        return (float)(node.WinScore / node.VisitCount + explorationConstant * Math.Sqrt(Math.Log(node.Parent?.VisitCount ?? 1) / node.VisitCount));
    }


    public MCTSNode? SelectBestChild(MCTSNode node, float explorationConstant = 1.41f)
    {
        MCTSNode? bestChild = null;
        float bestValue = float.MinValue;

        foreach (var child in node.Children)
        {
            float utcValue = GetUTCValue(child, explorationConstant);
            if (utcValue > bestValue)
            {
                bestValue = utcValue;
                bestChild = child;
            }
        }

        return bestChild;
    }

    public MCTSNode? FindLeafNode(MCTSNode node)
    {
        while (node.Children.Count > 0)
        {
            node = SelectBestChild(node) ?? node;
        }
        return node;
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

    private double Rollout(Board board, PieceColor currentPlayerColor, int depth = 0)
    {

        List<MoveDto> legalMoves = LegalMoveGenerator.Generate(board, currentPlayerColor);


        if (depth >= 1000)
        {
            if (legalMoves.Count == 0)
            {
                return LegalMoveGenerator.isWhiteWin(board) ? 1 : LegalMoveGenerator.isBlackWin(board) ? -1 : 0;
            }
            // just use evaluation function if depth is too high
            MinimaxEval minimaxEval = new MinimaxEval(-1);
            return minimaxEval.EvaluateBoard(board);
        }

        if (legalMoves.Count == 0)
        {
            return LegalMoveGenerator.isWhiteWin(board) ? 1 : LegalMoveGenerator.isBlackWin(board) ? -1 : 0;
        }
        // Select a random move
        MoveDto randomMove = legalMoves[new Random().Next(legalMoves.Count)];
        ApplyMove(board, randomMove);
        double result = Rollout(board, currentPlayerColor == PieceColor.White ? PieceColor.Black : PieceColor.White, depth + 1);
        UndoMove(board, randomMove);
        return result;
    }

    private void Backpropagate(MCTSNode? node, double result)
    {
        while (node != null)
        {
            node.VisitCount++;
            node.WinScore += result;
            node = node.Parent;
        }
    }

}

class MCTSNode
{
    public Board BoardState { get; set; }
    public PieceColor AiColor { get; set; }
    public int VisitCount { get; set; }
    public double WinScore { get; set; }
    public List<MCTSNode> Children { get; set; }
    public MCTSNode? Parent { get; set; }

    public PieceColor CurrentPlayerColor { get; set; }
    public List<MoveDto> PossibleMoves { get; set; } = new List<MoveDto>();
    public MCTSNode(Board boardState, PieceColor aiColor, PieceColor currentPlayerColor)
    {
        BoardState = boardState;
        AiColor = aiColor;
        CurrentPlayerColor = currentPlayerColor;
        VisitCount = 0;
        WinScore = 0.0;
        Children = new List<MCTSNode>();
    }
}