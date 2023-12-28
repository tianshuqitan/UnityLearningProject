using System;
using System.Collections.Generic;

namespace Tools.Excel
{
    public class SkillTest
    {
        public int id;
        public string name;
        
        public override string ToString()
        {
            return $"SkillTest{{" +
                   $"id: {id}" +
                   $"name: {name}" +
                   $"}}";
        }
    }
}