using SMTP.Controllers;
using SMTP.Entities;
using System;
using System.Net.Sockets;
using System.Text;

namespace SMTP
{
    public class ClientObject
    {
        private readonly TcpClient _client;
        private readonly СommandСontroller _сommands;
        private readonly NetworkStream _stream;

        /// <summary>
        ///     Создает объект клиента для общения с ним
        /// </summary>
        /// <param name="tcpClient">клиент</param>
        public ClientObject(TcpClient tcpClient)
        {
            _client = tcpClient;
            _сommands = new СommandСontroller(new UserController(), new SettingController(), new DomainController());
            _stream = tcpClient.GetStream();
        }

        /// <summary>
        ///     Получает сообщения от клиента.
        /// </summary>
        /// <param name="stream">Поток</param>
        /// <returns>Возращает сообщение для сервера</returns>
        private string GetClientMessage()
        {
            var builder = new StringBuilder();
            var data = new byte[1024];

            do
            {
                int bytes = _stream.Read(data, 0, data.Length);
                builder.Append(Encoding.UTF8.GetString(data, 0, bytes));
            }
            while (_stream.DataAvailable);

            var messageClient = builder.ToString().Trim();

            Console.WriteLine("C: " + messageClient);

            return messageClient;
        }

        /// <summary>
        ///     Отправляет сообщение клиенту.
        /// </summary>
        private void SendMessageToClient(string message)
        {
            message += "\r\n";
            Console.Write("S: " + message);
            var data = Encoding.UTF8.GetBytes(message);
            _stream.Write(data, 0, data.Length);
        }

        /// <summary>
        ///     Процесс общения клиента с сервером
        /// </summary>
        public void Process()
        {
            try
            {
                SendMessageToClient("");
                while (true)
                {
                    var messageClient = GetClientMessage();

                    if (messageClient != string.Empty)
                    {
                        var messageServer = string.Empty;

                        if (messageClient.StartsWith("HELO"))
                        {
                            messageServer = _сommands.CommandHelo();
                        }
                        else if (messageClient.StartsWith("EHLO"))
                        {
                            messageServer = _сommands.CommandEhlo();
                        }
                        else if (messageClient.StartsWith("MAIL FROM"))
                        {
                            messageServer = _сommands.CommandMailFrom(messageClient);
                        }
                        else if (messageClient.StartsWith("RCPT TO"))
                        {
                            messageServer = _сommands.CommandRcptTo(messageClient);
                        }
                        else if (messageClient.StartsWith("DATA"))
                        {
                            SendMessageToClient("354 Start mail input; end with <CRLF>.<CRLF>");

                            var clientMessage = new StringBuilder();

                            while (true)
                            {
                                var message = GetClientMessage();

                                if (message == ".")
                                {
                                    break;
                                }

                                clientMessage.Append(message);
                            }

                            messageServer = _сommands.CommandData(clientMessage.ToString());
                        }
                        else if (messageClient.StartsWith("QUIT"))
                        {
                            SendMessageToClient("221 mail.yamong.ru close connection. See you soon");
                            _client.Close();
                            break;
                        }

                        messageServer = "500 This command does not exist";
                        SendMessageToClient(messageServer);
                    }
                    else
                    {
                        _client.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                if (_stream != null)
                    _stream.Close();
                if (_client != null)
                    _client.Close();
            }
        }
    }
}
