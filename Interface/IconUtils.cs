using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

namespace LibgenDesktop.Interface
{
    internal static class IconUtils
    {
        private static Icon cachedIcon;

        static IconUtils()
        {
            cachedIcon = null;
        }

        public static Icon GetAppIcon()
        {
            if (cachedIcon != null)
            {
                return cachedIcon;
            }
            string appFilePath = Assembly.GetExecutingAssembly().Location;
            Uri uri;
            try
            {
                uri = new Uri(appFilePath);
            }
            catch (UriFormatException)
            {
                uri = new Uri(Path.GetFullPath(appFilePath));
            }
            if (uri.IsFile)
            {
                if (!File.Exists(appFilePath))
                {
                    throw new FileNotFoundException(appFilePath);
                }
                StringBuilder iconPath = new StringBuilder(260);
                iconPath.Append(appFilePath);
                int index = 0;
                IntPtr handle = SafeNativeMethods.ExtractAssociatedIcon(new HandleRef(null, IntPtr.Zero), iconPath, ref index);
                if (handle != IntPtr.Zero)
                {
                    cachedIcon = Icon.FromHandle(handle);
                }
            }
            return cachedIcon;
        }
    }

    [SuppressUnmanagedCodeSecurity]
    internal static class SafeNativeMethods
    {
        [DllImport("shell32.dll", EntryPoint = "ExtractAssociatedIcon", CharSet = CharSet.Auto)]
        internal static extern IntPtr ExtractAssociatedIcon(HandleRef hInst, StringBuilder iconPath, ref int index);
    }
}
