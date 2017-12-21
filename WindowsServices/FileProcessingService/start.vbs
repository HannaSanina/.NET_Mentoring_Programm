set wmi = GetObject("winmgmts:\\.\root\CIMV2") 
Set services = wmi.ExecQuery("SELECT * FROM Win32_Service WHERE Name = 'ScanerProcessingService'")
For Each service in services
   service.StartService()
Next