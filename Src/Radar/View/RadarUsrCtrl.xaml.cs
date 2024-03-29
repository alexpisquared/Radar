﻿using StandardLib.Extensions;
using StandardLib.Helpers;

namespace Radar;

public partial class RadarUsrCtrl
{
  delegate void NoArgDelegate();
  delegate void IntArgDelegate(int stationIndex);
  delegate void OneArgDelegate(string title);
  readonly bool _isStandalole = true;
  readonly RadarPicCollector _radarPicCollector = new();
  readonly DispatcherTimer _animation_Timer = new(), _picIndex__Timer = new(), _getFromWebTimer = new();
  DateTime _curImageTime;
  bool _isAnimated = true, _forward = true, _isSpeedMeasuring = false;
  int _fwdPace = 128, _curPicIdx = 0, _animationLength = 151;
  const int _bakPace = 1, _pause500ms = 500;

  public RadarUsrCtrl()
  {
    InitializeComponent();

    DataContext = this;

    _picIndex__Timer.Interval = TimeSpan.FromMilliseconds(20);
    _animation_Timer.Interval = TimeSpan.FromMilliseconds(_fwdPace);
    _getFromWebTimer.Interval = TimeSpan.FromMilliseconds(30); //get rigth away, then reget every 5 min.
    _picIndex__Timer.Tick += async (s, e) => await picIndex__Timer_Tick(s, e); // new EventHandler(picIndex__Timer_TickAsync);
    _animation_Timer.Tick += async (s, e) => await dTimerAnimation_Tick(s, e); // new EventHandler(dTimerAnimation_Tick);
    _getFromWebTimer.Tick += async (s, e) => await fetchFromWeb____Tick(s, e); // new EventHandler(fetchFromWeb____Tick);
    _getFromWebTimer.Start();
    _animation_Timer.Start();

    //KeyUp += (s, e) => OnKeyDown__(e.Key);
    MouseWheel += async (s, e) => { if (e.Delta > 0) await showNextAsync(); else await showPrevAsync(); };

    tbBuildTime.Header = """VerHelper.CurVerStr("");""";

    _ = keyFocusBtn.Focus();
  }
  public double AlarmThreshold { set => tbt.Text = string.Format("AlarmThreshold: {0:N2}", value); }
  async void onKeyDownAsync(object s, System.Windows.Input.KeyEventArgs e) => await OnKeyDown__Async(e.Key);
  async Task moveTimeAsync(int timeMin)
  {
    _animation_Timer.Stop();

    var time = _curImageTime.AddMinutes(timeMin);

    if (time > DateTime.Now)
      return;

    _curImageTime = time;

    var idx = _radarPicCollector.IdxTime(_curImageTime);
    if (idx < 0)
    {
      _ = updateClock(_curImageTime);
      await fetchFromWebBegin();
    }
    else
      await showPicX(idx);
  }
  async Task showPrevAsync() { _animation_Timer.Stop(); await showPicX(--_curPicIdx); }
  async Task showNextAsync() { _animation_Timer.Stop(); await showPicX(++_curPicIdx); }
  async Task showPicX(int idx)
  {
    try
    {
      _curPicIdx = idx < 0 ? 0 : idx >= _radarPicCollector.Pics.Count ? _radarPicCollector.Pics.Count - 1 : idx;

      if (_radarPicCollector.Pics.Count < 1) return;

      _curImageTime = _radarPicCollector.Pics[_curPicIdx].ImageTime;

      _image.Source = await BitmapForWpfHelper.BitmapToBitmapSource(_radarPicCollector.Pics[_curPicIdx].Bitmap);
      if (_image.Source == null)
      {
        await Task.Delay(9);
        return;
      }

      //if (_radarPicCollector.Pics[_curPicIdx].PicOffset.X != 0)
      {
        _image.SetValue(Canvas.LeftProperty, (double)_radarPicCollector.Pics[_curPicIdx].PicOffset.X);
        _image.SetValue(Canvas.TopProperty, (double)_radarPicCollector.Pics[_curPicIdx].PicOffset.Y);
      }

      LTitle.Text = $"{_radarPicCollector.Pics[_curPicIdx].ImageTime:HH:mm}    {_curPicIdx + 1,2} / {_radarPicCollector.Pics.Count}     {RadarPicCollector.RainOrSnow}";

      //RMeasr.Text = _radarPicCollector.Pics[_curPicIdx].Measure.ToString("N3");

      var t = updateClock(_radarPicCollector.Pics[_curPicIdx].ImageTime);
    }
    catch (Exception ex) { LTitle.Text = ex.Message; }
  }
  DateTime updateClock(DateTime t)
  {
    hourHandTransform.Angle = (t.Hour * 30) + (t.Minute / 2) - 180;
    minuteHandTransform.Angle = (t.Minute * 6) - 180;

    var mySolidColorBrush = new System.Windows.Media.SolidColorBrush();
    var b = (byte)(128 - 127.0 * Math.Cos((hourHandTransform.Angle + 120) * 3.141 / 360));
    mySolidColorBrush.Color = System.Windows.Media.Color.FromRgb(b, b, b);

    ClockFace.Fill = mySolidColorBrush;

    //Debug.WriteLine(string.Format("h:{0} angle:{1} rad:{2} Sin:{3} Byte:{4}",
    //    t.Hour,
    //    hourHandTransform.Angle,
    //    (3.141 * hourHandTransform.Angle / 180),
    //    Math.Sin(3.141 * hourHandTransform.Angle / 180),
    //    b));

    return t;
  }
  public async Task OnKeyDown__Async(Key key)
  {
    switch (key)
    {
      default: LTitle.Text = string.Format(">>Unhandled key: {0}", key); break;

      case Key.NumPad0:
      case Key.D0: _animationLength = _radarPicCollector.Pics.Count; break;
      case Key.NumPad1:
      case Key.D1: _animationLength = 7; break;
      case Key.NumPad2:
      case Key.D2: _animationLength = 13; break;
      case Key.NumPad3:
      case Key.D3: _animationLength = 19; break;
      case Key.NumPad4:
      case Key.D4: _animationLength = 25; break;
      case Key.NumPad5:
      case Key.D5: _animationLength = 31; break;
      case Key.NumPad6:
      case Key.D6: _animationLength = 37; break;
      case Key.NumPad7:
      case Key.D7: _animationLength = 43; break;
      case Key.NumPad8:
      case Key.D8: _animationLength = 49; break;
      case Key.NumPad9:
      case Key.D9: _animationLength = 55; break;
      case Key.Space: toggleAnimation(); break;
      case Key.M: speedMeasure(); break;
      case Key.R: RadarPicCollector.RainOrSnow = "RAIN"; _ = _radarPicCollector.DownloadRadarPics(); /*Bpr.BeepClk()*/; break;
      case Key.S: RadarPicCollector.RainOrSnow = "SNOW"; _ = _radarPicCollector.DownloadRadarPics(); /*Bpr.BeepClk()*/; break;

      case Key.Add: _fwdPace /= 2; if (_fwdPace == 0) _fwdPace = 1; _animation_Timer.Interval = TimeSpan.FromMilliseconds(_fwdPace); break;
      case Key.Subtract: _fwdPace *= 2; _animation_Timer.Interval = TimeSpan.FromMilliseconds(_fwdPace); break;

      case Key.F5: await fetchFromWebBegin(); break;
      case Key.F6: setNewImageSource(@"/Radar;component/WKR_roads.gif"); break;// C:\0\0\web\Radar\WKR_roads.gif"); break; //King (default)
      case Key.F7: setNewImageSource(@"C:\0\0\web\Radar\WSO_roads.gif"); break; //London
      //case Key.F8: _imageRoads.Visibility = _imageRoads.Visibility == Visibility.Hidden ? Visibility.Visible : Visibility.Hidden; break;

      case Key.Home: await showPicX(0); break;
      case Key.End: await showPicX(_radarPicCollector.Pics.Count - 1); break;

      case Key.Down: await moveTimeAsync(10); break;
      case Key.Up: await moveTimeAsync(-10); break;

      case Key.Right:
      case Key.OemPeriod: await showNextAsync(); break;
      case Key.Left:
      case Key.OemComma: await showPrevAsync(); break;

      case Key.Tab: return;
      case Key.Escape: if (_isStandalole) WpfUtils.FindParentWindow(this)?.Close(); else WpfUtils.FindParentWindow(this)?.Hide(); break;
      case Key.Delete: LTitl2.Text = deleteOldSmallImages(OneDrive.WebCacheFolder); break;
    }
  }
  string deleteOldSmallImages(string path, int deleteLessThanBytes = 25000)
  {
    try
    {
      var di = new DirectoryInfo(Path.Combine(path, "weather.gc.ca"));
      var oldSmallPics = di.GetFiles("*.GIF").Where(fi => fi.Length < deleteLessThanBytes && DateTime.Now - fi.LastWriteTime > TimeSpan.FromDays(2)).ToList();
      var report = $"Deleting {oldSmallPics.Count} / {di.GetFiles("*.GIF").Length:N0}  smaller  {deleteLessThanBytes:N0} bytes... \n";
      try { oldSmallPics.ForEach(fi => File.Delete(fi.FullName)); } catch (Exception ex) { report += ex.Log(); }
      return report;
    }
    catch (Exception ex) { return ex.Log(); }
  }
  void setNewImageSource(string s)
  {
    try
    {
      var bi3 = new BitmapImage();
      bi3.BeginInit();
      bi3.UriSource = new Uri(s);
      bi3.EndInit();
      //_imageRoads.Source = bi3;

    }
    catch (Exception ex)
    {
      LTitle.Text = ex.Message;
    }
  }
  void showListboxSelectedImage(object s, SelectionChangedEventArgs args) //tu: !!!This way is less memory used and faster released.
  {
    var list = ((ListBox)s);
    if (list == null)
      return;

    if (list.SelectedIndex < 0)
      return;

    var selection = list.SelectedItem.ToString();
    if (string.IsNullOrEmpty(selection))
      return;

    var bi = new BitmapImage(new Uri(selection));
    _image.Source = bi;
    LTitle.Text = string.Format("{0}x{1} {2}", bi.PixelWidth, bi.PixelHeight, bi.Format);
  }
  List<MyImage> AllImages(string dir)
  {
    var result = new List<MyImage>();
    //reach (string filename in Directory.GetFiles(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures)))//TU: Environment.SpecialFolder...
    foreach (var filename in Directory.GetFiles(System.IO.Path.Combine(dir, "---www.weatheroffice.gc.ca-data-radar-temp_image*.Gif")))
    {
      try
      {
        result.Add(new MyImage(new BitmapImage(new Uri(filename)), System.IO.Path.GetFileNameWithoutExtension(filename)));
      }
      catch (Exception ex) { System.Diagnostics.Trace.WriteLine(ex.Message, System.Reflection.MethodInfo.GetCurrentMethod()?.Name); if (System.Diagnostics.Debugger.IsAttached) System.Diagnostics.Debugger.Break(); throw; }
    }
    return result;
  }
  void toggleAnimation()
  {
    _isAnimated = !_isAnimated;
    _animation_Timer.IsEnabled = _isAnimated;
  }
  void speedMeasure() { _isSpeedMeasuring = !_isSpeedMeasuring; _picIndex__Timer.IsEnabled = _isSpeedMeasuring; }
  async Task dTimerAnimation_Tick(object? s, EventArgs e)
  {
    if (_forward)
    {
      if (_animation_Timer.Interval == TimeSpan.FromMilliseconds(_pause500ms))
        _animation_Timer.Interval = TimeSpan.FromMilliseconds(_fwdPace);

      if (++_curPicIdx >= _radarPicCollector.Pics.Count - 1)
      {
        _forward = false;
        _animation_Timer.Interval = TimeSpan.FromMilliseconds(_pause500ms);
      }
    }
    else
    {
      if (_animation_Timer.Interval == TimeSpan.FromMilliseconds(_pause500ms))
        _animation_Timer.Interval = TimeSpan.FromMilliseconds(_bakPace);

      _curPicIdx -= 2;// -= 32 / fwdPace;
      if (_curPicIdx < _radarPicCollector.Pics.Count + 1 - _animationLength)
      {
        _forward = true;
        _animation_Timer.Interval = TimeSpan.FromMilliseconds(_pause500ms);
        Process.GetCurrentProcess().MinWorkingSet = Process.GetCurrentProcess().MinWorkingSet; //tu: Finally we found a quite simple solution. When closing our window and minimize the application to tray we free memory with the following line:
      }
    }
    await showPicX(_curPicIdx);
  }
  async Task picIndex__Timer_Tick(object? s, EventArgs e)
  {
    _curPicIdx = _curPicIdx == _radarPicCollector.Pics.Count - 1
      ? _radarPicCollector.Pics.Count - _animationLength
      : _radarPicCollector.Pics.Count - 1;

    await showPicX(_curPicIdx);
  }
  async Task fetchFromWeb____Tick(object? s, EventArgs e)
  {
    _getFromWebTimer.Stop();
    _getFromWebTimer.Interval = TimeSpan.FromMinutes(5); //reget every 5 min
    await fetchFromWebBegin();
  }
  async Task fetchFromWebBegin()
  {
    btnSnow.IsEnabled = !(btnRain.IsEnabled = (RadarPicCollector.RainOrSnow != "RAIN"));
    LTitle.Text = "Going for it....";
    MainCanvas.Background = System.Windows.Media.Brushes.DarkKhaki;

    for (var stationIndex = 0; stationIndex < _radarPicCollector.StationCount - 1; stationIndex++)
    {
      //new IntArgDelegate(fetchFromWebBlockingCall_FetchRad).BeginInvoke(i, null, null);
      _ = _radarPicCollector.DownloadRadarPicsNextBatch(stationIndex);
      await updateUIAsync("Not sure ...");
    }

    _ = keyFocusBtn.Focus();
  }
  void fetchFromWebBlockingCall_FetchRad(int stationIndex) =>
    MainCanvas.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new OneArgDelegate(updateUI_), _radarPicCollector.DownloadRadarPicsNextBatch(stationIndex));
  async void updateUI_(string title) => await updateUIAsync(title);
  async Task updateUIAsync(string title)
  {
    _animationLength = _radarPicCollector.Pics.Count - 1;

    await OnKeyDown__Async(Key.End);
    await OnKeyDown__Async(Key.NumPad3);
    LTitle.Text += title;
    _getFromWebTimer.Start();

    //Bpr.Beep2of2();
  }
  void onPopupTpl(object s, RoutedEventArgs e) { }//new RadarTpl.MainWindow().Show(); }
  async void UserControl_Loaded(object sender, RoutedEventArgs e)
  {
    LTitl2.Text = "° ° °";
    await Task.Delay(5000);
    LTitl2.Text = deleteOldSmallImages(OneDrive.WebCacheFolder);
    LTitle.Foreground =
    LTitl2.Foreground = EnvCanRadarUrlHelper.IsDark ? System.Windows.Media.Brushes.White : System.Windows.Media.Brushes.DarkBlue;
  }
  async void onRain(object s, RoutedEventArgs e) { RadarPicCollector.RainOrSnow = "RAIN"; await fetchFromWebBegin(); }
  async void onSnow(object s, RoutedEventArgs e) { RadarPicCollector.RainOrSnow = "SNOW"; await fetchFromWebBegin(); }
  async void onF5(object s, RoutedEventArgs e) => await fetchFromWebBegin();
  async void onKeyDown__(object s, KeyEventArgs e) => await OnKeyDown__Async(e.Key);
  async void keyFocusBtn_ClickAsync(object s, System.Windows.RoutedEventArgs e) => await OnKeyDown__Async(Key.Space);
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
    catch (Exception ex) { LTitle.Text = ex.Message; }
  }
}

public class UriToBitmapConverter : IValueConverter
{
  public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
  {
    var bi = new BitmapImage();
    bi.BeginInit();
    bi.DecodePixelWidth = 100;
    bi.CacheOption = BitmapCacheOption.OnLoad;
    bi.UriSource = new Uri(value?.ToString() ?? "");
    bi.EndInit();
    return bi;
  }
  public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) => throw new Exception("The method or operation is not implemented.");
}

public static class WpfUtils
{
  public static Window? FindParentWindow(FrameworkElement? element)
  {
    if (element?.Parent == null) return null;

    if (element.Parent as Window != null) return element.Parent as Window;

    return element.Parent as FrameworkElement != null ? FindParentWindow(element.Parent as FrameworkElement) : null;
  }
}
