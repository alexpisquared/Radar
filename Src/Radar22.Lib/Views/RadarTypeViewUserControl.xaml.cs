namespace xEnvtCanRadar.Views;

public partial class RadarTypeViewUserControl : UserControl
{
  readonly Bpr bpr = new();
  const string _urlRoot = "https://dd.meteo.gc.ca/radar/";
  const double _periodMs = 32; // below 20 - no faster; 32 is OK.
  const int _pauseFrames = 50, _maxFrames = 25_000; // prevent from running forever 
  CancellationTokenSource? _cts;
  bool _loaded, isPlaying;
  public RadarTypeViewUserControl() => InitializeComponent();
  public static readonly DependencyProperty ScaleFactorProperty = DependencyProperty.Register("ScaleFactor", typeof(double), typeof(RadarTypeViewUserControl), new PropertyMetadata(1.0)); public double ScaleFactor { get => (double)GetValue(ScaleFactorProperty); set => SetValue(ScaleFactorProperty, value); }
  public static readonly DependencyProperty ScaleFacto_Property = DependencyProperty.Register("ScaleFacto_", typeof(double), typeof(RadarTypeViewUserControl), new PropertyMetadata(1.0)); public double ScaleFacto_ { get => (double)GetValue(ScaleFacto_Property); set => SetValue(ScaleFacto_Property, value); }

  async void OnReload(object s, RoutedEventArgs e)
  {
    var riis0 = await ReLoad(int.Parse(((FrameworkElement)s).Tag?.ToString() ?? "11"));
    _loaded = true;
    chkIsPlaying.IsChecked = true;

    if (riis0.Count > 1)
    {
      await Task.Delay(500);
      await PseudoChart(riis0);
    }
  } // max is 480 == 2 days on 10 per hour basis.
  async Task<List<RadarImageInfo>> ReLoad(int takeLastCount)
  {
    var radarImageInfoList = new List<RadarImageInfo>();

    if (DesignerProperties.GetIsInDesignMode(this)) return radarImageInfoList; //tu: design mode

    chkIsPlaying.IsChecked = false;

    await Task.Delay(TimeSpan.FromMilliseconds(_periodMs));
    var sw = Stopwatch.GetTimestamp();
    var loader = new WebDirectoryLoader();

    try
    {
      if (DateTime.Today.Month > 3)                // interesting solution
        PreciTp = PreciTp.Replace("SNOW", "RAIN"); // interesting solution

      var riis0 = await loader.ParseFromHtmlUsingRegex($"{_urlRoot}{UrlSuffix}", PreciTp, takeLastCount);
      lbxAllPics.Items.Clear();

      if (riis0.Count < 1)
      {
        Debug.WriteLine($"No files found for {_urlRoot}{UrlSuffix} * {PreciTp}"); // _ = MessageBox.Show($"No files found for {_urlRoot}{UrlSuffix} * {PreciTp}", "Error88a", MessageBoxButton.OK, MessageBoxImage.Error);
      }
      else
      {
        riis0.ForEach(rii0 =>
        {
          radarImageInfoList.Add(rii0);
          _ = lbxAllPics.Items.Add(rii0);
        });
      }

      if (radarImageInfoList.Count > 0)
      {
        chkIsPlaying.Content = $"_{UrlSuffix}   {riis0.Count} imgs   {radarImageInfoList.First().ImgTime:ddd HH:mm}÷{radarImageInfoList.Last().ImgTime:HH:mm}   {Stopwatch.GetElapsedTime(sw).TotalSeconds:N1}s   {loader.CalulateSlope(radarImageInfoList):N2}↕";
        ScaleFactor = AutoScale ? (loader.CalulateAvgSize(radarImageInfoList) - 10) * .15 : 1; // 13÷-35 => .3÷2.5
      }
      else
      {
        chkIsPlaying.Content = $"_{UrlSuffix}   ▄▀▄▀▄▀  NO RADAR IMAGES?!?!?!?  ▄▀▄▀▄▀ ";
        ScaleFactor = 1; // 13÷-35 => .3÷2.5
      }
    }
    catch (Exception ex) { bpr.Error(); if (Debugger.IsAttached) Debugger.Break(); else throw; }

    return radarImageInfoList;
  }

