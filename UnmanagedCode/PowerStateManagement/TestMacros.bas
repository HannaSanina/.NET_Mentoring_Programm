Attribute VB_Name = "NewMacros"
Sub TestBattery()
Dim powerTool As PowerManagement

Set powerTool = New PowerStateManagement.PowerManagement

res = powerTool.GetBatteryStateAsString

MsgBox res

res = powerTool.GetLastSleepTimeAsString

MsgBox res

res = powerTool.GetLastWakeTimeAsString

MsgBox res

res = powerTool.GetPowerInformationAsString

MsgBox res

res = powerTool.ReserveHibernationFileAsString

MsgBox res

res = powerTool.DeleteHibernationFileAsString

MsgBox res

End Sub


