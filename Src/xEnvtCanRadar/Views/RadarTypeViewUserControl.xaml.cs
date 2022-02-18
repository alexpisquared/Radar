namespace xEnvtCanRadar.Views;

public partial class RadarTypeViewUserControl : UserControl
{
  const string _urlRoot = "https://dd.meteo.gc.ca/radar/";
  const double _fpsPeriod = .04;
  const int _pauseFrames = 25;
  const int _maxFrames = 25 * 60;
  readonly CancellationTokenSource _cts = new();
  bool _loaded;

  public RadarTypeViewUserControl() => InitializeComponent();

  async void OnLoaded(object s, RoutedEventArgs e) { await ReLoad(31); _loaded = true; }
  async void OnReload(object s, RoutedEventArgs e) => await ReLoad(int.Parse(((FrameworkElement)s).Tag?.ToString() ?? "0")); // max is 480 == 2 days on 10 per hour basis.
  async Task ReLoad(int takeLastCount)
  {
    chkIsPlaying.IsChecked = false;
    await Task.Delay(TimeSpan.FromSeconds(_fpsPeriod));

    try
    {
      var gifurls = await new WebDirectoryLoader().ParseFromHtmlUsingRegex($"{_urlRoot}{UrlSuffix}", PreciTp, takeLastCount);

      var list = new List<RI>();
      lbxAllPics.Items.Clear();

      gifurls.ForEach(imgFile => /**/ list.Add(new RI { GifUrl = $"{_urlRoot}{UrlSuffix}/{imgFile}", FileName = Path.GetFileNameWithoutExtension(imgFile) }));
      gifurls.ForEach(imgFile => lbxAllPics.Items.Add(new RI { GifUrl = $"{_urlRoot}{UrlSuffix}/{imgFile}", FileName = Path.GetFileNameWithoutExtension(imgFile) }));      //lbxAllPics.ItemsSource = list;

      chkIsPlaying.Content = $"_{UrlSuffix}      {gifurls.Count} files      {list.First().ImgTime:ddd HH:mm} ÷ {list.Last().ImgTime:ddd HH:mm}";

      Beep.Play();

      await Task.Delay(1000);

      using var timer = new PeriodicTimer(TimeSpan.FromSeconds(_fpsPeriod));
      await RunTimer(timer);
    }
    catch (Exception ex) { Hand.Play(); MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error); }
  }

  async Task RunTimer(PeriodicTimer timer)
  {
    try
    {
      var prv = StartPlaying;
      chkIsPlaying.IsChecked = true;
      StartPlaying = prv;
      var counter = 0;
      while (await timer.WaitForNextTickAsync(_cts.Token))
      {
        if (chkIsPlaying.IsChecked != true || counter > _maxFrames)
        {
          //_cts.Cancel(); // _need to re-new _cts after this.
          WriteLine($"-- Cancelling at counter {counter}   {UrlSuffix}.");
          return;
        }

        var c = ++counter % (lbxAllPics.Items.Count + _pauseFrames);

        if (c == lbxAllPics.Items.Count && StartPlaying == "0")
          chkIsPlaying.IsChecked = false;

        lbxAllPics.SelectedIndex = c >= lbxAllPics.Items.Count ? lbxAllPics.Items.Count : c;
      }
    }
    catch (TaskCanceledException ex) { WriteLine($"++ {ex.Message}       {UrlSuffix}."); Hand.Play(); }
    catch (Exception ex) { WriteLine($"-- {ex.Message}.   {ex.GetType().Name}."); Hand.Play(); }
  }

  public string UrlSuffix { get; set; } = "{name}PRECIPET/GIF/WKR";
  public string PreciTp { get; set; } = "Snow";
  public string StartPlaying { get; set; } = "0";

  async void chkIsPlaying_Checked(object sender, RoutedEventArgs e)
  {
    if (!_loaded) return;
    StartPlaying = "1";
    using var timer = new PeriodicTimer(TimeSpan.FromSeconds(_fpsPeriod));
    await RunTimer(timer);
  }

  void lbxAllPics_GotFocus(object sender, RoutedEventArgs e) => chkIsPlaying.IsChecked = false;
  void lbxAllPics_SelectionChanged(object sender, SelectionChangedEventArgs e) { }
  void lbxAllPics_LostFocus(object sender, RoutedEventArgs e) { }
}