using SMTP.Controllers;
using System;
using System.Net.Sockets;
using System.Text;

namespace SMTP
{
    public class ClientObject
    {
        private readonly TcpClient client;
        private readonly СommandСontroller сommands = new СommandСontroller();
        private string messageS, messageC;
        private byte[] data = new byte[1024];

        /// <summary>
        ///     Создает объект клиента для общения с ним
        /// </summary>
        /// <param name="tcpClient">клиент</param>
        public ClientObject(TcpClient tcpClient)
        {
            client = tcpClient;
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
                        else if (messageC.StartsWith("RCPT TO")) messageS = сommands.CommandRcptTo(messageC);
                        else if (messageC.StartsWith("DATA"))
                        {
                            messageS = "354 Start mail input; end with <CRLF>.<CRLF>";
                            SendMessageServerToClient(stream);
                            messageS = сommands.CommandData(stream);
                        }
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
                    else client.Close();
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
