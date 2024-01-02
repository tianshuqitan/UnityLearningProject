using System;
using System.Linq;

namespace Tools.Excel
{
    public static class ReflectHelper
    {
        public static Type GetGenericTypeByName(string name)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            return assemblies.Select(assembly => assembly.GetType(name)).FirstOrDefault(tmpType => tmpType != null);
        }
    }
}