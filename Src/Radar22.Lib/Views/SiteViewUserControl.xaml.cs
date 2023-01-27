using AmbienceLib;

namespace xEnvtCanRadar.Views;

public partial class SiteViewUserControl : UserControl
{
  readonly Bpr bpr = new();
  public SiteViewUserControl()
  {
    InitializeComponent();
    DataContext = this;
  }
  async void OnLoaded(object sender, RoutedEventArgs e)
  {
    try
    {
      var gifurls = await (new WebDirectoryLoader()).ParseFromHtmlUsingRegex(RootUrl);
      tbxTitle.Text = $"{RootUrl}   {gifurls.Count} files";
      foreach (var imgFile in gifurls)
      {
        lbx.Items.Add(new Logic.RI { GifUrl = $"{RootUrl}/{imgFile}", FileName = Path.GetFileNameWithoutExtension(imgFile)});
      }

      bpr.Tick();
    }
    catch (Exception ex) { bpr.Error(); MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error); }
  }

  DateTimeOffset getTime(string item) => DateTimeOffset.Now;

  public string RootUrl { get; set; } = "PRECIPET/GIF/WKR";
}