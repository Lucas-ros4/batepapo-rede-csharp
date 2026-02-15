using System;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using System.Collections.Generic;

class ClienteInfo   //servidor guarda os clientes para poder identificar eles
{
    public int Id;  
    public StreamWriter Writer;  
}

class Program
{
    static List<ClienteInfo> clientes = new List<ClienteInfo>();  //lista global de clientes
    static int proximoId = 1;   //recebe o proximo numero de id para o proximo cliente

    static void Main()
    {
        TcpListener servidor = new TcpListener(IPAddress.Parse("127.0.0.1"), 5000);
        servidor.Start();

        Console.WriteLine("Servidor iniciado...");

        while (true)   //aceita infinitos clientes(isso pode dar problemas futuramente)
        {
            TcpClient cliente = servidor.AcceptTcpClient();
            Thread t = new Thread(() => AtenderCliente(cliente));
            t.Start();
        }
    }

    static void AtenderCliente(TcpClient cliente)
    {
        NetworkStream stream = cliente.GetStream();
        StreamReader reader = new StreamReader(stream); 
        StreamWriter writer = new StreamWriter(stream);
        writer.AutoFlush = true;

        int id;

        lock (clientes)  //parte responsavel por gerar e atribuir o id do cliente, e adicionar ele a lista de clientes
        { //só funcionou com o lock, sem ele dava erro de concorrencia, assim somente um thread pode acessar essa parte do codigo por vez
            id = proximoId++;  //soma o id para o proximo cliente
            clientes.Add(new ClienteInfo { Id = id, Writer = writer }); //adiciona o cliente a lista de clientes
        }

        writer.WriteLine($"SEU_ID:{id}");
        Broadcast($"[Sistema] Usuario {id} entrou no chat");
        Console.WriteLine($"Cliente {id} conectado");

        try
        {
            while (true)
            {
                string msg = reader.ReadLine();
                if (msg == null) break;

                Broadcast($"[Usuario {id}]: {msg}");
            }
        }
        catch { }

        lock (clientes)
            clientes.RemoveAll(c => c.Id == id);

        Broadcast($"[Sistema] Usuario {id} saiu do chat");
        cliente.Close();

        Console.WriteLine($"Cliente {id} desconectado");
    }

    static void Broadcast(string msg)
    {
        lock (clientes)
        {
            foreach (var c in clientes)
            {
                try
                {
                    c.Writer.WriteLine(msg);
                }
                catch { }
            }
        }
    }
}
