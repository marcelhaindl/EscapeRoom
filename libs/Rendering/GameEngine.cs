using System.Reflection.Metadata.Ecma335;
using System.Text.Json.Nodes;
using System.Security.Cryptography;
using Newtonsoft.Json;

namespace libs;

public sealed class GameEngine
{
    private static GameEngine? _instance; // Singleton instance of the GameEngine
    private IGameObjectFactory gameObjectFactory; // Factory to create game objects
    private string _dialogMessage = "Hello you, welcome to Escape Room! Complete the code within the time limit to escape! Use the arrow keys to move. Press q to quit."; // Default dialog message
    private string _type = "output"; // Type of dialog message
    public bool won = false; // Flag indicating if the player has won
    private int _remainingTimeInSeconds = 300; // Remaining time in seconds
    public Thread timerThread; // Thread for the countdown timer
    private bool _firstStart = true; // Flag indicating the first start of the game
    private int _amountOfRooms = 2; // Number of rooms/levels in the game
    private int _currentRoom = 1; // Current room/level the player is in
    public bool timerStarted = false; // Flag indicating if the timer has started

    // Singleton pattern implementation
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

    // Private constructor to prevent external instantiation
    private GameEngine()
    {
        gameObjectFactory = new GameObjectFactory(); // Initialize the game object factory
        timerThread = new Thread(CountdownTimer); // Initialize the countdown timer thread
    }

    private GameObject? _player; // Reference to the player object

    private Map map = new Map(); // Map of the game

    private List<GameObject> gameObjects = new List<GameObject>(); // List of game objects

    // Get the map of the game
    public Map GetMap()
    {
        return map;
    }

    // Clear the list of game objects
    public void ClearGameObjects()
    {
        gameObjects.Clear();
    }

    // Get the player object
    public GameObject GetPlayer()
    {
        return _player;
    }

    // Set the dialog message
    public void SetDialogMessage(string type, string message)
    {
        _type = type;
        _dialogMessage = message;
    }

    // Get the dialog message
    public string GetDialogMessage()
    {
        return _dialogMessage;
    }

    // Setup the game environment and initialize objects
    public void Setup()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8; // Set console encoding to UTF-8
        Console.WriteLine("Setting up game...");

        try
        {
            ClearGameObjects(); // Clear existing game objects

            // Read game data from a JSON file
            dynamic gameData = FileHandler.ReadJson();

            if (gameData == null)
            {
                Console.WriteLine("Error: Game data could not be loaded.");
                return;
            }

            // Set map dimensions
            map.MapWidth = gameData.map.width;
            map.MapHeight = gameData.map.height;

            // Create and add the player object
            dynamic player = gameData.player;
            player.Type = GameObjectType.Player;
            AddGameObject(CreateGameObject(player));

            // Create and add wall objects
            foreach (var wall in gameData.walls)
            {
                wall.Type = GameObjectType.Obstacle;
                AddGameObject(CreateGameObject(wall));
            }

            GenerateExit(); // Generate the exit object

            // Shuffle and add interactable game objects
            int lengthOfInteractableGameObjects = gameData.interactableGameObjects.Count;
            List<int> shuffledIndexArray = ShuffleIndexArray(lengthOfInteractableGameObjects).ToList();
            foreach (var interactableGameObject in gameData.interactableGameObjects)
            {
                interactableGameObject.Type = GameObjectType.InteractableGameObject;
                interactableGameObject.idx = shuffledIndexArray[gameData.interactableGameObjects.IndexOf(interactableGameObject)];
                AddGameObject(CreateGameObject(interactableGameObject));
            }

            // Set remaining time based on the number of interactable objects
            _remainingTimeInSeconds = lengthOfInteractableGameObjects * 30;

            GenerateObstacles(); // Generate random obstacles

            if (_player == null)
            {
                Console.WriteLine("Error: No player object found in game data.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred during setup: {ex.Message}");
        }
    }

    // Render the game state to the console
    public void Render()
    {
        Console.Clear();

        if (_firstStart)
        {
            DrawDialog("output", "Welcome to Escape Room! Complete the codes within the time limit to escape the rooms! Use the arrow keys to move. Press q to quit. Feel ready? Press any key to start the game.");
            _firstStart = false;
            return;
        }

        map.Initialize(); // Initialize the map

        PlaceGameObjects(); // Place game objects on the map

        // Render the map
        for (int i = 0; i < map.MapHeight; i++)
        {
            for (int j = 0; j < map.MapWidth; j++)
            {
                DrawObject(map.Get(i, j));
            }
            Console.WriteLine();
        }
        Console.WriteLine();

        // Display remaining time
        Console.ForegroundColor = _remainingTimeInSeconds < 10 ? ConsoleColor.Red : ConsoleColor.White;
        Console.WriteLine("Time Remaining: {0}:{1:00}", _remainingTimeInSeconds / 60, _remainingTimeInSeconds % 60);
        Console.WriteLine();
        DrawDialog(_type, _dialogMessage);
    }

