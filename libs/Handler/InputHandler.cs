namespace libs;

public sealed class InputHandler
{

    private static InputHandler? _instance;
    private GameEngine engine;

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

    private InputHandler()
    {
        //INIT PROPS HERE IF NEEDED
        engine = GameEngine.Instance;
    }

    public void Handle(ConsoleKeyInfo keyInfo)
    {
        GameObject player = engine.GetPlayer();

        if (player != null)
        {
            // Handle keyboard input to move the player
            switch (keyInfo.Key)
            {
                case ConsoleKey.UpArrow:
                    player.Move(0, -1);
                    player.CharRepresentation = '▲';
                    if(!GameEngine.Instance.timerStarted) {
                        GameEngine.Instance.timerThread.Start();
                        GameEngine.Instance.timerStarted = true;
                    }
                    break;
                case ConsoleKey.DownArrow:
                    player.Move(0, 1);
                    player.CharRepresentation = '▼';
                    if(!GameEngine.Instance.timerStarted) {
                        GameEngine.Instance.timerThread.Start();
                        GameEngine.Instance.timerStarted = true;
                    }
                    break;
                case ConsoleKey.LeftArrow:
                    player.Move(-1, 0);
                    player.CharRepresentation = '◄';
                    if(!GameEngine.Instance.timerStarted) {
                        GameEngine.Instance.timerThread.Start();
                        GameEngine.Instance.timerStarted = true;
                    }
                    break;
                case ConsoleKey.RightArrow:
                    player.Move(1, 0);
                    player.CharRepresentation = '►';
                    if(!GameEngine.Instance.timerStarted) {
                        GameEngine.Instance.timerThread.Start();
                        GameEngine.Instance.timerStarted = true;
                    }
                    break;
                case ConsoleKey.Q:
                    Console.Clear();
                    Environment.Exit(0);
                    break;
                default:
                    break;
            }
        }

    }

}