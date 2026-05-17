using System;
using System.Linq;

namespace NoOpArmy.WiseFeline
{
    /// <summary>
    /// Contains utility methods for working with data types defined as actions and considerations and ...
    /// You should not need to use this unless you are making debug/editor tools for Wise Feline
    /// </summary>
    public static class ReflectionUtilities
    {
        /// <summary>
        /// Get all types derived from a specific type defined in the project.
        /// </summary>
        /// <param name="baseType"></param>
        /// <returns></returns>
        public static Type[] GetAllDerivedTypes(Type baseType)
        {
            var typesList = AppDomain.CurrentDomain.GetAssemblies()
                // alternative: .GetExportedTypes()
                .SelectMany(domainAssembly => domainAssembly.GetTypes())
                .Where(type => type.IsSubclassOf(baseType)
                // alternative: => parentType.IsAssignableFrom(type)
                // alternative: && type != typeof(B)
                // alternative: && ! type.IsAbstract
                ).ToArray();
            return typesList;
        }
    }
}
