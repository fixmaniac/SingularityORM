using System;


namespace Singularity.ORM
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class ForeignKeyAttribute : Attribute
    {
        public string TableName
        {
            get;
            set;
        }
        public ForeignKeyAttribute(string tableName)
        {
            this.TableName = tableName;
        }
    }
}
