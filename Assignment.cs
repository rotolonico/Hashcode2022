using System.Collections.Generic;

namespace Hashcode2022
{
    public class Assignment
    {
        public KeyValuePair<string, Project> project;
        public bool isDone;
        public List<KeyValuePair<string, Contributor>> contributors;
    }
}