using System.Diagnostics;
using System.Management;
using System.Runtime.InteropServices;

public class SystemUtil
{
    #region 内存
    #region 获得内存信息API
    [DllImport("kernel32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool GlobalMemoryStatusEx(ref MEMORY_INFO mi);
    //定义内存的信息结构
    [StructLayout(LayoutKind.Sequential)]
    private struct MEMORY_INFO
    {
        public uint DWLength; //当前结构体大小
        public uint DWMemoryLoad; //当前内存使用率
        public ulong ullTotalPhys; //总计物理内存大小
        public ulong ullAvailPhys; //可用物理内存大小
        public ulong ullTotalPagefile; //总计交换文件大小
        public ulong ullAvailPagefile; //总计交换文件大小
        public ulong ullTotalVirtual; //总计虚拟内存大小
        public ulong ullAvailVirtual; //可用虚拟内存大小
        public ulong ullAvailExtendedVirtual; //保留 这个值始终为0
    }
    private static MEMORY_INFO GetMemoryInfo()
    {
        MEMORY_INFO MemoryInfo = new MEMORY_INFO();
        MemoryInfo.DWLength = (uint)System.Runtime.InteropServices.Marshal.SizeOf(MemoryInfo);
        GlobalMemoryStatusEx(ref MemoryInfo);
        return MemoryInfo;
    }
    #endregion
    /// <summary>
    /// 获取系统内存
    /// </summary>
    /// <returns></returns>
    public static string GetSysMemory()
    {
        MEMORY_INFO MemoryInfo = GetMemoryInfo();
        return ConvertBytes((long)MemoryInfo.ullTotalPhys);
    }
    /// <summary>
    /// 获取剩余内存
    /// </summary>
    /// <returns></returns>
    public static string GetUnUsedMemory()
    {
        MEMORY_INFO MemoryInfo = GetMemoryInfo();
        return ConvertBytes((long)MemoryInfo.ullAvailPhys);
    }
    /// <summary>
    /// 获取已使用内存
    /// </summary>
    /// <returns></returns>
    public static string GetUsedMemory()
    {
        MEMORY_INFO MemoryInfo = GetMemoryInfo();
        return ConvertBytes((long)(MemoryInfo.ullTotalPhys - MemoryInfo.ullAvailPhys));
    }
    /// <summary>
    /// 获取应用使用内存
    /// </summary>
    /// <param name="appName"></param>
    /// <returns></returns>
    public static string GetAppMemory(string appName)
    {
        Process CurrentProcess = Process.GetProcessesByName(appName)[0];
        PerformanceCounter Perform = new PerformanceCounter("Process", "Working Set - Private", CurrentProcess.ProcessName);
        return ConvertBytes((long)Perform.NextValue());
    }
    /// <summary>
    /// 获取当前应用使用内存
    /// </summary>
    /// <returns></returns>
    public static string GetCurrentAppMemory()
    {
        long memory = Process.GetCurrentProcess().PrivateMemorySize64;
        return ConvertBytes(memory);
    }
    #endregion
    #region CPU
    /// <summary>
    /// 获取CPU使用率
    /// </summary>
    /// <returns></returns>
    public static string GetUsedCPU()
    {
        ManagementClass mc = new ManagementClass("Win32_PerfFormattedData_PerfOS_Processor");
        ManagementObjectCollection moc = mc.GetInstances();
        List<string> list = new List<string>();
        foreach (ManagementObject mo in moc)
        {
            if (mo["Name"].ToString() == "_Total")
            {
                list.Add(mo["PercentProcessorTime"].ToString());
            }
        }
        return list.Sum(s => int.Parse(s)) + "%";
    }
    #endregion
    #region 硬盘
    public static string GetUsedDisk()
    {
        ManagementClass mc = new ManagementClass("Win32_PerfFormattedData_PerfDisk_PhysicalDisk");
        ManagementObjectCollection moc = mc.GetInstances();
        List<string> list = new List<string>();
        foreach (ManagementObject mo in moc)
        {
            if (mo["Name"].ToString() == "_Total")
            {
                list.Add(mo["PercentDiskTime"].ToString());
            }
        }
        return list.Sum(s => int.Parse(s)) + "%";
    }
    #endregion
    #region 电脑信息
    /// <summary>
    /// 获取CPU序列号
    /// </summary>
    /// <returns></returns>
    public static string GetCpuID()
    {
        ManagementClass mc = new ManagementClass("Win32_Processor");
        ManagementObjectCollection moc = mc.GetInstances();
        List<string> list = new List<string>();
        foreach (ManagementObject mo in moc)
        {
            list.Add(mo.Properties["ProcessorId"].Value.ToString());
        }
        return string.Join("|", list);
        //若需要获取所有属性，可迭代ManagementObject.Properties
        //foreach (PropertyData pd in mo.Properties)
        //{
        //    Console.WriteLine(pd.Name + "----" + pd.Value);
        //}
    }
    /// <summary>
    /// 获取Mac地址
    /// </summary>
    /// <returns></returns>
    public static string GetMac()
    {
        ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
        ManagementObjectCollection moc = mc.GetInstances();
        List<string> list = new List<string>();
        foreach (ManagementObject mo in moc)
        {
            if ((bool)mo["IPEnabled"] == true)
            {
                list.Add(mo["MacAddress"].ToString());
            }
        }
        return string.Join("|", list);
    }
    /// <summary>
    /// 获取IP地址
    /// </summary>
    /// <returns></returns>
    public static string GetIp()
    {
        ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
        ManagementObjectCollection moc = mc.GetInstances();
        List<string> list = new List<string>();
        foreach (ManagementObject mo in moc)
        {
            if ((bool)mo["IPEnabled"] == true)
            {
                list.Add(((Array)mo.Properties["IpAddress"].Value).GetValue(0).ToString());
            }
        }
        return string.Join("|", list);
    }
    /// <summary>
    /// 获取硬盘序列号
    /// </summary>
    /// <returns></returns>
    public static string GetDiskID()
    {
        ManagementClass mc = new ManagementClass("Win32_DiskDrive");
        ManagementObjectCollection moc = mc.GetInstances();
        List<string> list = new List<string>();
        foreach (ManagementObject mo in moc)
        {
            list.Add(mo.Properties["Model"].Value.ToString());
        }
        return string.Join("|", list);
    }
    /// <summary>
    /// 获取系统名称
    /// </summary>
    /// <returns></returns>
    public static string GetSystemName()
    {
        ManagementClass mc = new ManagementClass("Win32_ComputerSystem");
        ManagementObjectCollection moc = mc.GetInstances();
        List<string> list = new List<string>();
        foreach (ManagementObject mo in moc)
        {
            list.Add(mo["Name"].ToString());
        }
        return string.Join("|", list);
        // return System.Environment.GetEnvironmentVariable("ComputerName")
    }
    /// <summary>
    /// 获取当前登录用户
    /// </summary>
    /// <returns></returns>
    public static string GetLoginUser()
    {
        ManagementClass mc = new ManagementClass("Win32_ComputerSystem");
        ManagementObjectCollection moc = mc.GetInstances();
        List<string> list = new List<string>();
        foreach (ManagementObject mo in moc)
        {
            list.Add(mo["UserName"].ToString());
        }
        return string.Join("|", list);
    }
    /// <summary>
    /// 获取电脑类型
    /// </summary>
    /// <returns></returns>
    public static string GetPcType()
    {
        ManagementClass mc = new ManagementClass("Win32_ComputerSystem");
        ManagementObjectCollection moc = mc.GetInstances();
        List<string> list = new List<string>();
        foreach (ManagementObject mo in moc)
        {
            list.Add(mo["SystemType"].ToString());
        }
        return string.Join("|", list);
    }
    /// <summary>
    /// 获取系统内存
    /// </summary>
    /// <returns></returns>
    public static string GetSysMemory2()
    {
        ManagementClass mc = new ManagementClass("Win32_ComputerSystem");
        ManagementObjectCollection moc = mc.GetInstances();
        List<string> list = new List<string>();
        foreach (ManagementObject mo in moc)
        {
            list.Add(mo["TotalPhysicalMemory"].ToString());
        }
        return string.Join("|", list);
    }
    /// <summary>
    /// 获取剩余内存
    /// </summary>
    /// <returns></returns>
    public static string GetUnUsedMemory2()
    {
        ManagementClass mc = new ManagementClass("Win32_PerfFormattedData_PerfOS_Memory");
        ManagementObjectCollection moc = mc.GetInstances();
        long memory = 0;
        foreach (ManagementObject mo in moc)
        {
            memory += long.Parse(mo.Properties["AvailableMBytes"].Value.ToString());
        }
        return ConvertBytes(memory);
    }
    /// <summary>
    /// 获取电脑品牌
    /// </summary>
    /// <returns></returns>
    public static string GetPcBrand()
    {
        ManagementClass mc = new ManagementClass("Win32_ComputerSystem");
        ManagementObjectCollection moc = mc.GetInstances();
        List<string> list = new List<string>();
        foreach (ManagementObject mo in moc)
        {
            list.Add(mo["Manufacturer"].ToString());
        }
        return string.Join("|", list);
    }
    /// <summary>
    /// 获取电脑型号
    /// </summary>
    /// <returns></returns>
    public static string GetPcModel()
    {
        ManagementClass mc = new ManagementClass("Win32_ComputerSystem");
        ManagementObjectCollection moc = mc.GetInstances();
        List<string> list = new List<string>();
        foreach (ManagementObject mo in moc)
        {
            list.Add(mo["SystemFamily"].ToString());
        }
        return string.Join("|", list);
    }
    #endregion
    public static string ConvertBytes(long len)
    {
        double dlen = len;
        string[] sizes = { "B", "KB", "MB", "GB", "TB" };
        int order = 0;
        while (dlen >= 1024 && order + 1 < sizes.Length)
        {
            order++;
            dlen = dlen / 1024;
        }
        return String.Format("{0:0.##} {1}", dlen, sizes[order]);
    }
}