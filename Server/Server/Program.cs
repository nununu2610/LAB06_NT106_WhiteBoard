using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

class Server
{
    private TcpListener listener;
    private List<TcpClient> clients = new List<TcpClient>();
    private int port = 9000;
    private readonly object _lock = new object();
    private const int maxClients = 5;

    public void Start()
    {
        listener = new TcpListener(IPAddress.Any, port);
        listener.Start();
        Console.WriteLine("Server started on port " + port);
        ListenForClients();
    }

    private void ListenForClients()
    {
        while (true)
        {
            TcpClient client = listener.AcceptTcpClient();

            lock (_lock)
            {
                if (clients.Count >= maxClients)
                {
                    // Quá giới hạn, đóng kết nối client mới
                    Console.WriteLine("Client rejected - max clients reached");
                    client.Close();
                    continue;
                }
                clients.Add(client);
                Console.WriteLine("Client connected. Total clients: " + clients.Count);

                // TODO: Gửi email cảnh báo nếu clients.Count == maxClients
            }

            Thread clientThread = new Thread(HandleClient);
            clientThread.IsBackground = true;
            clientThread.Start(client);
        }
    }

    private void HandleClient(object obj)
    {
        TcpClient client = (TcpClient)obj;
        NetworkStream stream = client.GetStream();
        byte[] buffer = new byte[4096];

        try
        {
            while (true)
            {
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                if (bytesRead == 0) break; // client ngắt kết nối

                string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                Console.WriteLine("Received: " + message);

                // Phát lại message cho các client khác
                Broadcast(message, client);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Client disconnected: " + ex.Message);
        }
        finally
        {
            lock (_lock)
            {
                clients.Remove(client);
                Console.WriteLine("Client disconnected. Total clients: " + clients.Count);
            }
            client.Close();
        }
    }

    private void Broadcast(string message, TcpClient excludeClient)
    {
        byte[] data = Encoding.UTF8.GetBytes(message + "\n");
        List<TcpClient> disconnectedClients = new List<TcpClient>();

        lock (_lock)
        {
            foreach (var c in clients)
            {
                if (c != excludeClient)
                {
                    try
                    {
                        NetworkStream stream = c.GetStream();
                        if (stream.CanWrite)
                        {
                            stream.Write(data, 0, data.Length);
                            stream.Flush();
                        }
                    }
                    catch
                    {
                        disconnectedClients.Add(c);
                    }
                }
            }

            // Remove disconnected clients
            foreach (var dc in disconnectedClients)
            {
                clients.Remove(dc);
                dc.Close();
                Console.WriteLine($"Client disconnected. Total clients: {clients.Count}");
            }
        }
    }


    static void Main()
    {
        Server s = new Server();
        s.Start();
    }
}
