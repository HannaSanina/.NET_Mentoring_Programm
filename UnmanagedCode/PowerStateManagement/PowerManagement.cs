using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PowerStateManagement
{
    [ComVisible(true)]
    [Guid("8E2C74B2-8B52-4C12-8FCF-23F86DE02EE4")]
    [ClassInterface(ClassInterfaceType.None)]
    public class PowerManagement : IPowerManagement
    {
        [DllImport("Powrprof.dll", SetLastError = true)]
        static extern bool SetSuspendState(bool hibernate, bool forceCritical, bool disableWakeEvent);

        [DllImport("powrprof.dll", SetLastError = true)]
        static extern uint CallNtPowerInformation(int InformationLevel, IntPtr lpInputBuffer, int nInputBufferSize, [Out]  SYSTEM_POWER_INFORMATION[] lpOutputBuffer, int nOutputBufferSize);

        [DllImport("powrprof.dll", SetLastError = true)]
        static extern uint CallNtPowerInformation(int InformationLevel, IntPtr lpInputBuffer, int nInputBufferSize, [Out]  SYSTEM_BATTERY_STATE[] lpOutputBuffer, int nOutputBufferSize);

        [DllImport("powrprof.dll", SetLastError = true)]
        static extern uint CallNtPowerInformation(int InformationLevel, IntPtr lpInputBuffer, int nInputBufferSize, [Out]  uint[] lpOutputBuffer, int nOutputBufferSize);

        [DllImport("powrprof.dll", SetLastError = true)]
        static extern uint CallNtPowerInformation(int InformationLevel, bool lpInputBuffer, int nInputBufferSize, [Out]  uint[] lpOutputBuffer, int nOutputBufferSize);


        public int GetLastSleepTime()
        {
            int sleepTime = 0;
            int procCount = Environment.ProcessorCount;
            uint[] procInfo = new uint[procCount];
            uint retval = CallNtPowerInformation(
                InformationLevel.LastSleepTime,
                IntPtr.Zero,
                0,
                procInfo,
                procInfo.Length * Marshal.SizeOf(typeof(uint))
            );

            if (retval == 0)
            {
                sleepTime = Convert.ToInt32(procInfo[0] * 100 / 1000000000);
            }

            return sleepTime;
        }

        public int GetLastWakeTime()
        {
            int awakeTime = 0;
            int procCount = Environment.ProcessorCount;
            uint[] procInfo = new uint[procCount];
            uint retval = CallNtPowerInformation(
                InformationLevel.LastWakeTime,
                IntPtr.Zero,
                0,
                procInfo,
                procInfo.Length * Marshal.SizeOf(typeof(uint))
            );

            if (retval == 0)
            {
                awakeTime = Convert.ToInt32(procInfo[0] * 100 / 1000000000);
            }

            return awakeTime;
        }

        public SYSTEM_BATTERY_STATE GetBatteryState()
        {
            SYSTEM_BATTERY_STATE result;
            int procCount = Environment.ProcessorCount;
            SYSTEM_BATTERY_STATE[] procInfo = new SYSTEM_BATTERY_STATE[procCount];
            uint retval = CallNtPowerInformation(
                InformationLevel.SystemBatteryState,
                IntPtr.Zero,
                0,
                procInfo,
                procInfo.Length * Marshal.SizeOf(typeof(SYSTEM_BATTERY_STATE))
            );
            if (retval == 0)
            {
                result = procInfo[0];
            }
            else
            {
                throw new Exception("Can't get battery state");
            }
            return result;
        }

        public SYSTEM_POWER_INFORMATION GetPowerInformation()
        {
            SYSTEM_POWER_INFORMATION result;
            int procCount = Environment.ProcessorCount;
            SYSTEM_POWER_INFORMATION[] procInfo = new SYSTEM_POWER_INFORMATION[procCount];
            uint retval = CallNtPowerInformation(
                InformationLevel.SystemPowerInformation,
                IntPtr.Zero,
                0,
                procInfo,
                procInfo.Length * Marshal.SizeOf(typeof(SYSTEM_POWER_INFORMATION))
            );
            if (retval == 0)
            {
                result = procInfo[0];
            }
            else
            {
                throw new Exception("Can't get power information");
            }
            return result;
        }

        public bool ReserveHibernationFile()
        {
            bool isSuccess = false;
            int procCount = Environment.ProcessorCount;
            uint[] procInfo = new uint[procCount];
            uint retval = CallNtPowerInformation(
                InformationLevel.SystemReserveHiberFile,
                true,
                0,
                procInfo,
                procInfo.Length * Marshal.SizeOf(typeof(uint))
            );

            if (retval == 0)
            {
                isSuccess = true;
            }
            return isSuccess;
        }

        public bool DeleteHibernationFile()
        {
            bool isSuccess = false;
            int procCount = Environment.ProcessorCount;
            uint[] procInfo = new uint[procCount];
            uint retval = CallNtPowerInformation(
                InformationLevel.SystemReserveHiberFile,
                false,
                0,
                procInfo,
                procInfo.Length * Marshal.SizeOf(typeof(uint))
            );

            if (retval == 0)
            {
                isSuccess = true;
            }
            return isSuccess;
        }

        public void Sleep()
        {
            SetSuspendState(true, false, false);
        }

        public string GetLastSleepTimeAsString()
        {
           int result = GetLastSleepTime();
           return $"Last sleep time {result} seconds";
        }

        public string GetLastWakeTimeAsString()
        {
            int result = GetLastWakeTime();
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
