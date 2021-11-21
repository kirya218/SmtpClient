using SMTP.Controllers;
using SMTP.Models;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;

namespace SMTP
{
    public class ClientObject
    {
        private string passsword = "hQ86vJf7K6k9e3HtEWcG";
        private TcpClient client;
        private readonly СommandСontroller сommands = new();
        private string messageS, messageC, host;
        private int port;

        /// <summary>
        ///     Создает объект клиента для общения с ним
        /// </summary>
        /// <param name="tcpClient">клиент</param>
        public ClientObject(TcpClient tcpClient, int port, string host)
        {
            client = tcpClient;
            this.port = port;
            this.host = host;
        }

        /// <summary>
        ///     Процесс общения клиента с сервером
        /// </summary>
        public void Process()
        {
            NetworkStream stream = null;
            try
            {
                stream = client.GetStream();
                while (true)
                {
                    byte[] data = new byte[1024];
                    StringBuilder builder = new();
                    int bytes = 0;
                    do
                    {
                        bytes = stream.Read(data, 0, data.Length);
                        builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                    }
                    while (stream.DataAvailable);

                    messageC = builder.ToString();

                    Console.WriteLine("C: " + messageC);
                    if (messageC.StartsWith("EHLO")) messageS = сommands.CommandEhlo();
                    else if (messageC.StartsWith("MAIL FROM")) messageS = сommands.CommandMailFrom(messageC);
                    else if (messageC.StartsWith("RCPT TO")) messageS = сommands.CommandRcptTo(messageC);
                    else if (messageC.StartsWith("DATA")) messageS = сommands.CommandData();
                    else if (messageC.StartsWith("STARTSSL")) messageS = сommands.CommandStartSsl();
                    else if (messageC.StartsWith("LOGIN")) messageS = сommands.CommandLogin();
                    else if (messageC.StartsWith("AUTH")) messageS = сommands.CommandAuth(messageC);
                    else if (messageC.StartsWith("SUBJECT")) messageS = сommands.CommandSubject(messageC);
                    else if (messageC.StartsWith("BODY")) messageS = сommands.CommandBody(messageC);
                    else if (messageC.StartsWith("ISHTML")) messageS = сommands.CommandIsHtml();
                    else if (messageC.StartsWith("SEND")) messageS = сommands.CommandSend(host, port);
                    else if (messageC.StartsWith("QUIT")) messageS = сommands.CommandQuit(client);
                    else messageS = "500 Данной команды не существует";

                    data = Encoding.Unicode.GetBytes(messageS);
                    stream.Write(data, 0, data.Length);
                    messageS = string.Empty;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                if (stream != null)
                    stream.Close();
                if (client != null)
                    client.Close();
            }
        }
    }
}
