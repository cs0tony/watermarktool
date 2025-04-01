using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;
using System.Windows.Forms;

namespace watermarktool
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public string CurrentTime { get; set; }
        private const int GWL_EXSTYLE = -20;
        private const int WS_EX_TRANSPARENT = 0x00000020;
        private const int WS_EX_LAYERED = 0x00080000;
        [DllImport("user32.dll")]
        private static extern int GetWindowLong(IntPtr hwnd, int index);

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hwnd, int index, int newStyle);
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            this.Opacity = 0.2;
            this.Width = Screen.PrimaryScreen.Bounds.Width;
            this.Height = Screen.PrimaryScreen.Bounds.Height;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            IntPtr hwnd = new WindowInteropHelper(this).Handle;
            int extendedStyle = GetWindowLong(hwnd, GWL_EXSTYLE);
            SetWindowLong(hwnd, GWL_EXSTYLE, extendedStyle | WS_EX_LAYERED | WS_EX_TRANSPARENT);
            GenerateWatermarks();
        }

        private DispatcherTimer timer;

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            if (timer != null)
            {
                timer.Stop();
                timer = null;
            }
        }

        private double verticalSpacing = 400;
        private double horizontalSpacing = 400;
        private FontFamily fontFamily = new FontFamily("Arial");
        private double fontSize = 22;
        private Brush textBrush = new SolidColorBrush(Color.FromRgb(125, 125, 125));
        private double angle = -45;

        public void SetAngle(double angle)
        {
            this.angle = angle;
            int count = WatermarkCanvas.Children.Count;
            for (int i = 0; i < count; i++)
            {
                ((TextBlock)WatermarkCanvas.Children[i]).RenderTransform = new RotateTransform(angle);
            }
        }

        private void GenerateWatermarks()
        {
            // 配置水印文本的行间距和列间距

            string watermarkText = $"{Environment.MachineName}  " + DateTime.Now.ToString("yyyyMMdd HH:mm");

            // 配置字体和颜色

            PlaceWatermarks(watermarkText);

            // 创建Timer更新水印时间
            timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(1000)
            };
            timer.Tick += (sender, e) =>
            {
                DateTime currentDateTime = DateTime.Now;
                int seconds = currentDateTime.Second;
                int secondsToNextMinutes = 60 - seconds;
                timer.Interval = TimeSpan.FromMilliseconds(secondsToNextMinutes*1000);
                watermarkText = $"{Environment.MachineName}  " + currentDateTime.ToString("yyyyMMdd HH:mm");
                int count = WatermarkCanvas.Children.Count;
                for (int i = 0; i < count; i++)
                {
                    ((TextBlock)WatermarkCanvas.Children[i]).Text = watermarkText;
                }
            };
            timer.Start();
        }

        private void PlaceWatermarks(string text)
        {
            // 获取窗口尺寸
            double windowWidth = this.ActualWidth;
            double windowHeight = this.ActualHeight;

            // 根据行间距和列间距重复绘制文本
            for (double y = -verticalSpacing; y < windowHeight + verticalSpacing; y += verticalSpacing)
            {
                for (double x = -horizontalSpacing; x < windowWidth + horizontalSpacing; x += horizontalSpacing)
                {
                    TextBlock textBlock = new TextBlock
                    {
                        Text = text,
                        FontFamily = fontFamily,
                        FontSize = fontSize,
                        FontWeight = FontWeights.Bold,
                        Foreground = textBrush,
                        RenderTransform = new RotateTransform(angle)  // 旋转角度
                    };

                    // 设置TextBlock的位置
                    Canvas.SetLeft(textBlock, x);
                    Canvas.SetTop(textBlock, y);

                    // 添加到Canvas
                    WatermarkCanvas.Children.Add(textBlock);
                }
            }
        }
    }
}