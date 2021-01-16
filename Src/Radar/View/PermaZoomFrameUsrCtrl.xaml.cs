using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Radar.View
{
  public partial class PermaZoomFrameUsrCtrl : UserControl
  {
    public PermaZoomFrameUsrCtrl()
    {
      InitializeComponent();
      DataContext = this;
    }
    public static readonly DependencyProperty ImageURLProperty = DependencyProperty.Register("ImageURL", typeof(string), typeof(PermaZoomFrameUsrCtrl), new PropertyMetadata("https://weather.gc.ca/data/satellite/goes_ecan_vvi_100.jpg")); public string ImageURL { get { return (string)GetValue(ImageURLProperty); } set { SetValue(ImageURLProperty, value); } }
  }
}
