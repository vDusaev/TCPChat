using System;
using System.Threading;

namespace ChatServer
{
    class Program
    {
        static void Main(string[] args)
        {
            ServerObject server = null;
            try
            {
                server = new ServerObject();
                Thread thread = new Thread(new ThreadStart(server.Listener));
                thread.Start();
            }
            catch (Exception ex)
            {
                server.Disconnect();
                Console.WriteLine(ex.Message);
            }
        }
    }
}
