using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using PixelMeasure;

namespace PixelMeasureApp;
public partial class MainWindow : Window
{
  readonly string url = "https://dd.meteo.gc.ca/today/radar/DPQPE/GIF/CASKR/20251013T0000Z_MSC_Radar-DPQPE_CASKR_Rain-Contingency.gif";
  public MainWindow() => InitializeComponent();

  async void Image_MouseUp(object sender, MouseButtonEventArgs e)
  {
    var img = (Image)sender;

    var p = e.GetPosition(img);
    p = new Point(Math.Min(Math.Max(p.X, 40), img.ActualWidth - 40), Math.Min(Math.Max(p.Y, 40), img.ActualHeight - 40)); // safePositionNoCloserThan40ToTheimgEdges
    var cmph = await PicMea.CalcMphInTheAreaAsync(url, (int)p.X, (int)p.Y); // 0 ÷ 350
    Title = /*tbkReport.Text = */$"{p.X:N0},{p.Y:N0}\t{(int)(cmph * 100),4} |{new string('·', (int)(10 * cmph))}|";


    var bmp = new BitmapImage(new Uri(url));
    var dv = new DrawingVisual();
    using var dc = dv.RenderOpen();
    var rect = new Rect(p.X - 40, p.Y - 40, 80, 80);
    dc.DrawRectangle(Brushes.Transparent, new Pen(Brushes.Red, 1), rect);
    dc.Close();

    var target = new RenderTargetBitmap(bmp.PixelWidth, bmp.PixelHeight, bmp.DpiX, bmp.DpiY, PixelFormats.Pbgra32);
    target.Render(dv);

    // Keep original image by creating a new RenderTargetBitmap to capture the current image
    var originalBmp = new RenderTargetBitmap((int)img.ActualWidth, (int)img.ActualHeight, bmp.DpiX, bmp.DpiY, PixelFormats.Pbgra32);
    originalBmp.Render(img);

    // Combine the original image with the new bitmap generated above
    var drawingVisual = new DrawingVisual();
    using (var drawingContext = drawingVisual.RenderOpen())
    {
      drawingContext.DrawImage(originalBmp, new Rect(0, 0, originalBmp.PixelWidth, originalBmp.PixelHeight));
      drawingContext.DrawImage(target, new Rect(0, 0, target.PixelWidth, target.PixelHeight));
      drawingContext.Close();
    }

    img.Source = new RenderTargetBitmap(
        (int)img.ActualWidth, (int)img.ActualHeight,
        bmp.DpiX, bmp.DpiY, PixelFormats.Pbgra32);
    img.Source = new DrawingImage(drawingVisual.Drawing);
  }

  void OnClose(object sender, RoutedEventArgs e) => Close();
}
