using System.Collections.Generic;
using System.IO;

namespace SMTP
{
    /// <summary>
    ///     Обрабатывает запрос настроек
    /// </summary>
    public class Settings
    {
        /// <summary>
        ///     Smtp-сервер, с которого производится отправление почты. Например, smtp.yandex.ru
        /// </summary>
        public string host { get; set; }

        /// <summary>
        ///     Порт, используемый smp-сервером.
        /// </summary>
        public string port { get; set; }

        /// <summary>
        ///     Собирает информацию с файла "Настройки"
        /// </summary>
        public Settings()
        {
            List<string[]> text2 = new List<string[]>();
            string[] text = File.ReadAllText(@"..\..\..\..\files\settings.txt").Split(';');
            foreach (var item in text)
            {
                text2.Add(item.Split(':'));
            }
            host = text2[0][1];
            port = text2[1][1];
        }
    }
}
