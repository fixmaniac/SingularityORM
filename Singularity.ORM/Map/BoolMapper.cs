using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Singularity.ORM.SQL;
using Singularity.ORM.Enum;

namespace Singularity.ORM.Map
{
    /// <summary>
    /// Exposes method responsible for convert Boolean type fields
    /// </summary>
    public class BoolMapper : EntityMapper<bool>
    {
        /// <summary>
        /// (...ctor)
        /// </summary>
        /// <param name="context"></param>
        /// <param name="obj"></param>
        public BoolMapper(SQLprovider context, Object obj)
            : base(context, obj)
        {

        }

        /// <summary>
        /// Type Converter
        /// </summary>
        /// <returns></returns>
        public override bool Convert()
        {
            return (Int16)Row == 1 ? true : false;
        }
    }
}
