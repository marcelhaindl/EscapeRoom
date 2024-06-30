namespace libs
{
    // Factory class for creating GameObject instances
    public class GameObjectFactory : IGameObjectFactory
    {
        // Method to create a GameObject from a dynamic object
        public GameObject CreateGameObject(dynamic obj)
        {
            // Check if the provided object is null
            if (obj == null)
            {
                Console.WriteLine("Invalid game object data");
                return null; // Return null if the object is null
            }
            
            // Retrieve the type of the game object
            int type = (int)obj.Type;


            GameObject newObj = type switch
            {
                // Create a Player object from the dynamic object
                (int)GameObjectType.Player => obj.ToObject<Player>(),
                // Create an Obstacle object from the dynamic object
                (int)GameObjectType.Obstacle => obj.ToObject<Obstacle>(),
                // Create an InteractableGameObject object from the dynamic object
                (int)GameObjectType.InteractableGameObject => obj.ToObject<InteractableGameObject>(),
                // Create an Exit object from the dynamic object
                (int)GameObjectType.Exit => obj.ToObject<Exit>(),
                _ => null // Default case for unknown game object types, returns null for unknown types
            };

            if (newObj == null)
            {
                // Handle unknown game object types
                Console.WriteLine("Unknown GameObject type");
            }

            // Return the created game object
            return newObj;
        }
    }
}
