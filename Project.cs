using System.Collections.Generic;

namespace Hashcode2022
{
    public class Project
    {
        public string id;
        public int duration;
        public int bestBefore;
        public int score;
        public List<KeyValuePair<string, int>> roles = new();
    }
}