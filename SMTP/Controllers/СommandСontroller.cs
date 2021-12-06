using SMTP.Entities;
using SMTP.Interfaces;
using SMTP.Models;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;

namespace SMTP.Controllers
{
    /// <summary>
    ///     Контролер для выполнения команд
    /// </summary>
    public class СommandСontroller : ICommands
    {
        private ModelInfo info = new ModelInfo();
        private ModelMessage message = new ModelMessage();
        private ModelAuthorization authorization = new ModelAuthorization();
        private ModelAdditionalOptions options = new ModelAdditionalOptions();
        private ModelCommands commandsSMTP = new ModelCommands();
        private Users users = new Users();
        private List<string> emailTO = new List<string>();
        private List<string> nicknameSend = new List<string>();

        /// <summary>
        ///     Ответ сервера, если пользователь уже вводил данную комнду или повторяет её.
        /// </summary>
        private const string ErrorString = "You have already entered this command";

        /// <summary>
        ///     Список для всех команд, которые пользователь уже ввел.
        /// </summary>
        private List<string> commands = new List<string>();

        public string CommandHelo()
        {
            if (!commands.Contains("HELO"))
            {
                commands.Add("HELO");
                return "250 domain name should be qualified";
            }
            else return ErrorString;
        }

        public string CommandEhlo()
        {
            if (!commands.Contains("EHLO"))
            {
                commands.Add("EHLO");
                return "250-8BITMIME\r\n250-SIZE\r\n250-STARTSSL\r\n250 LOGIN";
            }
            else return ErrorString;
        }

        public string CommandMailFrom(string messageClient)
        {
            if (!commands.Contains("MAIL FROM"))
            {
                commands.Add("MAIL FROM");
                return CheckMail(messageClient);
            }
            else return ErrorString;
        }

        public string CommandRcptTo(string messageClient, string domain, bool relay)
        {
            commands.Add("RCPT TO");
            string[] email = GetClearEmail(messageClient).Split('@');
            List<string> nicks = users.GetUsers();
            if (relay == false)
            {
                if (email[1] == domain)
                {
                    if (!nicks.Contains(email[0])) return "550 unknown user account";
                    else
                    {
                        nicknameSend.Add(email[0]);
                        return "250 ok";
                    }
                }
                else return "This server accepts only emails with its own domain";
            }
            else
            {
                if (email[1] == domain)
                {
                    if (nicks.Contains(email[0]))
                    {
                        nicknameSend.Add(email[0]);
                        return "250 ok";
                    }
                    else return "550 unknown user account";
                }
                else return CheckMail(messageClient);
            }
        }

        public string CommandData(NetworkStream stream)
        {
            string message = string.Empty;

            if (commands.Contains("MAIL FROM") && commands.Contains("RCPT TO"))
            {
                if (!commands.Contains("DATA")) commands.Add("DATA");

                do
                {
                    StringBuilder builder = new StringBuilder();
                    byte[] data = new byte[1024];
                    int bytes = stream.Read(data, 0, data.Length);
                    message = builder.Append(Encoding.UTF8.GetString(data, 0, bytes)).ToString();
                    message = message.Replace("\r\n", string.Empty);
                    if (message.Contains("Subject"))
                    {
                        string[] otvet = message.Split(':');
                        this.message.Subject = otvet[1];
                        message = string.Empty;
                    }
                    else if (message.Contains("Content-Type"))
                    {
                        string[] otvet = message.Split(':');
                        if (otvet[1] == "text/plain")
                            options.IsBodyHTML = false;
                        else if (otvet[1] == "html")
                            options.IsBodyHTML = true;
                        message = string.Empty;
                    }
                    if (message != ".")
                    {
                        if (message != string.Empty)
                            this.message.Body += message + "\r\n";
                        data = Encoding.UTF8.GetBytes(" ");
                        stream.Write(data, 0, data.Length);
                    }
                }
                while (message != ".");
                return "250 ok";
            }
            else return "First enter 'MAIL FROM' and 'RCPT TO'";
        }

        public string CommandStartSsl()
        {
            if (!commands.Contains("STARTSSL"))
            {
                commands.Add("STARTSSL");
                options.EnableSSL = true;
                return "250 ok";
            }
            else return "You have already enabled SSL";
        }

        public string CommandLogin()
        {
            if (!commands.Contains("LOGIN"))
                return "Enter your username and password via <CRLF>:<CRLF>. Starting the command with <CR LF>AUTH<CR LF>";
            else return ErrorString;
        }

        public string CommandAuth(string messageClient)
        {
            if (!commands.Contains("AUTH"))
            {
                string[] cred = messageClient.Split(':');
                authorization.Login = cred[1];
                authorization.Password = cred[2];
                return "250 ok";
            }
            else return ErrorString;
        }

        public string CommandSend(string host, int port, bool relay)
        {
            message.To = emailTO;
            var Obj = CheckInfo(host, port, relay);
            if (Obj == string.Empty)
                return "250 ok";
            else
                return Obj;
        }

        /// <summary>
        ///     Проверяет наличие всех обезательных полей для отправления сообщения на другой сервер
        /// </summary>
        private string CheckInfo(string host, int port, bool relay)
        {
            string messageServer = string.Empty;
            if (message.From != null && (message.To.Count != 0 || nicknameSend.Count != 0) && message.Body != null && message.Subject != null)
            {
                CreateMessagesController createMessage;
                SetSettingSMTP settings;
                info.Authorization = authorization;
                info.Message = message;
                info.Options = options;
                info.Port = port;
                info.Host = host;
                if (message.To.Count != 0)
                {
                    if (authorization.Login != null && authorization.Password != null)
                    {
                        if (options.EnableSSL == true)
                        {
                            settings = new SetSettingSMTP(info);
                        }
                        else return "245 Turn on SSL";
                    }
                    else return "245 Logn In";
                }

                if (nicknameSend.Count != 0)
                    foreach (var item in nicknameSend)
                        createMessage = new CreateMessagesController(info, item);
                return string.Empty;

            }
            else
            {
                messageServer = "Not all fields are filled in.\r\nThese commands were not executed: ";
                foreach (var item in commands)
                {
                    if (!commandsSMTP.commands.Contains(item))
                        messageServer += item + "; ";
                }
                return messageServer;
            }
        }



        /// <summary>
        ///     Проверяет email
        /// </summary>
        /// <param name="email">email</param>
        /// <returns>Возращает текстовый ответ(ошибка или все прошло успешно)</returns>
        private string CheckMail(string clientMessage)
        {
            string email = GetClearEmail(clientMessage);
            if (email == string.Empty) return "354 Start typing mail '<', finish with '>'";
            else if (Regex.IsMatch(@"^([a-z0-9_-]+\.)*[a-z0-9_-]+@[a-z0-9_-]+(\.[a-z0-9_-]+)*\.[a-z]{2,6}$", email))
                return "Incorrectly entered email";
            else
            {
                if (clientMessage.StartsWith("MAIL FROM")) message.From = email;
                else if (clientMessage.StartsWith("RCPT TO")) emailTO.Add(email);
                return "250 ok";
            }
        }

        /// <summary>
        ///     Очищает сообщения пользователя(email) от всех знаков кроме самой почты.
        /// </summary>
        /// <param name="message">Сообщение пользователя</param>
        /// <returns>Чистый email</returns>
        private string GetClearEmail(string message)
        {
            string email = string.Empty;
            bool fg = false;
            foreach (var item in message)
            {
                if (item == '<')
                    fg = true;
                if (fg == true)
                {
                    if (item != '<' && item != '>')
                        email += item;
                }
            }
            return email;
        }
    }
}
