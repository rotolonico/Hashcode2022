using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Hashcode2022
{
    public static class IOHandler
    {
        public static List<string> LoadFile(string path) => File.ReadAllLines(path).ToList();

        public static void SaveFile(string path, List<string> file) => File.WriteAllLines(path, file);
    }
}