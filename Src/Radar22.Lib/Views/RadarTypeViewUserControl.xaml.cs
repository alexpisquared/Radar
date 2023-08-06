namespace xEnvtCanRadar.Views;

public partial class RadarTypeViewUserControl : UserControl
{
  readonly Bpr bpr = new();
  const string _urlRoot = "https://dd.meteo.gc.ca/radar/";
  const double _fpsPeriod = .04;
  const int _pauseFrames = 25, _maxFrames = 25 * 60; // prevent from running forever 
  CancellationTokenSource? _cts;
  bool _loaded, isPlaying;
  public RadarTypeViewUserControl() => InitializeComponent();
  public static readonly DependencyProperty ScaleFactorProperty = DependencyProperty.Register("ScaleFactor", typeof(double), typeof(RadarTypeViewUserControl), new PropertyMetadata(1.0)); public double ScaleFactor { get => (double)GetValue(ScaleFactorProperty); set => SetValue(ScaleFactorProperty, value); }

  async void OnReload(object s, RoutedEventArgs e)
  {
    var riis0 = await ReLoad(int.Parse(((FrameworkElement)s).Tag?.ToString() ?? "11"));
    _loaded = true;
    chkIsPlaying.IsChecked = true;

    if (riis0.Count > 1)
    {
      await Task.Delay(2500);
      await PseudoChart(riis0);
    }
  } // max is 480 == 2 days on 10 per hour basis.
  async Task<List<RadarImageInfo>> ReLoad(int takeLastCount)
  {
    var riis1 = new List<RadarImageInfo>();

    if (DesignerProperties.GetIsInDesignMode(this)) return riis1; //tu: design mode

    chkIsPlaying.IsChecked = false;

    await Task.Delay(TimeSpan.FromSeconds(_fpsPeriod));
    var sw = Stopwatch.GetTimestamp();
    var ff = new WebDirectoryLoader();

    try
    {
      if (DateTime.Today.Month > 3)
        PreciTp = PreciTp.Replace("SNOW", "RAIN");

      var riis0 = await ff.ParseFromHtmlUsingRegex($"{_urlRoot}{UrlSuffix}", PreciTp, takeLastCount);
      lbxAllPics.Items.Clear();

      if (riis0.Count < 1)
      {
        Debug.WriteLine($"No files found for {_urlRoot}{UrlSuffix} * {PreciTp}"); // _ = MessageBox.Show($"No files found for {_urlRoot}{UrlSuffix} * {PreciTp}", "Error88a", MessageBoxButton.OK, MessageBoxImage.Error);
      }
      else
      {
        riis0.ForEach(rii0 =>
        {
          riis1.Add(rii0);
          _ = lbxAllPics.Items.Add(rii0);
        });
      }

      chkIsPlaying.Content = $"_{UrlSuffix}   {riis0.Count} imgs   {riis1.First().ImgTime:ddd HH:mm}÷{riis1.Last().ImgTime:HH:mm}   {Stopwatch.GetElapsedTime(sw).TotalSeconds:N1}s   {ff.CalulateSlope(riis1):N2}↕";

      ScaleFactor = AutoScale ? (ff.CalulateAvgSize(riis1) - 10) * .15 : 1; // 13÷-35 => .3÷2.5
    }
    catch (Exception ex) { bpr.Error(); if (Debugger.IsAttached) Debugger.Break(); else _ = MessageBox.Show(ex.Message, "Error888", MessageBoxButton.OK, MessageBoxImage.Error); }
    finally { }

    return riis1;
  }

  async Task PseudoChart(List<RadarImageInfo> riis0)
  {
    var rp = "       0.1*mm/h \r\n";
    foreach (var rii0 in riis0.TakeLast(21))
    {
      var tlcl = rii0.ImgTime.ToLocalTime();
      var cmph = await PicMea.CalcMphInTheAreaAsync(rii0.GifUrl);
      rp += $" {tlcl,5:H:mm}{(int)(cmph*100),6} {new string(' ', (int)(10 * cmph))}■ \r\n";
    }

    lblTL.Text = rp;
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

        lbxAllPics.SelectedIndex = c >= lbxAllPics.Items.Count ? lbxAllPics.Items.Count : c;

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
    using var timer = new PeriodicTimer(TimeSpan.FromSeconds(_fpsPeriod));
    await RunTimer(timer);
  }
  async void chkIsPlaying_Unchecked(object sender, RoutedEventArgs e) { WriteLine($"-- Cancelling   {UrlSuffix}."); try { _cts?.Cancel(); } catch (Exception ex) { WriteLine($"-- {ex.GetType().Name}: \t{ex.Message}.   "); await bpr.ErrorAsync(); } }
  void lbxAllPics_GotFocus(object sender, RoutedEventArgs e) => chkIsPlaying.IsChecked = false;
  void lbxAllPics_SelectionChanged(object sender, SelectionChangedEventArgs e) { }
  void lbxAllPics_LostFocus(object sender, RoutedEventArgs e) { }
}