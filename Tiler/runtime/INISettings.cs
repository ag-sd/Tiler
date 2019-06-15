using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using log4net;

namespace Tiler.runtime
{
   
    public static class INISettings
    {
        private static readonly ILog log = 
            LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static readonly Lazy<string> iniFile = new Lazy<string>(() => 
            Path.GetDirectoryName(Application.ExecutablePath) +
            Path.DirectorySeparatorChar + Application.ProductName + ".ini");
        
        private const int Capacity = 512;

        public static (Placement, string) GetAppPlacement(string appName)
        {
            var placement = Placement.ByKey(ReadValue(appName, "region", "None"));
            var desktop = ReadValue(appName, "desktop", Screen.PrimaryScreen.DeviceName);
            if (Placement.None != placement)
            {
                log.Info($"Found saved placement {placement} for {appName} on desktop {desktop}");
            }
            return (placement, desktop);
        }

        public static void SavePlacement(Placement placement)
        {
            WriteValue("Placements", placement.Name, placement.AsINISetting());
        }

        public static void DeletePlacement(Placement placement)
        {
            WriteValue("Placements", placement.Name, null);
        }

        public static void SaveAppPlacement(string appName, Placement placement, string desktop)
        {
            WriteValue(appName, "region", placement.Name);
            WriteValue(appName, "desktop", desktop);
        }

        public static Dictionary<string, Placement> GetAllPlacements()
        {
            var dict = new Dictionary<string, Placement>();
            var allPlacements = ReadKeyValuePairs("Placements", iniFile.Value);
            foreach (var placementStr in allPlacements)
            {
                var kvp = placementStr.Split('=');
                var pParams = kvp[1].Split(',');
                var placement = new Placement(kvp[0], 
                    float.Parse(pParams[0]), float.Parse(pParams[1]), 
                    float.Parse(pParams[2]), float.Parse(pParams[3]), true);
                log.Info($"Found custom placement {placement}");
                dict.Add(placement.Name, placement);
            }
            return dict;
        }
        
        private static string ReadValue(string section, string key, string defaultValue = "") 
        { 
            var value = new StringBuilder(Capacity); 
            GetPrivateProfileString(section, key, defaultValue, value, value.Capacity, iniFile.Value); 
            return value.ToString(); 
        } 
        
        private static void WriteValue(string section, string key, string value) 
        { 
            WritePrivateProfileString(section, key, value, iniFile.Value);
        }
        
        private static IEnumerable<string> ReadKeyValuePairs(string section, string filePath)
        {
            var capacity = Capacity;
            while (true) 
            { 
                var returnedString = Marshal.AllocCoTaskMem(capacity * sizeof(char)); 
                var size = GetPrivateProfileSection(section, returnedString, capacity, filePath); 
  
                if (size == 0) 
                { 
                    Marshal.FreeCoTaskMem(returnedString); 
                    return new string[]{}; 
                } 
  
                if (size < capacity - 2) 
                { 
                    var result = Marshal.PtrToStringAuto(returnedString, size - 1); 
                    Marshal.FreeCoTaskMem(returnedString); 
                    var keyValuePairs = result.Split('\0'); 
                    return keyValuePairs; 
                } 
  
                Marshal.FreeCoTaskMem(returnedString); 
                capacity *= 2; 
            } 
        }
        
        
        
        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        private static extern int GetPrivateProfileString(string section, string key, string defaultValue, StringBuilder value, int size, string filePath);

        [DllImport("kernel32", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool WritePrivateProfileString(string section, string key, string value, string filePath);
        
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)] 
        private static extern int GetPrivateProfileSection(string section, IntPtr keyValue, int size, string filePath); 
    }
}