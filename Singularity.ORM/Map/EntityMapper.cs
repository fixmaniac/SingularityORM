using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Singularity.ORM.SQL;
using Singularity.ORM.Enum;

namespace Singularity.ORM.Map
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class EntityMapper<T> : IMapper<T>, IEntityMapper
    {
        public SQLprovider Context { get; set; }
        public Object Row { get; set; }
        public EntityMapper(SQLprovider context, Object obj)
        {
            this.Context = context;
            this.Row = obj;
        }
        public abstract T Convert();
        public object Map()
        {
            return Convert();
        }
    }

    /// <summary>
    /// Exposes method responsible for convert Boolean type fields
    /// </summary>
    public class BoolMapper : EntityMapper<bool>
    {
        public BoolMapper(SQLprovider context, Object obj)
            : base(context, obj)
        {

        }
        public override bool Convert()
        {
            return (Int16)Row == 1 ? true : false;
        }
    }

    /// <summary>
    /// Exposes method responsible for convert Entity type fields
    /// </summary>
    public class BusinessMapper : EntityMapper<EntityProvider>
    {
        private Type type;
        public BusinessMapper(SQLprovider context, Object obj, Type propType)
            : base(context, obj)
        {
            type = propType;
        }

        public override EntityProvider Convert()
        {
            EntityProvider value = null;
            using (SQLprovider prv = new SQLprovider(Context.Credentials))
            {
                value = (EntityProvider)prv.FindById[type, (int)Row];
            }
            return value;
        }
    }

    /// <summary>
    /// Exposes method responsible for convert  fields represented 
    /// by type assignable from Enum
    /// </summary>
    /// <typeparam name="U"></typeparam>
    public class EnumMapper<U> : EntityMapper<U>
    {
        public EnumMapper(SQLprovider context, Object obj) :
            base(context, obj)
        {
            if (!typeof(U).IsEnum)
            {
                throw new ArgumentException("Nieprawidłowy typ. Oczekiwano pola o typie Enum");
            }
        }

        public override U Convert()
        {
            return (U)System.Enum.Parse(typeof(U), (string)Row);
        }
    }

    /// <summary>
    /// Exposes method responsible for convert Byte[] to String type fields
    /// </summary>    
    public class ByteStringMapper : EntityMapper<string>
    {
        public ByteStringMapper(SQLprovider context, Object obj) :
            base(context, obj)
        {

        }

        public override string Convert()
        {
            string result = "";
            switch (Context.Credentials.Collation)
            {
                case Collation.UTF8    : result = Encoding.UTF8.GetString((byte[])Row);
                    break;
                case Collation.ASCII   : result = Encoding.ASCII.GetString((byte[])Row);
                    break;
                case Collation.UNICODE : result = Encoding.Unicode.GetString((byte[])Row);
                    break;
                case Collation.LATIN1  : result = Encoding.GetEncoding(1252).GetString((byte[])Row);
                    break;
            }
            return result;
        }
    }
}
