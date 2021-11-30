using System.Management;

public partial class WMIWin32Class
{
    public class WMIWin32ClassProperty
    {
        public string PropertyType { get; set; }
        public string PropertyName { get; set; }
        public string? PropertyValue { get; set; }
        public PropertyData PropertyData { get; set; }

        public WMIWin32ClassProperty(string propertyType, string propertyName, string propertyValue, PropertyData propertyData)
        {
            PropertyType = propertyType ?? throw new ArgumentNullException(nameof(propertyType));
            PropertyName = propertyName ?? throw new ArgumentNullException(nameof(propertyName));
            PropertyValue = propertyValue ?? throw new ArgumentNullException(nameof(propertyValue));
            PropertyData = propertyData ?? throw new ArgumentNullException(nameof(propertyData));
        }
    }

    public WMIWin32ClassNames WMIWin32ClassName { get; set; }
    public List<WMIWin32ClassProperty> WMIWin32ClassProperties { get; set; } = new();

    public WMIWin32Class(WMIWin32ClassNames wmiWin32ClassName)
    {
        WMIWin32ClassName = wmiWin32ClassName;
        if (OperatingSystem.IsWindows())
        {
            try
            {
                using (ManagementClass wmiManagementClass = new(wmiWin32ClassName.ToString()))
                {
                    foreach (ManagementObject managementObj in wmiManagementClass.GetInstances())
                    {
                        foreach (PropertyData prop in managementObj.Properties)
                        {
                            WMIWin32ClassProperties.Add(new(prop.Type.ToString(), prop.Name.ToString(), GetPropValueString(prop), prop));
                        }
                    }
                }
            }
            catch
            {
            }
        }
    }

    private static string GetPropValueString(PropertyData prop)
    {
        if (OperatingSystem.IsWindows())
        {
            if (prop.Value is null) return string.Empty;
            Type type = prop.Value.GetType();
            if (type.IsArray)
            {
                if (type == typeof(string[]))
                {
                    return MakeStringFromObjs(prop.Value as object[]);
                }
                if (type == typeof(UInt16[]))
                {
                    ushort[]? usValues = prop.Value as ushort[];
                    if (usValues is not null && usValues.Length > 0)
                    {
                        object[] values = new object[usValues.Length];
                        for (int i = 0; i < values.Length; i++)
                        {
                            values[i] = Convert.ToString(usValues[i]);
                        }
                        return MakeStringFromObjs(values);
                    }
                }
            }
            else if (prop.Type.ToString() == "Object")
            {
                if (type.IsInstanceOfType(prop.Value)) return $"IsInstanceOfType {{{prop.Value.ToString() ?? "null"}}}";
                return $"ReferenceType {{{prop.Value.ToString() ?? "null"}}}";
            }
            else
            {
                return prop.Value.ToString() ?? string.Empty;
            }
        }
        return string.Empty;
    }

    private static string MakeStringFromObjs(object[]? objs)
    {
        if (objs != null)
        {
            string ret = "[ ";
            foreach (object obj in objs)
            {
                ret += obj.ToString() + ", ";
            }
            return ret.Substring(0, ret.Length - 2) + " ]";
        }
        return string.Empty;
    }
}
