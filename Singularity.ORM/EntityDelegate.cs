using System;


namespace Singularity.ORM
{
    /// <summary>
    /// Delegate on an etity properties modifying
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <param name="entity"></param>
    public delegate void EntityDelegate<TEntity>(TEntity entity);  
}
