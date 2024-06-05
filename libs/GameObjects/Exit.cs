namespace libs;

public class Exit : GameObject {
    public Exit () : base(){
        Type = GameObjectType.Exit;
        CharRepresentation = '█';
        Color = ConsoleColor.Red;
    }
}