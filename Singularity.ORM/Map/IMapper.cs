using System;
using System.Collections.Generic;
using System.Linq;
using Singularity.ORM.SQL;

namespace Singularity.ORM.Map
{
    /// <summary>
    /// Exposes methods that map miscallenoeus field types
    /// </summary>
    public interface IEntityMapper
    {
        SQLprovider Context { get; set; }
        Object Row { get; set; }
        object Map();
    }

    /// <summary>
    /// Exposes a method that convert IDataReader type to .NET OOP type <typeparamref name="T"/>
    /// </summary>
    /// <typeparam name="T">The element type of the returned value</typeparam>
    public interface IMapper<T>
    {
        T Convert();
    }
}
