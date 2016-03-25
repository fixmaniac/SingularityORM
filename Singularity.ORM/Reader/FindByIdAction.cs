using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using Singularity.ORM.SQL;
using Singularity.ORM.Conditions;

namespace Singularity.ORM.Reader
{
    /// <summary>
    /// Data reader returned one particular record matched 
    /// with unique ID key
    /// </summary>
    [DataObject]
    public class FindByIdAction : SQLActionBase
    {

        public FindByIdAction(SQLprovider provider)
        {
            base.Provider = provider;
            base.Connection = provider.Connection;
        }

        public virtual IBaseRecord this[Type type, int id]
        {
            get
            {
                return base.Result<IBaseRecord>(type, new SQLCondition.And(
                    new RecordCondition.Equal("id", id),
                    new RecordCondition.Limit(1)));
            }
        }
    }
}
