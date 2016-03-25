using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Singularity.ORM
{
    #region interfaces
    public interface IBaseRecord
    {
        int Id { get; set; }
    }

    public interface ISqlTransaction : IDisposable
    {
        int LastInsertedId { get; set; }
        TableCollection Tables { get; }
        void Commit();
        void Rollback();
        void BeginTransaction(System.Data.IsolationLevel isolationLevel);
    }
    #endregion
}
