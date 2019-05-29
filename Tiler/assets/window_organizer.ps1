####################### POWERSHELL PINVOKE API #######################

Function Set-Windows-For-Process{
    <#
        .SYNOPSIS
            Sets the window size (height,width) and coordinates (x,y) of
            a process window.

        .DESCRIPTION
            Sets the window size (height,width) and coordinates (x,y) of
            a process window.

        .PARAMETER ProcessName
            Name of the process to determine the window characteristics

        .PARAMETER X
            Set the position of the window in pixels from the top.

        .PARAMETER Y
            Set the position of the window in pixels from the left.

        .PARAMETER Width
            Set the width of the window.

        .PARAMETER Height
            Set the height of the window.

        .PARAMETER Passthru
            Display the output object of the window.

        .NOTES
            Name: Set-Window
            Author: Boe Prox
            Version History
                1.0//Boe Prox - 11/24/2015
                    - Initial build
                1.1//JosefZ (https://superuser.com/users/376602/josefz) - 19.05.2018
                    - treats more process instances of supplied process name properly

        .OUTPUT
            System.Automation.WindowInfo

        .EXAMPLE
            Get-Process powershell | Set-Window -X 2040 -Y 142 -Passthru

            ProcessName Size     TopLeft  BottomRight
            ----------- ----     -------  -----------
            powershell  1262,642 2040,142 3302,784   

            Description
            -----------
            Set the coordinates on the window for the process PowerShell.exe

    #>
    [OutputType('System.Automation.WindowInfo')]
    [cmdletbinding()]
    Param (
        [parameter(ValueFromPipelineByPropertyName=$True)]
        $ProcessName,
        [int]$X,
        [int]$Y,
        [int]$Width,
        [int]$Height,
        [switch]$Passthru
    )
    Begin {
        Try{
            [void][Window]
        } Catch {
        Add-Type @"
              using System;
              using System.Runtime.InteropServices;
              using System.Collections.Generic;
              using System.Text;

              public class Window {
                [DllImport("user32.dll")]
                [return: MarshalAs(UnmanagedType.Bool)]
                public static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

                [DllImport("User32.dll")]
                public extern static bool MoveWindow(IntPtr handle, int x, int y, int width, int height, bool redraw);

                [DllImport("user32.dll", SetLastError = true)]
                [return: MarshalAs(UnmanagedType.Bool)]
                private extern static bool GetWindowPlacement(IntPtr hWnd, ref WINDOWPLACEMENT lpwndpl);

                [DllImport("user32.dll", SetLastError = true)]
                [return: MarshalAs(UnmanagedType.Bool)]
                public extern static bool SetWindowPlacement(IntPtr hWnd, ref WINDOWPLACEMENT lpwndpl);

                [DllImport("user32.dll", SetLastError=true)]
                private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint processId);

                private delegate bool EnumWindowDelegate(IntPtr hWnd, IntPtr lParam);
                [DllImport("user32.dll", EntryPoint = "EnumDesktopWindows", ExactSpelling = false, CharSet = CharSet.Auto, SetLastError = true)]
                private static extern bool EnumDesktopWindows(IntPtr hDesktop, EnumWindowDelegate lpEnumCallbackFunction, IntPtr lParam);

                [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
                private static extern int GetWindowText(IntPtr hwnd,StringBuilder lpString, int cch);

                [DllImport("user32.dll", SetLastError = true)]
                [return: MarshalAs(UnmanagedType.Bool)]
                private static extern bool IsWindowVisible(IntPtr hWnd);


                public static WINDOWPLACEMENT GetPlacement(IntPtr hwnd)
                {
                  WINDOWPLACEMENT placement = new WINDOWPLACEMENT();
                  placement.length = Marshal.SizeOf(placement);
                  GetWindowPlacement(hwnd, ref placement);
                  return placement;
                }

                public static Dictionary<uint, List<IntPtr>> GetWindowsByProcessId()
                {
                    Dictionary<uint, List<IntPtr>> result = new Dictionary<uint, List<IntPtr>>();
                    GCHandle setHandle = GCHandle.Alloc(result);

                    try {
                        EnumDesktopWindows(IntPtr.Zero, GroupDesktopWindows, GCHandle.ToIntPtr(setHandle));
                    } finally {
                        if (setHandle.IsAllocated) setHandle.Free();
                    }
                    return result;
                }

                private static bool GroupDesktopWindows(IntPtr hWnd, IntPtr lParam)
                {
                    GCHandle gch = GCHandle.FromIntPtr(lParam);
                    Dictionary<uint, List<IntPtr>> dict = gch.Target as Dictionary<uint, List<IntPtr>>;
                    if (dict == null)
                    {
                        throw new InvalidCastException("GCHandle Target could not be cast as Dictionary<uint, List<IntPtr>>");
                    }

                    // Get the window's title.
                    StringBuilder sb_title = new StringBuilder(1024);
                    int length = GetWindowText(hWnd, sb_title, sb_title.Capacity);
                    string title = sb_title.ToString();

                    // If the window is visible and has a title, save it.
                    if (IsWindowVisible(hWnd) && string.IsNullOrEmpty(title) == false)
                    {
                        uint  processId = 0;
                        GetWindowThreadProcessId((IntPtr)hWnd, out processId);
                        // Console.WriteLine("PROCESS ID Value " + processId + " Window Title is " +  title);

                        // ADD HERE
                        if(dict.ContainsKey(processId)) {
                            dict[processId].Add(hWnd);
                        } else {
                            List<IntPtr> list = new List<IntPtr>();
                            list.Add(hWnd);
                            dict.Add(processId, list);
                        }
                    }

                    // Return true to indicate that we
                    // should continue enumerating windows.
                    return true;
                }
                
              }
              public struct RECT
              {
                public int Left;        // x position of upper-left corner
                public int Top;         // y position of upper-left corner
                public int Right;       // x position of lower-right corner
                public int Bottom;      // y position of lower-right corner
              }

              public struct POINT
              {
                public int x;
                public int y;
              }

              public struct WINDOWPLACEMENT
              {
                public int length;
                public int flags;
                public int showCmd;
                public POINT ptMinPosition;
                public POINT ptMaxPosition;
                public RECT rcNormalPosition;
              }

              public enum ShowWindowCommands : int
              {
                Hide = 0,
                Normal = 1,
                Minimized = 2,
                Maximized = 3,
              }
"@
        }
    }
    Process {
        $all_processes_with_windows = [Window]::GetWindowsByProcessId();

        $processIds = (Get-Process -Name $ProcessName).Id
        foreach($id in $processIds) {
            if ( $id -eq [System.IntPtr]::Zero ) { Continue }
            if($all_processes_with_windows.ContainsKey($id)) {
                Write-Host -ForegroundColor Green "$ProcessName found. Adjusting all windows for this process..."
                forEach($handle in $all_processes_with_windows[$id]) {
                    $placement = [Window]::GetPlacement($handle)

                    $Rectangle = New-Object RECT
                    # Change as required
                    $Rectangle.Left = $X
                    $Rectangle.Top  = $Y
                    $Rectangle.Right = $X + $Width
                    $Rectangle.Bottom = $Y + $Height
                    $placement.rcNormalPosition = $Rectangle
                    # Unmaximize the window if it is maximized
                    $placement.showCmd = 1 
                    #Set it
                    $ret = [Window]::SetWindowPlacement($Handle, [ref]$placement)
                    Write-Host $ret
                }
            } 

        }
    }
}
####################### POWERSHELL PINVOKE API #######################


