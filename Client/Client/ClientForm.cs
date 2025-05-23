using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

enum DrawingMode
{
    FreeHand,
    Rectangle,
    Ellipse,
    Line
}



namespace Client
{
    public partial class ClientForm : Form
    {
        private Rectangle GetRectangleFromPoints(Point p1, Point p2)
        {
            return new Rectangle(
                Math.Min(p1.X, p2.X),
                Math.Min(p1.Y, p2.Y),
                Math.Abs(p1.X - p2.X),
                Math.Abs(p1.Y - p2.Y));
        }
        List<Point> currentLine = null; 

        List<List<Point>> lines = new List<List<Point>>();

       
        private TcpClient client;
        private NetworkStream stream;


        private Point startPoint;    // điểm bắt đầu vẽ (MouseDown)
        private Point lastPoint;     // điểm cuối cùng vẽ (dùng cho FreeHand hoặc vẽ)
        private Point currentPoint;  // điểm hiện tại khi kéo chuột (MouseMove) dùng để vẽ tạm thời
        private bool isDrawing = false;
        private DrawingMode currentMode;  // enum bạn tự định nghĩa như Line, Rectangle, Ellipse, FreeHand


        private Bitmap drawingBitmap;
        private Graphics graphics;


        private Color previousColor = Color.Black;

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

          
            panelWhiteboard.BackgroundImageLayout = ImageLayout.None;

            ConnectToServer();

            numericUpDownThickness.Minimum = 1;
            numericUpDownThickness.Maximum = 10;
            numericUpDownThickness.Value = 2;
            numericUpDownThickness.ValueChanged += NumericUpDownThickness_ValueChanged;

            panelWhiteboard.MouseDown += PanelWhiteboard_MouseDown;
            panelWhiteboard.MouseMove += PanelWhiteboard_MouseMove;
            panelWhiteboard.MouseUp += panelWhiteboard_MouseUp;

            btnInsertImage.Click += btnInsertImage_Click;
            buttonEnd.Click += buttonEnd_Click;
            btnChooseColor.Click += btnChooseColor_Click;

            comboBoxDrawMode.Items.Add("Freehand");
            comboBoxDrawMode.Items.Add("Rectangle");
            comboBoxDrawMode.Items.Add("Ellipse");
            comboBoxDrawMode.Items.Add("Line");
            comboBoxDrawMode.SelectedIndex = 0; // chọn mặc định là Freehand
            comboBoxDrawMode.SelectedIndexChanged += ComboBoxDrawMode_SelectedIndexChanged;

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
                Console.WriteLine("[CLIENT] Received DRAW message: " + msg);

                string[] parts = msg.Split(';');
                if (parts.Length != 8) return; // cần đủ 8 phần

                string shape = parts[1];

