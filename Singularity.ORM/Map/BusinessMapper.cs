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
    /// Exposes method responsible for convert Entity type fields
    /// </summary>
    public class BusinessMapper : EntityMapper<EntityProvider>
    {
        private Type type;
        /// <summary>
        /// (...ctor)
        /// </summary>
        /// <param name="context"></param>
        /// <param name="obj"></param>
        /// <param name="propType"></param>
        public BusinessMapper(SQLprovider context, Object obj, Type propType)
            : base(context, obj)
        {
            type = propType;
        }

        /// <summary>
        /// Type Converter
        /// </summary>
        /// <returns></returns>
        public override EntityProvider Convert()
        {
            EntityProvider value = null;
            using (SQLprovider prv = new SQLprovider(Context.Credentials))
            {
                value = (EntityProvider)prv.FindById[type, (int)Row];
            }
            return value;
        }
    }
}
