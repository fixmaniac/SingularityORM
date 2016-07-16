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
    /// Exposes method responsible for convert  fields represented 
    /// by type assignable from Enum
    /// </summary>
    /// <typeparam name="U"></typeparam>
    public class EnumMapper<U> : EntityMapper<U>
    {
        /// <summary>
        /// (...ctor)
        /// </summary>
        /// <param name="context"></param>
        /// <param name="obj"></param>
        public EnumMapper(SQLprovider context, Object obj) :
            base(context, obj)
        {
            if (!typeof(U).IsEnum)
            {
                throw new ArgumentException("Nieprawidłowy typ. Oczekiwano pola o typie Enum");
            }
        }

        /// <summary>
        /// Type Converter
        /// </summary>
        /// <returns></returns>
        public override U Convert()
        {
            return (U)System.Enum.Parse(typeof(U), (string)Row);
        }
    }
}
