using System;

namespace libs
{
    public class InteractableGameObject : GameObject
    {
        private static Random random = new Random();

        public InteractableGameObject(int idx, string question, string answer) : base(idx, question, answer, GameObjectType.InteractableGameObject)
        {
            Type = GameObjectType.InteractableGameObject;
            CharRepresentation = '☺';
            Color = GetRandomConsoleColor();
        }

        private ConsoleColor GetRandomConsoleColor()
        {
            ConsoleColor[] consoleColors = (ConsoleColor[])ConsoleColor.GetValues(typeof(ConsoleColor));
            return consoleColors[random.Next(consoleColors.Length)];
        }
    }
}