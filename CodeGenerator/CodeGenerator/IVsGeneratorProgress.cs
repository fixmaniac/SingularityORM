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
