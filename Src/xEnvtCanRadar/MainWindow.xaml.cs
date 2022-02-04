namespace xEnvtCanRadar;

public partial class MainWindow : Window
{
  public MainWindow()
  {
    InitializeComponent();
    DataContext = this;
  }
  async void Window_Loaded(object sender, RoutedEventArgs e)
  {
    try
    {
      var url = "https://dd.meteo.gc.ca/radar/PRECIPET/GIF/WKR";
      var gifurls = await (new WebDirectoryLoader()).UseRegex(url);
      Title = $"Found {gifurls.Count} files";
      foreach (var item in gifurls)
      {
        //lbx.Items.Add(item); 
        lbx.Items.Add( new RI { GifUrl = $"{url}/{item}", FileName = item, ImgTime = getTime(item)});
      }
    }
    catch (Exception ex)
    {
      MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
    }
  }

  DateTimeOffset getTime(string item)
  {
    return //DateTimeOffset.Parse(item) ?? 
      DateTimeOffset.Now;
  }

  void Button_Click(object sender, RoutedEventArgs e) => Close();
}
