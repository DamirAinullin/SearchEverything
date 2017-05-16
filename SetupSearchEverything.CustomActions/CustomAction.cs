using System;
using System.IO;
using System.Threading;
using Microsoft.Deployment.WindowsInstaller;

namespace SetupSearchEverything.CustomActions
{
    // ReSharper disable once UnusedMember.Global
    public static class CustomActions
    {
        [CustomAction]
        // ReSharper disable once UnusedMember.Global
        public static ActionResult InstallEverythingService(Session session)
        {
            var logger = new Logger(session, nameof(InstallEverythingService));
            try
            {
                logger.Log("Start {0}", nameof(InstallEverythingService));
                string installFolder = FindExtensionFolder();
                var everythingProcessManager = new EverythingProcessManager(installFolder);
                if (everythingProcessManager.IsServiceInstalled())
                {
                    if (everythingProcessManager.IsServiceRunning())
                    {
                        everythingProcessManager.StopService();
                    }
                    everythingProcessManager.UninstallService();
                }
                everythingProcessManager.InstallService();
                // wait untill service will be fine
                Thread.Sleep(3000);
                everythingProcessManager.StartClientInBackgroundMode();
                return ActionResult.Success;
            }
            catch (Exception ex)
            {
                logger.Log("Failure: {0}", ex);
                return ActionResult.Failure;
            }
            finally
            {
                logger.Log("End {0}", nameof(InstallEverythingService));
            }
        }

        [CustomAction]
        // ReSharper disable once UnusedMember.Global
        public static ActionResult UninstallEverythingService(Session session)
        {
            var logger = new Logger(session, nameof(UninstallEverythingService));
            try
            {
                logger.Log("Start {0}", nameof(UninstallEverythingService));
                string installFolder = FindExtensionFolder();
                var everythingProcessManager = new EverythingProcessManager(installFolder);
                if (everythingProcessManager.IsServiceInstalled())
                {
                    if (everythingProcessManager.IsServiceRunning())
                    {
                        everythingProcessManager.StopService();
                    }
                    everythingProcessManager.UninstallService();
                }

                everythingProcessManager.StopClient();

                var cleanuper = new Cleanuper(logger);
                cleanuper.DeleteDirectory(installFolder);

                return ActionResult.Success;
            }
            catch (Exception ex)
            {
                logger.Log("Failure: {0}", ex);
                return ActionResult.Failure;
            }
            finally
            {
                logger.Log("End {0}", nameof(UninstallEverythingService));
            }
        }

        private static string FindExtensionFolder()
        {
            var files = Directory.GetFiles(@"C:\Program Files (x86)\Microsoft Visual Studio 14.0\Common7\IDE\Extensions",
                "SearchEverything.dll", SearchOption.AllDirectories);
            return files.Length != 0 ? Directory.GetParent(files[0]).FullName : String.Empty;
        }
    }
}
