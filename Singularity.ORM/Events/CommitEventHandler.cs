using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Singularity.ORM.Events
{
    /// <summary>
    /// Handler for transaction commit event invoke
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void CommitEventHandler(object sender, CommitEventArgs e);

    public class CommitEventArgs : EventArgs
    {
        public CommitEventArgs(ISqlTransaction transaction,
                  IQueryable<IGrouping<IBaseRecord, BusinessObject>> collection)
        {
            this.Transaction = transaction;
            this.Collection = collection;
        }
        public ISqlTransaction Transaction { get; set; }
        public IQueryable<IGrouping<IBaseRecord, BusinessObject>> Collection { get; set; }

    }
}
