using System.Runtime.InteropServices;

namespace PowerStateManagement
{
    [ComVisible(true)]
    [Guid("69E39A4B-7106-41A6-B5CF-3A6FA0B4E6D5")]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface IPowerManagement
    {
        ulong GetLastSleepTime();
        ulong GetLastWakeTime();
        SYSTEM_BATTERY_STATE GetBatteryState();
        SYSTEM_POWER_INFORMATION GetPowerInformation();
        bool ReserveHibernationFile();
        bool DeleteHibernationFile();
        void Sleep();
    }
}