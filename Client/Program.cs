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
                    Console.Write("C:");
                    string message = Console.ReadLine();
                    byte[] data = Encoding.Unicode.GetBytes(message);
                    stream.Write(data, 0, data.Length);

                    data = new byte[1024];
                    StringBuilder builder = new();
                    int bytes = 0;
                    do
                    {
                        bytes = stream.Read(data, 0, data.Length);
                        builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                    }
                    while (stream.DataAvailable);

                    message = builder.ToString();
                    Console.WriteLine("S: " + message);
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
