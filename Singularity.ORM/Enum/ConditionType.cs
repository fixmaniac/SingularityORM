using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Singularity.ORM.Enum
{
    /// <summary>
    /// Record condition type
    /// </summary>
    public enum ConditionType
    {
        Equal,
        NotEqual,
        Null,
        Like,
        NotLike,
        In,
        Limit,
        Sort,        
        Other
    }
}
