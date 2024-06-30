namespace libs; // Define the namespace

using Newtonsoft.Json; // Import the Newtonsoft.Json library for JSON serialization

public class Map
{
    // Two-dimensional arrays representing the map layers
    private char[,] RepresentationalLayer; // Layer to represent characters on the map
    private GameObject?[,] GameObjectLayer; // Layer to hold game objects

    // Variables to store the map's dimensions
    private int _mapWidth;
    private int _mapHeight;

    // Default constructor that initializes the map with a default size
    public Map()
    {
        _mapWidth = 10;
        _mapHeight = 10;
        RepresentationalLayer = new char[_mapHeight, _mapWidth];
        GameObjectLayer = new GameObject[_mapHeight, _mapWidth];
    }

    // Constructor that initializes the map with specified dimensions
    public Map(int width, int height)
    {
        _mapWidth = width;
        _mapHeight = height;
        RepresentationalLayer = new char[_mapHeight, _mapWidth];
        GameObjectLayer = new GameObject[_mapHeight, _mapWidth];
    }

    // Method to initialize the map layers and populate with default values
    public void Initialize()
    {
        RepresentationalLayer = new char[_mapHeight, _mapWidth];
        GameObjectLayer = new GameObject[_mapHeight, _mapWidth];

        // Loop through the map and fill with default Floor objects
        for (int i = 0; i < GameObjectLayer.GetLength(0); i++)
        {
            for (int j = 0; j < GameObjectLayer.GetLength(1); j++)
            {
                GameObjectLayer[i, j] = new Floor();
            }
        }
    }

    // Property to get and set the map width
    public int MapWidth
    {
        get { return _mapWidth; } // Getter
        set { _mapWidth = value; Initialize(); } // Setter that reinitializes the map
    }

    // Property to get and set the map height
    public int MapHeight
    {
        get { return _mapHeight; } // Getter
        set { _mapHeight = value; Initialize(); } // Setter that reinitializes the map
    }

    // Method to get a game object at a specific position
    public GameObject Get(int x, int y)
    {
        return GameObjectLayer[x, y];
    }

    // Method to place a game object on the map
    public void Set(GameObject gameObject)
    {
        if (gameObject != null) // Check if the game object is not null
        {
            int posY = gameObject.PosY;
            int posX = gameObject.PosX;

            // Check if the position is within the map bounds
            if (posX >= 0 && posX < _mapWidth &&
                    posY >= 0 && posY < _mapHeight)
            {
                // Place the game object on the map layers
                GameObjectLayer[posY, posX] = gameObject;
                RepresentationalLayer[gameObject.PosY, gameObject.PosX] = gameObject.CharRepresentation;
            }
        }
    }
}
