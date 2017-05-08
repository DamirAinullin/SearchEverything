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
            Process.Start(Path.Combine(_extensionPath,
                $@"Everything\{_bitness}\Everything.exe"), "-install-service");
        }

        public void UninstallService()
        {
            Process.Start(Path.Combine(_extensionPath,
                $@"Everything\{_bitness}\Everything.exe"), "-uninstall-service");
        }

        public void StartService()
        {
            Process.Start(Path.Combine(_extensionPath,
                $@"Everything\{_bitness}\Everything.exe"), "-start-service");
        }

        public void StopService()
        {
            Process.Start(Path.Combine(_extensionPath,
                $@"Everything\{_bitness}\Everything.exe"), "-stop-service");
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
            Process.Start(Path.Combine(_extensionPath, $@"Everything\{_bitness}\Everything.exe"), "-startup");
        }

        public int GetNumberOfEverythingProcess()
        {
            var processes = Process.GetProcessesByName("everything");
            return processes.Length;
        }
    }
}
