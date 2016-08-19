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
using Singularity.ORM.Enum;

namespace Singularity.ORM.Conditions
{
    public abstract class RecordCondition : SQLCondition 
    {
        #region Equal

        public class Equal : RecordCondition
        {
            protected override string Link
            {
                get
                {
                    return string.Empty;
                }
            }
            public Equal(string field, object cond)
            {
                base.BuildCondition(ConditionType.Equal, field, cond, out base.Condition);
            }
        }

        #endregion

        #region NotEqual

        public class NotEqual : RecordCondition
        {
            protected override string Link
            {
                get
                {
                    return string.Empty;
                }
            }
            public NotEqual(string field, object cond)
            {
                base.BuildCondition(ConditionType.NotEqual, field, cond, out base.Condition);
            }
        }

        #endregion

        #region Like

        public class Like : RecordCondition
        {
            protected override string Link
            {
                get
                {
                    return string.Empty;
                }
            }
            public Like(string field, object cond)
            {
                base.BuildCondition(ConditionType.Like, field, cond, out base.Condition);
            }
        }

        #endregion

        #region NotLike

        public class NotLike : RecordCondition
        {
            protected override string Link
            {
                get
                {
                    return string.Empty;
                }
            }
            public NotLike(string field, object cond)
            {
                base.BuildCondition(ConditionType.NotLike, field, cond, out base.Condition);
            }
        }

        #endregion

        #region Null

        public class Null : RecordCondition
        {
            protected override string Link
            {
                get
                {
                    return string.Empty;
                }
            }
            public Null(string field, bool cond)
            {
                base.BuildCondition(ConditionType.Null, field, cond, out base.Condition);
            }
        }

        #endregion

        #region In

        public class In : RecordCondition
        {
            protected override string Link
            {
                get
                {
                    return string.Empty;
                }
            }
            public In(string field, params object[] cond)
            {
                base.BuildCondition(ConditionType.In, field, cond, out base.Condition);
            }
        }

        #endregion

        #region Sort

        public class Sort : RecordCondition
        {
            protected override string Link
            {
                get
                {
                    return string.Empty;
                }
            }
            public Sort(SortOrder cond)
            {
                base.BuildCondition(ConditionType.Sort, null, cond, out base.Condition);
            }
        }

        #endregion

        #region Limit

        public class Limit : RecordCondition
        {
            protected override string Link
            {
                get
                {
                    return string.Empty;
                }
            }
            public Limit(int cond)
            {
                base.BuildCondition(ConditionType.Limit, null, cond, out base.Condition);
            }
        }

        #endregion
    }
}
