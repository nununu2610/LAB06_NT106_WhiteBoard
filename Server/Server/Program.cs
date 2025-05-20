using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
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

    // Lưu lịch sử các message (DRAW;..., IMAGE;...)
    private List<string> whiteboardHistory = new List<string>();

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
                    Console.WriteLine("Client rejected - max clients reached");
                    client.Close();
                    continue;
                }

                clients.Add(client);
                Console.WriteLine("Client connected. Total clients: " + clients.Count);

                // Gửi email cảnh báo khi đủ maxClients
                if (clients.Count == maxClients)
                {
                    SendAlertEmail();
                }

                // Gửi lại lịch sử whiteboard cho client mới
                SendHistoryToClient(client);
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
        StringBuilder sb = new StringBuilder();

        try
        {
            while (true)
            {
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                if (bytesRead == 0) break; // client ngắt kết nối

                string chunk = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                sb.Append(chunk);

                string allData = sb.ToString();
                int newlineIndex;
                while ((newlineIndex = allData.IndexOf('\n')) >= 0)
                {
                    string message = allData.Substring(0, newlineIndex).Trim();
                    if (!string.IsNullOrEmpty(message))
                    {
                        Console.WriteLine("Received: " + message);

                        lock (_lock)
                        {
                            // Lưu lại lịch sử (chỉ DRAW và IMAGE)
                            if (message.StartsWith("DRAW") || message.StartsWith("IMAGE"))
                            {
                                whiteboardHistory.Add(message);
                            }
                        }

                        // Phát lại message cho các client khác
                        Broadcast(message, client);
                    }
                    allData = allData.Substring(newlineIndex + 1);
                }
                sb.Clear();
                sb.Append(allData);
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

    private void SendHistoryToClient(TcpClient client)
    {
        try
        {
            NetworkStream stream = client.GetStream();
            foreach (string msg in whiteboardHistory)
            {
                byte[] data = Encoding.UTF8.GetBytes(msg + "\n");
                stream.Write(data, 0, data.Length);
                stream.Flush();
                Thread.Sleep(1); // Delay nhỏ để tránh quá tải mạng
            }
            Console.WriteLine("Sent whiteboard history to new client.");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Lỗi khi gửi lịch sử cho client mới: " + ex.Message);
        }
    }

    private void SendAlertEmail()
    {
        try
        {
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress("nnhu7732@NhomNN.nt106"); // Email hợp lệ
            mail.To.Add("nnhu7732@NhomNN.nt106"); // Người nhận
            mail.Subject = "Cảnh báo: Đã có đủ 5 Client kết nối";
            mail.Body = "Hiện tại Server đã có 5 Client đang hoạt dộng";

            SmtpClient smtp = new SmtpClient("192.168.102.93", 587); // port 587 thường dùng
            smtp.Credentials = new NetworkCredential("nnhu7732@NhomNN.nt106", "Nt106Uit@@");
            smtp.EnableSsl = true; // Nếu server hỗ trợ SSL/TLS

            // Bỏ qua lỗi chứng chỉ (chỉ test, không dùng trong production)
            System.Net.ServicePointManager.ServerCertificateValidationCallback =
                (sender, certificate, chain, sslPolicyErrors) => true;

            smtp.Send(mail);
            Console.WriteLine("Gửi mail cảnh báo tới quản trị viên thành công");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Lỗi khi gửi email: " + ex.Message);
            if (ex.InnerException != null)
                Console.WriteLine("Inner Exception: " + ex.InnerException.Message);
        }
    }



    static void Main()
    {
        Server s = new Server();
        s.Start();
    }
}
