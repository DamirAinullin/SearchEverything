using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;

namespace SearchEverything.Utilities
{
    public class EverythingProcessManager
    {
        private readonly string _extensionPath;
        private readonly string _bitness = Environment.Is64BitOperatingSystem ? "x64" : "x86";

        public EverythingProcessManager(string extensionPath)
        {
            _extensionPath = extensionPath;
        }

        public void InstallService()
        {
            var process = Process.Start(Path.Combine(_extensionPath, $@"Everything\{_bitness}\Everything.exe"), "-install-service");
            process?.WaitForExit(10000);
            var service = ServiceController.GetServices().FirstOrDefault(s => s.ServiceName == "Everything");
            service?.WaitForStatus(ServiceControllerStatus.Running, new TimeSpan(0, 0, 0, 15));
        }

        public void UninstallService()
        {
            var process = Process.Start(Path.Combine(_extensionPath, $@"Everything\{_bitness}\Everything.exe"), "-uninstall-service");
            process?.WaitForExit(10000);
        }

        public void StartService()
        {
            var process = Process.Start(Path.Combine(_extensionPath, $@"Everything\{_bitness}\Everything.exe"), "-start-service");
            process?.WaitForExit(10000);
            var service = ServiceController.GetServices().FirstOrDefault(s => s.ServiceName == "Everything");
            service?.WaitForStatus(ServiceControllerStatus.Running, new TimeSpan(0, 0, 0, 15));
        }

        public void StopService()
        {
            var process = Process.Start(Path.Combine(_extensionPath, $@"Everything\{_bitness}\Everything.exe"), "-stop-service");
            process?.WaitForExit(10000);
            var service = ServiceController.GetServices().FirstOrDefault(s => s.ServiceName == "Everything");
            service?.WaitForStatus(ServiceControllerStatus.Stopped, new TimeSpan(0, 0, 0, 15));
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
            var process = Process.Start(Path.Combine(_extensionPath, $@"Everything\{_bitness}\Everything.exe"), "-startup");
            process?.WaitForExit(10000);
        }

        public int GetNumberOfEverythingProcess()
        {
            var processes = Process.GetProcessesByName("everything");
            return processes.Length;
        }
    }
}
