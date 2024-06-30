namespace libs
{
    // Interface defining a factory for creating GameObject instances
    public interface IGameObjectFactory
    {
        // Method to create a GameObject from a dynamic object
        public GameObject CreateGameObject(dynamic obj);
    }
}
