using System.IO;
using System.Text.Json;

namespace libs;

public class GameObject : IGameObject, IMovement, ICloneable
{
    private char _charRepresentation = '#';

    private ConsoleColor _color;

    private int _posX;
    private int _posY;
    public GameObjectType Type;

    public string question;
    public string answer;
    public int idx;

    public GameObject()
    {
        this._posX = 0;
        this._posY = 0;
        this._color = ConsoleColor.Gray;
    }

    public GameObject(int posX, int posY)
    {
        this._posX = posX;
        this._posY = posY;
    }

    public GameObject(int posX, int posY, GameObjectType type)
    {
        this._posX = posX;
        this._posY = posY;
        this.Type = type;
    }

    public GameObject(int idx, string question, string answer, GameObjectType type) {
        this.idx = idx;
        this.question = question;
        this.answer = answer;
        this.Type = type;
    }

    public object Clone()
    {
        return MemberwiseClone();
    }

    public char CharRepresentation
    {
        get { return _charRepresentation; }
        set { _charRepresentation = value; }
    }

    public ConsoleColor Color
    {
        get { return _color; } 
        set { _color = value; } 
    }

    public int PosX
    {
        get { return _posX; } 
        set { _posX = value; } 
    }

    public int PosY
    {
        get { return _posY; } 
        set { _posY = value; } 
    }


    public void Interact()
    {
        if (Type == GameObjectType.InteractableGameObject)
        {
            GameEngine.Instance.SetDialogMessage("output", "Q" + (idx + 1).ToString() + ": " + question);
        }
        else if (Type == GameObjectType.Exit)
        {
            GameEngine.Instance.SetDialogMessage("input", "Do you know the answer? Type it into the console to escape the room!");
        }
        else
        {
            GameEngine.Instance.SetDialogMessage("output", "Remember all answers to escape the room!");
        }
    }

    public void Move(int dx, int dy)
    {
        GameEngine.Instance.GetMap().Get(_posY + dy, _posX + dx).Interact();
        if (GameEngine.Instance.GetMap().Get(_posY + dy, _posX + dx).Type == GameObjectType.Obstacle || GameEngine.Instance.GetMap().Get(_posY + dy, _posX + dx).Type == GameObjectType.InteractableGameObject)
        {
            return;
        }
        if (GameEngine.Instance.GetMap().Get(_posY + dy, _posX + dx).Type == GameObjectType.Exit)
        {
            if (GameEngine.Instance.won)
            {
                _posX += dx;
                _posY += dy;
                GameEngine.Instance.SetDialogMessage("output", "You have escaped the room! Congratulations!");
                GameEngine.Instance.Render();
                Console.ReadKey();
                Environment.Exit(0);
            }
            else
            {
                return;
            }
        }

        _posX += dx;
        _posY += dy;
    }
}
