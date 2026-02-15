using System;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;

class Program
{
    static void Main()
    {
        TcpListener servidor = new TcpListener(IPAddress.Parse("127.0.0.1"), 5000);

        servidor.Start();
        Console.WriteLine("Servidor iniciado... aguardando cliente");

        TcpClient cliente = servidor.AcceptTcpClient();
        Console.WriteLine("Cliente conectado!");

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
                Console.WriteLine("Cliente: " + msg);
            }
        }).Start();

        while (true)
        {
            string msg = Console.ReadLine();
            writer.WriteLine(msg);
        }
    }
}
