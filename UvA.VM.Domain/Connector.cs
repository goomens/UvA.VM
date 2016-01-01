using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UvA.VM.Domain.Entities;

namespace UvA.VM.Domain
{
    public class Connector
    {
        static readonly string ImagesFolder = ConfigurationManager.AppSettings["ImagesFolder"];
        static readonly string MachineFolder = ConfigurationManager.AppSettings["MachineFolder"];
        static readonly string MachineFolderLocal = ConfigurationManager.AppSettings["MachineFolderLocal"];
        static readonly string NetworkName = ConfigurationManager.AppSettings["NetworkName"];
        static readonly string DHCPScope = ConfigurationManager.AppSettings["DHCPScope"];


        string TargetMachine;

        public Connector(string targetMachine)
        {
            TargetMachine = targetMachine;
        }

        Command CreateCommand(string cmd, string name = null)
        {
            Command z = new Command(cmd);
            if (name != null)
                z.Parameters.Add("Name", name);
            z.Parameters.Add("ComputerName", TargetMachine);
            return z;
        }

        dynamic ExecuteCommand(Command cmd)
        {
            var ps = PowerShell.Create();
            ps.Commands.AddCommand(cmd);
            dynamic obj = null;
            try
            {
                obj = ps.Invoke();
            }
            catch (InvalidOperationException)
            {
                if (ps.Streams.Error.Any())
                    throw ps.Streams.Error[0].Exception;
            }
            return obj;
        }

        dynamic ExecuteCommandSingle(Command cmd)
        {
            var obj = ExecuteCommand(cmd);
            return obj[0];
        }

        public VMObject GetVM(string name)
        {
            try
            {
                var obj = ExecuteCommandSingle(CreateCommand("Get-VM", name));
                return GetVMInfo(obj);
            }
            catch (Exception ex)
            {
                Logger.Log("GetVM", ex.ToString());
                throw;
            }
        }

        VMObject GetVMInfo(dynamic obj)
        {
            var cmd = CreateCommand("Get-VMNetworkAdapter");
            cmd.Parameters.Add("VMName", obj.Name);
            var ad = ExecuteCommandSingle(cmd);

            return new VMObject()
            {
                Name = obj.Name,
                UpTime = obj.Uptime,
                CPUUsage = obj.CPUUsage,
                Heartbeat = obj.Heartbeat == null ? "" : obj.Heartbeat.ToString(),
                Status = obj.PrimaryStatusDescription,
                State = obj.State.ToString(),
                MacAddress = ad.MacAddress,
                MemoryAssigned = obj.MemoryAssigned
            };
        }

        public IEnumerable<VMObject> GetVMs(string name = null)
        {
            var obj = ExecuteCommand(CreateCommand("Get-VM", name));
            List<VMObject> vms = new List<VMObject>();
            foreach (var o in obj)
                vms.Add(GetVMInfo(o));
            return vms;
        }

        public void StartVM(string name)
        {
            ExecuteCommandSingle(CreateCommand("Start-VM", name));
        }

        public void StopVM(string name)
        {
            ExecuteCommandSingle(CreateCommand("Stop-VM", name));
        }

        public void CreateVM(VMInitObject vm)
        {
            Console.WriteLine("CreateVM " + vm.Name);
            if (!string.IsNullOrEmpty(vm.IPAddress) && string.IsNullOrEmpty(vm.MacAddress))
                throw new InvalidOperationException("MAC address must be specified");
            
            if (GetVMs(vm.Name).Any())
                throw new InvalidOperationException("A virtual machine with that name already exists.");
            string path = null;
            try
            {
                path = string.Format("{0}{1}.vhdx", MachineFolder, vm.Name);
                File.Copy(ImagesFolder + vm.ImageName, path);

                var cmd = CreateCommand("New-VM", vm.Name);
                cmd.Parameters.Add("MemoryStartupBytes", vm.Memory);
                cmd.Parameters.Add("VHDPath", Path.Combine(MachineFolderLocal, $"{vm.Name}.vhdx"));
                ExecuteCommand(cmd);

                Thread.Sleep(2000);

                if (!string.IsNullOrEmpty(vm.MacAddress))
                {
                    cmd = CreateCommand("Set-VMNetworkAdapter");
                    cmd.Parameters.Add("VMName", vm.Name);
                    cmd.Parameters.Add("StaticMacAddress", vm.MacAddress);
                    ExecuteCommand(cmd);
                }

                Thread.Sleep(1000);

                cmd = CreateCommand("Connect-VMNetworkAdapter");
                cmd.Parameters.Add("VMName", vm.Name);
                cmd.Parameters.Add("SwitchName", NetworkName);
                ExecuteCommand(cmd);

                if (!string.IsNullOrEmpty(vm.IPAddress))
                {
                    cmd = new Command("Add-DhcpServerv4Reservation");
                    cmd.Parameters.Add("ScopeId", DHCPScope);
                    cmd.Parameters.Add("IPAddress", vm.IPAddress);
                    cmd.Parameters.Add("ClientId", vm.MacAddress);
                    cmd.Parameters.Add("Name", vm.Name);
                    ExecuteCommand(cmd);
                }
            }
            catch (Exception ex)
            {
                if (File.Exists(path))
                {
                    try 
                    {
                        var cmd = CreateCommand("Remove-VM", vm.Name);
                        cmd.Parameters.Add("Force");
                        ExecuteCommand(cmd);
                    }
                    catch
                    {

                    }
                    File.Delete(path);
                }

                Logger.Log("NewVM", ex.ToString());
                throw;
            }
        }
    }
}
