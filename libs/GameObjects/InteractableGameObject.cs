namespace libs;

public class InteractableGameObject : GameObject {

    public InteractableGameObject () : base(){
        Type = GameObjectType.InteractableGameObject;
        CharRepresentation = '☺';
        Color = ConsoleColor.Red;
    }
}