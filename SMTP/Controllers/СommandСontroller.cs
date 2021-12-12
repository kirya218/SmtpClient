using SMTP.Entities;
using SMTP.Interfaces;
using SMTP.Models;
using System;
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
        private Users users = new Users();
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
                    if (!nicks.Contains(email[0])) return "550 No such user here";
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
                    else return "550 No such user here";
                }
                else return CheckMail(messageClient);
            }
        }

        public string CommandData(NetworkStream stream)
        {
            if (commands.Contains("MAIL FROM") && commands.Contains("RCPT TO"))
            {
                if (!commands.Contains("DATA")) commands.Add("DATA");

                string message;
                do
                {
                    StringBuilder builder = new StringBuilder();
                    byte[] data = new byte[1024];
                    int bytes = stream.Read(data, 0, data.Length);
                    message = builder.Append(Encoding.UTF8.GetString(data, 0, bytes)).ToString();
                    Console.WriteLine(message);
                    if (message.Replace("\r\n", string.Empty) != ".")
                    {
                        if (message != string.Empty)
                            this.message.Body += message;
                    }
                    else break;
                }
                while (message != ".");
                return CheckInfo();
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

        /// <summary>
        ///     Проверяет наличие всех обезательных полей для отправления сообщения на другой сервер
        /// </summary>
        private string CheckInfo()
        {
            CreateMessagesController createMessage;
            info.Authorization = authorization;
            info.Message = message;
            info.Options = options;
            if (message.To != null)
            {
                if (authorization.Login != null && authorization.Password != null)
                {
                    if (options.EnableSSL == true)
                    {
                        var a = new SetSettingSMTP(info);
                    }
                    else return "245 Turn on SSL";
                }
                else return "245 Logn In";
            }

            if (nicknameSend.Count != 0)
                foreach (var item in nicknameSend)
                    createMessage = new CreateMessagesController(info, item);
            return "250 OK";
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
                else if (clientMessage.StartsWith("RCPT TO")) message.To.Add(email);
                return "250 ok";
            }
        }

        /// <summary>
        ///     Очищает сообщения пользователя(email) от всех знаков кроме самой почты.
        /// </summary>
        /// <param name="message">Сообщение пользователя</param>
        /// <returns>Чистый email</returns>
        private static string GetClearEmail(string message)
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
                    {
                        email += item;
                    }
                    else if (item == '>') break;
                }
            }
            return email;
        }
    }
}
