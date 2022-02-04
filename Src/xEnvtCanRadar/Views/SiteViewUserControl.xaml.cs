namespace xEnvtCanRadar.Views;

public partial class SiteViewUserControl : UserControl
{
  public SiteViewUserControl()
  {
    InitializeComponent();
    DataContext = this;
  }
  async void OnLoaded(object sender, RoutedEventArgs e)
  {
    try
    {
      var gifurls = await (new WebDirectoryLoader()).UseRegex(RootUrl);
      tbxTitle.Text = $"Found {gifurls.Count} files";
      foreach (var imgFile in gifurls)
      {
        lbx.Items.Add(new RI { GifUrl = $"{RootUrl}/{imgFile}", FileName =         Path.GetFileNameWithoutExtension( imgFile), ImgTime = getTime(imgFile) });
      }
    }
    catch (Exception ex)    {      MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);    }
  }

  DateTimeOffset getTime(string item) => DateTimeOffset.Now;

  public string RootUrl { get; set; } = "https://dd.meteo.gc.ca/radar/PRECIPET/GIF/WKR";
}