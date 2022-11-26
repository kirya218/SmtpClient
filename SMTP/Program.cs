using SMTP.Controllers;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace SMTP
{
    public class Server
    {
        static TcpListener listener;
        static void Main(string[] args)
        { 
            try
            {
                var settings = new SettingController();
                var setting = settings.GetSettings();
                listener = new TcpListener(IPAddress.Parse(setting.Host), setting.Port);
                listener.Start();
                Console.WriteLine("Server IP: " + setting.Host + " Server PORT: " + setting.Port);
                Console.WriteLine("S: Waiting for connection...");

                while (true)
                {
                    var client = listener.AcceptTcpClient();
                    var clientObject = new ClientObject(client);

                    Console.WriteLine("S: The client connects to server...");

                    var clientThread = new Thread(new ThreadStart(clientObject.Process));
                    clientThread.Start();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                if (listener != null)
                {
                    listener.Stop();
                }
            }
        }
    }
}
