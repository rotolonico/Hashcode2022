using System.Collections.Generic;

namespace Hashcode2022
{
    public class Contributor
    {
        public string id;
        public List<KeyValuePair<string, int>> skills = new();
        public int isOccupiedUntil = -1;
    }
}