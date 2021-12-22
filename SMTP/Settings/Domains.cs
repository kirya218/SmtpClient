using System.Collections.Generic;
using System.IO;

namespace SMTP.Settings
{
    public static class Domain
    {
        /// <summary>
        ///     Все домены поддерживающий мой сервер.
        /// </summary>
        public static Dictionary<string, string[]> SettingsAllDomains = new Dictionary<string, string[]>();

        /// <summary>
        ///     Собирает всю информацию об актуальных настройках домена.
        /// </summary>
        static Domain()
        {
            string[] text = File.ReadAllLines("Files/domains.txt");
            string domain = string.Empty, login = string.Empty, port = string.Empty;
            foreach (var item in text)
            {
                if (item.Contains("domain"))
                {
                    string[] k = item.Split(':');
                    domain = k[1];
                }
                else if (item.Contains("port"))
                {
                    string[] k = item.Split(':');
                    port = k[1];
                }
                else if (item.Contains("login"))
                {
                    string[] k = item.Split(':');
                    login = k[1];
                }
                else if (item.Contains("password"))
                {
                    string[] k = item.Split(':');
                    string password = k[1];
                    string[] value = { port, login, password };
                    SettingsAllDomains.Add(domain, value);
                }
            }
        }
    }
}
