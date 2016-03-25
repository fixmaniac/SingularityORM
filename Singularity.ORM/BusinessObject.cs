using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using Singularity.ORM.SQL;
using Singularity.ORM.Enum;
using Singularity.ORM.Exceptions;

namespace Singularity.ORM
{
    public class BusinessObject : BusinessObjectBase
    {
        public FieldState State { get; private set; }
        internal int ID { get; private set; }
        internal IBaseRecord Row { get; private set; }
        internal Type Type { get; private set; }
        internal string PropertyName { get; set; }
        internal object Value { get; set; }

        internal BusinessObject(FieldState state, IBaseRecord obj, Type type, string propertyName, object value)
        {
            if (!String.IsNullOrEmpty(propertyName))
                verifyProperty(type, propertyName);

            this.Row = obj;
            this.ID = obj.Id == 0 ? GenerateIdx() : obj.Id;
            this.State = state;
            this.Type = type;
            this.PropertyName = propertyName;
            this.Value = value;
        }

        public static Int32 GenerateIdx()
        {
            var bytes = new byte[sizeof(Int64)];
            RNGCryptoServiceProvider Gen = new RNGCryptoServiceProvider();
            Gen.GetBytes(bytes);
            return -BitConverter.ToInt32(bytes, 0);
        }

        private void verifyProperty(Type type, string propertyName)
        {
            var fields = SQLprovider.PropertiesFromType<IDictionary<string, Type>>(type);
            if (!fields.ContainsKey(propertyName))
                throw new FieldNotFoundException(propertyName, type);
        }

        public override BusinessObject Clone()
        {
            return (BusinessObject)this.MemberwiseClone();
        }
    }
}
