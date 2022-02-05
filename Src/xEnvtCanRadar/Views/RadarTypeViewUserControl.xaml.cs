namespace xEnvtCanRadar.Views;

public partial class RadarTypeViewUserControl : UserControl
{
  public RadarTypeViewUserControl() { InitializeComponent(); }

  async void OnLoaded(object sender, RoutedEventArgs e)
  {
    try
    {
      var gifurls = await (new WebDirectoryLoader()).ParseFromHtmlUsingRegex(RootUrl);
      tbxTitle.Text = $"{RootUrl}   {gifurls.Count} files";
      foreach (var imgFile in gifurls.Where(r => r.EndsWith(PreciTp)))
      {
        lbx.Items.Add(new RI { GifUrl = $"{RootUrl}/{imgFile}", FileName = Path.GetFileNameWithoutExtension(imgFile), ImgTime = getTime(imgFile) });
      }

      Beep.Play();
    }
    catch (Exception ex) { Hand.Play(); MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error); }
  }

  DateTimeOffset getTime(string item) => DateTimeOffset.Now;

  public string RootUrl { get; set; } = "https://dd.meteo.gc.ca/radar/PRECIPET/GIF/WKR";
  public string PreciTp { get; set; } = "Snow";
}