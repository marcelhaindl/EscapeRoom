using System;

namespace libs
{
    public class Menu
    {
        public void DisplayMenu()
        {
            Console.WriteLine("Escape Room Game ()");
            Console.WriteLine("1. START");
            Console.WriteLine("2. LOAD");
            Console.WriteLine("3. SAVE");
            Console.WriteLine("4. QUIT");

            string playerInput = Console.ReadLine();
            string choice = playerInput.ToLower();

            switch (choice)
            {
                case "1" or "start":
                    StartGame();
                    break;
                case "2" or "load":
                    LoadGame();
                    break;
                case "3" or "save":
                    SaveGame();
                    break;
                case "4" or "quit":
                    ExitGame();
                    break;
                default:
                    Console.WriteLine("Invalid choice. Please try again.");
                    DisplayMenu();
                    break;
            }
        }

        private void StartGame()
        {
            // Implement game start logic
            Console.WriteLine("Starting game...");
        }

        private void LoadGame()
        {
            // Implement game load logic
            Console.WriteLine("Loading game...");
        }

        private void SaveGame()
        {
            // Implement game save logic
            Console.WriteLine("Saving game...");
        }

        private void ExitGame()
        {
            Environment.Exit(0);
        }
    }
}
