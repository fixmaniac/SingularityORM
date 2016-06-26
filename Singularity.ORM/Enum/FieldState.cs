using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Singularity.ORM.Enum
{
    /// <summary>
    /// Enum represented state of business record
    /// </summary>
    public enum FieldState
    {
        Unchanged,
        Added,
        Modified,
        Deleted,
        Detached
    }
}
