﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeGenerator.Schema;

namespace CodeGenerator
{
    public class EntityBuilder : IDisposable
    {
        private readonly string version = "1.0";
        private readonly  Entity entity;
        internal readonly EntityClassWriter writer;

        internal Field ID { get; private set; }

        private IEnumerable<string> usings
        {
            get
            {
                 yield return "Singularity.ORM";
                 yield return "Singularity.ORM.SQL";
                 yield return "Singularity.ORM.Validation";
                 yield return "using Singularity.ORM.Conditions;";
            }
        }

        public EntityBuilder(Entity entity, EntityClassWriter writer)
        {
            this.entity = entity;
            this.writer = writer;

            this.ID = new Field()
            {
                entity = this.entity,
                Name   = "Id",
                Type   = "int"
            };
        }

        public void Build()
        {

            addHeader();
            addUsings();
            addEnumerators();

            this.writer.Write("/// <summary>");
            this.writer.Write("/// Entity  object {0}.", (object)this.entity.Name);            
            this.writer.Write("/// </summary>");
            this.writer.Write("[System.CodeDom.Compiler.GeneratedCode(\"Singularity.CodeGenerator\", \"{0}\")]", this.version);
            this.writer.Write("public partial class {0} : EntityProvider, INotifyPropertyChanged, IBaseRecord {{", 
                (object)this.entity.Name);
            this.writer.WriteLine();
            this.writer.Write("internal static readonly string tableName = \"{0}\"", this.entity.TableName);
            this.writer.WriteLine();
            
            foreach (Field field in this.entity.Fields) {
                this.writer.Write("private {0} {1};", field.Type, field.Name.ToLower());
            }

            addFields();

            this.writer.Write("public event PropertyChangedEventHandler PropertyChanged;");
            this.writer.WriteLine();
            this.writer.Write("private void NotifyPropertyChanged([CallerMemberName] String propertyName = \"\")");
            this.writer.Write("{{   ");
            this.writer.Write("    if (PropertyChanged != null)  ");
            this.writer.Write("    {{ ");
            this.writer.Write("        PropertyChanged(this, new DbPropertyChangedEventArgs(propertyName, this)); ");
            this.writer.Close();
            this.writer.WriteLine();
            this.writer.Close();
            this.writer.WriteLine();


            addEntityTable();
            
        }

        private void addHeader()
        {
            this.writer.WriteLine();
            this.writer.Write("//----------------------------------------------------------------------------------");
            this.writer.Write("// <autogenerated>");
            this.writer.Write("//\t\tThis code was generated by a tool.");
            this.writer.Write("//\t\tChanges to this file may cause incorrect behaviour and will be lost ");
            this.writer.Write("//\t\tif the code is regenerated.");
            this.writer.Write("// </autogenerated>");
            this.writer.Write("//----------------------------------------------------------------------------------");             
            //this.writer.Close();;
            this.writer.WriteLine();
        }

        private void addUsings()
        {
            this.writer.Write("using System;");
            this.writer.Write("using System.ComponentModel;");
            this.writer.Write("using System.Runtime.CompilerServices;");
            this.writer.Write("using System.Collections.Generic;");
            this.writer.Write("using System.Linq;");
            this.writer.Write("using System.Text;");

            usings.ToList().ForEach(delegate(string str) {
                this.writer.Write("using {0};", (object)str);
            });
        }

        private void addEnumerators()
        {
            if (this.entity.Enums != null) {
                this.entity.Enums.ToList().ForEach(delegate(Enumerator enumerator)  {
                    this.writer.Write("public enum {0} {{ ", enumerator.Name);
                    foreach (string item in enumerator.Items)
                    {
                        this.writer.Write("{0}, ", item);
                    }
                    this.writer.Close();
                    this.writer.WriteLine();
                });
                //this.writer.Close();;
                //this.writer.Close();;
                this.writer.WriteLine();
            }
        }

        private void addFields()
        {
           
            this.entity.Fields.ToList().ForEach(delegate(Field field) {                
                if (field.Mendatory)
                    this.writer.Write("[Mendatory]");
                if (field.Length > 0                   
                    && (field.Type == "string" || field.Type == "String"))
                    this.writer.Write("[TextMaxLength({0})]", field.Length);
                else if 
                    (field.Length > 0 && (field.Type != "string" || field.Type != "String"))
                    throw new InvalidOperationException
                        ("Field {0} is not a string type and due to that it cannot be set a length value" + field.Name);               
                addField(field);

            });
        }

