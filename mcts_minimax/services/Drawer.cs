

class Drawer
{

    public SemaphoreSlim Semaphore { get; set; }
    public Drawer()
    {
        Semaphore = new SemaphoreSlim(1, 1);
    }

    public void Print(int col, int row, string text, ConsoleColor consoleColor = ConsoleColor.White)
    {
        try
        {
            Console.ForegroundColor = consoleColor;
            Semaphore.Wait();
            Console.SetCursorPosition(col, row);
            Console.Write(text);
        }
        finally
        {
            Console.ForegroundColor = ConsoleColor.White;
            Semaphore.Release();
        }

    }

    public void Clear()
    {
        try
        {
            Semaphore.Wait();
            Console.Clear();
        }
        finally
        {
            Semaphore.Release();
        }
    }


    public void SetCursorPosition(int col, int row)
    {
        try
        {
            Semaphore.Wait();
            Console.SetCursorPosition(col, row);
        }
        finally
        {
            Semaphore.Release();
        }
    }
}