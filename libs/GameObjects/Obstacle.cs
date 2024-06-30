namespace libs;

// Obstacle class that inherits from GameObject
public class Obstacle : GameObject 
{
    // Constructor to initialize Obstacle properties
    public Obstacle() : base() 
    {
        // Set the type of the game object to Obstacle
        this.Type = GameObjectType.Obstacle;
        // Set the character representation for the Obstacle
        this.CharRepresentation = '█';
        // Set the color for the Obstacle
        this.Color = ConsoleColor.White;
    }
}
