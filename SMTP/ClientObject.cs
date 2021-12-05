using SMTP.Controllers;
using System;
using System.Net.Sockets;
using System.Text;

namespace SMTP
{
    public class ClientObject
    {
        //hQ86vJf7K6k9e3HtEWcG
        private TcpClient client;
        private readonly СommandСontroller сommands = new();
        private string messageS, messageC, host, domain;
        private bool relay;
        private int port;
        private byte[] data = new byte[1024];

        /// <summary>
        ///     Создает объект клиента для общения с ним
        /// </summary>
        /// <param name="tcpClient">клиент</param>
        public ClientObject(TcpClient tcpClient, Settings settings)
        {
            client = tcpClient;
            port = Convert.ToInt32(settings.Port);
            host = settings.Host;
            relay = settings.Relay;
            domain = settings.Domain;
        }

        /// <summary>
        ///     Получает сообщения от клиента.
        /// </summary>
        /// <param name="stream">Поток</param>
        /// <returns>Возращает сообщение для сервера</returns>
        private void GetClientMessages(NetworkStream stream)
        {
            StringBuilder builder = new StringBuilder();
            do
            {
                int bytes = stream.Read(data, 0, data.Length);
                builder.Append(Encoding.UTF8.GetString(data, 0, bytes));
            }
            while (stream.DataAvailable);
            messageC = builder.ToString().Replace("\r\n", string.Empty);
            if (messageC != string.Empty)
                Console.WriteLine("C: " + messageC);
        }

        /// <summary>
        ///     Отправляет сообщение клиенту.
        /// </summary>
        private void SendMessageServerToClient(NetworkStream stream)
        {
            messageS += "\r\n";
            Console.Write("S: " + messageS);
            data = Encoding.UTF8.GetBytes(messageS);
            stream.Write(data, 0, data.Length);
            messageS = string.Empty;
        }

        /// <summary>
        ///     Начало разговора с сервреом
        /// </summary>
        /// <param name="stream">поток данных</param>
        private void StartServerTalk(NetworkStream stream)
        {
            messageS = "220 smtp.yamong.ru Hello";
            SendMessageServerToClient(stream);
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
                StartServerTalk(stream);
                while (true)
                {
                    GetClientMessages(stream);
                    if (messageC != string.Empty)
                    {
                        if (messageC.StartsWith("HELO")) messageS = сommands.CommandHelo();
                        else if (messageC.StartsWith("EHLO")) messageS = сommands.CommandEhlo();
                        else if (messageC.StartsWith("MAIL FROM")) messageS = сommands.CommandMailFrom(messageC);
                        else if (messageC.StartsWith("RCPT TO")) messageS = сommands.CommandRcptTo(messageC, domain, relay);
                        else if (messageC.StartsWith("DATA"))
                        {
                            messageS = "354 Start writing a message to finish writing <CRLF>.<CRLF>.";
                            SendMessageServerToClient(stream);
                            messageS = сommands.CommandData(stream);
                            if (messageS != "250 ok")
                                SendMessageServerToClient(stream);
                            else
                                messageS = сommands.CommandSend(host, port, relay);
                        }
                        else if (messageC.StartsWith("STARTSSL")) messageS = сommands.CommandStartSsl();
                        else if (messageC.StartsWith("LOGIN")) messageS = сommands.CommandLogin();
                        else if (messageC.StartsWith("AUTH")) messageS = сommands.CommandAuth(messageC);
                        else if (messageC.StartsWith("QUIT"))
                        {
                            messageS = "221 mail.yamong.ru close connection. See you soon";
                            SendMessageServerToClient(stream);
                            client.Close();
                            break;
                        }
                        else messageS = "500 This command does not exist";
                        SendMessageServerToClient(stream);
                    }
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
