using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMTP.Models
{
    /// <summary>
    ///     Модель информации для отправки сообщения на другой серверы
    /// </summary>
    public class ModelInfo
    {
        /// <summary>
        ///     Модель сообщение
        /// </summary>
        public ModelMessage Message { get; set; }

        /// <summary>
        ///     Модель авторизации
        /// </summary>
        public ModelAuthorization Authorization { get; set; }

        /// <summary>
        ///     Модель допольнительных опций
        /// </summary>
        public ModelAdditionalOptions Options { get; set; }

        /// <summary>
        ///     Порт
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        ///     TCP/IP
        /// </summary>
        public string Host { get; set; }
    }
}
