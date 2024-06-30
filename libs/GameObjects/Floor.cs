namespace libs
{
    // Floor class inheriting from GameObject
    public class Floor : GameObject
    {
        // Constructor
        public Floor() : base()
        {
            // Set type and character representation
            Type = GameObjectType.Floor;
            CharRepresentation = '.';
        }
    }
}
