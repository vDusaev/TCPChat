using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ChatClient
{
    class Program
    {
        static string userName;
        static TcpClient tcpClient;
        static NetworkStream networkStream;

        static void Main(string[] args)
        {
            Console.WriteLine("Введите свое имя:");
            userName = Console.ReadLine();

            tcpClient = new TcpClient();
            try
            {
                tcpClient.Connect("127.0.0.1", 8888);
                networkStream = tcpClient.GetStream();

                string message = userName;
                byte[] data = Encoding.UTF8.GetBytes(message);
                networkStream.Write(data, 0, data.Length);

                Thread thread = new Thread(new ThreadStart(ReceiveMessage));
                thread.Start();
                Console.WriteLine("Добро пожаловать, {0}", userName);
                SendMessage();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                Disconnect();
            }
        }

        static void SendMessage()
        {
            Console.WriteLine("Введите сообщение: ");
            while (true)
            {
                string message = Console.ReadLine();
                byte[] data = Encoding.UTF8.GetBytes(message);
                networkStream.Write(data);
            }
        }
        static void ReceiveMessage()
        {
            while (true)
            {
                try
                {
                    byte[] data = new byte[64];
                    StringBuilder builder = new StringBuilder();
                    int bytes = 0;
                    do
                    {
                        bytes = networkStream.Read(data, 0, data.Length);
                        builder.Append(Encoding.UTF8.GetString(data), 0, bytes);
                    } while (networkStream.DataAvailable);

                    Console.WriteLine(builder.ToString());
                }
                catch (Exception)
                {
                    Console.WriteLine("Подключение прервано!");
                    Console.ReadLine();
                    Disconnect();
                }
            }
        }

        static void Disconnect()
        {
            if (networkStream != null)
            {
                networkStream.Close();
            }
            if (tcpClient != null)
            {
                tcpClient.Close();
            }
            Environment.Exit(0);
        }
    }
}
