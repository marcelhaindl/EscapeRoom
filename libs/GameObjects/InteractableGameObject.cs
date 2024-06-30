using System;

namespace libs
{
    // InteractableGameObject class that inherits from GameObject
    public class InteractableGameObject : GameObject
    {
        // Static Random instance to generate random values
        private static Random random = new Random();

        // Constructor to initialize InteractableGameObject properties
        public InteractableGameObject(int idx, string question, string answer) 
            : base(idx, question, answer, GameObjectType.InteractableGameObject)
        {
            // Set the type of the game object to InteractableGameObject
            Type = GameObjectType.InteractableGameObject;
            // Set the character representation for the InteractableGameObject
            CharRepresentation = '☺';
            // Set a random color for the InteractableGameObject
            Color = GetRandomConsoleColor();
        }

        // Private method to get a random ConsoleColor
        private ConsoleColor GetRandomConsoleColor()
        {
            // Get all possible ConsoleColor values
            ConsoleColor[] consoleColors = (ConsoleColor[])ConsoleColor.GetValues(typeof(ConsoleColor));
            // Return a random ConsoleColor from the array
            return consoleColors[random.Next(consoleColors.Length)];
        }
    }
}
