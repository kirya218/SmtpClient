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
        private string messageS, messageC, host;
        private int port;
        private byte[] data = new byte[1024];

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
        ///     Получает сообщения от клиента.
        /// </summary>
        /// <param name="stream">Поток</param>
        /// <returns>Возращает сообщение для сервера</returns>
        private StringBuilder GetClientMessages(NetworkStream stream)
        {
            StringBuilder builder = new StringBuilder();
            do
            {
                int bytes = stream.Read(data, 0, data.Length);
                builder.Append(Encoding.UTF8.GetString(data, 0, bytes));
            }
            while (stream.DataAvailable);
            Console.WriteLine("C: " + messageC);

            return builder;
        }

        /// <summary>
        ///     Отправляет сообщение клиенту.
        /// </summary>
        private void SendMessageServerToClient(NetworkStream stream)
        {
            data = Encoding.UTF8.GetBytes(messageS);
            stream.Write(data, 0, data.Length);
            messageS = string.Empty;
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
                    messageC = GetClientMessages(stream).ToString();
                    messageC = messageC.Replace("\r\n", string.Empty);
                    if (messageC.StartsWith("HELO")) messageS = сommands.CommandHelo();
                    else if (messageC.StartsWith("MAIL FROM")) messageS = сommands.CommandMailFrom(messageC);
                    else if (messageC.StartsWith("RCPT TO")) messageS = сommands.CommandRcptTo(messageC);
                    else if (messageC.StartsWith("DATA"))
                    {
                        messageS = "354 Start writing a message to finish writing <CRLF>.<CRLF>.";
                        SendMessageServerToClient(stream);
                        messageS = сommands.CommandData(stream);
                    }
                    else if (messageC.StartsWith("SEND")) messageS = сommands.CommandSend(host, port);
                    else if (messageC.StartsWith("STARTSSL")) messageS = сommands.CommandStartSsl();
                    else if (messageC.StartsWith("LOGIN")) messageS = сommands.CommandLogin();
                    else if (messageC.StartsWith("AUTH")) messageS = сommands.CommandAuth(messageC);
                    else if (messageC.StartsWith("QUIT")) messageS = сommands.CommandQuit(client);
                    else messageS = "500 This command does not exist";
                    SendMessageServerToClient(stream);
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
