using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Singularity.ORM.SQL;
using Singularity.ORM.Enum;

namespace Singularity.ORM.Map
{
    /// <summary>
    /// Exposes method responsible for convert Byte[] to String type fields
    /// </summary>    
    public class ByteStringMapper : EntityMapper<string>
    {
        /// <summary>
        /// (...ctor)
        /// </summary>
        /// <param name="context"></param>
        /// <param name="obj"></param>
        public ByteStringMapper(SQLprovider context, Object obj) :
            base(context, obj)
        {

        }

        /// <summary>
        /// Type Converter
        /// </summary>
        /// <returns></returns>
        public override string Convert()
        {
            string result = "";
            switch (Context.Credentials.Collation)
            {
                case Collation.UTF8: result = Encoding.UTF8.GetString((byte[])Row);
                    break;
                case Collation.ASCII: result = Encoding.ASCII.GetString((byte[])Row);
                    break;
                case Collation.UNICODE: result = Encoding.Unicode.GetString((byte[])Row);
                    break;
                case Collation.LATIN1: result = Encoding.GetEncoding(1252).GetString((byte[])Row);
                    break;
            }
            return result;
        }
    }
}