                if (int.TryParse(parts[2], out int argb) &&
                    float.TryParse(parts[3], out float thickness) &&
                    int.TryParse(parts[4], out int x1) &&
                    int.TryParse(parts[5], out int y1) &&
                    int.TryParse(parts[6], out int x2) &&
                    int.TryParse(parts[7], out int y2))
                {
                    Color c = Color.FromArgb(argb);
                    Point p1 = new Point(x1, y1);
                    Point p2 = new Point(x2, y2);

                    this.Invoke((MethodInvoker)delegate
                    {
                        lock (drawingBitmap)
                        {
                            using (Graphics g = Graphics.FromImage(drawingBitmap))
                            using (Pen pen = new Pen(c, thickness))
                            {
                                switch (shape)
                                {
                                    case "Line":
                                        g.DrawLine(pen, p1, p2);
                                        break;
                                    case "Rectangle":
                                        g.DrawRectangle(pen, GetRectangleFromPoints(p1, p2));
                                        break;
                                    case "Ellipse":
                                        g.DrawEllipse(pen, GetRectangleFromPoints(p1, p2));
                                        break;
                                    case "FreeHand":
                                        g.DrawLine(pen, p1, p2); // Mỗi đoạn vẽ tay là một line nhỏ
                                        break;
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
            else if (e.Button == MouseButtons.Left)
            {
                isDrawing = true;
                startPoint = e.Location;   // điểm bắt đầu vẽ cho các kiểu
                lastPoint = e.Location;    // dùng cho vẽ FreeHand
                currentPoint = e.Location; // cập nhật điểm hiện tại cho vẽ tạm thời
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
            if (isDrawing)
            {
                if (currentMode == DrawingMode.FreeHand)
                {
                    using (Graphics g = Graphics.FromImage(drawingBitmap))
                    {
                        using (Pen pen = new Pen(currentColor, penThickness))
                        {
                            g.DrawLine(pen, lastPoint, e.Location);
                        }
                    }

                    // Gửi dữ liệu đoạn line vừa vẽ để đồng bộ
                    SendDrawCommand("FreeHand", lastPoint, e.Location, currentColor, penThickness);

                    lastPoint = e.Location;
                    panelWhiteboard.Invalidate();
                }
                else
                {
                    // Cập nhật điểm hiện tại để vẽ tạm thời trong Paint event
                    currentPoint = e.Location;
                    panelWhiteboard.Invalidate();
                }
            }
            // phần resize/move ảnh không thay đổi
            else if (isResizingImage)
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
        }



        private void SendDrawCommand(string shape, Point start, Point end, Color color, float thickness)
        {
            string message = $"DRAW;{shape};{color.ToArgb()};{thickness};{start.X};{start.Y};{end.X};{end.Y}";
            SendMessage(message);
        }


        private void panelWhiteboard_MouseUp(object sender, MouseEventArgs e)
        {
            if (isDrawing)
            {
                isDrawing = false;

                if (currentMode != DrawingMode.FreeHand)
                {
                    using (Graphics g = Graphics.FromImage(drawingBitmap))
                    {
                        Pen pen = new Pen(currentColor, penThickness);

                        switch (currentMode)
                        {
                            case DrawingMode.Line:
                                g.DrawLine(pen, startPoint, e.Location);
                                break;
                            case DrawingMode.Rectangle:
                                g.DrawRectangle(pen, GetRectangleFromPoints(startPoint, e.Location));
                                break;
                            case DrawingMode.Ellipse:
                                g.DrawEllipse(pen, GetRectangleFromPoints(startPoint, e.Location));
                                break;
                        }
                    }

                    // gửi hình vẽ đến server
                    SendDrawCommand(currentMode.ToString(), startPoint, e.Location, currentColor, penThickness);
                    panelWhiteboard.Invalidate();
                }
            }

            isResizingImage = false;
            isMovingImage = false;
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
                graphics.DrawImage(drawingBitmap, 0, 0);

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

        private void btnIncreaseThickness_Click(object sender, EventArgs e)
        {
            if (penThickness < numericUpDownThickness.Maximum)
            {
                penThickness++;
                numericUpDownThickness.Value = penThickness;  // Cập nhật thanh số, đồng thời gọi event ValueChanged
            }
        }

        private void btnDecreaseThickness_Click(object sender, EventArgs e)
        {
            if (penThickness > numericUpDownThickness.Minimum)
            {
                penThickness--;
                numericUpDownThickness.Value = penThickness;  // Cập nhật thanh số, đồng thời gọi event ValueChanged
            }
        }

        private void panelWhiteboard_Paint(object sender, PaintEventArgs e)
        {
            lock (drawingBitmap)
            {
                e.Graphics.Clear(Color.White);
                e.Graphics.DrawImageUnscaled(drawingBitmap, Point.Empty);

                if (isDrawing && currentMode != DrawingMode.FreeHand)
                {
                    using (Pen pen = new Pen(currentColor, penThickness))
                    {
                        Rectangle rect = GetRectangleFromPoints(startPoint, currentPoint);
                        switch (currentMode)
                        {
                            case DrawingMode.Line:
                                e.Graphics.DrawLine(pen, startPoint, currentPoint);
                                break;
                            case DrawingMode.Rectangle:
                                e.Graphics.DrawRectangle(pen, rect);
                                break;
                            case DrawingMode.Ellipse:
                                e.Graphics.DrawEllipse(pen, rect);
                                break;
                        }
                    }
                }

                // Vẽ ảnh nếu có
                if (currentImage != null && currentImageRect != Rectangle.Empty)
                {
                    e.Graphics.DrawImage(currentImage, currentImageRect);
                    e.Graphics.FillRectangle(Brushes.Gray,
                        currentImageRect.Right - resizeHandleSize,
                        currentImageRect.Bottom - resizeHandleSize,
                        resizeHandleSize, resizeHandleSize);
                }
            }
        }




        private void ClientForm_Load(object sender, EventArgs e)
        {
            comboBoxDrawMode.Items.Clear(); // XÓA TẤT CẢ TRƯỚC KHI THÊM MỚI

            comboBoxDrawMode.Items.Add("FreeHand");
            comboBoxDrawMode.Items.Add("Line");
            comboBoxDrawMode.Items.Add("Rectangle");
            comboBoxDrawMode.Items.Add("Ellipse");

            comboBoxDrawMode.SelectedIndex = 0; // Chọn mục đầu tiên mặc định

            panelWhiteboard.Paint += panelWhiteboard_Paint;
        }


        private void ComboBoxDrawMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedMode = comboBoxDrawMode.SelectedItem.ToString();

            switch (selectedMode)
            {
                case "FreeHand":
                    currentMode = DrawingMode.FreeHand;
                    break;
                case "Line":
                    currentMode = DrawingMode.Line;
                    break;
                case "Rectangle":
                    currentMode = DrawingMode.Rectangle;
                    break;
                case "Ellipse":
                    currentMode = DrawingMode.Ellipse;
                    break;
                default:
                    currentMode = DrawingMode.FreeHand;
                    break;
            }
        }


        private void chkEraser_CheckedChanged(object sender, EventArgs e)
        {
            if (chkEraser.Checked)
            {
                previousColor = currentColor;  // Lưu màu hiện tại
                currentColor = Color.White;    // Màu nền để tẩy
                currentMode = DrawingMode.FreeHand; // Chuyển sang chế độ vẽ FreeHand
            }
            else
            {
                // Quay về màu vẽ bình thường, ví dụ màu đã chọn trước đó (có thể lưu trong biến khác)
                currentColor = previousColor; // biến lưu màu trước khi bật eraser
            }   
        }
    }
}
