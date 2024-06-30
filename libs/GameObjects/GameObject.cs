using System;
using System.IO;
using System.Text.Json;

namespace libs
{
    // GameObject class implementing multiple interfaces
    public class GameObject : IGameObject, IMovement, ICloneable
    {
        // Private fields
        private char _charRepresentation = '#';
        private ConsoleColor _color;
        private int _posX;
        private int _posY;

        // Public properties
        public GameObjectType Type; // Type of the game object
        public string question;     // Question associated with an InteractableGameObject
        public string answer;       // Answer associated with an InteractableGameObject
        public int idx;             // Index associated with an InteractableGameObject

        // Default constructor
        public GameObject()
        {
            this._posX = 0;
            this._posY = 0;
            this._color = ConsoleColor.Gray;
        }

        // Constructor with position parameters
        public GameObject(int posX, int posY)
        {
            this._posX = posX;
            this._posY = posY;
        }

        // Constructor with position and type parameters
        public GameObject(int posX, int posY, GameObjectType type)
        {
            this._posX = posX;
            this._posY = posY;
            this.Type = type;
        }

        // Constructor with index, question, answer, and type parameters (for InteractableGameObject)
        public GameObject(int idx, string question, string answer, GameObjectType type)
        {
            this.idx = idx;
            this.question = question;
            this.answer = answer;
            this.Type = type;
        }

        // Method to clone the object
        public object Clone()
        {
            return MemberwiseClone();
        }

        // Property for character representation of the object
        public char CharRepresentation
        {
            get { return _charRepresentation; }
            set { _charRepresentation = value; }
        }

        // Property for console color of the object
        public ConsoleColor Color
        {
            get { return _color; }
            set { _color = value; }
        }

        // Property for X position of the object
        public int PosX
        {
            get { return _posX; }
            set { _posX = value; }
        }

        // Property for Y position of the object
        public int PosY
        {
            get { return _posY; }
            set { _posY = value; }
        }

        // Method to interact with the object
        public void Interact()
        {
            // Handle interaction based on the type of the object
            if (Type == GameObjectType.InteractableGameObject)
            {
                // Display question for InteractableGameObject
                GameEngine.Instance.SetDialogMessage("output", "Q" + (idx + 1).ToString() + ": " + question);
            }
            else if (Type == GameObjectType.Exit)
            {
                // Prompt user for answer at Exit
                GameEngine.Instance.SetDialogMessage("input", "Do you know the answer? Type it into the console to escape the room!");
            }
            else
            {
                // Default message for other types of objects
                GameEngine.Instance.SetDialogMessage("output", "Remember all answers to escape the room!");
            }
        }

        // Method to move the object
        public void Move(int dx, int dy)
        {
            // Interact with the target position on the map
            GameEngine.Instance.GetMap().Get(_posY + dy, _posX + dx).Interact();

            // Handle movement based on the type of the target position
            if (GameEngine.Instance.GetMap().Get(_posY + dy, _posX + dx).Type == GameObjectType.Obstacle ||
                GameEngine.Instance.GetMap().Get(_posY + dy, _posX + dx).Type == GameObjectType.InteractableGameObject)
            {
                return; // Cannot move if obstructed by obstacle or InteractableGameObject
            }

            if (GameEngine.Instance.GetMap().Get(_posY + dy, _posX + dx).Type == GameObjectType.Exit)
            {
                // Handle movement to Exit object
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
                    return; // Cannot exit if not all conditions are met
                }
            }

            // Move the object
            _posX += dx;
            _posY += dy;
        }
    }
}
