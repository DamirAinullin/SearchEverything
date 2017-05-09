using System;
using System.Diagnostics;
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
            /* Debugger.Break();
             var logger = new Logger(session, nameof(InstallEverythingService));
             try
             {
                 logger.Log("Start {0}", nameof(InstallEverythingService));
                 var installFolder = session.GetTargetPath("INSTALLFOLDER");
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
             }*/
            return ActionResult.Success;
        }

        [CustomAction]
        // ReSharper disable once UnusedMember.Global
        public static ActionResult UninstallEverythingService(Session session)
        {
            /*var logger = new Logger(session, nameof(UninstallEverythingService));
            try
            {
                logger.Log("Start {0}", nameof(UninstallEverythingService));
                var installFolder = session.GetTargetPath("INSTALLFOLDER");
                var everythingProcessManager = new EverythingProcessManager(installFolder);
                if (everythingProcessManager.IsServiceInstalled())
                {
                    if (everythingProcessManager.IsServiceRunning())
                    {
                        everythingProcessManager.StopService();
                    }
                    everythingProcessManager.UninstallService();
                }
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
            }*/
            return ActionResult.Success;
        }
    }
}
