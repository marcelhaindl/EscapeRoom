using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace libs
{
    public class Dialog
    {
        private Dictionary<int, DialogNode> dialogNodes;

        public Dialog(string filePath)
        {
            LoadDialog(filePath);
        }

        private void LoadDialog(string filePath)
        {
            string json = File.ReadAllText(filePath);
            dialogNodes = JsonConvert.DeserializeObject<Dictionary<int, DialogNode>>(json);
        }

        public void StartDialog()
        {
            int currentNodeId = 0;

            while (true)
            {
                var currentNode = dialogNodes[currentNodeId];
                Console.WriteLine(currentNode.Text);

                if (currentNode.Choices == null || currentNode.Choices.Length == 0)
                    break;

                for (int i = 0; i < currentNode.Choices.Length; i++)
                {
                    Console.WriteLine($"{i + 1}. {dialogNodes[currentNode.Choices[i]].Text}");
                }

                int choice = int.Parse(Console.ReadLine()) - 1;
                currentNodeId = currentNode.Choices[choice];
            }
        }
    }
}
