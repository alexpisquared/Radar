using RadarPicCollect;
using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Radar
{
  public partial class BitmapForWpfHelperWayWindow : Window
  {
    RadarPicCollector _radarPicCollector = new RadarPicCollector();
    Image _image = new Image();
    int _curPicIdx = 0;

    public BitmapForWpfHelperWayWindow()
    {
      InitializeComponent();

      Content = _image;

      initPics();
    }

    async void initPics()
    {
      _radarPicCollector.DownloadRadarPicsNextBatch();

      _image.Source = await BitmapForWpfHelper.BitmapToBitmapSource(_radarPicCollector.Pics[_curPicIdx].Bitmap);
      _image.Width = 580;
      _image.Height = 480;
    }
    void showPrev() { showPicX(--_curPicIdx < 0 ? _radarPicCollector.Pics.Count - 1 : _curPicIdx); }      //showPicFromSquence(--_curPicIdx < 0 ? 0 : _curPicIdx);
    void showNext() { showPicX(++_curPicIdx >= _radarPicCollector.Pics.Count - 1 ? _radarPicCollector.Pics.Count - 1 : _curPicIdx); }     //showPicFromSquence(++_curPicIdx >= wpc.Pics.Count ? 0 : _curPicIdx);
    async void showPicX(int x)
    {
      try
      {
        _curPicIdx = x;

        _image.Source = await BitmapForWpfHelper.BitmapToBitmapSource(_radarPicCollector.Pics[_curPicIdx].Bitmap);

        Title = string.Format("{0}  {1,2}/{2}  ({3}-{4}={5:N1})   {6}",
          ((PicDetail)_radarPicCollector.Pics[x]).ImageTime.ToString("ddd HH:mm"),
          x + 1, _radarPicCollector.Pics.Count,
          ((PicDetail)_radarPicCollector.Pics[x]).ImageTime.ToString("ddd HH:mm"),
          ((PicDetail)_radarPicCollector.Pics[_radarPicCollector.Pics.Count - 1]).ImageTime.ToString("HH:mm"),
          new TimeSpan(((PicDetail)_radarPicCollector.Pics[_radarPicCollector.Pics.Count - 1]).ImageTime.Ticks - ((PicDetail)_radarPicCollector.Pics[x]).ImageTime.Ticks).TotalHours,
          RadarPicCollector.RainOrSnow);
      }
      catch (Exception ex)
      {
        Title = ex.Message;
      }
    }
    void processKeyPress(KeyEventArgs e)
    {
      switch (e.Key)
      {
        default: Debug.WriteLine(string.Format(">>Unhandled key: {0}", e.Key)); break;
        case Key.F5: Title = _radarPicCollector.DownloadRadarPicsNextBatch(); break;

        case Key.Home: showPicX(0); break;
        case Key.End: showPicX(_radarPicCollector.Pics.Count - 1); break;

        case Key.Up:
        case Key.Left: showPrev(); break;

        case Key.Down:
        case Key.Right: showNext(); break;
        case Key.Tab: return;
        case Key.Escape: return;
      }
    }

    private void Window_KeyUp(object sender, KeyEventArgs e)
    {
    }
    private void Window_KeyDown(object sender, KeyEventArgs e)
    {
      processKeyPress(e);
    }
  }
}
