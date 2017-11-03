set calc = CreateObject("PowerStateManagement.PowerManagement")

res = calc.GetLastSleepTime()

WScript.Echo(res)

res = calc.GetLastWakeTime()

WScript.Echo(res)

res = calc.ReserveHibernationFile()

WScript.Echo(res)

res = calc.DeleteHibernationFile()

WScript.Echo(res)