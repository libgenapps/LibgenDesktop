using System;
using System.Net;
using static LibgenDesktop.Models.Settings.AppSettings;

namespace LibgenDesktop.Models.Utils
{
    internal static class NetworkUtils
    {
        public static WebProxy CreateProxy(NetworkSettings networkSettings)
        {
            WebProxy result;
            if (networkSettings.UseProxy)
            {
                result = new WebProxy(networkSettings.ProxyAddress, networkSettings.ProxyPort.Value);
                if (!String.IsNullOrEmpty(networkSettings.ProxyUserName))
                {
                    result.Credentials = new NetworkCredential(networkSettings.ProxyUserName, networkSettings.ProxyPassword);
                }
            }
            else
            {
                result = new WebProxy();
            }
            return result;
        }
    }
}
