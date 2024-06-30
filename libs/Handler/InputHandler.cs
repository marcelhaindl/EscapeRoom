namespace libs;

public sealed class InputHandler
{
    // Singleton instance of InputHandler
    private static InputHandler? _instance;

    // Reference to the game engine
    private GameEngine engine;

    // Singleton pattern implementation to get the instance of InputHandler
    public static InputHandler Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new InputHandler();
            }
            return _instance;
        }
    }

    // Private constructor to prevent external instantiation
    private InputHandler()
    {
        // Initialize properties if needed
        engine = GameEngine.Instance;
    }

    // Handle keyboard input
    public void Handle(ConsoleKeyInfo keyInfo)
    {
        GameObject player = engine.GetPlayer();

        if (player != null)
        {
            // Handle keyboard input to move the player
            switch (keyInfo.Key)
            {
                case ConsoleKey.UpArrow:
                    player.Move(0, -1); // Move player up
                    player.CharRepresentation = '▲'; // Set player character to up arrow
                    // Start timer thread if not already started
                    if (!GameEngine.Instance.timerStarted) 
                    {
                        GameEngine.Instance.timerThread.Start();
                        GameEngine.Instance.timerStarted = true;
                    }
                    break;
                case ConsoleKey.DownArrow:
                    player.Move(0, 1); // Move player down
                    player.CharRepresentation = '▼'; // Set player character to down arrow
                    // Start timer thread if not already started
                    if (!GameEngine.Instance.timerStarted) 
                    {
                        GameEngine.Instance.timerThread.Start();
                        GameEngine.Instance.timerStarted = true;
                    }
                    break;
                case ConsoleKey.LeftArrow:
                    player.Move(-1, 0); // Move player left
                    player.CharRepresentation = '◄'; // Set player character to left arrow
                    // Start timer thread if not already started
                    if (!GameEngine.Instance.timerStarted) 
                    {
                        GameEngine.Instance.timerThread.Start();
                        GameEngine.Instance.timerStarted = true;
                    }
                    break;
                case ConsoleKey.RightArrow:
                    player.Move(1, 0); // Move player right
                    player.CharRepresentation = '►'; // Set player character to right arrow
                    // Start timer thread if not already started
                    if (!GameEngine.Instance.timerStarted) 
                    {
                        GameEngine.Instance.timerThread.Start();
                        GameEngine.Instance.timerStarted = true;
                    }
                    break;
                case ConsoleKey.Q:
                    Console.Clear(); // Clear the console
                    Environment.Exit(0); // Exit the application
                    break;
                default:
                    break;
            }
        }
    }
}
