abstract class BasePanel
{
    public virtual void OnStartUp()
    {
        // Logic to handle when the base panel starts up
        Console.WriteLine("Base Panel Started Up");
    }


    public virtual void OnClose()
    {
        // Logic to handle when the base panel closes
        Console.WriteLine("Base Panel Closed");
    }
}