using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;

namespace Singularity.ORM.SQL
{
    #region EventArgs (INotifyPropertyChanged)

    public class DbPropertyChangedEventArgs : PropertyChangedEventArgs
    {
        public DbPropertyChangedEventArgs(string paramName, object obj)
            : base(paramName)
        {
            this.Row = obj;
        }
        public object Row { get; set; }
    }
    #endregion
}
