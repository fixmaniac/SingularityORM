using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace CodeGenerator
{
    public class EntityClassWriter : IDisposable
    {
        private string ident = "";
        private readonly TextWriter writer;
        public MemoryStream Stream { get; set; }
        public int Ident
        {
            get
            {
                return this.ident.Length;
            }
            set
            {
                Debug.Assert(value >= 0);
                this.ident = new string('\t', value);
            }
        }

        public EntityClassWriter()
        {
            this.Stream = new MemoryStream();
            this.writer = new StreamWriter(this.Stream);
        }

        public EntityClassWriter(string fileName)
        {
            this.writer = (TextWriter)new StreamWriter(fileName, false, Encoding.UTF8);
        }

        void IDisposable.Dispose()
        {
            this.writer.Close();
        }

        public void Write(string line)
        {
            this.writer.WriteLine(this.ident + line);
            if (!(line != "") || (int)line[line.Length - 1] != 123)
                return;
            ++this.Ident;
        }

        public void Write(string format, params object[] arr)
        {
            ++this.Ident;
            this.writer.WriteLine(this.ident + format, arr);
            if (!(format != "") || (int)format[format.Length - 1] != 123)
                return;
           
        }

        public void WriteLine()
        {
            this.writer.WriteLine();
        }

        public void Close()
        {
            --this.Ident;
            this.Write("}");
        }
    }
}
