using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace CodeGenerator
{
    /// <summary>
    /// Transforms a single input file into a single output file that 
    /// can be compiled or added to a project. 
    /// Any COM component that implements the IVsSingleFileGenerator is a custom tool.
    /// </summary>
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("3634494C-492F-4F91-8009-4541234E4E99")]
    [ComImport]
    public interface IVsSingleFileGenerator
    {
        [return: MarshalAs(UnmanagedType.BStr)]
        string GetDefaultExtension();

        void Generate([MarshalAs(UnmanagedType.LPWStr)] string wszInputFilePath, 
                      [MarshalAs(UnmanagedType.BStr)] string bstrInputFileContents, 
                      [MarshalAs(UnmanagedType.LPWStr)] string wszDefaultNamespace, 
                      out IntPtr rgbOutputFileContents, 
                      [MarshalAs(UnmanagedType.U4)] out int pcbOutput, 
                            IVsGeneratorProgress pGenerateProgress);
    }
}
