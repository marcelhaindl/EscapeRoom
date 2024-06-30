namespace libs;

// Player class that inherits from GameObject
public class Player : GameObject
{
    // Singleton instance of Player
    private static Player? _instance;

    // Public property to get the singleton instance of Player
    public static Player Instance
    {
        get
        {
            // If instance is null, create a new Player instance
            if (_instance == null)
            {
                _instance = new Player();
            }
            return _instance;
        }
    }

    // Constructor to initialize Player properties
    public Player() : base()
    {
        // Set the type of the game object to Player
        Type = GameObjectType.Player;
        // Set the character representation for the Player
        CharRepresentation = '►';
        // Set the color for the Player
        Color = ConsoleColor.DarkBlue;
    }
}
