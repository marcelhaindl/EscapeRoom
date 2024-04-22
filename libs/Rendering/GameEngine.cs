﻿using System.Reflection.Metadata.Ecma335;
using System.Text.Json.Nodes;
using System.Security.Cryptography;
using Newtonsoft.Json;

namespace libs;

public sealed class GameEngine
{
    private static GameEngine? _instance;
    private IGameObjectFactory gameObjectFactory;

    public static GameEngine Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new GameEngine();
            }
            return _instance;
        }
    }

    private GameEngine()
    {
        //INIT PROPS HERE IF NEEDED
        gameObjectFactory = new GameObjectFactory();
    }

    private GameObject? _player;

    private Map map = new Map();

    private List<GameObject> gameObjects = new List<GameObject>();

    public Map GetMap()
    {
        return map;
    }

    public void ClearGameobjects()
    {
        gameObjects.Clear();
    }

    public GameObject GetPlayer()
    {
        return _player;
    }

    public void Setup()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.WriteLine("Setting up game...");

        try
        {
            // Correct usage of static class method
            dynamic gameData = FileHandler.ReadJson();

            if (gameData == null)
            {
                Console.WriteLine("Error: Game data could not be loaded.");
                return;
            }

            map.MapWidth = gameData.map.width;
            map.MapHeight = gameData.map.height;

            dynamic player = gameData.player;
            List<dynamic> walls = new List<dynamic>(gameData.walls);
            dynamic exit = gameData.exit;

            player.Type = GameObjectType.Player;
            exit.Type = GameObjectType.Exit;

            AddGameObject(CreateGameObject(player));
            AddGameObject(CreateGameObject(exit));

            foreach (var wall in gameData.walls)
            {
                wall.Type = GameObjectType.Obstacle;
                AddGameObject(CreateGameObject(wall));
            }

            if (_player == null)
            {
                Console.WriteLine("Error: No player object found in game data.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred during setup: {ex.Message}");

        }
    }

    public void Render()
    {
        Console.Clear();

        map.Initialize();

        PlaceGameObjects();

        //Render the map
        for (int i = 0; i < map.MapHeight; i++)
        {
            for (int j = 0; j < map.MapWidth; j++)
            {
                DrawObject(map.Get(i, j));
            }
            Console.WriteLine();
        }
    }


    // Method to create GameObject using the factory from clients
    public GameObject CreateGameObject(dynamic obj)
    {
        return gameObjectFactory.CreateGameObject(obj);
    }

    public void AddGameObject(GameObject gameObject)
    {
        gameObjects.Add(gameObject);
    }

    private void PlaceGameObjects()
    {
        gameObjects.ForEach(delegate (GameObject obj)
        {
            map.Set(obj);
        });
        _player = gameObjects.Find(x => x.Type == GameObjectType.Player);
        map.Set(_player);
    }

    private void DrawObject(GameObject gameObject)
    {
        Console.ResetColor();

        if (gameObject != null)
        {
            Console.ForegroundColor = gameObject.Color;
            Console.Write(gameObject.CharRepresentation);
        }

        else
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write(' ');
        }
    }
}