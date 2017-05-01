using System;
using System.Runtime.InteropServices;
using System.Text;

namespace SearchEverything.EverythingApi
{
    public static class EverythingNativeApi
    {
        #region DllImport
        [DllImport(EverythingManager.Dll, CharSet = CharSet.Unicode)]
        public static extern int Everything_SetSearchW(string lpSearchString);
        [DllImport(EverythingManager.Dll)]
        public static extern void Everything_SetMatchPath(bool bEnable);
        [DllImport(EverythingManager.Dll)]
        public static extern void Everything_SetMatchCase(bool bEnable);
        [DllImport(EverythingManager.Dll)]
        public static extern void Everything_SetMatchWholeWord(bool bEnable);
        [DllImport(EverythingManager.Dll)]
        public static extern void Everything_SetRegex(bool bEnable);
        [DllImport(EverythingManager.Dll)]
        public static extern void Everything_SetMax(int dwMax);
        [DllImport(EverythingManager.Dll)]
        public static extern void Everything_SetOffset(int dwOffset);

        [DllImport(EverythingManager.Dll)]
        public static extern bool Everything_GetMatchPath();
        [DllImport(EverythingManager.Dll)]
        public static extern bool Everything_GetMatchCase();
        [DllImport(EverythingManager.Dll)]
        public static extern bool Everything_GetMatchWholeWord();
        [DllImport(EverythingManager.Dll)]
        public static extern bool Everything_GetRegex();
        [DllImport(EverythingManager.Dll)]
        public static extern UInt32 Everything_GetMax();
        [DllImport(EverythingManager.Dll)]
        public static extern UInt32 Everything_GetOffset();
        [DllImport(EverythingManager.Dll, CharSet = CharSet.Unicode)]
        public static extern string Everything_GetSearchW();
        [DllImport(EverythingManager.Dll)]
        public static extern StateCode Everything_GetLastError();

        [DllImport(EverythingManager.Dll, CharSet = CharSet.Unicode)]
        public static extern bool Everything_QueryW(bool bWait);

        [DllImport(EverythingManager.Dll)]
        public static extern void Everything_SortResultsByPath();

        [DllImport(EverythingManager.Dll)]
        public static extern int Everything_GetNumFileResults();
        [DllImport(EverythingManager.Dll)]
        public static extern int Everything_GetNumFolderResults();
        [DllImport(EverythingManager.Dll)]
        public static extern int Everything_GetNumResults();
        [DllImport(EverythingManager.Dll)]
        public static extern int Everything_GetTotFileResults();
        [DllImport(EverythingManager.Dll)]
        public static extern int Everything_GetTotFolderResults();
        [DllImport(EverythingManager.Dll)]
        public static extern int Everything_GetTotResults();
        [DllImport(EverythingManager.Dll)]
        public static extern bool Everything_IsVolumeResult(int nIndex);
        [DllImport(EverythingManager.Dll)]
        public static extern bool Everything_IsFolderResult(int nIndex);
        [DllImport(EverythingManager.Dll)]
        public static extern bool Everything_IsFileResult(int nIndex);
        [DllImport(EverythingManager.Dll, CharSet = CharSet.Unicode)]
        public static extern void Everything_GetResultFullPathNameW(int nIndex, StringBuilder lpString, int nMaxCount);
        [DllImport(EverythingManager.Dll)]
        public static extern void Everything_Reset();
        #endregion
    }
}