  async Task PseudoChart(List<RadarImageInfo> riis0)
  {
    var rp = "       0.1*mm/h \r\n";
    foreach (var rii0 in riis0.TakeLast(41))
    {
      var tlcl = rii0.ImgTime.ToLocalTime();
      var cmph = await PicMea.CalcMphInTheAreaAsync(rii0.GifUrl); // 0 ÷ 4_000
      rp += $" {tlcl,5:H:mm}{(int)(cmph * 100),6} {new string(' ', (int)(3.33 * cmph))}■ \r\n";

      ScaleFacto_ = Math.Min(3, .5 + Math.Log(1 + (cmph * 10), 2.5));
      ScaleFactor = AutoScale ? ScaleFacto_ : 1;
    }

    lblTL.Text = rp;
    Console.Beep(200, 200);
  }

  async Task RunTimer(PeriodicTimer timer)
  {
    try
    {
      var counter = 0;
      _cts = new CancellationTokenSource();
      while (await timer.WaitForNextTickAsync(_cts.Token))
      {
        if (counter > _maxFrames)
        {
          WriteLine($"-- Cancelling at counter {counter}   {UrlSuffix}.");
          chkIsPlaying.IsChecked = false;
          return;
        }

        var c = ++counter % (lbxAllPics.Items.Count + _pauseFrames);

        if (c == lbxAllPics.Items.Count && StartPlaying == "0")
          chkIsPlaying.IsChecked = false;

        if (c >= lbxAllPics.Items.Count + (_pauseFrames / 2))
          lbxAllPics.SelectedIndex = 0;
        else if (c < lbxAllPics.Items.Count)
          lbxAllPics.SelectedIndex = c;

        if (_cts.Token.IsCancellationRequested) // Poll on this property if you have to do other cleanup before throwing.
        {
          WriteLine($"-- CancellationRequested => Cancelling {UrlSuffix}.");
          // Clean up here, then...
          _cts.Token.ThrowIfCancellationRequested();
        }
      }
    }
    catch (TaskCanceledException ex) /**/ { WriteLine($"-- {ex.GetType().Name}: \t{ex.Message}.   "); await bpr.ErrorAsync(); }
    catch (OperationCanceledException ex) { WriteLine($"-- {ex.GetType().Name}: \t{ex.Message}.   "); await bpr.TickAsync(); }
    catch (Exception ex)             /**/ { WriteLine($"-- {ex.GetType().Name}: \t{ex.Message}.   "); await bpr.ErrorAsync(); }
    finally { _cts?.Dispose(); WriteLine($"-- finally      {UrlSuffix}.\n"); }
  }

  public string UrlSuffix { get; set; } = "{name}PRECIPET/GIF/WKR";
  public string PreciTp { get; set; } = "RAIN.gif";
  public string StartPlaying { get; set; } = "0";
  public bool AutoScale { get; set; } = false;
  public bool IsPlaying
  {
    get => isPlaying; set
    {
      if (isPlaying == value) return;

      isPlaying = value;
      if (value) chkIsPlaying_Checked(value, new RoutedEventArgs());
      else chkIsPlaying_Unchecked(value, new RoutedEventArgs());
    }
  }

  async void chkIsPlaying_Checked(object sender, RoutedEventArgs e)
  {
    WriteLine($"-- <<<<<<<<<    {UrlSuffix}   {(_loaded ? "Starting" : "Not yet")}.");
    if (!_loaded) return;
    StartPlaying = "1";
    using var timer = new PeriodicTimer(TimeSpan.FromMilliseconds(_periodMs));
    await RunTimer(timer);
  }
  async void chkIsPlaying_Unchecked(object sender, RoutedEventArgs e) { WriteLine($"-- Cancelling   {UrlSuffix}."); try { _cts?.Cancel(); } catch (Exception ex) { WriteLine($"-- {ex.GetType().Name}: \t{ex.Message}.   "); await bpr.ErrorAsync(); } }
  void lbxAllPics_GotFocus(object sender, RoutedEventArgs e) => chkIsPlaying.IsChecked = false;
  void lbxAllPics_SelectionChanged(object sender, SelectionChangedEventArgs e) { }
  void lbxAllPics_LostFocus(object sender, RoutedEventArgs e) { }
}


//     _ = WinAPI.PrintWindow(handle, hdc, 1); // captures a window bitmap even if the window is covered by other windows or if it is off-screen. 

