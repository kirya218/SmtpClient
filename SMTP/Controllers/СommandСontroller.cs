﻿using SMTP.Entities;
using SMTP.Interfaces;
using SMTP.Models;
using SMTP.Setup;
using System;
using System.Collections.Generic;
using System.Net;
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
        private Users users = new Users();
        private List<string> nicknameSend = new List<string>();

        /// <summary>
        ///     Хост, IP(может > 1) domain.
        /// </summary>
        private IPHostEntry HostInfo { get; set; }

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
            if (!commands.Contains("HELO")) commands.Add("HELO");
            return "250 domain name should be qualified";
        }

        public string CommandEhlo()
        {
            if (!commands.Contains("EHLO")) commands.Add("EHLO");
            return "250-8BITMIME\r\n250-SIZE\r\n250-STARTSSL\r\n250 LOGIN";
        }

        public string CommandMailFrom(string messageClient)
        {
            if (!commands.Contains("MAIL FROM")) commands.Add("MAIL FROM");
            return CheckMail(messageClient);
        }

        public string CommandRcptTo(string messageClient)
        {
            if (!commands.Contains("RCPT TO")) commands.Add("RCPT TO");
            string[] email = GetClearEmail(messageClient).Split('@');
            List<string> nicks = Users.GetUsers();
            if (Settings.Relay == false)
            {
                if (email[1] == Settings.Domain)
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
                if (email[1] == Settings.Domain)
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
                        {
                            ModelMessage.FullMessage += message;
                            MessageToServer(message);
                        }
                    }
                    else break;
                }
                while (message != ".");
                return CheckInfo();
            }
            else return "First enter 'MAIL FROM' and 'RCPT TO'";
        }

        /// <summary>
        ///     Проверяет наличие всех обезательных полей для отправления сообщения на другой сервер
        /// </summary>
        private string CheckInfo()
        {
            CreateMessagesController createMessage;
            if (ModelMessage.To.Count != 0)
            {
                var a = new SetSettingSMTP(HostInfo);
            }

            if (nicknameSend.Count != 0)
                foreach (var item in nicknameSend)
                    createMessage = new CreateMessagesController(item);
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

            string MX = "smtp." + email.Split('@')[1];

            try
            {
                HostInfo = Dns.GetHostEntry(MX);
                if (HostInfo.AddressList.Length == 0)
                    return "550 no IP addresses with this domain were found";
            }
            catch
            {
                return "550 no such domain was found";
            }

            if (email == string.Empty) return "354 Start typing mail '<', finish with '>'";
            else if (Regex.IsMatch(@"^([a-z0-9_-]+\.)*[a-z0-9_-]+@[a-z0-9_-]+(\.[a-z0-9_-]+)*\.[a-z]{2,6}$", email))
                return "Incorrectly entered email";
            else
            {
                if (clientMessage.StartsWith("MAIL FROM")) ModelMessage.From = email;
                else if (clientMessage.StartsWith("RCPT TO")) ModelMessage.To.Add(email);
                return "250 ok";
            }
        }

        /// <summary>
        ///     Создает сообщение для сервера.
        /// </summary>
        /// <param name="clientMessage">сообщение клиента</param>
        private void MessageToServer(string clientMessage)
        {
            string[] message = clientMessage.Split("\r\n");
            for (int i = 0; i < message.Length; i++)
            {
                if (message[i].Contains("Subject"))
                {
                    string[] k = message[i].Split(':');
                    ModelMessage.Subject = k[1];
                }
                else if (message[i] == "")
                {
                    for (int j = i + 1; j < message.Length; j++)
                    {
                        ModelMessage.Body += message[j];
                    }
                    break;
                }
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
