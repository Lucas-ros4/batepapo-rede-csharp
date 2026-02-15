using System;
using System.Net.Sockets;
using System.IO;
using System.Threading;

class Program
{
    static void Main()
    {
        TcpClient cliente = new TcpClient("127.0.0.1", 5000);

        Console.WriteLine("Conectado ao servidor!");

        NetworkStream stream = cliente.GetStream();
        StreamReader reader = new StreamReader(stream);
        StreamWriter writer = new StreamWriter(stream);
        writer.AutoFlush = true;

        new Thread(() =>
        {
            while (true)
            {
                string msg = reader.ReadLine();
                if (msg == null) break;
                Console.WriteLine("Servidor: " + msg);
            }
        }).Start();

        while (true)
        {
            string msg = Console.ReadLine();
            writer.WriteLine(msg);
        }
    }
}
