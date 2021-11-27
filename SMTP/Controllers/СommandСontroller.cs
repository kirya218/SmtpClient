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
        private ModelInfo info =  new ModelInfo();
        private ModelMessage message = new ModelMessage();
        private ModelAuthorization authorization = new ModelAuthorization();
        private ModelAdditionalOptions options = new ModelAdditionalOptions();
        private ModelCommands commandsSMTP = new ModelCommands();
        private List<string> emailTO = new List<string>();
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
                return "\r\n250-8BITMIME\r\n250-SIZE\r\n250-STARTSSL\r\n250-LOGIN";
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

        public string CommandRcptTo(string messageClient)
        {
            commands.Add("RCPT TO");
            return CheckMail(messageClient);
        }

        public string CommandData(NetworkStream stream)
        {
            string message = string.Empty;

            if (commands.Contains("MAIL FROM") && commands.Contains("RCPT TO"))
                if (!commands.Contains("DATA"))
                {
                    commands.Add("DATA");
                    do
                    {
                        StringBuilder builder = new StringBuilder();
                        byte[] data = new byte[1024];
                        int bytes = stream.Read(data, 0, data.Length);
                        message = builder.Append(Encoding.UTF8.GetString(data, 0, bytes)).ToString();
                        string[] messageWords = message.Split(' ');
                        foreach (var item in messageWords)
                        {
                            if (item.Contains("sub"))
                            {
                                string[] otvet = item.Split(':');
                                this.message.Subject = otvet[1];
                                message = string.Empty;
                                break;
                            }
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
                else return ErrorString;
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

        public string CommandSend(string host, int port)
        {
            message.To = emailTO;
            var Obj = CheckInfo(host, port);
            if (Obj == string.Empty)
                return "250 ok";
            else
                return Obj;
        }

        public string CommandQuit(TcpClient client)
        {
            client.Close();
            return "221 Close connection. See you soon";
        }

        /// <summary>
        ///     Проверяет наличие всех обезательных полей для отправления сообщения на другой сервер
        /// </summary>
        private string CheckInfo(string host, int port)
        {
            string messageServer = string.Empty;
            if (message.From != string.Empty && message.To.Count != 0 && message.Body != string.Empty && message.Subject != string.Empty)
            {
                if (authorization.Login != string.Empty && authorization.Password != string.Empty)
                    if (options.EnableSSL == true)
                    {
                        info.Authorization = authorization;
                        info.Message = message;
                        info.Options = options;
                        info.Port = port;
                        info.Host = host;
                        SetSettingSMTP settings = new SetSettingSMTP(info);
                    }
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
        private string CheckMail(string email)
        {
            string mail = string.Empty;
            bool fg = false;
            foreach (var item in email)
            {
                if (item == '<')
                    fg = true;
                if (fg == true)
                {
                    if (item != '<' && item != '>')
                        mail += item;
                }
            }
            if (mail == string.Empty) return "354 Start typing mail '<', finish with '>'";
            else if (Regex.IsMatch(@"^([a-z0-9_-]+\.)*[a-z0-9_-]+@[a-z0-9_-]+(\.[a-z0-9_-]+)*\.[a-z]{2,6}$", mail))
                return "Incorrectly entered email";
            else
            {
                if (email.StartsWith("MAIL FROM")) message.From = mail;
                else if (email.StartsWith("RCPT TO")) emailTO.Add(mail);
                return "250 ok";
            }
        }
    }
}
