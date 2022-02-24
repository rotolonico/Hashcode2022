using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            string outputPath = file.Substring(0, file.Length - 3) + "out";
            string name = "[" + file.Substring(file.LastIndexOf(Path.DirectorySeparatorChar) + 1,
                file.Length - file.LastIndexOf(Path.DirectorySeparatorChar) - 1) + "]";

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
            var i = ParseInput(input);
            var o = new Output { assignments = new List<KeyValuePair<string, List<string>>>()};

            i.projects = i.projects.OrderBy(p => p.Value.bestBefore).ToList();

            foreach (var p in i.projects)
            {
                var includeProject = true;
                var currentContributors = new List<Contributor>();
                
                foreach (var r in p.Value.roles)
                {
                    var contributor = i.contributors.FirstOrDefault(c =>
                        !c.Value.isWorking && c.Value.skills.Any(s => s.Key == r.Key && s.Value >= r.Value));
                    

                    if (contributor.Equals(default(KeyValuePair<string, Contributor>)))
                    {
                        includeProject = false;
                        break;
                    }

                    contributor.Value.isWorking = true;
                    currentContributors.Add(contributor.Value);
                    
                }

                if (!includeProject)
                {
                    currentContributors.ForEach(c => c.isWorking = false);
                    continue;
                }

                o.assignments.Add(new KeyValuePair<string, List<string>>(p.Key, currentContributors.Select(c => c.id).ToList()));
                o.p++;
            }
            
            return ParseOutput(o);
        }

        private static Input ParseInput(IReadOnlyList<string> input)
        {
            var i = new Input
            {
                c = int.Parse(input[0].Split(' ')[0]), p = int.Parse(input[0].Split(' ')[1]),
                contributors = new List<KeyValuePair<string, Contributor>>(),
                projects = new List<KeyValuePair<string, Project>>()
            };

            var contributorsLeft = i.c;
            var projectsLeft = i.p;
            var currentLine = 0;

            while (contributorsLeft > 0)
            {
                currentLine++;

                var line = input[currentLine].Split(' ');
                var contributor = new Contributor {id = line[0], skills = new List<KeyValuePair<string, int>>()};

                for (var j = 0; j < int.Parse(line[1]); j++)
                {
                    currentLine++;
                    contributor.skills.Add(new KeyValuePair<string, int>(input[currentLine].Split(' ')[0],
                        int.Parse(input[currentLine].Split(' ')[1])));
                }

                i.contributors.Add(new KeyValuePair<string, Contributor>(contributor.id, contributor));

                contributorsLeft--;
            }

            while (projectsLeft > 0)
            {
                currentLine++;
                
                var line = input[currentLine].Split(' ');
                var project = new Project
                {
                    id = line[0], duration = int.Parse(line[1]), score = int.Parse(line[2]),
                    bestBefore = int.Parse(line[3]), roles = new List<KeyValuePair<string, int>>()
                };

                for (var j = 0; j < int.Parse(line[4]); j++)
                {
                    currentLine++;
                    project.roles.Add(new KeyValuePair<string, int>(input[currentLine].Split(' ')[0],
                        int.Parse(input[currentLine].Split(' ')[1])));
                }

                i.projects.Add(new KeyValuePair<string, Project>(project.id, project));

                projectsLeft--;
            }

            return i;
        }

        private static List<string> ParseOutput(Output output)
        {
            var o = new List<string> {output.p.ToString()};

            foreach (var a in output.assignments)
            {
                o.Add(a.Key);
                o.Add(string.Join(' ', a.Value));
            }

            return o;
        }
    }
}