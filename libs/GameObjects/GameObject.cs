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

    public string question = "";
    public string answer = "";

    public GameObject()
    {
        this._posX = 5;
        this._posY = 5;
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

    public GameObject(int posX, int posY, GameObjectType type, string question, string answer)
    {
        this._posX = posX;
        this._posY = posY;
        this.Type = type;
        this.question = question;
        this.answer = answer;
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
        get { return _color; }  // Corrected from 'color' to '_color'
        set { _color = value; } // Corrected from 'color' to '_color'
    }

    public int PosX
    {
        get { return _posX; }  // Corrected from 'posX' to '_posX'
        set { _posX = value; } // Corrected from 'posX' to '_posX'
    }

    public int PosY
    {
        get { return _posY; }  // Corrected from 'posY' to '_posY'
        set { _posY = value; } // Corrected from 'posY' to '_posY'
    }

    public void Interact()
    {
        if (Type == GameObjectType.InteractableGameObject)
        {
            GameEngine.Instance.SetDialogMessage(question);
        } else {
            GameEngine.Instance.SetDialogMessage("This object is not interactable.");
        }
    }

    public void Move(int dx, int dy)
    {
        if (GameEngine.Instance.GetMap().Get(_posY + dy, _posX + dx).Type == GameObjectType.Obstacle || GameEngine.Instance.GetMap().Get(_posY + dy, _posX + dx).Type == GameObjectType.Exit)
        {
            return;
        }
        GameEngine.Instance.GetMap().Get(_posY + dy, _posX + dx).Interact();
        if (GameEngine.Instance.GetMap().Get(_posY + dy, _posX + dx).Type == GameObjectType.InteractableGameObject)
        {
            return;
        }

        _posX += dx;
        _posY += dy;
    }
}
