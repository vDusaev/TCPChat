using System;
using System.Net.Sockets;
using System.Text;

namespace ChatServer
{
    public class ClientObject
    {
        protected internal string Id { get; private set; }
        protected internal NetworkStream networkStream { get; private set; }
        
        private string userName;
        private TcpClient tcpClient;
        private ServerObject server;

        public ClientObject(TcpClient tcpClient, ServerObject server)
        {
            Id = Guid.NewGuid().ToString();
            this.tcpClient = tcpClient;
            this.server = server;
            server.AddConnection(this);
        }

        protected internal void Process()
        {
            try
            {
                networkStream = tcpClient.GetStream();
                
                userName = GetMessage();
                string message = userName + " вошел в чат";

                server.BroadcastMessage(message, this.Id);
                Console.WriteLine(message);

                while (true)
                {
                    try
                    {
                        message = GetMessage();
                        message = String.Format("{0}: {1}", userName, message);
                        Console.WriteLine(message);
                        server.BroadcastMessage(message, Id);
                    }
                    catch
                    {
                        message = String.Format("{0}: покинул чат", userName);
                        Console.WriteLine(message);
                        server.BroadcastMessage(message, Id);
                        break;
                    }
                }
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                server.RemoveConnection(Id);
                Close();
            }
        }

        private string GetMessage()
        {
            byte[] data = new byte[64];
            StringBuilder builder = new StringBuilder();
            int bytes = 0;
            do
            {
                bytes = networkStream.Read(data, 0, data.Length);
                builder.Append(Encoding.UTF8.GetString(data), 0, bytes);
            } while (networkStream.DataAvailable);

            return builder.ToString();
        }

        protected internal void Close()
        {
            if (networkStream != null)
            {
                networkStream.Close();
            }
            if (tcpClient != null)
            {
                tcpClient.Close();
            }
        }
    }
}
