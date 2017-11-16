set wmi = GetObject("winmgmts:\\.\root\CIMV2") 
Set services = wmi.ExecQuery("SELECT * FROM Win32_Service WHERE Name = 'ConsoleApplication1'")
For Each service in services
   service.StopService()
Next