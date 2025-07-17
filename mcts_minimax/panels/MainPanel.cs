
using System.Xml;
using Microsoft.Extensions.DependencyInjection;

class MainPanel : BasePanel
{

    Drawer drawer = null!;
    private AppContext appContext = null!;

    public override void OnStartUp()
    {

        this.appContext = ServiceLocator.Instance.GetService<AppContext>()!;
        this.drawer = ServiceLocator.Instance.GetService<Drawer>()!;

        string builder =
         @"
        Main Menu
        ----------
        1. Single Player
        2. Minimax
        3. MCTS
        4. Quit

        press number to select an option
        ";

        Console.WriteLine(builder);

        Run();
    }


    private void Run()
    {
        while (true)
        {
            ConsoleKeyInfo keyInfo = Console.ReadKey(true);
            if (keyInfo.Key == ConsoleKey.NumPad1)
            {
                appContext.IsPlayerTurn = true;
                appContext.currentGameMode = GameMode.SinglePlayer;
                appContext.CurrentPlayerColor = PieceColor.White;
                appContext.ChangePanel(new GamePanel());
            }
            else if (keyInfo.Key == ConsoleKey.NumPad2)
            {
                appContext.IsPlayerTurn = true;
                appContext.currentGameMode = GameMode.Minimax;
                appContext.CurrentPlayerColor = PieceColor.White;
                appContext.ChangePanel(new GamePanel());
            }

            else if (keyInfo.Key == ConsoleKey.NumPad3)
            {
                appContext.IsPlayerTurn = true;
                appContext.currentGameMode = GameMode.MCTS;
                appContext.CurrentPlayerColor = PieceColor.White;
                appContext.ChangePanel(new GamePanel());
            }
            else if (keyInfo.Key == ConsoleKey.NumPad4)
            {
                Environment.Exit(0);
            }
            Thread.Sleep(100); // To prevent high CPU usage
        }
    }

    public override void OnClose()
    {
        // Render logic for the main panel
        Console.WriteLine("Closing Main Panel");
    }
}