using System;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;

namespace SearchEverything
{
    public class EverythingServiceManager
    {
        private readonly string _bitness = Environment.Is64BitOperatingSystem ? "x64" : "x86";

        public void InstallService()
        {
            Process.Start($@"Everything\{_bitness}\Everything.exe", "-install-service");
        }

        public void UninstallService()
        {
            Process.Start($@"Everything\{_bitness}\Everything.exe", "-uninstall-service");
        }

        public void StartService()
        {
            Process.Start($@"Everything\{_bitness}\Everything.exe", "-start-service");
        }

        public void StopService()
        {
            Process.Start($@"Everything\{_bitness}\Everything.exe", "-stop-service");
        }

        public bool IsServiceInstalled()
        {
            return ServiceController.GetServices().Any(s => s.ServiceName == "Everything");
        }

        public bool IsServiceRunning()
        {
            var service = ServiceController.GetServices().FirstOrDefault(s => s.ServiceName == "Everything");
            return service?.Status == ServiceControllerStatus.Running;
        }

        public void StartClientInBackgroundMode()
        {
            Process.Start($@"Everything\{_bitness}\Everything.exe", "-startup");
        }
    }
}
