using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;


namespace DP.Common
{
    public static class ConfigurationHelper
    {
        /// <summary>
        /// Apps the settings.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public static string appSettings(string key)
        {
            string value = string.Empty;
            if (ConfigurationManager.AppSettings[key] != null)
            {
                value = ConfigurationManager.AppSettings[key].ToString();
            }           
            return value;
        }
               

        /// <summary>
        /// Connections the strings.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public static string connectionStrings(string key)
        {
            string value = string.Empty;
            if (ConfigurationManager.ConnectionStrings[key] != null)
            {
                value = ConfigurationManager.ConnectionStrings[key].ConnectionString;
            }
            return value;
        }

    }
}
