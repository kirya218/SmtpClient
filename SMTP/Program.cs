using SMTP.Setup;
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
                listener = new TcpListener(IPAddress.Parse(Settings.Host), int.Parse(Settings.Port));
                listener.Start();
                Console.WriteLine("Server IP: " + Settings.Host + " Server PORT: " + Settings.Port);
                Console.WriteLine("S: Waiting for connection...");

                while (true)
                {
                    TcpClient client = listener.AcceptTcpClient();
                    ClientObject clientObject = new ClientObject(client);
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
