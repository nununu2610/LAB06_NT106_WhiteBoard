using System;
using System.Drawing;
using System.IO;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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

        private Color currentColor = Color.Black;
        private int penThickness = 2;

        private Image currentImage = null;
        private Rectangle currentImageRect = Rectangle.Empty;
        private bool isMovingImage = false;
        private bool isResizingImage = false;
        private Point mouseDownPos;
        private Point imageMoveStartPos;
        private Point resizeStartPos;

        private const int resizeHandleSize = 10;
        private const int MaxImageWidth = 200;
        private const int MaxImageHeight = 200;

        public ClientForm()
        {
            InitializeComponent();

            drawingBitmap = new Bitmap(panelWhiteboard.Width, panelWhiteboard.Height);
            graphics = Graphics.FromImage(drawingBitmap);
            graphics.Clear(Color.White);

            panelWhiteboard.BackgroundImage = drawingBitmap;
            panelWhiteboard.BackgroundImageLayout = ImageLayout.None;

            ConnectToServer();

            numericUpDownThickness.Minimum = 1;
            numericUpDownThickness.Maximum = 10;
            numericUpDownThickness.Value = 2;
            numericUpDownThickness.ValueChanged += NumericUpDownThickness_ValueChanged;

            panelWhiteboard.MouseDown += PanelWhiteboard_MouseDown;
            panelWhiteboard.MouseMove += PanelWhiteboard_MouseMove;
            panelWhiteboard.MouseUp += PanelWhiteboard_MouseUp;

            btnInsertImage.Click += btnInsertImage_Click;
            buttonEnd.Click += buttonEnd_Click;
            btnChooseColor.Click += btnChooseColor_Click;
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
                MessageBox.Show("Cannot connect to server: " + ex.Message);
                Environment.Exit(0);
            }
        }

        private void ListenData()
        {
            byte[] buffer = new byte[4096];
            StringBuilder sb = new StringBuilder();

            while (client != null && client.Connected)
            {
                try
                {
                    if (stream.DataAvailable)
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
                    Thread.Sleep(10);
                }
                catch (IOException ex)
                {
                    Console.WriteLine("IO Error: " + ex.Message);
                    break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                    break;
                }
            }

            this.Invoke((MethodInvoker)delegate
            {
                MessageBox.Show("Lost connection to server.");
                Application.Exit();
            });
        }

        private void ProcessDrawingMessage(string msg)
        {
            if (msg.StartsWith("DRAW"))
            {
                string[] parts = msg.Split(';');
                if (parts.Length != 7) return;

                if (int.TryParse(parts[1], out int x1) &&
                    int.TryParse(parts[2], out int y1) &&
                    int.TryParse(parts[3], out int x2) &&
                    int.TryParse(parts[4], out int y2) &&
                    int.TryParse(parts[5], out int thickness) &&
                    int.TryParse(parts[6], out int argb))
                {
                    Color c = Color.FromArgb(argb);
                    this.Invoke((MethodInvoker)delegate
                    {
                        lock (drawingBitmap)
                        {
                            using (Graphics g = Graphics.FromImage(drawingBitmap))
                            {
                                using (Pen p = new Pen(c, thickness))
                                {
                                    g.DrawLine(p, x1, y1, x2, y2);
                                }
                            }
                            panelWhiteboard.Invalidate();
                        }
                    });
                }
            }
            else if (msg.StartsWith("IMAGE"))
            {
                string[] parts = msg.Split(';');
                if (parts.Length < 6) return;

                if (int.TryParse(parts[1], out int x) &&
                    int.TryParse(parts[2], out int y) &&
                    int.TryParse(parts[3], out int w) &&
                    int.TryParse(parts[4], out int h))
                {
                    string base64Image = parts[5];
                    try
                    {
                        byte[] imgBytes = Convert.FromBase64String(base64Image);
                        using (MemoryStream ms = new MemoryStream(imgBytes))
                        {
                            Image img = Image.FromStream(ms);
                            this.Invoke((MethodInvoker)delegate
                            {
                                lock (drawingBitmap)
                                {
                                    using (Graphics g = Graphics.FromImage(drawingBitmap))
                                    {
                                        g.DrawImage(img, new Rectangle(x, y, w, h));
                                    }
                                    panelWhiteboard.Invalidate();
                                }
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error processing image: " + ex.Message);
                    }
                }
            }
        }

        private void SendMessage(string msg)
        {
            try
            {
                if (client != null && client.Connected && stream != null && stream.CanWrite)
                {
                    byte[] data = Encoding.UTF8.GetBytes(msg + "\n");
                    stream.Write(data, 0, data.Length);
                    stream.Flush();
                }
                else
                {
                    Console.WriteLine("Cannot send - connection not available");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Send error: " + ex.Message);
            }
        }

        private void PanelWhiteboard_MouseDown(object sender, MouseEventArgs e)
        {
            if (IsNearResizeHandle(e.Location))
            {
                isResizingImage = true;
                resizeStartPos = e.Location;
            }
            else if (currentImageRect.Contains(e.Location))
            {
                isMovingImage = true;
                mouseDownPos = e.Location;
                imageMoveStartPos = new Point(currentImageRect.X, currentImageRect.Y);
            }
            else
            {
                isDrawing = true;
                lastPoint = e.Location;
            }
        }

        private string GetPicturesFolderPath()
        {
            try
            {
                // Lấy đường dẫn file thực thi (Client.exe)
                string exePath = System.Reflection.Assembly.GetExecutingAssembly().Location;

                // Lấy thư mục chứa file exe (D:\LAB06_NT106_WhiteBoard\Client\Client\bin\Debug\)
                string exeDirectory = Path.GetDirectoryName(exePath);

                // Đi lên 4 cấp thư mục để ra thư mục LAB06_NT106_WhiteBoard
                DirectoryInfo dir = Directory.GetParent(exeDirectory); // bin\Debug
                dir = Directory.GetParent(dir.FullName); // Client
                dir = Directory.GetParent(dir.FullName); // Client
                dir = Directory.GetParent(dir.FullName); // LAB06_NT106_WhiteBoard

                if (dir == null)
                    throw new Exception("Không tìm thấy thư mục gốc dự án");

                // Kết hợp với thư mục Pictures
                string picturesPath = Path.Combine(dir.FullName, "Pictures");

                // Tạo thư mục nếu chưa tồn tại
                if (!Directory.Exists(picturesPath))
                {
                    Directory.CreateDirectory(picturesPath);
                }

                return picturesPath;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xác định thư mục lưu ảnh: {ex.Message}\nẢnh sẽ được lưu vào thư mục ứng dụng");
                return Application.StartupPath;
            }
        }


        private void SaveWhiteboardImage()
        {
            try
            {
                string picturesPath = GetPicturesFolderPath();

                // Đảm bảo thư mục tồn tại
                if (!Directory.Exists(picturesPath))
                {
                    Directory.CreateDirectory(picturesPath);
                }

                // Tạo tên file với timestamp
                string fileName = $"Whiteboard_{DateTime.Now:yyyyMMdd_HHmmss}.png";
                string fullPath = Path.Combine(picturesPath, fileName);

                // Lưu ảnh
                lock (drawingBitmap)
                {
                    drawingBitmap.Save(fullPath, System.Drawing.Imaging.ImageFormat.Png);
                }

                // Hiển thị thông báo
                MessageBox.Show($"Đã lưu ảnh thành công tại:\n{fullPath}", "Thông báo",
                               MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Mở thư mục chứa ảnh
                System.Diagnostics.Process.Start("explorer.exe", $"/select,\"{fullPath}\"");
            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show("Lỗi: Không có quyền truy cập thư mục đích.", "Lỗi",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi lưu ảnh:\n{ex.Message}", "Lỗi",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void PanelWhiteboard_MouseMove(object sender, MouseEventArgs e)
        {
            if (isResizingImage)
            {
                int dx = e.X - resizeStartPos.X;
                int dy = e.Y - resizeStartPos.Y;
                currentImageRect.Width = Math.Max(20, currentImageRect.Width + dx);
                currentImageRect.Height = Math.Max(20, currentImageRect.Height + dy);
                resizeStartPos = e.Location;
                RedrawWhiteboard();
            }
            else if (isMovingImage)
            {
                int dx = e.X - mouseDownPos.X;
                int dy = e.Y - mouseDownPos.Y;
                currentImageRect.X = imageMoveStartPos.X + dx;
                currentImageRect.Y = imageMoveStartPos.Y + dy;
                RedrawWhiteboard();
            }
            else if (isDrawing)
            {
                using (Graphics g = Graphics.FromImage(drawingBitmap))
                {
                    using (Pen pen = new Pen(currentColor, penThickness))
                    {
                        g.DrawLine(pen, lastPoint, e.Location);
                    }
                }
                SendMessage($"DRAW;{lastPoint.X};{lastPoint.Y};{e.X};{e.Y};{penThickness};{currentColor.ToArgb()}");
                lastPoint = e.Location;
                panelWhiteboard.Invalidate();
            }
        }

        private void PanelWhiteboard_MouseUp(object sender, MouseEventArgs e)
        {
            if (isDrawing) isDrawing = false;
            if (isMovingImage)
            {
                isMovingImage = false;
                SendCurrentImagePosition();
            }
            if (isResizingImage)
            {
                isResizingImage = false;
                SendCurrentImagePosition();
            }
        }

        private bool IsNearResizeHandle(Point pt)
        {
            Rectangle handle = new Rectangle(
                currentImageRect.Right - resizeHandleSize,
                currentImageRect.Bottom - resizeHandleSize,
                resizeHandleSize, resizeHandleSize);
            return handle.Contains(pt);
        }

        private async void btnInsertImage_Click(object sender, EventArgs e)
        {
            string url = txtImageUrl.Text.Trim();
            if (string.IsNullOrEmpty(url))
            {
                MessageBox.Show("Vui lòng nhập URL ảnh");
                return;
            }

            try
            {
                Image img = await LoadImageFromUrl(url);
                if (img == null)
                {
                    MessageBox.Show("Không tải được ảnh từ URL");
                    return;
                }

                Size newSize = ResizeToFit(img.Size, MaxImageWidth, MaxImageHeight);
                Bitmap resizedImage = new Bitmap(img, newSize);
                currentImage = resizedImage;
                currentImageRect = new Rectangle(50, 50, resizedImage.Width, resizedImage.Height);

                RedrawWhiteboard();
                SendCurrentImagePosition();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải ảnh: " + ex.Message);
            }
        }

        private Size ResizeToFit(Size original, int maxWidth, int maxHeight)
        {
            float ratioX = (float)maxWidth / original.Width;
            float ratioY = (float)maxHeight / original.Height;
            float ratio = Math.Min(ratioX, ratioY);
            return new Size((int)(original.Width * ratio), (int)(original.Height * ratio));
        }

        private async Task<Image> LoadImageFromUrl(string url)
        {
            using (HttpClient http = new HttpClient())
            {
                var response = await http.GetAsync(url);
                if (!response.IsSuccessStatusCode) return null;
                using (var stream = await response.Content.ReadAsStreamAsync())
                {
                    return Image.FromStream(stream);
                }
            }
        }

        private void RedrawWhiteboard()
        {
            lock (drawingBitmap)
            {
                graphics.Clear(Color.White);
                if (currentImage != null && currentImageRect != Rectangle.Empty)
                {
                    graphics.DrawImage(currentImage, currentImageRect);
                    graphics.FillRectangle(Brushes.Gray,
                        currentImageRect.Right - resizeHandleSize,
                        currentImageRect.Bottom - resizeHandleSize,
                        resizeHandleSize, resizeHandleSize);
                }
            }
            panelWhiteboard.Invalidate();
        }

        private void SendCurrentImagePosition()
        {
            if (currentImage == null || currentImageRect == Rectangle.Empty) return;

            try
            {
                string base64;
                using (MemoryStream ms = new MemoryStream())
                {
                    // Chỉ lưu phần ảnh gốc, không lưu cả bitmap đã vẽ
                    if (currentImage is Bitmap bmp)
                    {
                        bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    }
                    else
                    {
                        using (Bitmap temp = new Bitmap(currentImage))
                        {
                            temp.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                        }
                    }
                    base64 = Convert.ToBase64String(ms.ToArray());
                }

                string msg = $"IMAGE;{currentImageRect.X};{currentImageRect.Y};{currentImageRect.Width};{currentImageRect.Height};{base64}";
                SendMessage(msg);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error sending image: " + ex.Message);
            }
        }

        private void NumericUpDownThickness_ValueChanged(object sender, EventArgs e)
        {
            penThickness = (int)numericUpDownThickness.Value;
        }

        private void btnChooseColor_Click(object sender, EventArgs e)
        {
            ColorDialog dlg = new ColorDialog();
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                currentColor = dlg.Color;
            }
        }

        private void buttonEnd_Click(object sender, EventArgs e)
        {
            // Lưu ảnh trước khi thoát
            SaveWhiteboardImage();

            // Đóng kết nối và ứng dụng
            try
            {
                if (client != null && client.Connected)
                {
                    SendMessage("DISCONNECT");
                    client.Close();
                }
            }
            catch { }

            Application.Exit();
        }
    }
}
