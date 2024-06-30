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

            // Initialize a new GameObject instance
            GameObject newObj = new GameObject();
            // Retrieve the type of the game object
            int type = (int)obj.Type;

            // Create specific game object types based on the type value
            switch (type)
            {
                case (int)GameObjectType.Player:
                    // Create a Player object from the dynamic object
                    newObj = obj.ToObject<Player>();
                    break;
                case (int)GameObjectType.Obstacle:
                    // Create an Obstacle object from the dynamic object
                    newObj = obj.ToObject<Obstacle>();
                    break;
                case (int)GameObjectType.InteractableGameObject:
                    // Create an InteractableGameObject object from the dynamic object
                    newObj = obj.ToObject<InteractableGameObject>();
                    break;
                case (int)GameObjectType.Exit:
                    // Create an Exit object from the dynamic object
                    newObj = obj.ToObject<Exit>();
                    break;
                default:
                    // Handle unknown game object types
                    Console.WriteLine("Unknown GameObject type");
                    return null; // Return null for unknown types
            }

            // Return the created game object
            return newObj;
        }
    }
}
