namespace xEnvtCanRadar.Views;

public partial class RadarTypeViewUserControl : UserControl
{
  public RadarTypeViewUserControl() { InitializeComponent(); }

  async void OnLoaded(object sender, RoutedEventArgs e)
  {
    try
    {
      var gifurls = await (new WebDirectoryLoader()).ParseFromHtmlUsingRegex(RootUrl, PreciTp);
      tbxTitle.Text = $"{RootUrl}   {gifurls.Count} files";
      foreach (var imgFile in gifurls)
      {
        lbx.Items.Add(new RI { GifUrl = $"{RootUrl}/{imgFile}", FileName = Path.GetFileNameWithoutExtension(imgFile), ImgTime = getTime(imgFile) });
      }
      Beep.Play();

      await Task.Delay(1000);
      chkIsPlaying.IsChecked = true;

      var cts = new CancellationTokenSource();

      using var timer = new PeriodicTimer(TimeSpan.FromSeconds(.04));
      var counter = 0;
      var pause = 25;

      while (await timer.WaitForNextTickAsync(cts.Token))
      {
        //if (counter == 555)            cts.Cancel();

        if (chkIsPlaying.IsChecked != true) continue;

        var c = ++counter % (lbx.Items.Count+pause);

        lbx.SelectedIndex = c >= lbx.Items.Count  ? lbx.Items.Count : c;
      }
    }
    catch (Exception ex) { Hand.Play(); MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error); }
  }

  DateTimeOffset getTime(string item) => DateTimeOffset.Now;

  public string RootUrl { get; set; } = "https://dd.meteo.gc.ca/radar/PRECIPET/GIF/WKR";
  public string PreciTp { get; set; } = "Snow";
}