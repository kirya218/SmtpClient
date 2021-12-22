using System.Collections.Generic;

namespace SMTP.Models
{
    /// <summary>
    ///     Модель сообщения
    /// </summary>
    public static class ModelMessage
    {
        /// <summary>
        ///     Откуда будет послано сообщение
        /// </summary>
        public static string From { get; set; }

        /// <summary>
        ///     Куда будет отправлено сообщение
        /// </summary>
        public static List<string> To = new List<string>();

        /// <summary>
        ///     Содержимое сообщения
        /// </summary>
        public static string Body { get; set; }

        /// <summary>
        ///     Тема сообщения
        /// </summary>
        public static string Subject { get; set; }

        /// <summary>
        ///     Полное сообщение.
        /// </summary>
        public static string FullMessage { get; set; }
    }
}
