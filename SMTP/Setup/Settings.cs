using System;
using System.Collections.Generic;
using System.IO;

namespace SMTP.Setup
{
    /// <summary>
    ///     Обрабатывает запрос настроек
    /// </summary>
    public static class Settings
    {
        /// <summary>
        ///     Smtp-сервер, с которого производится отправление почты. Например, smtp.yandex.ru
        /// </summary>
        public static string Host { get; set; }

        /// <summary>
        ///     Порт, используемый smp-сервером.
        /// </summary>
        public static string Port { get; set; }

        /// <summary>
        ///     Открывает передачу на другие сервера.
        /// </summary>
        public static bool Relay { get; set; }

        /// <summary>
        ///     Домен сервера.
        /// </summary>
        public static string Domain { get; set; }

        /// <summary>
        ///     Собирает информацию с файла "Настройки"
        /// </summary>
        static Settings()
        {
            List<string[]> text2 = new List<string[]>();
            string[] text = File.ReadAllLines("Files/settings.txt");
            foreach (var item in text)
            {
                text2.Add(item.Split(':'));
            }
            Host = text2[0][1];
            Port = text2[1][1];
            Relay = Convert.ToBoolean(text2[2][1]);
            Domain = text2[3][1];
        }
    }
}
