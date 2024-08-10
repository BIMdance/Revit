using System.Runtime.InteropServices;

namespace BIMdance.Revit.Utils.Revit;

public static class RevitKeyPress
{
    private static IntPtr RevitWindowPtr => ComponentManager.ApplicationWindow;

    [DllImport("user32.dll")]
    private static extern bool SetForegroundWindow(IntPtr hWnd);
    
    [DllImport("user32.dll")]
    private static extern void keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);

    public static void Post(Keys key, int count = 1)
    {
        SetForegroundWindow(RevitWindowPtr);
        count = count < 1 ? 1 : count;
        for (var i = 0; i < count; i++)
        {
            keybd_event((byte)key, 0, 0, 0);
            keybd_event((byte)key, 0, 2, 0);
        }
    }

    public enum Keys
    {
        Esc = 0x1B,
        F9  = 0x78,
    }
}