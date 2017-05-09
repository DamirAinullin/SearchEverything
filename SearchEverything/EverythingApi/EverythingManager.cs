using System;
using System.IO;
using System.Runtime.InteropServices;

namespace SearchEverything.EverythingApi
{
    public class EverythingManager
    {
        public const string Dll = "Everything.dll";

        [DllImport("kernel32.dll", EntryPoint = "LoadLibrary", SetLastError = true)]
        private static extern IntPtr LoadLibrary(string lpFileName);

        private string GetTheDllHandle(string dllName)
        {
            IntPtr dllHandle = LoadLibrary(dllName); // the dllHandle=IntPtr.Zero

            if (dllHandle == IntPtr.Zero)
            {
                return Marshal.GetLastWin32Error().ToString(); // Error Code while loading DLL
            }
            return dllHandle.ToString();  // Loading done!
        }

        public void Init(string extensionPath)
        {
            var sdkPath = Path.Combine(extensionPath, @"Everything\x86", Dll);
            GetTheDllHandle(sdkPath);
        }
    }
}