$applications = @{
    "powershell_ise" = "LEFT"
    "notepad++"      = "CENTER"
    "OUTLOOK"        = "RIGHT"
    "slack"          = "LEFT_THIRD"
    #"chrome"         = "CENTER_THIRD"
	"chrome"         = "CENTER"
}


function sectionize($width, $height) {

    $height = $height + 5

    $33_pct_wd = [Math]::Floor($width * 0.33)
    $50_pct_ht = [Math]::Floor($height* 0.5)
    $75_pct_ht = [Math]::Floor($height* 0.25)

    $s_0_x = 0

    $s_1_x = $33_pct_wd - 14

    $s_2_x = $33_pct_wd + $33_pct_wd - 28

    $s_3_w = $width - $33_pct_wd - $33_pct_wd - 30

    $s_4_h = $height - $75_pct_ht

    $s_5_h = $height - $75_pct_ht - $75_pct_ht
    
    $sections = @{
        "LEFT"          = @($s_0_x, 0,          $33_pct_wd, $height)
        "CENTER"        = @($s_1_x, 0,          $33_pct_wd, $height)
        "RIGHT"         = @($s_2_x, 0,          $s_3_w,     $height)
        "LEFT_THIRD"    = @($s_0_x, $75_pct_ht, $33_pct_wd, $s_4_h)
        "CENTER_THIRD"  = @($s_1_x, $75_pct_ht, $33_pct_wd, $s_5_h)
    }
    $sections
}

function process_applications($regions) {
    foreach($key in $applications.Keys) {
        #Check if app exists
        if(Get-Process $key -ErrorAction SilentlyContinue) {
            #Check if region exists
            $region = $regions[ $applications[$key] ]
            if($region) {
                Write-Host -ForegroundColor Yellow "Setting position of $key to : $region"
                Set-Windows-For-Process -ProcessName $key -X $region[0] -Y $region[1] -Width $region[2] -Height $region[3]
  
            } else {
                Write-Host -ForegroundColor Cyan "Could not find a valid region for $key. Region specified was"  $applications[$key]
            }
        } else {
            Write-Host -ForegroundColor Cyan "$key is not currently running. Skipping this application"
        }
    }
}




$resolution = Get-WmiObject -Class Win32_DesktopMonitor -Filter {DeviceID = 'DesktopMonitor1'} | Select-Object ScreenWidth,ScreenHeight
Write-Host -ForegroundColor Green "Screen Resolution is $resolution"

$sections = sectionize $resolution.ScreenWidth $resolution.ScreenHeight
Write-Host -ForegroundColor Green "Resolved sections are : " ($sections | Out-String)

Get-Process |where {$_.mainWindowTItle}  | format-table name, mainwindowtitle –AutoSize

process_applications $sections








