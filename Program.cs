using System;
using System.IO;

namespace Hashcode2022
{
    static class Program
    {
        static void Main(string[] args)
        {
            var path = Environment.GetCommandLineArgs()[0];
            path = path.Substring(0, path.LastIndexOf(Path.DirectorySeparatorChar)) + Path.DirectorySeparatorChar + "res" + Path.DirectorySeparatorChar;
            
            var algorithm = new Algorithm(System.IO.Path.GetDirectoryName(path));
            algorithm.Run();
        }
    }
}