using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SMTP.Interfaces
{
    /// <summary>
    ///     Интерфейс для команд.
    /// </summary>
    public interface ICommands
    {
        /// <summary>
        ///     Команда HELO. Выводит раcширения.
        /// </summary>
        /// <returns>Ответ сервера</returns>
        string CommandHelo();

        /// <summary>
        ///     Команда MAIL FROM. Назначает кто будет посылать письма.
        /// </summary>
        /// <param name="messageClient">Запрос пользователя</param>
        /// <returns>Ответ сервера</returns>
        string CommandMailFrom(string messageClient);

        /// <summary>
        ///     Команда RCPT TO. Назначает кому будет отправлено письмо.
        /// </summary>
        /// <param name="messageClient">Запрос пользователя</param>
        /// <returns>Ответ сервера</returns>
        string CommandRcptTo(string messageClient, string domen, bool relay);

        /// <summary>
        ///     Команда DATA. Начинает запись самого письма.
        /// </summary>
        /// <returns>Ответ сервера</returns>
        string CommandData(NetworkStream stream);

        /// <summary>
        ///     Команда STARTSSL, включает защищенное соединение.
        /// </summary>
        /// <returns>Ответ сервера</returns>
        string CommandStartSsl();

        /// <summary>
        ///     Команда LOGIN. Начинает авторизацию.
        /// </summary>
        /// <returns>Ответ сервера</returns>
        string CommandLogin();

        /// <summary>
        ///     Команда AUTH. Задает логин и пароль пользователя.
        /// </summary>
        /// <param name="messageClient">Запрос пользователя</param>
        /// <returns>Ответ сервера</returns>
        string CommandAuth(string messageClient);

        /// <summary>
        ///     Команда SEND. Отправляет сообщение.
        /// </summary>
        /// <param name="host">Хост</param>
        /// <param name="port">Порт</param>
        /// <returns>Ответ сервера</returns>
        string CommandSend(string host, int port);

        /// <summary>
        ///     Команда выхода.
        /// </summary>
        /// <param name="client">Клиент</param>
        /// <returns>Ответ сервера</returns>
        string CommandQuit(TcpClient client);
    }
}
