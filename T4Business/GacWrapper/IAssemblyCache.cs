﻿using System;
using System.Runtime.InteropServices;

namespace T4Business.GacWrapper
{
    [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("e707dcde-d1cd-11d2-bab9-00c04f8eceae")]
    internal interface IAssemblyCache
    {
        [PreserveSig()]
        int UninstallAssembly(
            int flags,
            [MarshalAs(UnmanagedType.LPWStr)]
                String assemblyName,
            InstallReference refData,
            out AssemblyCacheUninstallDisposition disposition);
       
        [PreserveSig()]
        int QueryAssemblyInfo(
            int flags,
            [MarshalAs(UnmanagedType.LPWStr)]
                String assemblyName,
            ref AssemblyInfo assemblyInfo);
        [PreserveSig()]
        int Reserved        (
            int flags,
            IntPtr pvReserved,
            out Object ppAsmItem,
            [MarshalAs(UnmanagedType.LPWStr)]
                String assemblyName);
        [PreserveSig()]
        int Reserved(out Object ppAsmScavenger);
       
        [PreserveSig()]
        int InstallAssembly(
            int flags,
            [MarshalAs(UnmanagedType.LPWStr)]
                String assemblyFilePath,
            InstallReference refData);
    }
}