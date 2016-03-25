using System;
using System.Collections.Generic;
using System.Reflection;

namespace Singularity.ORM
{
    [Serializable]
    public sealed class PropertyPathResolver
    {
        public Dictionary<string, Type> Nodes { get; set; }
        public PropertyPathResolver(Type type, string path)
        {
            Type currentType = type;
            foreach (string propertyName in path.Split('.'))
            {
                PropertyInfo property = currentType.GetProperty(propertyName);
                currentType = property.PropertyType;
                Nodes.Add(propertyName, currentType);
            }
        }
    }
}
