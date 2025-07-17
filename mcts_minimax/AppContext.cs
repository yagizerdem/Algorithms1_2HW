class AppContext
{
    public GameMode currentGameMode { get; set; }
    public BasePanel? currentPanel { get; set; }
    public PieceColor CurrentPlayerColor { get; set; }
    public bool IsPlayerTurn { get; set; } = false;


    public void ChangePanel(BasePanel newPanel)
    {
        currentPanel?.OnClose();
        currentPanel = newPanel;
        currentPanel?.OnStartUp();
    }

}