        private void addField(Field field)
        {
            this.writer.Write("public {0} {1}", field.Type, field.Name);
            this.writer.Write("{{");
            this.writer.Write("    get {{ return {0}; }}", field.Name.ToLower());
            this.writer.Write("    set ");
            this.writer.Write("    {{  ");
            this.writer.Write("        {0} = value;", field.Name.ToLower());
            this.writer.Write("        NotifyPropertyChanged();");
            this.writer.Close();
            this.writer.Close();
            this.writer.WriteLine();
        }


        private void addEntityTable()
        {
            this.writer.Write("public partial class Tables");
            this.writer.Write("{{ ");
            this.writer.WriteLine();
            this.writer.Write("/// <summary>");
            this.writer.Write("/// Table class for entity {0}.", (object)this.entity.Name);
            this.writer.Write("/// </summary>");                        
            this.writer.Write(" public sealed class {0}Table : EntityTable//<{0}>", this.entity.Name); 
            this.writer.Write(" {{");
            this.writer.Write(" /// <summary>");
            this.writer.Write(" /// ..(ctor)");
            this.writer.Write(" /// </summary>");
            this.writer.Write(" /// <param name=\"transaction\">Transaction</param>");
            this.writer.Write(" public {0}Table(ISqlTransaction transaction)", this.entity.Name);
            this.writer.Write("      : base(transaction)  ");
            this.writer.Write("  {{ ");
            this.writer.WriteLine();
            this.writer.Close();
            this.writer.WriteLine();
            this.writer.Write("/// <summary>");
            this.writer.Write("/// Indexer responsible for returning particular entity object instance ");
            this.writer.Write("/// using simple condition.");
            this.writer.Write("/// </summary>");
            this.writer.Write("/// <param name=\"field\">field name which is used as a condition");
            this.writer.Write("/// <param name=\"value\">field value used as a condition");
            this.writer.Write("/// <seealso cref=\"{0}\"/>", (object)this.entity.Name);
            this.writer.Write(" [System.Runtime.CompilerServices.IndexerName(\"FindBy\")]");
            this.writer.Write(" public {0} this[string field, object value]", this.entity.Name);
            this.writer.Write("  {{ ");
            this.writer.Write("    get ");
            this.writer.Write("       {{ ");
            this.writer.Write("         return base.FindBy<{0}>(field, value);", this.entity.Name);
            this.writer.Close();
            this.writer.WriteLine();
            this.writer.Close();            
            this.writer.WriteLine();
            this.writer.WriteLine();
            this.writer.Write("/// <summary>");
            this.writer.Write("/// Indexer responsible for returning particular entity object instance ");
            this.writer.Write("/// using unique ID field.");
            this.writer.Write("/// </summary>");
            this.writer.Write("/// <param name=\"id\">ID key unique value");
            this.writer.Write("/// <seealso cref=\"{0}\"/>", (object)this.entity.Name);
            this.writer.Write(" [System.Runtime.CompilerServices.IndexerName(\"FindBy\")]");
            this.writer.Write(" public {0} this[int id]", this.entity.Name);
            this.writer.Write("  {{    ");
            this.writer.Write("    get ");
            this.writer.Write("       {{  ");
            this.writer.Write("        return base.FindByID<{0}>(id);",  this.entity.Name);
            this.writer.Close();
            this.writer.WriteLine();
            this.writer.Close();
            this.writer.WriteLine();
            this.writer.WriteLine();
            this.writer.WriteLine();
            this.writer.Write("/// <summary>");
            this.writer.Write("/// Method returning first instance of particular entity object");
            this.writer.Write("/// using multiple condition.");
            this.writer.Write("/// </summary>");
            this.writer.Write("/// <param name=\"condition\">condition");
            this.writer.Write("/// <seealso cref=\"typeof(SQLCondition)\"/>");
            this.writer.Write("/// <seealso cref=\"{0}\"/>", (object)this.entity.Name);
            this.writer.Write(" public {0} GetFirst(SQLCondition condition)",    this.entity.Name);
            this.writer.Write("  {{    ");
            this.writer.Write("       return base.GetFirst<{0}>(condition); ",   this.entity.Name);
            this.writer.Close();            
            this.writer.WriteLine();
            this.writer.WriteLine();
            this.writer.Write("/// <summary>");
            this.writer.Write("/// Method returning last instance of particular entity object");
            this.writer.Write("/// using multiple condition.");
            this.writer.Write("/// </summary>");
            this.writer.Write("/// <param name=\"condition\">condition");
            this.writer.Write("/// <seealso cref=\"typeof(SQLCondition)\"/>");
            this.writer.Write("/// <seealso cref=\"{0}\"/>", (object)this.entity.Name);
            this.writer.Write(" public {0} GetLast(SQLCondition condition)",      this.entity.Name);
            this.writer.Write("  {{     ");
            this.writer.Write("       return base.GetLast<{0}>(condition);",     this.entity.Name);
            this.writer.Close();
            this.writer.WriteLine();
            this.writer.WriteLine();
            this.writer.Write("/// <summary>");
            this.writer.Write("/// Method returning collection of entities");
            this.writer.Write("/// using multiple condition within limiting of result.");
            this.writer.Write("/// </summary>");
            this.writer.Write("/// <param name=\"condition\">condition");
            this.writer.Write("/// <seealso cref=\"typeof(SQLCondition)\"/>");
            this.writer.Write("/// <seealso cref=\"{0}\"/>", (object)this.entity.Name);
            this.writer.Write(" public IEnumerable<{0}> GetLimited(SQLCondition condition, int limit)", this.entity.Name);
            this.writer.Write("  {{      ");
            this.writer.Write("       return base.GetLimited<{0}>(condition, limit);", this.entity.Name);
            this.writer.Close();
            this.writer.WriteLine();
            this.writer.WriteLine();
            this.writer.Write("/// <summary>");
            this.writer.Write("/// Method returning collection of entities");
            this.writer.Write("/// using multiple condition.");
            this.writer.Write("/// </summary>");
            this.writer.Write("/// <param name=\"condition\">condition");
            this.writer.Write("/// <seealso cref=\"typeof(SQLCondition)\"/>");
            this.writer.Write("/// <seealso cref=\"{0}\"/>", (object)this.entity.Name);
            this.writer.Write(" public IEnumerable<{0}> GetRows(SQLCondition condition)", this.entity.Name);
            this.writer.Write("  {{      ");
            this.writer.Write("       return base.GetRows<{0}>(condition);", this.entity.Name);
            this.writer.Close();
            this.writer.WriteLine();
            this.writer.WriteLine();
            this.writer.Write("/// <summary>");
            this.writer.Write("/// Method adding an entire new instance of entity object");
            this.writer.Write("/// to current transaction.");
            this.writer.Write("/// </summary>");
            this.writer.Write("/// <param name=\"row\">{0} row", (object)this.entity.Name);           
            this.writer.Write("/// <seealso cref=\"{0}\"/>", (object)this.entity.Name);
            this.writer.Write(" public void Add({0} row)", this.entity.Name);
            this.writer.Write("  {{       ");
            this.writer.Write("     base.Add<{0}>(row);", this.entity.Name);
            this.writer.Close();
            this.writer.WriteLine();
            this.writer.WriteLine();
            this.writer.Write(" /// <summary>");
            this.writer.Write(" /// Add table to current transaction");
            this.writer.Write(" /// </summary>");
            this.writer.Write(" /// <param name=\"transaction\">Transaction</param>");
            this.writer.Write(" public static {0}Table GetInstance(ISqlTransaction transaction)", this.entity.Name);
            this.writer.Write("  {{        ");
            this.writer.Write("     if (transaction == null) return null; ");
            this.writer.Write("        return ({0}Table)transaction.Tables[typeof({0}Table)];",   this.entity.Name);
            this.writer.Close();
            this.writer.WriteLine();
            this.writer.WriteLine();
            this.writer.Close();
            this.writer.WriteLine();
            this.writer.Close();
            this.writer.WriteLine();
            //this.writer.Close();;
            //this.writer.Close();;
            this.writer.WriteLine();
        }


        void IDisposable.Dispose()
        {
            ((IDisposable)this.writer).Dispose();
        }

    }
}
