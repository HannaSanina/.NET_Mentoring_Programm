using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace PowerStateManagement
{
    [ComVisible(true)]
    [Guid("8E2C74B2-8B52-4C12-8FCF-23F86DE02EE4")]
    [ClassInterface(ClassInterfaceType.None)]
    public class PowerManagement : IPowerManagement, IFormatFunctions
    {

        public ulong GetLastSleepTime()
        {
            ulong time;
            uint retval = DllFunctionImporter.GetTime(
                InformationLevel.LastSleepTime,
                IntPtr.Zero,
                0,
                out time,
                Marshal.SizeOf(typeof(ulong))
            );

            HandleException(retval, InformationLevel.LastSleepTime);

            return time * 100 / 1000000000; //function return value in units by 100 nanoseconds in each
        }

        public ulong GetLastWakeTime()
        {
            ulong time;
            uint retval = DllFunctionImporter.GetTime(
                InformationLevel.LastWakeTime,
                IntPtr.Zero,
                0,
                out time,
                Marshal.SizeOf(typeof(ulong))
            );

            HandleException(retval, InformationLevel.LastWakeTime);
           
            return time * 100 / 1000000000; //function return value in units by 100 nanoseconds in each;
        }

        public SYSTEM_BATTERY_STATE GetBatteryState()
        {
            SYSTEM_BATTERY_STATE batteryState = new SYSTEM_BATTERY_STATE();
            uint retval = DllFunctionImporter.GetBatteryState(
                InformationLevel.SystemBatteryState,
                IntPtr.Zero,
                0,
                out batteryState,
                Marshal.SizeOf(typeof(SYSTEM_BATTERY_STATE))
            );
            HandleException(retval, InformationLevel.SystemBatteryState);

            return batteryState;
        }

        public SYSTEM_POWER_INFORMATION GetPowerInformation()
        {
            SYSTEM_POWER_INFORMATION systemInfo = new SYSTEM_POWER_INFORMATION();
            uint retval = DllFunctionImporter.GetSystemPowerInfo(
                InformationLevel.SystemPowerInformation,
                IntPtr.Zero,
                0,
                out systemInfo,
                Marshal.SizeOf(typeof(SYSTEM_POWER_INFORMATION))
            );

            HandleException(retval, InformationLevel.SystemPowerInformation);

            return systemInfo;
        }

        public bool ReserveHibernationFile()
        {
            bool isSuccess = false;

            uint retval = DllFunctionImporter.ChangeHibernateFile(
                InformationLevel.SystemReserveHiberFile,
                true,
                0,
                out isSuccess,
                Marshal.SizeOf(typeof(bool))
            );

            HandleException(retval, InformationLevel.SystemReserveHiberFile);

            return isSuccess;
        }

        public bool DeleteHibernationFile()
        {
            bool isSuccess = false;

            uint retval = DllFunctionImporter.ChangeHibernateFile(
                InformationLevel.SystemReserveHiberFile,
                false,
                0,
                out isSuccess,
                Marshal.SizeOf(typeof(bool))
            );

            HandleException(retval, InformationLevel.SystemReserveHiberFile);

            return isSuccess;
        }

        public void Sleep()
        {
            DllFunctionImporter.SetSuspendState(true, false, false);
        }

        private void HandleException(uint isFunctionSuccess, int fuctionCode)
        {
            if (isFunctionSuccess != 0)
            {
                throw new Win32Exception($"Execution failed. Error in function {fuctionCode}");
            }
        }

        public string GetLastSleepTimeAsString()
        {
           ulong result = GetLastSleepTime();
           return $"Last sleep time {result} seconds";
        }

        public string GetLastWakeTimeAsString()
        {
            ulong result = GetLastWakeTime();
            return $"Last wake time {result} seconds";
        }

        public string GetBatteryStateAsString()
        {
            SYSTEM_BATTERY_STATE result = GetBatteryState();
            return $"Battery state: AcOnLine = {result.AcOnLine}, Charging = {result.Charging}, MaxCapacity ={result.MaxCapacity}";
        }

        public string GetPowerInformationAsString()
        {
            SYSTEM_POWER_INFORMATION result = GetPowerInformation();
            return $"Battery state: CoolingMode = {result.CoolingMode}, Idleness = {result.Idleness}, TimeRemaining = {result.TimeRemaining}";
        }

        public string ReserveHibernationFileAsString()
        {
            bool result = ReserveHibernationFile();
            return $"Reservation of hibernation file successful: {result}";
        }

        public string DeleteHibernationFileAsString()
        {
            bool result = DeleteHibernationFile();
            return $"Deletion of hibernation file successful: {result}";
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SYSTEM_POWER_INFORMATION
    {
        public uint MaxIdlenessAllowed;
        public uint Idleness;
        public uint TimeRemaining;
        public byte CoolingMode;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SYSTEM_BATTERY_STATE
    {
        public byte AcOnLine;
        public byte BatteryPresent;
        public byte Charging;
        public byte Discharging;
        public byte spare1;
        public byte spare2;
        public byte spare3;
        public byte spare4;
        public UInt32 MaxCapacity;
        public UInt32 RemainingCapacity;
        public Int32 Rate;
        public UInt32 EstimatedTime;
        public UInt32 DefaultAlert1;
        public UInt32 DefaultAlert2;
    }
}
