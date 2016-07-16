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
    /// Exposes method responsible for convert GUID type fields
    /// </summary>
    public class GuidMapper : EntityMapper<Guid>
    {
        /// <summary>
        /// (...ctor)
        /// </summary>
        /// <param name="context"></param>
        /// <param name="obj"></param>
        public GuidMapper(SQLprovider context, Object obj)
            : base(context, obj)
        {

        }

        /// <summary>
        /// Type Converter
        /// </summary>
        /// <returns></returns>
        public override Guid Convert()
        {
            return new Guid((System.String)Row);
        }
    }
}
