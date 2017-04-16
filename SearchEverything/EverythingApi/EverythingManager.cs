using System;
using System.IO;
using System.Runtime.InteropServices;

namespace SearchEverything.EverythingApi
{
    public class EverythingManager
    {
        public const string Dll = "Everything.dll";

        [DllImport("kernel32.dll", EntryPoint = "LoadLibrary", SetLastError = true)]
        public static extern IntPtr LoadLibrary(string lpFileName);

        public string GetTheDllHandle(string dllName)
        {
            IntPtr dllHandle = LoadLibrary(dllName); // the dllHandle=IntPtr.Zero

            if (dllHandle == IntPtr.Zero)
                return Marshal.GetLastWin32Error().ToString(); // Error Code while loading DLL
            else
                return dllHandle.ToString();  // Loading done !
        }


        public void Init()
        {
            var sdkPath = Path.Combine(Directory.GetCurrentDirectory(), "Everything", CpuType(), Dll);
            string result = GetTheDllHandle(sdkPath);
        }

        private static string CpuType()
        {
            return /*Environment.Is64BitOperatingSystem ? "x64" :*/ "x86";
        }
    }
}
