using System;
using System.Diagnostics;
using System.Reflection;
using System.IO;
using System.Runtime.InteropServices;
using System.IO;
using System.Text;
using System.Xml;
using Microsoft.Win32;
using CodeGenerator.Schema;

namespace CodeGenerator
{
    /// <summary>
    /// Singularity ORM Custom Tool for generating entities 
    /// based upon a specific XML schema
    /// </summary>
    [ComVisible(true)]
    [Guid("EFA99881-4AE7-40FB-8043-1D2CADA4B996")]        
    public class SingularityCodeGenerator : IVsSingleFileGenerator
    {
        private string codeFileNameSpace = string.Empty;
        private string codeFilePath = string.Empty;        
        private IVsGeneratorProgress codeGeneratorProgress;
      
        public string GetDefaultExtension()
        {            
            return ".cs";
        }

        protected byte[] GenerateCode(string inputFileName, string inputFileContent)
        {
            if (inputFileName.EndsWith(".entity.xml", StringComparison.InvariantCultureIgnoreCase))
            {
                XmlReader xml = (XmlReader)new XmlTextReader(inputFileName);               
                Entity entity = Entity.Load(xml);               
                EntityBuilder builder = new EntityBuilder(entity);
                string result = "";
                bool err = false;
                try
                {
                    builder.Build();
                }
                catch (Exception e)
                {
                    err = true;
                    result = string.Format("[Error]: {0}",e.Message);
                }
                finally
                {
                    if (!err)
                        result = builder.sb.ToString();
                }                
                byte[] bytes = Encoding.UTF8.GetBytes(result);                
                return bytes;
            }
            return Encoding.GetEncoding(1250).GetBytes("");
        }

        public void Generate(string wszInputFilePath, string bstrInputFileContents, 
                            string wszDefaultNamespace, out IntPtr rgbOutputFileContents, 
                            out int pcbOutput, IVsGeneratorProgress pGenerateProgress)
        {
            if (bstrInputFileContents == null)
                throw new ArgumentNullException(bstrInputFileContents);
            this.codeFilePath = wszInputFilePath;
            this.codeFileNameSpace = wszDefaultNamespace;
            this.codeGeneratorProgress = pGenerateProgress;
            byte[] source = this.GenerateCode(wszInputFilePath, bstrInputFileContents);
            if (source == null)
            {
                rgbOutputFileContents = IntPtr.Zero;
                pcbOutput = 0;
            }
            else
            {
                pcbOutput = source.Length;
                rgbOutputFileContents = Marshal.AllocCoTaskMem(pcbOutput);
                Marshal.Copy(source, 0, rgbOutputFileContents, pcbOutput);
            }
        }

        protected virtual void GeneratorErrorCallback(bool warning, int level, string message, int line, int column)
        {
            IVsGeneratorProgress generatorProgress = this.CodeGeneratorProgress;
            if (generatorProgress == null)
                return;
            generatorProgress.GeneratorError(warning, level, message, line, column);
        }

        internal IVsGeneratorProgress CodeGeneratorProgress
        {
            get
            {
                return this.codeGeneratorProgress;
            }
        }

        [ComRegisterFunction]
        public static void RegisterClass(Type t)
        {
            GuidAttribute guidAttribute = t.GetCustomAttribute<GuidAttribute>();
            using (RegistryKey key = Registry.LocalMachine.CreateSubKey(GetCSKey("SingularityCodeGenerator")))
            {
                key.SetValue("", "Singularity ORM custom entities generator");
                key.SetValue("CLSID", string.Format("{{{0}}}", guidAttribute.Value));
                key.SetValue("GeneratesDesignTimeSource", 0);
            }
        }

        [ComUnregisterFunction]
        public static void UnregisterClass(Type t)
        {
            Registry.LocalMachine.DeleteSubKey(GetCSKey("SingularityCodeGenerator"), false);
        }

        private static string GetCSKey(string toolName)
        {
            return string.Format
                ("SOFTWARE\\Microsoft\\VisualStudio\\12.0\\Generators\\{{FAE04EC1-301F-11D3-BF4B-00C04F79EFBC}}\\{0}", 
                toolName);
        }

    }
}
