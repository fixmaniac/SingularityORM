using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Singularity.ORM.SQL;
using Singularity.ORM.Enum;

namespace Singularity.ORM.Map
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class EntityMapper<T> : IMapper<T>, IEntityMapper
    {
        // properties
        public SQLprovider Context { get; set; }
        public Object Row { get; set; }

        /// <summary>
        /// (...ctor)
        /// </summary>
        /// <param name="context"></param>
        /// <param name="obj"></param>
        public EntityMapper(SQLprovider context, Object obj)
        {
            this.Context = context;
            this.Row = obj;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public abstract T Convert();
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public object Map()
        {
            return Convert();
        }
    }              
}
