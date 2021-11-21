using System.Collections.Generic;

namespace SMTP.Models
{
    /// <summary>
    ///     Модель сообщения
    /// </summary>
    public class ModelMessage
    {
        /// <summary>
        ///     Откуда будет послыно сообщение
        /// </summary>
        public string From { get; set; }

        /// <summary>
        ///     Куда будет отправлено сообщение
        /// </summary>
        public List<string> To { get; set; }

        /// <summary>
        ///     Содержимое сообщения
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        ///     Тема сообщения
        /// </summary>
        public string Subject { get; set; }
    }
}
