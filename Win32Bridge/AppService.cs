using System;
using System.Diagnostics;
using Windows.ApplicationModel.AppService;
using Windows.ApplicationModel.Background;
using Windows.Foundation.Collections;
using Microsoft.Win32;

namespace Win32Bridge
{
    public class AppService
    {
        BackgroundTaskDeferral _serviceDeferral;
        AppServiceConnection _connection;

        public async void RunAppService()
        {
            _connection = new AppServiceConnection
            {
                AppServiceName = "ProgramAppBridge",
                PackageFamilyName = Windows.ApplicationModel.Package.Current.Id.FamilyName
            };
            _connection.RequestReceived += OnRequestReceived;
            AppServiceConnectionStatus status = await _connection.OpenAsync();
            var msg = string.Empty;
            switch (status)
            {
                case AppServiceConnectionStatus.Success:
                    msg = "Connection established - waiting for requests";
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine(msg);
                    break;
                case AppServiceConnectionStatus.AppNotInstalled:
                    msg = "The app AppServicesProvider is not installed.";
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(msg);
                    return;
                case AppServiceConnectionStatus.AppUnavailable:
                    msg = "The app AppServicesProvider is not available.";
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(msg);
                    return;
                case AppServiceConnectionStatus.AppServiceUnavailable:
                    msg = $"The app AppServicesProvider is installed but it does not provide the app service {_connection.AppServiceName}.";
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(msg);
                    return;
                case AppServiceConnectionStatus.Unknown:
                    msg = "An unkown error occurred while we were trying to open an AppServiceConnection.";
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(msg);
                    return;
            }
        }

        private async void OnRequestReceived(AppServiceConnection sender, AppServiceRequestReceivedEventArgs args)
        {
            //Get a deferral so we can use an awaitable API to respond to the message
            var messageDeferral = args.GetDeferral();

            // Get the message and the command to process here
            var message = args.Request.Message;
            var command = message["Command"] as string;

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Processing Command '" + command + "'");
            switch (command)
            {
                case "getprinters":
                    {
                        var returnMessage = new ValueSet();
                        returnMessage.Add("result", "success");
                        returnMessage.Add("data", GetAllPrinter());
                        await args.Request.SendResponseAsync(returnMessage);
                        messageDeferral.Complete();
                        break;
                    }
                case "getcpu":
                    {
                        var returnMessage = new ValueSet();
                        returnMessage.Add("result", "success");
                        returnMessage.Add("data", GetClientCpu());
                        await args.Request.SendResponseAsync(returnMessage);
                        messageDeferral.Complete();
                        break;
                    }
                case "getmemory":
                    {
                        var returnMessage = new ValueSet();
                        returnMessage.Add("result", "success");
                        returnMessage.Add("data", GetClientMemory());
                        await args.Request.SendResponseAsync(returnMessage);
                        messageDeferral.Complete();
                        break;
                    }

                case "openie":
                    {
                        Process.Start("IEXPLORE.EXE", "https://msdn.microsoft.com/en-us/default.aspx");
                        var returnMessage = new ValueSet();
                        returnMessage.Add("result", "success");
                        returnMessage.Add("data", "Open IE with msdn.microsoft.com completed");
                        await args.Request.SendResponseAsync(returnMessage);
                        messageDeferral.Complete();
                        break;
                    }
                default:
                    {
                        var returnMessage = new ValueSet { { "Result", "error undefined command " + command } };
                        await args.Request.SendResponseAsync(returnMessage);
                        messageDeferral.Complete();
                        break;
                    }
            }
        }


        public string GetAllPrinter()
        {
            var msg = "List of All Printer";
            foreach (string printer in System.Drawing.Printing.PrinterSettings.InstalledPrinters)
            {
                msg += printer + "\n";
            }

            return msg;
        }

        public string GetClientCpu()
        {
            var msg = string.Empty;
            msg += "Client CPU Data" + "\n";

            var registrykeyHklm = Registry.LocalMachine;
            const string keyPath = @"HARDWARE\DESCRIPTION\System\CentralProcessor\0";
            var registrykeyCpu = registrykeyHklm.OpenSubKey(keyPath, false);
            if (registrykeyCpu != null)
            {
                var cpuSpeed = registrykeyCpu.GetValue("~MHz").ToString();
                var processorNameString = (string)registrykeyCpu.GetValue("ProcessorNameString");

                registrykeyHklm.Close();
                registrykeyCpu.Close();

                msg += "CPU Speed : " + cpuSpeed + " MHz" + "\n";
                msg += "CPU Name : " + processorNameString + "\n";
            }

            return msg;
        }

        public string GetClientMemory()
        {
            var msg = string.Empty;
            msg += "Client Memory Data" + "\n";
            var computerInfo = new Microsoft.VisualBasic.Devices.ComputerInfo();

            var diff = Convert.ToDouble(1024 * 1024 * 1024);
            msg += "TotalPhysicalMemory : " + (Convert.ToDouble(computerInfo.TotalPhysicalMemory) / diff).ToString("0.00") + " GB" + "\n";
            msg += "TotalVirtualMemory : " + (Convert.ToDouble(computerInfo.TotalVirtualMemory) / diff).ToString("0.00") + " GB" + "\n";
            msg += "AvailablePhysicalMemory : " + (Convert.ToDouble(computerInfo.AvailablePhysicalMemory) / diff).ToString("0.00") + " GB" + "\n";
            msg += "AvailableVirtualMemory : " + (Convert.ToDouble(computerInfo.AvailableVirtualMemory) / diff).ToString("0.00") + " GB" + "\n";
            msg += "InstalledUICulture : " + computerInfo.InstalledUICulture + "\n";
            msg += "OSFullName : " + computerInfo.OSFullName + "\n";
            msg += "OSPlatform : " + computerInfo.OSPlatform + "\n";
            msg += "OSVersion : " + computerInfo.OSVersion + "\n";
            return msg;
        }
    }
}
