if (OperatingSystem.IsWindows())
{
    WMIWin32Class wmiWin32Class = new(WMIWin32Class.WMIWin32ClassNames.Win32_PerfRawData_Counters_ProcessorInformation);
    Console.WriteLine($"{wmiWin32Class.WMIWin32ClassName}\n");
    foreach (WMIWin32Class.WMIWin32ClassProperty wmiWin32ClassProperty in wmiWin32Class.WMIWin32ClassProperties)
    {
        Console.WriteLine(
            $"\t{wmiWin32ClassProperty.PropertyType} :: {wmiWin32ClassProperty.PropertyName} :: {wmiWin32ClassProperty.PropertyValue}\n");
    }
}
