namespace libs;

public class Box : GameObject {

    public Box () : base(){
        Type = GameObjectType.Box;
        CharRepresentation = '■';
        Color = ConsoleColor.Cyan;
    }
}