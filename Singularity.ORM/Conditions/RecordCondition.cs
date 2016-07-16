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
    }
}
