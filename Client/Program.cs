using System;
using System.Net.Sockets;
using System.Text;

namespace Client
{
    public class Program
    {
        static void Main(string[] args)
        {
            TcpClient client;
            Console.Write("IP для подключения: ");
            string ip = Console.ReadLine();
            Console.Write("Порт: ");
            int port = Convert.ToInt32(Console.ReadLine());
            try
            {
                client = new TcpClient(ip, port);
                NetworkStream stream = client.GetStream();

                while (true)
                {
                    byte[] data = new byte[1024];
                    StringBuilder builder = new();
                    int bytes = 0;
                    do
                    {
                        bytes = stream.Read(data, 0, data.Length);
                        builder.Append(Encoding.UTF8.GetString(data, 0, bytes));
                    }
                    while (stream.DataAvailable);

                    string message = builder.ToString();
                    Console.Write("S: " + message);

                    Console.Write("C: ");
                    message = Console.ReadLine() + "\r\n";
                    data = Encoding.UTF8.GetBytes(message);
                    stream.Write(data, 0, data.Length);
                }
            }
            catch
            {
                Console.WriteLine("Не удалось подключится к серверу, проверьте введные данные!");
                return;
            }
        }
    }
}
