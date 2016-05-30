using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Singularity.ORM.Enum;

namespace Singularity.ORM.Conditions
{
    [DebuggerDisplay("ActualCondition={Condition}")]
    public abstract class SQLCondition
    {
        public static readonly SQLCondition Empty = new EmptyCondition();
        public string Condition = String.Empty;
        protected abstract string Link { get; }

        protected virtual void BuildCondition(ConditionType type, string field, object cond, out string result)
        {
            Func<string> method = delegate()
            {

                string _result = "";
                if (cond != null)
                {
                    if (cond.GetType() == typeof(bool))
                        cond = (bool)cond == true ? 1 : 0;
                    else if (typeof(IBaseRecord).IsAssignableFrom(cond.GetType()))
                        cond = ((IBaseRecord)cond).Id;
                }
                switch (type)
                {
                    case ConditionType.Equal: _result = cond == null
                                                            ? String.Format("{0} is null", field)
                                                            : String.Format("{0} = '{1}'", field, cond);
                        break;
                    case ConditionType.NotEqual: _result = cond == null
                                                            ? String.Format("{0} is not null", field)
                                                            : String.Format("{0} <> '{1}'", field, cond);
                        break;
                    case ConditionType.Like: _result =
                                                             String.Format("{0} like '%{1}%'", field, cond);
                        break;
                    case ConditionType.NotLike: _result =
                                                             String.Format("{0} not like '%{1}%' ", field, cond);
                        break;
                    case ConditionType.Sort: _result =
                                                             String.Format("order by Id {0} ", cond);
                        break;
                    case ConditionType.Limit: _result =
                                                             String.Format("limit {0} ", cond);
                        break;
                    case ConditionType.Null: _result =
                                                           (bool)cond == true
                                                            ? String.Format("{0} is null", field)
                                                            : String.Format("{0} is not null", field);
                        break;
                    case ConditionType.In:
                        {
                            object[] args = (object[])cond;
                            object[] flattened = args
                                  .Select(a => a is object[] ? (object[])a : new object[] { a })
                                  .SelectMany(a => a)
                                  .ToArray();
                            string[] arr1 = ((IEnumerable)flattened[0]).Cast<object>()
                                 .Select(x => String.Format("'{0}'", x.ToString()))
                                 .ToArray();
                            string[] arr2 = ((IEnumerable)args).Cast<object>()
                                .Select(x => String.Format("'{0}'", x.ToString()))
                                .ToArray();

                            _result = String.Format("{0} in ({1})", field, String.Join
                                     (",", args.Length == 1 &&
                                       typeof(Array).IsAssignableFrom(args[0].GetType())
                                       ? arr1 : arr2));
                            break;
                        }
                }
                return _result;
            };
            result = method.Invoke();
        }

        public class EmptyCondition : SQLCondition
        {
            public EmptyCondition()
            {
                base.Condition = "1 = 1";
            }
            protected override string Link
            {
                get
                {
                    return string.Empty;
                }
            }
        }

        public abstract class CompoundCondition : SQLCondition
        {
            public CompoundCondition(params SQLCondition[] arr)
            {
                StringBuilder sb = new StringBuilder();
                int i = 0;
                arr.Cast<SQLCondition>().ToList().ForEach(delegate(SQLCondition cond)
                {
                    if (String.IsNullOrEmpty(cond.Condition))
                        return;
                    if (i != 0
                        && !cond.Condition.StartsWith("limit")
                        && !cond.Condition.StartsWith("order")
                        )
                        sb.Append(this.Link);

                    sb.Append(" ");
                    sb.Append(cond.Condition);
                    sb.Append(" ");
                    i = i + 1;
                });
                base.Condition = sb.ToString();
            }
        }
        public class And : CompoundCondition
        {
            public And(params SQLCondition[] arr)
                : base(arr)
            {
            }

            protected override string Link
            {
                get
                {
                    return " AND ";
                }
            }
        }

        public class Or : CompoundCondition
        {
            public Or(params SQLCondition[] arr)
                : base(arr)
            {
            }

            protected override string Link
            {
                get
                {
                    return " OR ";
                }
            }
        }

        public static SQLCondition operator &(SQLCondition cond1, SQLCondition cond2)
        {
            if (cond1 == Empty) return cond2;
            if (cond2 == Empty) return cond1;

            return new And(cond1, cond2);
        }

        public static SQLCondition operator |(SQLCondition cond1, SQLCondition cond2)
        {
            if (cond1 == Empty) return cond2;
            if (cond2 == Empty) return cond1;

            return new Or(cond1, cond2);
        }
    }
}
