using System.Reflection.Metadata.Ecma335;
using System.Text.Json.Nodes;
using System.Security.Cryptography;
using Newtonsoft.Json;

namespace libs;

public sealed class GameEngine
{
    private static GameEngine? _instance;
    private IGameObjectFactory gameObjectFactory;
    private string _dialogMessage = "Hello you, welcome to Escape Room! Use the arrow keys to move the player. Press 'q' to quit.";
    private string _type = "output";
    public bool won = false;
    private bool initialsPlaced = false;

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

    public void SetDialogMessage(string type, string message)
    {
        _type = type;
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

            player.Type = GameObjectType.Player;

            AddGameObject(CreateGameObject(player));

            foreach (var wall in gameData.walls)
            {
                wall.Type = GameObjectType.Obstacle;
                AddGameObject(CreateGameObject(wall));
            }

            GenerateExit();

            int lengthOfInteractableGameObjects = gameData.interactableGameObjects.Count;
            List<int> shuffledIndexArray = ShuffleIndexArray(lengthOfInteractableGameObjects).ToList();

            foreach (var interactableGameObject in gameData.interactableGameObjects)
            {
                interactableGameObject.Type = GameObjectType.InteractableGameObject;
                interactableGameObject.idx = shuffledIndexArray[gameData.interactableGameObjects.IndexOf(interactableGameObject)];
                AddGameObject(CreateGameObject(interactableGameObject));
            }

            GenerateObstacles();

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
        DrawDialog(_type, _dialogMessage);
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
        // Loop through all game objects and place them on the map
        gameObjects.ForEach(delegate (GameObject obj)
        {
            if (obj.Type != GameObjectType.InteractableGameObject)
            {
                map.Set(obj);
            }
        });
        // Place the interactable game objects on the map with a random position where no other object is placed
        List<GameObject> arrayOfInteractableGameObjects = gameObjects.FindAll(x => x.Type == GameObjectType.InteractableGameObject);
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

    private void DrawDialog(string type, string message)
    {
        int messageLength = message.Length;
        int borderLength = 38;
        int totalHeight = 5 + (messageLength / borderLength) * 2;

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
            if (correct)
            {
                SetDialogMessage("output", "Correct! The door is now open.");
                won = true;
            }
            else
            {
                SetDialogMessage("output", "Incorrect! Try again.");
                won = false;
            }
            Render();
        }
    }

    private bool CheckCode(string code)
    {
        List<GameObject> arrayOfInteractableGameObjects = gameObjects.FindAll(x => x.Type == GameObjectType.InteractableGameObject);
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


    private (int x, int y) getRandomPosition()
    {
        Map map = GameEngine.Instance.GetMap();
        Random random = new Random();
        // Get Exit
        GameObject exitObject = gameObjects.Find(x => x.Type == GameObjectType.Exit);
        // Get Player
        GameObject playerObject = gameObjects.Find(x => x.Type == GameObjectType.Player);
        // Get Positions Around Exit
        List<(int, int)> positionsAroundExit = GetPositionsAroundObject(exitObject);
        // Get Positions Around Player
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

    // Function that randomly generates small blocks of 2x2, 2x3, 3x2, 3x3, 4,2, 4x3, 4x4, 2x4, or 3x4 of obstacles on the map
    public void GenerateObstacles()
    {
        Random random = new Random();
        int mapWidth = map.MapWidth;
        int mapHeight = map.MapHeight;

        for (int i = 0; i < mapHeight; i++)
        {
            for (int j = 0; j < mapWidth; j++)
            {
                if (random.Next(0, 100) < 5) // Percentage chance of generating an obstacle
                {
                    int obstacleWidth = random.Next(1, 3);  // Width of an obstacle
                    int obstacleHeight = random.Next(1, 3); // Height of an obstacle

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

                                // Get Exit
                                GameObject exitObject = gameObjects.Find(x => x.Type == GameObjectType.Exit);
                                // Get Player
                                GameObject playerObject = gameObjects.Find(x => x.Type == GameObjectType.Player);
                                // Get Positions Around Exit
                                List<(int, int)> positionsAroundExit = GetPositionsAroundObject(exitObject);
                                // Get Positions Around Player
                                List<(int, int)> positionsAroundPlayer = GetPositionsAroundObject(playerObject);

                                // Check if the obstacle is placed around the exit or the player
                                if (positionsAroundExit.Contains((l, k)) || positionsAroundPlayer.Contains((l, k)))
                                {
                                    continue;
                                }

                                // Remove the object on this certain position
                                gameObjects.RemoveAll(obj => obj.PosX == l && obj.PosY == k);
                                AddGameObject(obstacle);
                                map.Set(obstacle);
                            }
                        }
                    }
                }
            }
        }
    }

    private List<(int, int)> GetPositionsAroundObject(GameObject obj)
    {
        List<(int, int)> positions = new List<(int, int)>();

        int x = obj.PosX;
        int y = obj.PosY;

        // Add original position
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

    // Function that randomly creates an exit on one of the four sides of the map and adds the exit to the game objects list
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
        // Remove the object on this certain position
        gameObjects.RemoveAll(obj => obj.PosX == exit.PosX && obj.PosY == exit.PosY);
        AddGameObject(exit);
    }
}