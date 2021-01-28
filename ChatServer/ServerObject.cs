using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ChatServer
{
    public class ServerObject
    {
        private TcpListener tcpListener;
        private List<ClientObject> clients = new List<ClientObject>();

        protected internal void AddConnection(ClientObject client)
        {
            clients.Add(client);
        }
        protected internal void RemoveConnection(string id)
        {
            ClientObject client = clients.FirstOrDefault(c => c.Id == id);
            if (client != null)
            {
                clients.Remove(client);
            }
        }

        protected internal void Listener()
        {
            try
            {
                tcpListener = new TcpListener(IPAddress.Any, 8888);
                tcpListener.Start();
                Console.WriteLine("Сервер запущен. Ожидание подключений...");

                while (true)
                {
                    TcpClient tcpClient = tcpListener.AcceptTcpClient();

                    ClientObject client = new ClientObject(tcpClient, this);
                    Thread thread = new Thread(new ThreadStart(client.Process));
                    thread.Start();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Disconnect();
            }
        }

        protected internal void BroadcastMessage(string message, string id)
        {
            byte[] data = Encoding.UTF8.GetBytes(message);

            foreach (var client in clients)
            {
                if (client.Id == id)
                {
                    continue;
                }
                client.networkStream.Write(data, 0, data.Length);
            }
        }

        protected internal void Disconnect()
        {
            tcpListener.Stop();

            foreach (var client in clients)
            {
                client.Close();
            }

            Environment.Exit(0);
        }
    }
}
