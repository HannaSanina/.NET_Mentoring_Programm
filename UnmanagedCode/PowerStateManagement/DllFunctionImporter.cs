using System;
using System.Runtime.InteropServices;

namespace PowerStateManagement
{
    public static class DllFunctionImporter
    {
        [DllImport("Powrprof.dll", SetLastError = true)]
        public static extern bool SetSuspendState(bool hibernate, bool forceCritical, bool disableWakeEvent);

        [DllImport("powrprof.dll", SetLastError = true, EntryPoint = "CallNtPowerInformation")]
        public static extern uint GetTime(int InformationLevel, IntPtr lpInputBuffer, int nInputBufferSize, out ulong lpOutputBuffer, int nOutputBufferSize);

        [DllImport("powrprof.dll", SetLastError = true, EntryPoint = "CallNtPowerInformation")]
        public static extern uint ChangeHibernateFile(int InformationLevel, bool lpInputBuffer, int nInputBufferSize, out bool lpOutputBuffer, int nOutputBufferSize);

        [DllImport("powrprof.dll", SetLastError = true, EntryPoint = "CallNtPowerInformation")]
        public static extern uint GetBatteryState(int InformationLevel, IntPtr lpInputBuffer, int nInputBufferSize, out SYSTEM_BATTERY_STATE lpOutputBuffer, int nOutputBufferSize);

        [DllImport("powrprof.dll", SetLastError = true, EntryPoint = "CallNtPowerInformation")]
        public static extern uint GetSystemPowerInfo(int InformationLevel, IntPtr lpInputBuffer, int nInputBufferSize, out SYSTEM_POWER_INFORMATION lpOutputBuffer, int nOutputBufferSize);

    }
}