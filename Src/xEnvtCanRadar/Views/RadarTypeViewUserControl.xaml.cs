namespace xEnvtCanRadar.Views;

public partial class RadarTypeViewUserControl : UserControl
{
  public RadarTypeViewUserControl() => InitializeComponent();

  async void OnLoaded(object sender, RoutedEventArgs e)
  {
    const string name = "https://dd.meteo.gc.ca/radar/";

    try
    {
      var gifurls = await (new WebDirectoryLoader()).ParseFromHtmlUsingRegex($"{name}{RootUrl}", PreciTp);

      var list = new List<RI>();
      gifurls.ForEach(imgFile => /**/ list.Add(new RI { GifUrl = $"{name}{RootUrl}/{imgFile}", FileName = Path.GetFileNameWithoutExtension(imgFile) }));
      gifurls.ForEach(imgFile => lbx.Items.Add(new RI { GifUrl = $"{name}{RootUrl}/{imgFile}", FileName = Path.GetFileNameWithoutExtension(imgFile) }));
      //lbx.ItemsSource = list;

      //tbxRange.Text = $"";
      tbxTitle.Text = $"{RootUrl}   {gifurls.Count} files   {list.First().ImgTime:ddd HH:mm}  ÷  {list.Last().ImgTime:ddd HH:mm}";

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

        var c = ++counter % (lbx.Items.Count + pause);

        lbx.SelectedIndex = c >= lbx.Items.Count ? lbx.Items.Count : c;
      }
    }
    catch (Exception ex) { Hand.Play(); MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error); }
  }

  public string RootUrl { get; set; } = "{name}PRECIPET/GIF/WKR";
  public string PreciTp { get; set; } = "Snow";
  public string Range { get; set; } = "Snow";
}