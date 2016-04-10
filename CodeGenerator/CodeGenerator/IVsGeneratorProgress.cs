using System;
using System.Runtime.InteropServices;

namespace CodeGenerator
{
    /// <summary>
    /// Enables the single file generator to report on its progress 
    /// and to provide additional warning and/or error information.
    /// </summary>
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("BED89B98-6EC9-43CB-B0A8-41D6E2D6669D")]
    [ComImport]
    public interface IVsGeneratorProgress
    {
        void GeneratorError(bool fWarning, 
            [MarshalAs(UnmanagedType.U4)] int dwLevel, 
            [MarshalAs(UnmanagedType.BStr)] string bstrError, 
            [MarshalAs(UnmanagedType.U4)] int dwLine, 
            [MarshalAs(UnmanagedType.U4)] int dwColumn);

        void Progress([MarshalAs(UnmanagedType.U4)] int nComplete, 
                      [MarshalAs(UnmanagedType.U4)] int nTotal);
    }
}
