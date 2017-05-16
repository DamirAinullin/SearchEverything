using System;
using System.IO;

namespace SetupSearchEverything.CustomActions
{
    public class Cleanuper
    {
        private readonly Logger _logger;

        public Cleanuper(Logger logger)
        {
            _logger = logger;
        }

        public void DeleteDirectory(string installFolder)
        {
            try
            {
                Directory.Delete(installFolder);
            }
            catch
            {
                PendingDeleteDirectory(installFolder);
            }
        }

        private void PendingDeleteDirectory(string directoryPath)
        {
            try
            {
                foreach (string directory in Directory.GetDirectories(directoryPath, "*", SearchOption.TopDirectoryOnly))
                {
                    PendingDeleteDirectory(directory);
                }

                _logger.Log($"Pending directory deletion: {directoryPath}");
                foreach (string file in Directory.GetFiles(directoryPath, "*", SearchOption.TopDirectoryOnly))
                {
                    NativeMethods.MoveFileEx(file, null, MoveFileFlags.DelayUntilReboot);
                }
                NativeMethods.MoveFileEx(directoryPath, null, MoveFileFlags.DelayUntilReboot);
            }
            catch (Exception ex)
            {
                _logger.Log(ex.ToString());
            }
        }
    }
}
