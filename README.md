# SingularityORM
Micro ORM for .NET 

Singularity ORM is a tiny and simply to use, object relational mapping framework for .NET environment, tailored to be used especially in projects responsible for exchange data with open source systems without their own API.

It's an entirely new approach towards micro ORM idea and due to that, might be completely separate from SQL queries, which are often required in the others similar projects.

Singularity.ORM is currently in the testing phase, and no giving any warranty.

#How to use

### 1. Connection
```c#
SQLprovider Context = new SQLprovider(new ProviderCredentials() { 
              Server = "127.0.0.1",
              Port = 3306,
              User = "root",
              Password = "",
              Database = "testORM",
              Collation = Singularity.ORM.Enum.Collation.UTF8            
            });
```
### 2. Create new instance of an employee object

```c#
 using (ISqlTransaction trans = Context.BeginTransaction())
                {
                    Tables.EmployeeTable   Employees   = Tables.EmployeeTable.GetInstance(trans);

                    Employee employee = new Employee()
                    {
                        Birth_date = new DateTime(1965, 7, 23),
                        First_name = "John",
                        Last_name = "Doe",
                        Gender = Gender.M,
                        Hire_date = DateTime.Now,                        
                    };
                    Employees.Add(employee);
                    trans.Commit();
                }
```

### 3. Searching data with multiple conditions (SQL JOIN)

```c#
 using (ISqlTransaction trans = Context.BeginTransaction())
                {
                    Tables.AbsenceTable    Absences    = Tables.AbsenceTable.GetInstance(trans);
                    var absences = Absences.GetRows(
                        new SQLCondition.And(new SQLCondition[] { 
                             new RecordCondition.Null("Employee.Dept.Dept_name", false),
                             new RecordCondition.Equal("Reason", Reason.other),
                             new RecordCondition.Equal("Employee.Gender", Gender.F)
                            })
                        );
                    foreach (var absence in absences)
                    {
                        Console.WriteLine(String.Format("Employee: {0} {1}, 
                         Absence in {2}, due to reason: {3}", absence.Employee.First_name,
                          absence.Employee.Last_name,
                          absence.Date,
                          absence.Reason));
                    } 
            }
```

#Sample usage scenarios

### 1.Database 
```sql
DROP DATABASE IF EXISTS testORM;
CREATE DATABASE testORM;
USE testORM;
DROP TABLE IF EXISTS departments, employees, absences;

CREATE TABLE departments (
    id      INT             NOT NULL AUTO_INCREMENT,
    dept_name VARCHAR(25)            NOT NULL,    
    PRIMARY KEY (id)                      -- Index built automatically on primary-key column                                           
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

CREATE TABLE employees (
    id      INT             NOT NULL AUTO_INCREMENT,
    birth_date  DATE            NOT NULL,
    first_name  VARCHAR(14)     NOT NULL,
    last_name   VARCHAR(16)     NOT NULL,
    gender      ENUM ('M','F')  NOT NULL,  -- Enumeration of either 'M' or 'F'  
    hire_date   DATE            NOT NULL,
    dept INT DEFAULT NULL,
    PRIMARY KEY (id),                      -- Index built automatically on primary-key column  
    FOREIGN KEY (dept) REFERENCES departments (id) ON DELETE SET NULL                                      
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

CREATE TABLE absences (
    id      INT                 NOT NULL AUTO_INCREMENT,
    employee INT 		NOT NULL,
    date  DATE            NOT NULL,    
    description  VARCHAR(50)     NOT NULL,
    reason       ENUM ('holiday','sickness','other')  NOT NULL,     
    PRIMARY KEY (id),
    FOREIGN KEY (employee) REFERENCES employees (id) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
```

### 2.Entity class
```c#
public enum Gender {
        M = 1,
        F = 2
    }

    public class Employee : EntityProvider, INotifyPropertyChanged, IBaseRecord
    {
        internal static readonly string tableName = "employees";

        private int id;
        private DateTime birth_date;
        private string first_name;
        private string last_name;
        private Gender gender;
        private DateTime hire_date;
        private Department dept;

        public int Id
        {
            get { return id; }
            set
            {
                id = value;
                NotifyPropertyChanged();
            }
        }
        public DateTime Birth_date
        {
            get { return birth_date; }
            set
            {
                birth_date = value;
                NotifyPropertyChanged();
            }
        }
        [Required]
        [TextMaxLength(14)]
        public string First_name
        {
            get { return first_name; }
            set
            {
                first_name = value;
                NotifyPropertyChanged();
            }
        }
        [Required]
        [TextMaxLength(16)]
        public string Last_name
        {
            get { return last_name; }
            set
            {
                last_name = value;
                NotifyPropertyChanged();
            }
        }
        [Required]
        public Gender Gender
        {
            get { return gender; }
            set
            {
                gender = value;
                NotifyPropertyChanged();
            }
        }        
        public DateTime Hire_date
        {
            get { return hire_date; }
            set
            {
                hire_date = value;
                NotifyPropertyChanged();
            }
        }

        public Department Dept
        {
            get { return dept; }
            set
            {
                dept = value;
                NotifyPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new DbPropertyChangedEventArgs(propertyName, this));
            }
        }

    }
```

### 3.Table class

```c#
 public partial class Tables
    {
        (...)
        
        public sealed class EmployeeTable : EntityTable//Employees
        {
            /// <summary>
            /// ..(CTOR)
            /// </summary>
            /// <param name="transaction">Transaction</param>
            public EmployeeTable(ISqlTransaction transaction)
                : base(transaction)
            {

            }
            [System.Runtime.CompilerServices.IndexerName("FindBy")]
            public Employee this[string field, object value]
            {
                get
                {
                    return base.FindBy<Employee>(field, value);
                }
            }

            [System.Runtime.CompilerServices.IndexerName("FindBy")]
            public Employee this[int id]
            {
                get
                {
                    return base.FindByID<Employee>(id);
                }
            }

            public Employee GetFirst(SQLCondition condition)
            {
                return base.GetFirst<Employee>(condition);
            }

            public Employee GetLast(SQLCondition condition)
            {
                return base.GetLast<Employee>(condition);
            }

            public IEnumerable<Employee> GetLimited(SQLCondition condition, int limit)
            {
                return base.GetLimited<Employee>(condition, limit);
            }

            public IEnumerable<Employee> GetRows(SQLCondition condition)
            {
                return base.GetRows<Employee>(condition);
            }

            public void Add(Employee row)
            {
                base.Add<Employee>(row);
            }

            public static Type RowType
            {
                get
                {
                    return typeof(Employee);
                }
            }

            public static EmployeeTable GetInstance(ISqlTransaction transaction)
            {
                if (transaction == null) return null;
                return (EmployeeTable)transaction.Tables[typeof(EmployeeTable)];
            }

        }
    }
```

