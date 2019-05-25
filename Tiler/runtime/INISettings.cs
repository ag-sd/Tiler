using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace Tiler.runtime
{
//    public class SavedPlacement
//    {
//        public Placement Placement { get; }
//        public string Desktop { get; }
//
//        public SavedPlacement(Placement placement, string desktop)
//        {
//            Placement = placement;
//            Desktop = desktop;
//        }
//    }
    
    public static class INISettings
    {
        private static readonly Lazy<string> ini_file = new Lazy<string>(() => Application.LocalUserAppDataPath + ".ini");
        
        private const int Capacity = 512;

        public static (Placement, string) GetPlacement(string appName)
        {
            var placement = Placement.ByKey(ReadValue(appName, "placement", "None"));
            var desktop = ReadValue(appName, "desktop", Screen.PrimaryScreen.DeviceName);
            return (placement, desktop);
        }

        public static void SavePlacement(string appName, Placement placement, string desktop)
        {
            WriteValue(appName, "placement", placement.Name);
            WriteValue(appName, "desktop", desktop);
        }
        
        private static string ReadValue(string section, string key, string defaultValue = "") 
        { 
            var value = new StringBuilder(Capacity); 
            GetPrivateProfileString(section, key, defaultValue, value, value.Capacity, ini_file.Value); 
            return value.ToString(); 
        } 
        
        private static void WriteValue(string section, string key, string value) 
        { 
            WritePrivateProfileString(section, key, value, ini_file.Value);
        }
        
        
        
        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        private static extern int GetPrivateProfileString(string section, string key, string defaultValue, StringBuilder value, int size, string filePath);

        [DllImport("kernel32", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool WritePrivateProfileString(string section, string key, string value, string filePath);
    }
}