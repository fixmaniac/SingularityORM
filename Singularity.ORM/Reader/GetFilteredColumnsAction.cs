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
    public class GetFilteredColumnsAction : SQLActionBase
    {

        public GetFilteredColumnsAction(SQLprovider provider)
        {
            base.Provider = provider;
            base.Connection = provider.Connection;
        }

        public virtual IEnumerable<object> this[Type type, SQLCondition cond, params string[] columns]
        {
            get
            {
                return base.Result<IEnumerable<object>>(type, cond, columns);
            }
        }
    }
}
