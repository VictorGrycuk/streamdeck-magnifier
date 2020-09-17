using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace Magnifier.Helpers
{
    internal static class ScreenHelper
    {
        private enum ProcessDPIAwareness
        {
            ProcessDPIUnaware = 0,
            ProcessSystemDPIAware = 1,
            ProcessPerMonitorDPIAware = 2
        }

        [DllImport("user32.dll")]
        static extern bool GetCursorPos(ref Point lpPoint);

        [DllImport("User32.Dll")]
        private static extern long SetCursorPos(int x, int y);

        [DllImport("shcore.dll")]
        private static extern int SetProcessDpiAwareness(ProcessDPIAwareness value);

        internal static Point GetMouseLocation()
        {
            if (Environment.OSVersion.Version.Major >= 6)
            {
                SetProcessDpiAwareness(ProcessDPIAwareness.ProcessPerMonitorDPIAware);
            }

            var cursorLocation = new Point();
            GetCursorPos(ref cursorLocation);

            return cursorLocation;
        }

        internal static void SetMouseLocation(int xPos, int yPos)
        {
            SetCursorPos(xPos, yPos);
        }
    }
}
