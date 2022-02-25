using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
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
                    //TimeHandler.StartTimer(name);
                    Console.WriteLine($"{name} Processing...");

                    var input = IOHandler.LoadFile(file);
                    var output = RunAlgorithm(input, o =>
                    {
                        IOHandler.SaveFile(outputPath, o);
                    });
                    
                    IOHandler.SaveFile(outputPath, output);

                    Console.WriteLine($"{name} Done!");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"{name} Failed!\n    Message: " + ex.Message + "\n    StackTrace: " +
                                      ex.StackTrace);
                }
            });
        }

        private List<string> RunAlgorithm(List<string> input, Action<List<string>> partialCallback)
        {
            var i = ParseInput(input);
            var o = new Output {assignments = new List<KeyValuePair<string, Assignment>>()};

            i.projects = i.projects.OrderByDescending(p => (float) p.Value.score / (p.Value.bestBefore + p.Value.duration))
                .ToList();

            var activeContributors = i.contributors;
            var activeProjects = i.projects;

            var day = 0;

            while (true)
            {
                partialCallback(ParseOutput(o));
                
                var result = AssignContributorsToProjects(day, o, activeContributors, activeProjects);

                activeContributors = result.availableContributors;
                activeProjects = result.availableProjects;

                if (o.assignments.Any(a => !a.Value.isDone))
                {
                    var minActiveAssignment = o.assignments.Where(a => !a.Value.isDone)
                        .MinBy(a => a.Value.project.Value.duration);
                    minActiveAssignment.Value.isDone = true;
                    day += minActiveAssignment.Value.project.Value.duration;
                    result.availableContributors.AddRange(minActiveAssignment.Value.contributors);
                    
                    for (var r = 0; r < minActiveAssignment.Value.project.Value.roles.Count; r++)
                    {
                        KeyValuePair<string, Contributor> cont = minActiveAssignment.Value.contributors[r];
                        KeyValuePair<string, int> role = minActiveAssignment.Value.project.Value.roles[r];

                        var skill = cont.Value.skills.Find(s => s.Key == role.Key);
                        if (skill.Value == role.Value)
                        {
                            cont.Value.skills.Remove(skill);
                            cont.Value.skills.Add(new KeyValuePair<string, int>(skill.Key, skill.Value + 1));
                        }
                    }
                    

                    continue;
                }

                break;
            }


            return ParseOutput(o);
        }

        [SuppressMessage("ReSharper.DPA", "DPA0001: Memory allocation issues")]
        private Result AssignContributorsToProjects(int day, Output o,
            List<KeyValuePair<string, Contributor>> contributors, List<KeyValuePair<string, Project>> projects)
        {
            var aAssignments = new List<KeyValuePair<string, Assignment>>();
            var aProjects = new List<KeyValuePair<string, Project>>(projects);
            var aContributors = new List<KeyValuePair<string, Contributor>>(contributors);

            foreach (var p in projects)
            {
                var includeProject = true;
                var currentContributors = new List<KeyValuePair<string, Contributor>>();

                foreach (var r in p.Value.roles)
                {
                    var contributor = contributors.FirstOrDefault(c =>
                        c.Value.isOccupiedUntil < day && c.Value.skills.Any(s => s.Key == r.Key && s.Value >= r.Value));


                    if (contributor.Equals(default(KeyValuePair<string, Contributor>)))
                    {
                        includeProject = false;
                        break;
                    }

                    contributor.Value.isOccupiedUntil = day + p.Value.duration;
                    currentContributors.Add(contributor);
                }

                if (!includeProject)
                {
                    currentContributors.ForEach(c => c.Value.isOccupiedUntil = day - 1);
                    continue;
                }

                var assignment = new KeyValuePair<string, Assignment>(p.Key, new Assignment
                {
                    project = p,
                    isDone = false,
                    contributors = currentContributors
                });
                o.assignments.Add(assignment);

                aAssignments.Add(assignment);
                aProjects.Remove(p);
                aContributors.RemoveAll(c => currentContributors.Contains(c));

                p.Value.isAssigned = true;

                o.p++;
            }

            return new Result
            {
                addedAssignments = aAssignments,
                availableProjects = aProjects,
                availableContributors = aContributors
            };
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
                o.Add(string.Join(' ', a.Value.contributors.Select(c => c.Value.id)));
            }

            return o;
        }
    }
}