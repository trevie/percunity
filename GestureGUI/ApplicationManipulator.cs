using InputManager;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;

namespace GestureGUI.cs
{
    public class ApplicationManipulator
    {
        Win32.WinEventDelegate dele = null;

        public ApplicationManipulator()
        {
            dele = new Win32.WinEventDelegate(WinEventProc);
            IntPtr m_hhook = Win32.SetWinEventHook(Win32.EVENT_SYSTEM_FOREGROUND, Win32.EVENT_SYSTEM_FOREGROUND, IntPtr.Zero, dele, 0, 0, Win32.WINEVENT_OUTOFCONTEXT);
        }

        public IntPtr WindowHandle { get; set; }

        public Rectangle WindowRect { get; set; }

        public bool AllowGesters 
        { 
            get { return WindowHandle != IntPtr.Zero; }
        }

        public void WinEventProc(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
        {
            IntPtr handle;
            Rectangle rect;
                
            GetActiveWindowProcess(out handle, out rect);

            if (handle != IntPtr.Zero)
            {
                WriteLog("Active Window Found");
            }
            else if (WindowHandle != IntPtr.Zero && handle == IntPtr.Zero)
            {
                WriteLog("Active Window Lost");
            }

            WindowHandle = handle;
            WindowRect = rect;

            if (WindowHandle == IntPtr.Zero)
            {
                return;
            }
        }

        public void MoveXY(int xmargin, int ymargin)
        {
            if (AllowGesters == false)
            {
                return;
            }

            var centerX = WindowRect.Left + WindowRect.Width / 2;
            var centerY = WindowRect.Top + WindowRect.Height / 2;

             Mouse.Move(centerX, centerY);
             Mouse.SendButton(Mouse.MouseButtons.MiddleDown);
             Mouse.Move(centerX + xmargin, centerY + ymargin);
             Mouse.SendButton(Mouse.MouseButtons.MiddleUp);
        }

        public void RotateXY(int xmargin, int ymargin)
        {
            if (AllowGesters == false)
            {
                return;
            }

            var centerX = WindowRect.Left + WindowRect.Width / 2;
            var centerY = WindowRect.Top + WindowRect.Height / 2;

            Mouse.Move(centerX, centerY);
            Mouse.SendButton(Mouse.MouseButtons.RightDown);
            Mouse.Move(centerX + xmargin, centerY + ymargin);
            Mouse.SendButton(Mouse.MouseButtons.RightDown);
        }

        public void ZoomIn()
        {
            if (AllowGesters == false)
            {
                return;
            }
            var centerX = WindowRect.Left + WindowRect.Width / 2;
            var centerY = WindowRect.Top + WindowRect.Height / 2;

            Mouse.Move(centerX, centerY);
            Mouse.Scroll(Mouse.ScrollDirection.Up);
        }

        public void ZoomOut()
        {
            if (AllowGesters == false)
            {
                return;
            }
            var centerX = WindowRect.Left + WindowRect.Width / 2;
            var centerY = WindowRect.Top + WindowRect.Height / 2;

            Mouse.Move(centerX, centerY);
            Mouse.Scroll(Mouse.ScrollDirection.Down);
        }

        public void WriteLog(string value)
        {
            Console.WriteLine(value);
        }

        private bool GetActiveWindowProcess(out IntPtr mainWindowHandle, out Rectangle rectangle)
        {
            uint processID;

            const int nChars = 256;
            IntPtr handle = IntPtr.Zero;
            StringBuilder Buff = new StringBuilder(nChars);
            handle = Win32.GetForegroundWindow();

            Win32.GetWindowThreadProcessId((IntPtr)handle, out processID);

            var process = Process.GetProcessById((int)processID);

            if (process != null && string.Equals(process.MainModule.FileName, System.Configuration.ConfigurationManager.AppSettings["AppPath"], StringComparison.CurrentCultureIgnoreCase))
            {
                RECT rect;
                Win32.GetWindowRect(handle, out rect);

                rectangle = new Rectangle(rect.Left, rect.Top, rect.Right - rect.Left, rect.Bottom - rect.Top);

                mainWindowHandle = handle;
                return true;
            }

            mainWindowHandle = IntPtr.Zero;
            rectangle = Rectangle.Empty;
            return false;
        }

    }


}
