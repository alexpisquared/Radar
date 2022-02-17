namespace xEnvtCanRadar.Views;

public partial class RadarTypeViewUserControl : UserControl
{
  const string urlRoot = "https://dd.meteo.gc.ca/radar/";
  const double _fpsPeriod = .04;

  public RadarTypeViewUserControl() => InitializeComponent();

  async void OnLoaded(object s, RoutedEventArgs e) => await ReLoad(31);
  async void OnReload(object s, RoutedEventArgs e) => await ReLoad(int.Parse(((FrameworkElement)s).Tag?.ToString() ?? "0")); // max is 480 == 2 days on 10 per hour basis.
  async Task ReLoad(int takeLastCount)
  {
    chkIsPlaying.IsChecked = false;
    await Task.Delay(TimeSpan.FromSeconds(_fpsPeriod));

    try
    {
      var gifurls = await new WebDirectoryLoader().ParseFromHtmlUsingRegex($"{urlRoot}{UrlSuffix}", PreciTp, takeLastCount);

      var list = new List<RI>();
      lbxAllPics.Items.Clear();

      gifurls.ForEach(imgFile => /**/ list.Add(new RI { GifUrl = $"{urlRoot}{UrlSuffix}/{imgFile}", FileName = Path.GetFileNameWithoutExtension(imgFile) }));
      gifurls.ForEach(imgFile => lbxAllPics.Items.Add(new RI { GifUrl = $"{urlRoot}{UrlSuffix}/{imgFile}", FileName = Path.GetFileNameWithoutExtension(imgFile) }));      //lbxAllPics.ItemsSource = list;

      chkIsPlaying.Content = $"_{UrlSuffix}      {gifurls.Count} files      {list.First().ImgTime:ddd HH:mm} ÷ {list.Last().ImgTime:ddd HH:mm}";

      Beep.Play();

      await Task.Delay(1000);

      var cts = new CancellationTokenSource();

      using var timer = new PeriodicTimer(TimeSpan.FromSeconds(_fpsPeriod));
      var counter = 0;
      var pause = 25;
      var prv = StartPlaying;
      chkIsPlaying.IsChecked = true;
      StartPlaying = prv;
      while (await timer.WaitForNextTickAsync(cts.Token))
      {
        //if (counter == 555)            cts.Cancel();

        if (chkIsPlaying.IsChecked != true) return;

        var c = ++counter % (lbxAllPics.Items.Count + pause);

        if (c == lbxAllPics.Items.Count && StartPlaying == "0")
          chkIsPlaying.IsChecked = false;

        lbxAllPics.SelectedIndex = c >= lbxAllPics.Items.Count ? lbxAllPics.Items.Count : c;
      }
    }
    catch (Exception ex) { Hand.Play(); MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error); }
  }

  public string UrlSuffix { get; set; } = "{name}PRECIPET/GIF/WKR";
  public string PreciTp { get; set; } = "Snow";
  public string StartPlaying { get; set; } = "0";

  void chkIsPlaying_Checked(object sender, RoutedEventArgs e) => StartPlaying = "1";
  void lbxAllPics_GotFocus(object sender, RoutedEventArgs e) => chkIsPlaying.IsChecked = false;
  void lbxAllPics_SelectionChanged(object sender, SelectionChangedEventArgs e) { }
  void lbxAllPics_LostFocus(object sender, RoutedEventArgs e) { }
}