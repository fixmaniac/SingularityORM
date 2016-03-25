using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using Singularity.ORM.SQL;
using Singularity.ORM.Conditions;

namespace Singularity.ORM.Reader
{
    /// <summary>
    /// Data reader returned one particular record that match single condition
    /// </summary>
    [DataObject]
    public class FindByAction : SQLActionBase
    {

        public FindByAction(SQLprovider provider)
        {
            base.Provider = provider;
            base.Connection = provider.Connection;
        }

        public virtual IBaseRecord this[Type type, string field, object cond]
        {
            get
            {
                return base.Result<IBaseRecord>(type, new SQLCondition.And(
                    new RecordCondition.Equal(field, cond),
                    new RecordCondition.Limit(1)));
            }
        }
    }
}
