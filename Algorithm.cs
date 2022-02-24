using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Hashcode2022
{
    class Algorithm
    {
        private readonly List<string> files = new List<string>();

        public Algorithm(string folderPath)
        {
            foreach (var file in Directory.GetFiles(folderPath))
            {
                if (file.EndsWith(".txt")) files.Add(file);
            }
        }

        public void Run()
        {
            Task.WaitAll(files.Select(AsyncRun).ToArray());
            Console.WriteLine("Done!");
        }

        private async Task AsyncRun(string file)
        {
            string outputPath = file.Substring(0, file.Length - 2) + "out";
            string name = "[" + file.Substring(file.LastIndexOf(Path.DirectorySeparatorChar) + 1, file.Length - file.LastIndexOf(Path.DirectorySeparatorChar) - 1) + "]";

            await Task.Run(() =>
            {
                try
                {
                    TimeHandler.StartTimer(name);
                    Console.WriteLine($"{name} Processing...");

                    var input = IOHandler.LoadFile(file);
                    var output = RunAlgorithm(input);
                    IOHandler.SaveFile(outputPath, output);

                    Console.WriteLine($"{name} Done! (Took: " + (TimeHandler.EndTimer(name)) + "ms to complete)");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"{name} Failed!\n    Message: " + ex.Message + "\n    StackTrace: " +
                                      ex.StackTrace);
                }
            });
        }

        private List<string> RunAlgorithm(List<string> input)
        {
            // Write magic stuff here :O
            return input;
        }
    }
}