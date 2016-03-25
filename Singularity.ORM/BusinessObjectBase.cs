using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Singularity.ORM
{
    public abstract class BusinessObjectBase : ICloneable
    {

        public abstract BusinessObject Clone();
        object ICloneable.Clone()
        {
            return this.Clone();
        }
    }
}
