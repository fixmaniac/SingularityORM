using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using Singularity.ORM.SQL;
using Singularity.ORM.Conditions;

namespace Singularity.ORM.Reader
{
    /// <summary>
    /// Data reader returned all rows that match condition
    /// </summary>
    [DataObject]
    public class GelAllRowsAction : SQLActionBase
    {

        public GelAllRowsAction(SQLprovider provider)
        {
            base.Provider = provider;
            base.Connection = provider.Connection;
        }

        public virtual IEnumerable<object> this[Type type, SQLCondition cond]
        {
            get
            {
                return base.Result<IEnumerable<object>>(type, cond);
            }
        }
    }
}
