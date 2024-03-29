﻿using StandardLib.Extensions;

namespace Radar.View;

public partial class PermaZoomFrameUsrCtrl
{
  public PermaZoomFrameUsrCtrl()
  {
    InitializeComponent();
    DataContext = this;
  }
  public static readonly DependencyProperty ImageURLProperty = DependencyProperty.Register("ImageURL", typeof(string), typeof(PermaZoomFrameUsrCtrl), new PropertyMetadata("https://weather.gc.ca/data/satellite/goes_ecan_1070_100.jpg")); public string ImageURL { get => (string)GetValue(ImageURLProperty); set => SetValue(ImageURLProperty, value); }
  public static readonly DependencyProperty AnimeURLProperty = DependencyProperty.Register("AnimeURL", typeof(string), typeof(PermaZoomFrameUsrCtrl), new PropertyMetadata("https://weather.gc.ca/data/satellite/goes_ecan_1070_100.jpg")); public string AnimeURL { get => (string)GetValue(AnimeURLProperty); set => SetValue(AnimeURLProperty, value); }

  void Hyperlink_RequestNavigate(object s, System.Windows.Navigation.RequestNavigateEventArgs e)
  {
    e.Handled = true;
    Debug.WriteLine(e.Uri.AbsoluteUri);
    try
    {
      _ = Process.Start(
        new ProcessStartInfo(e.Uri.AbsoluteUri)
        {
          UseShellExecute = true,
          Verb = "open"
        });
    }
    catch (Exception ex) { _ = ex.Log(); }
  }
}
