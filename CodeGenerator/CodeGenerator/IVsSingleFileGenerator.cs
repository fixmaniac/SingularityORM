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
