using System.Windows;
using System.Windows.Controls;

namespace Radar.View
{
  public partial class PermaZoomFrameUsrCtrl : UserControl
  {
    public PermaZoomFrameUsrCtrl()
    {
      InitializeComponent();
      DataContext = this;
    }
    public static readonly DependencyProperty ImageURLProperty = DependencyProperty.Register("ImageURL", typeof(string), typeof(PermaZoomFrameUsrCtrl), new PropertyMetadata("https://weather.gc.ca/data/satellite/goes_ecan_vvi_100.jpg")); public string ImageURL { get => (string)GetValue(ImageURLProperty); set => SetValue(ImageURLProperty, value); }
  }
}
