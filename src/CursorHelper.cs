using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Utils
{

    public static class NativeCursorHelper
    {
        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr CreateIconIndirect(ref ICONINFO iconInfo);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool GetIconInfo(IntPtr hIcon, out ICONINFO pIconInfo);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr CopyIcon(IntPtr hIcon);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool DestroyIcon(IntPtr hIcon);

        [StructLayout(LayoutKind.Sequential)]
        private struct ICONINFO
        {
            public bool fIcon; // true = icon, false = cursor
            public int xHotspot;
            public int yHotspot;
            public IntPtr hbmMask;
            public IntPtr hbmColor;
        }

        public static Cursor CreateCursorFromBitmap(Bitmap bmp, int xHotSpot, int yHotSpot)
        {
            IntPtr hIcon = bmp.GetHicon();
            try
            {
                if (!GetIconInfo(hIcon, out ICONINFO iconInfo))
                    throw new System.ComponentModel.Win32Exception(Marshal.GetLastWin32Error());

                // mark as cursor (not icon)
                iconInfo.fIcon = false;
                iconInfo.xHotspot = xHotSpot;
                iconInfo.yHotspot = yHotSpot;

                IntPtr hCursor = CreateIconIndirect(ref iconInfo);
                if (hCursor == IntPtr.Zero)
                    throw new System.ComponentModel.Win32Exception(Marshal.GetLastWin32Error());

                return new Cursor(hCursor);
            }
            finally
            {
                // Free the original icon handle
                DestroyIcon(hIcon);
            }
        }
    }
}