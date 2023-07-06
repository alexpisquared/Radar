using AmbienceLib;
using Radar22.Lib.Logic;

namespace xEnvtCanRadar.Views;

public partial class RadarTypeViewUserControl : UserControl
{
  readonly Bpr bpr = new();
  const string _urlRoot = "https://dd.meteo.gc.ca/radar/";
  const double _fpsPeriod = .04;
  const int _pauseFrames = 25, _maxFrames = 25 * 60; // prevent from running forever 
  CancellationTokenSource? _cts;
  bool _loaded, isPlaying;
  public RadarTypeViewUserControl()
  {
    InitializeComponent();
  }

  async void OnReload(object s, RoutedEventArgs e) { await ReLoad(int.Parse(((FrameworkElement)s).Tag?.ToString() ?? "11")); _loaded = true; chkIsPlaying.IsChecked = true; } // max is 480 == 2 days on 10 per hour basis.
  async Task ReLoad(int takeLastCount)
  {
    chkIsPlaying.IsChecked = false;

    await Task.Delay(TimeSpan.FromSeconds(_fpsPeriod));

    try
    {
      if (DateTime.Today.Month > 3)
        PreciTp = PreciTp.Replace("SNOW", "RAIN");

      var gifurls = await new WebDirectoryLoader().ParseFromHtmlUsingRegex($"{_urlRoot}{UrlSuffix}", PreciTp, takeLastCount);

      var list = new List<RI>();
      lbxAllPics.Items.Clear();

      if (gifurls.Count < 1)
      {
        MessageBox.Show($"No files found for {_urlRoot}{UrlSuffix} * {PreciTp}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        Debug.WriteLine($"No files found for {_urlRoot}{UrlSuffix} * {PreciTp}");
      }
      else
        gifurls.ForEach(imgFile =>
        {
          var r = new RI { GifUrl = $"{_urlRoot}{UrlSuffix}/{imgFile}", FileName = Path.GetFileNameWithoutExtension(imgFile) };
          list.Add(r);
          lbxAllPics.Items.Add(r);
        });

      chkIsPlaying.Content = $"_{UrlSuffix}      {gifurls.Count} files      {list.First().ImgTime:ddd HH:mm} ÷ {list.Last().ImgTime:ddd HH:mm}";

      //await Task.Delay(1000);
      //using var timer = new PeriodicTimer(TimeSpan.FromSeconds(_fpsPeriod));
      //await RunTimer(timer);
    }
    catch (Exception ex) { bpr.Error(); MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error); }
    finally { }
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