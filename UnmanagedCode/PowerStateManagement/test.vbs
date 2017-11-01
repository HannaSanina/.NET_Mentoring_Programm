set calc = CreateObject("PowerStateManagement.PowerManagement")

res = calc.GetBatteryStateAsString()

WScript.Echo(res)

res = calc.GetLastSleepTimeAsString()

WScript.Echo(res)

res = calc.GetLastWakeTimeAsString()

WScript.Echo(res)

res = calc.GetPowerInformationAsString()

WScript.Echo(res)

res = calc.ReserveHibernationFileAsString()

WScript.Echo(res)

res = calc.DeleteHibernationFileAsString()

WScript.Echo(res)