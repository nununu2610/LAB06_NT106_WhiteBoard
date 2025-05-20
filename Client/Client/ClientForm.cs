using System;
using System.Drawing;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Client
{
    public partial class ClientForm : Form
    {
        private TcpClient client;
        private NetworkStream stream;

        private bool isDrawing = false;
        private Point lastPoint;

        private Bitmap drawingBitmap;
        private Graphics graphics;

        private Color penColor = Color.Black;
        private int penThickness = 2;

        public ClientForm()
        {
            InitializeComponent();

            // Khởi tạo bitmap và graphics để vẽ lên panel
            drawingBitmap = new Bitmap(panelWhiteboard.Width, panelWhiteboard.Height);
            graphics = Graphics.FromImage(drawingBitmap);
            graphics.Clear(Color.White);

            panelWhiteboard.BackgroundImage = drawingBitmap;
            panelWhiteboard.BackgroundImageLayout = ImageLayout.None;

            // Kết nối server
            ConnectToServer();

            // Thiết lập các lựa chọn màu sắc, độ dày
            comboBoxColor.Items.AddRange(new string[] { "Black", "Red", "Green", "Blue" });
            comboBoxColor.SelectedIndex = 0;

            numericUpDownThickness.Minimum = 1;
            numericUpDownThickness.Maximum = 10;
            numericUpDownThickness.Value = 2;

            comboBoxColor.SelectedIndexChanged += ComboBoxColor_SelectedIndexChanged;
            numericUpDownThickness.ValueChanged += NumericUpDownThickness_ValueChanged;

            // Gán sự kiện chuột cho panel vẽ
            panelWhiteboard.MouseDown += PanelWhiteboard_MouseDown;
            panelWhiteboard.MouseMove += PanelWhiteboard_MouseMove;
            panelWhiteboard.MouseUp += PanelWhiteboard_MouseUp;
        }

        private void ConnectToServer()
        {
            try
            {
                client = new TcpClient("127.0.0.1", 9000);
                stream = client.GetStream();

                Thread listenThread = new Thread(ListenData);
                listenThread.IsBackground = true;
                listenThread.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không kết nối được tới server: " + ex.Message);
                Environment.Exit(0);
            }
        }

        private void ListenData()
        {
            byte[] buffer = new byte[4096];
            StringBuilder sb = new StringBuilder();

            while (true)
            {
                try
                {
                    int bytesRead = stream.Read(buffer, 0, buffer.Length);
                    if (bytesRead == 0) break;

                    string chunk = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    sb.Append(chunk);

                    string allData = sb.ToString();
                    int newlineIndex;
                    while ((newlineIndex = allData.IndexOf('\n')) >= 0)
                    {
                        string line = allData.Substring(0, newlineIndex).Trim();
                        if (line.Length > 0)
                            ProcessDrawingMessage(line);

                        allData = allData.Substring(newlineIndex + 1);
                    }
                    sb.Clear();
                    sb.Append(allData);
                }
                catch
                {
                    break;
                }
            }

            this.Invoke(new Action(() =>
            {
                MessageBox.Show("Đã mất kết nối tới server.");
                Application.Exit();
            }));
        }


        private void ProcessDrawingMessage(string msg)
        {
            if (!msg.StartsWith("DRAW")) return;

            string[] parts = msg.Split(';');
            if (parts.Length != 7) return;

            if (int.TryParse(parts[1], out int x1) &&
                int.TryParse(parts[2], out int y1) &&
                int.TryParse(parts[3], out int x2) &&
                int.TryParse(parts[4], out int y2) &&
                int.TryParse(parts[6], out int thickness))
            {
                Color color;
                try
                {
                    color = Color.FromName(parts[5]);
                }
                catch
                {
                    color = Color.Black; // Default color if parsing fails
                }

                if (panelWhiteboard.InvokeRequired)
                {
                    panelWhiteboard.Invoke(new Action(() =>
                    {
                        using (Pen pen = new Pen(color, thickness))
                        {
                            graphics.DrawLine(pen, x1, y1, x2, y2);
                        }
                        panelWhiteboard.Invalidate();
                    }));
                }
                else
                {
                    using (Pen pen = new Pen(color, thickness))
                    {
                        graphics.DrawLine(pen, x1, y1, x2, y2);
                    }
                    panelWhiteboard.Invalidate();
                }
            }
        }

        private void PanelWhiteboard_MouseDown(object sender, MouseEventArgs e)
        {
            isDrawing = true;
            lastPoint = e.Location;
        }

        private void PanelWhiteboard_MouseMove(object sender, MouseEventArgs e)
        {
            if (!isDrawing) return;

            Point currentPoint = e.Location;

            using (Pen pen = new Pen(penColor, penThickness))
            {
                graphics.DrawLine(pen, lastPoint, currentPoint);
            }
            panelWhiteboard.Invalidate();

            // Gửi dữ liệu vẽ cho server
            string message = $"DRAW;{lastPoint.X};{lastPoint.Y};{currentPoint.X};{currentPoint.Y};{penColor.Name};{penThickness}";
            SendMessage(message);

            // Comment tạm dòng gửi message cho server
            // string message = $"DRAW;{lastPoint.X};{lastPoint.Y};{currentPoint.X};{currentPoint.Y};{penColor.Name};{penThickness}";
            // SendMessage(message);

            lastPoint = currentPoint;
        }

        private void PanelWhiteboard_MouseUp(object sender, MouseEventArgs e)
        {
            isDrawing = false;
        }

        private void SendMessage(string message)
        {
            try
            {
                if (client != null && client.Connected && stream != null && stream.CanWrite)
                {
                    byte[] data = Encoding.UTF8.GetBytes(message + "\n");
                    stream.Write(data, 0, data.Length);
                    stream.Flush();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Send error: {ex.Message}");
                // Handle reconnect if needed
            }
        }

        private void ComboBoxColor_SelectedIndexChanged(object sender, EventArgs e)
        {
            penColor = Color.FromName(comboBoxColor.SelectedItem.ToString());
        }

        private void NumericUpDownThickness_ValueChanged(object sender, EventArgs e)
        {
            penThickness = (int)numericUpDownThickness.Value;
        }

        private void buttonEnd_Click(object sender, EventArgs e)
        {
            SaveWhiteboardImage();
            Application.Exit();
        }

        private void SaveWhiteboardImage()
        {
            try
            {
                // Tạo đường dẫn thư mục
                string directoryPath = @"C:\Users\ACER\Downloads\NT106\Lab06\Pictures";

                // Đảm bảo thư mục tồn tại, nếu không thì tạo mới
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                // Tạo tên file với timestamp
                string fileName = $"Whiteboard_{DateTime.Now:yyyyMMdd_HHmmss}.png";
                string fullPath = Path.Combine(directoryPath, fileName);

                // Lưu ảnh
                drawingBitmap.Save(fullPath, System.Drawing.Imaging.ImageFormat.Png);

                MessageBox.Show($"Đã lưu ảnh thành công tại:\n{fullPath}", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show("Lỗi: Không có quyền truy cập thư mục đích.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi lưu ảnh:\n{ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void panelWhiteboard_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
