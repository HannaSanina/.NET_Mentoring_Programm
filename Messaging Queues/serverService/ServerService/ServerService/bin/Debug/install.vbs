Set fso = CreateObject("Scripting.FileSystemObject")
path=fso.GetAbsolutePathName("ConsoleApplication1.exe")
set wmi = GetObject("winmgmts:\\.\root\CIMV2") 
set service = wmi.Get("Win32_Service")
result = service.Create("ConsoleApplication1", "ConsoleApplication1", path)