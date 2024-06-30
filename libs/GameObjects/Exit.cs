namespace libs
{
    // Exit class inheriting from GameObject
    public class Exit : GameObject
    {
        // Constructor
        public Exit() : base()
        {
            // Set type, character representation, and color
            Type = GameObjectType.Exit;
            CharRepresentation = '█';
            Color = ConsoleColor.Red;
        }
    }
}