    // Method to create a GameObject using the factory
    public GameObject CreateGameObject(dynamic obj)
    {
        return gameObjectFactory.CreateGameObject(obj);
    }

    // Add a game object to the list
    public void AddGameObject(GameObject gameObject)
    {
        gameObjects.Add(gameObject);
    }

    // Place game objects on the map
    private void PlaceGameObjects()
    {
        // Place non-interactable objects
        gameObjects.ForEach(delegate (GameObject obj)
        {
            if (obj.Type != GameObjectType.InteractableGameObject)
            {
                map.Set(obj);
            }
        });

        // Place interactable objects at random positions
        List<GameObject> arrayOfInteractableGameObjects = gameObjects.Where(gameObject => gameObject.Type == GameObjectType.InteractableGameObject).ToList();
        foreach (GameObject obj in arrayOfInteractableGameObjects)
        {
            if (obj.PosX == 0 && obj.PosY == 0)
            {
                obj.PosX = getRandomPosition().x;
                obj.PosY = getRandomPosition().y;
            }
            map.Set(obj);
        }

        // Place the player on the map
        _player = gameObjects.FirstOrDefault(gameObject => gameObject.Type == GameObjectType.Player);
        map.Set(_player);
    }

    // Draw an object on the console
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

    // Draw dialog messages on the console
    private void DrawDialog(string type, string message)
    {
        int messageLength = message.Length;
        int borderLength = 38;

        // Calculate the total height of the dialog box
        Console.WriteLine(new string('+', borderLength + 4));
        Console.WriteLine(string.Format("+ {0,-" + borderLength + "} +", ""));

        int startIndex = 0;
        while (startIndex < messageLength)
        {
            int endIndex = startIndex + borderLength;
            if (endIndex >= messageLength)
            {
                endIndex = messageLength;
            }
            else
            {
                while (endIndex > startIndex && message[endIndex] != ' ')
                {
                    endIndex--;
                }
            }

            string line = message.Substring(startIndex, endIndex - startIndex);
            string formattedLine = string.Format("+ {0,-" + borderLength + "} +", line);
            Console.WriteLine(formattedLine);

            startIndex = endIndex + 1;
        }

        Console.WriteLine(string.Format("+ {0,-" + borderLength + "} +", ""));
        Console.WriteLine(new string('+', borderLength + 4));

        if (type == "input")
        {
            Console.Write("+ ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            string inputMessage = "Input: ";
            Console.Write(inputMessage);
            Console.ResetColor();
            string res = Console.ReadLine();
            Console.WriteLine(new string('+', borderLength + 4));
            bool correct = CheckCode(res);
            if (correct && nextRoomAvailable())
            {
                _currentRoom++;
                SetDialogMessage("output", "Correct! You have escaped the room! Press ENTER to continue.");
                Setup();
            }
            else if (correct && !nextRoomAvailable())
            {
                SetDialogMessage("output", "Correct! You have escaped the last room! You have won the game! Press ENTER to exit.");
                won = true;
            }
            else
            {
                SetDialogMessage("output", "Incorrect! Try again. Press ENTER to continue.");
                won = false;
            }
            Render();
        }
    }

    // Check if the next room is available
    private bool nextRoomAvailable()
    {
        return _currentRoom < _amountOfRooms;
    }

    // Check if the provided code is correct
    private bool CheckCode(string code)
    {
        List<GameObject> arrayOfInteractableGameObjects = gameObjects.Where(gameObject => gameObject.Type == GameObjectType.InteractableGameObject).ToList();
        if (code.Length == arrayOfInteractableGameObjects.Count)
        {
            for (int i = 0; i < arrayOfInteractableGameObjects.Count; i++)
            {
                GameObject obj = arrayOfInteractableGameObjects[i];
                if (code[obj.idx].ToString() != obj.answer.ToString())
                {
                    return false;
                }
            }
            return true;
        }
        else
        {
            return false;
        }
    }

    // Get a random position on the map
    private (int x, int y) getRandomPosition()
    {
        Map map = GameEngine.Instance.GetMap();
        Random random = new Random();

        // Get the exit and player objects
        GameObject exitObject = gameObjects.FirstOrDefault(gameObject => gameObject.Type == GameObjectType.Exit);
        GameObject playerObject = gameObjects.FirstOrDefault(gameObject => gameObject.Type == GameObjectType.Player);

        // Get positions around the exit and player
        List<(int, int)> positionsAroundExit = GetPositionsAroundObject(exitObject);
        List<(int, int)> positionsAroundPlayer = GetPositionsAroundObject(playerObject);

        int xPosition = random.Next(1, map.MapWidth - 1);
        int yPosition = random.Next(1, map.MapHeight - 1);
        while (map.Get(yPosition, xPosition).Type != GameObjectType.Floor || positionsAroundExit.Contains((xPosition, yPosition)) || positionsAroundPlayer.Contains((xPosition, yPosition)))
        {
            xPosition = random.Next(1, map.MapWidth - 1);
            yPosition = random.Next(1, map.MapHeight - 1);
        }
        return (xPosition, yPosition);
    }

    // Shuffle an array of indices
    private int[] ShuffleIndexArray(int n)
    {
        if (n <= 1)
        {
            throw new ArgumentException("n must be a positive integer");
        }

        int[] numbers = Enumerable.Range(0, n).ToArray();
        Random rand = new Random();
        numbers = numbers.OrderBy(x => rand.Next()).ToArray();
        return numbers;
    }

    // Generate random obstacles on the map
    public void GenerateObstacles()
    {
        Random random = new Random();
        int mapWidth = map.MapWidth;
        int mapHeight = map.MapHeight;

        for (int i = 0; i < mapHeight; i++)
        {
            for (int j = 0; j < mapWidth; j++)
            {
                if (random.Next(0, 100) < 5) // 5% chance of generating an obstacle
                {
                    int obstacleWidth = random.Next(1, 3); // Width of the obstacle
                    int obstacleHeight = random.Next(1, 3); // Height of the obstacle

                    if (i + obstacleHeight < mapHeight && j + obstacleWidth < mapWidth)
                    {
                        for (int k = i; k < i + obstacleHeight; k++)
                        {
                            for (int l = j; l < j + obstacleWidth; l++)
                            {
                                GameObject obstacle = new GameObject();
                                obstacle.PosX = l;
                                obstacle.PosY = k;
                                obstacle.Type = GameObjectType.Obstacle;
                                obstacle.CharRepresentation = '█';
                                obstacle.Color = ConsoleColor.White;

                                // Get the exit and player objects
                                GameObject exitObject = gameObjects.FirstOrDefault(gameObject => gameObject.Type == GameObjectType.Exit);
                                GameObject playerObject = gameObjects.FirstOrDefault(gameObject => gameObject.Type == GameObjectType.Player);

                                // Get positions around the exit and player
                                List<(int, int)> positionsAroundExit = GetPositionsAroundObject(exitObject);
                                List<(int, int)> positionsAroundPlayer = GetPositionsAroundObject(playerObject);

                                // Check if the obstacle is placed around the exit or the player
                                if (positionsAroundExit.Contains((l, k)) || positionsAroundPlayer.Contains((l, k)))
                                {
                                    continue;
                                }

                                // Remove any object on this position
                                gameObjects.RemoveAll(obj => obj.PosX == l && obj.PosY == k);
                                AddGameObject(obstacle); // Add the obstacle to the list
                                map.Set(obstacle); // Set the obstacle on the map
                            }
                        }
                    }
                }
            }
        }
    }

    // Get positions around a specific object
    private List<(int, int)> GetPositionsAroundObject(GameObject obj)
    {
        List<(int, int)> positions = new List<(int, int)>();

        int x = obj.PosX;
        int y = obj.PosY;

        // Add the original position
        positions.Add((x, y));
        // Add positions to the left, right, up, and down of the object
        positions.Add((x - 1, y));
        positions.Add((x + 1, y));
        positions.Add((x, y - 1));
        positions.Add((x, y + 1));
        // Add diagonal positions
        positions.Add((x - 1, y - 1));
        positions.Add((x - 1, y + 1));
        positions.Add((x + 1, y - 1));
        positions.Add((x + 1, y + 1));

        return positions;
    }

    // Generate an exit on one of the four sides of the map
    public void GenerateExit()
    {
        Random random = new Random();
        int mapWidth = map.MapWidth;
        int mapHeight = map.MapHeight;

        int side = random.Next(0, 4); // Randomly choose a side of the map

        GameObject exit = new GameObject();
        exit.Type = GameObjectType.Exit;
        exit.CharRepresentation = '█';
        exit.Color = ConsoleColor.Red;

        // Set the position of the exit based on the chosen side
        switch (side)
        {
            case 0: // Top side
                exit.PosX = random.Next(1, mapWidth - 1);
                exit.PosY = 0;
                break;
            case 1: // Right side
                exit.PosX = mapWidth - 1;
                exit.PosY = random.Next(1, mapHeight - 1);
                break;
            case 2: // Bottom side
                exit.PosX = random.Next(1, mapWidth - 1);
                exit.PosY = mapHeight - 1;
                break;
            case 3: // Left side
                exit.PosX = 0;
                exit.PosY = random.Next(1, mapHeight - 1);
                break;
        }

        // Remove any object on this position
        gameObjects.RemoveAll(obj => obj.PosX == exit.PosX && obj.PosY == exit.PosY);
        AddGameObject(exit); // Add the exit to the list of game objects
    }

    // Countdown timer function
    private void CountdownTimer()
    {
        // Decrement the remaining time every second and render the game state
        while (_remainingTimeInSeconds > 0)
        {
            _remainingTimeInSeconds--;
            Thread.Sleep(1000);
            Render();
        }

        // If time is up, display a lose message and exit the game
        if (_remainingTimeInSeconds == 0)
        {
            SetDialogMessage("output", "Time is up! You lose.");
            Render();
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
            Environment.Exit(0);
        }
    }
}
