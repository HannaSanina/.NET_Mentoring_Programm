using System.Runtime.InteropServices;

namespace PowerStateManagement
{
    [ComVisible(true)]
    [Guid("69E39A4B-7106-41A6-B5CF-3A6FA0B4E567")]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface IFormatFunctions
    {
        string GetLastSleepTimeAsString();
        string GetLastWakeTimeAsString();
        string GetBatteryStateAsString();
        string GetPowerInformationAsString();
        string ReserveHibernationFileAsString();
        string DeleteHibernationFileAsString();

    }
}