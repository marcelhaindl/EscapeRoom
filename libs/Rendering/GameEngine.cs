using System.Reflection.Metadata.Ecma335;
using System.Text.Json.Nodes;
using System.Security.Cryptography;
using Newtonsoft.Json;
using System.Timers;

namespace libs;

public sealed class GameEngine
{
    private static GameEngine? _instance;
    private IGameObjectFactory gameObjectFactory;
    private string _dialogMessage = "Hello you, welcome to Escape Room! Use the arrow keys to move the player. Press 'q' to quit.";

    public static GameEngine Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new GameEngine();
            }
            return _instance;
        }
    }

    private GameEngine()
    {
        //INIT PROPS HERE IF NEEDED
        gameObjectFactory = new GameObjectFactory();
    }

    private GameObject? _player;

    private Map map = new Map();

    private List<GameObject> gameObjects = new List<GameObject>();

// Timer -----------------------------
    private System.Timers.Timer timer;
    private const int totalTime = 180000; // 3 minutes in milliseconds
    private int elapsedTime = 0;
// -----------------------------------

    public Map GetMap()
    {
        return map;
    }

    public void ClearGameobjects()
    {
        gameObjects.Clear();
    }

    public GameObject GetPlayer()
    {
        return _player;
    }

    public void SetDialogMessage(string message)
    {
        _dialogMessage = message;
    }
    public string GetDialogMessage()
    {
        return _dialogMessage;
    }

    public void Setup()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.WriteLine("Setting up game...");

        try
        {
            // Correct usage of static class method
            dynamic gameData = FileHandler.ReadJson();

            if (gameData == null)
            {
                Console.WriteLine("Error: Game data could not be loaded.");
                return;
            }

            map.MapWidth = gameData.map.width;
            map.MapHeight = gameData.map.height;

            dynamic player = gameData.player;
            List<dynamic> walls = new List<dynamic>(gameData.walls);
            dynamic exit = gameData.exit;
            List<dynamic> interactableGameObjects = new List<dynamic>(gameData.interactableGameObjects);

            player.Type = GameObjectType.Player;
            exit.Type = GameObjectType.Exit;

            AddGameObject(CreateGameObject(player));
            AddGameObject(CreateGameObject(exit));

            foreach (var wall in gameData.walls)
            {
                wall.Type = GameObjectType.Obstacle;
                AddGameObject(CreateGameObject(wall));
            }

            foreach (var interactableGameObject in gameData.interactableGameObjects)
            {
                interactableGameObject.Type = GameObjectType.InteractableGameObject;
                AddGameObject(CreateGameObject(interactableGameObject));
            }

            if (_player == null)
            {
                Console.WriteLine("Error: No player object found in game data.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred during setup: {ex.Message}");
        }

// Timer -----------------------------------------------------------------------------------
        timer = new System.Timers.Timer();

        timer.Interval = 1000; 
        timer.Elapsed += TimerElapsed;

        timer.Start();
    }

    private void TimerElapsed(object sender, System.Timers.ElapsedEventArgs e)
    {
        elapsedTime += 1000;

        if (elapsedTime >= totalTime)
        {
            timer.Stop();
            Console.WriteLine("Game over, Ran out of time!");
            FinishGame();
        }
    }

    private void FinishGame()
    {
        Environment.Exit(0); 
    }

// ------------------------------------------------------------------------

    public void Render()
    {
        Console.Clear();

        map.Initialize();

        PlaceGameObjects();

        //Render the map
        for (int i = 0; i < map.MapHeight; i++)
        {
            for (int j = 0; j < map.MapWidth; j++)
            {
                DrawObject(map.Get(i, j));
            }
            Console.WriteLine();
        }
        Console.WriteLine();
        DrawDialog(_dialogMessage);
    }


    // Method to create GameObject using the factory from clients
    public GameObject CreateGameObject(dynamic obj)
    {
        return gameObjectFactory.CreateGameObject(obj);
    }

    public void AddGameObject(GameObject gameObject)
    {
        gameObjects.Add(gameObject);
    }

    private void PlaceGameObjects()
    {
        gameObjects.ForEach(delegate (GameObject obj)
        {
            map.Set(obj);
        });
        _player = gameObjects.Find(x => x.Type == GameObjectType.Player);
        map.Set(_player);
    }

    private void DrawObject(GameObject gameObject)
    {
        Console.ResetColor();

        if (gameObject != null)
        {
            Console.ForegroundColor = gameObject.Color;
            Console.Write(gameObject.CharRepresentation);
        }

        else
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write(' ');
        }
    }

    private void DrawDialog(string message)
    {
        int messageLength = message.Length;
        int borderLength = 38;
        int totalHeight = 5 + (messageLength / borderLength) * 2;

        Console.WriteLine(new string('+', borderLength + 4));

        Console.WriteLine(string.Format("+ {0,-" + borderLength + "} +", ""));

        for (int i = 0; i < messageLength; i += borderLength)
        {
            string line = message.Substring(i, Math.Min(borderLength, messageLength - i));
            string formattedLine = string.Format("+ {0,-" + borderLength + "} +", line);
            Console.WriteLine(formattedLine);
        }

        Console.WriteLine(string.Format("+ {0,-" + borderLength + "} +", ""));

        Console.WriteLine(new string('+', borderLength + 4));
    }
}