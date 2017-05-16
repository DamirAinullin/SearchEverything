using System.Runtime.InteropServices;

namespace SetupSearchEverything.CustomActions
{
    public static class NativeMethods
    {
        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        internal static extern bool MoveFileEx(string lpExistingFileName, string lpNewFileName,
            MoveFileFlags dwFlags);
    }
}