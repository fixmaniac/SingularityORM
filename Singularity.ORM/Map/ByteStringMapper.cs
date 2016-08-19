/*
 *  Copyright (c) 2016, Łukasz Ligocki.
 *  All rights reserved.
 *
 *  Licensed under the Apache License, Version 2.0 (the "License");
 *  you may not use this file except in compliance with the License.
 *  You may obtain a copy of the License at

 *  http://www.apache.org/licenses/LICENSE-2.0

 *  Unless required by applicable law or agreed to in writing, software
 *  distributed under the License is distributed on an "AS IS" BASIS,
 *  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *  See the License for the specific language governing permissions and
 *  limitations under the License.
 *
 */

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
