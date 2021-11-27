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
                listener = new TcpListener(IPAddress.Parse(settings.host), int.Parse(settings.port));
                listener.Start();
                Console.WriteLine("Server IP: " + settings.host + " Server PORT: " + settings.port);
                Console.WriteLine("S: Waiting for connection...");

                while (true)
                {
                    TcpClient client = listener.AcceptTcpClient();
                    ClientObject clientObject = new(client, int.Parse(settings.port), settings.host);
                    Console.WriteLine("S: The client connects to port 1024...");

                    Thread clientThread = new(new ThreadStart(clientObject.Process));
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
