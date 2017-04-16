using System;
using System.IO;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace SearchEverything
{
    public class OpenFileManager
    {
        private static OpenFileManager _instance;
        private readonly IServiceProvider _serviceProvider;

        private OpenFileManager(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public static void CreateInstance(IServiceProvider serviceProvider)
        {
            if (_instance == null)
            {
                _instance = new OpenFileManager(serviceProvider);
            }
        }

        public static OpenFileManager GetInstance()
        {
            return _instance;
        }

        public IVsWindowFrame OpenDocumentInNewWindow(string filePath)
        {
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
            {
                return null;
            }

            IVsUIHierarchy hierarchy;
            uint itemId;
            IVsWindowFrame frame;
            if (!VsShellUtilities.IsDocumentOpen(_serviceProvider, filePath,
                VSConstants.LOGVIEWID_Primary, out hierarchy, out itemId, out frame))
            {
                VsShellUtilities.OpenDocument(_serviceProvider, filePath,
                    VSConstants.LOGVIEWID_Primary, out hierarchy, out itemId, out frame);
            }

            frame?.Show();
            return frame;
        }
    }
}