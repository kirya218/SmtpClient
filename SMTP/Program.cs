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
            Settings settings = new Settings();

            try
            {
                listener = new TcpListener(IPAddress.Parse(settings.Host), int.Parse(settings.Port));
                listener.Start();
                Console.WriteLine("Server IP: " + settings.Host + " Server PORT: " + settings.Port);
                Console.WriteLine("S: Waiting for connection...");

                while (true)
                {
                    TcpClient client = listener.AcceptTcpClient();
                    ClientObject clientObject = new ClientObject(client, settings);
                    Console.WriteLine("S: The client connects to server...");

                    Thread clientThread = new Thread(new ThreadStart(clientObject.Process));
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
                    listener.Stop();
            }
        }
    }
}
