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
    if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this)) return; //tu: design mode
    
    try
    {
      var ris = await (new WebDirectoryLoader()).ParseFromHtmlUsingRegex(RootUrl, ".gif");
      tbxTitle.Text = $"{RootUrl}   {ris.Count} files";
      foreach (var ri in ris)
        lbxRadarImages.Items.Add(ri);

      bpr.Tick();
    }
    catch (Exception ex) { bpr.Error(); if (Debugger.IsAttached) Debugger.Break(); else _ = MessageBox.Show(ex.Message, "Error888", MessageBoxButton.OK, MessageBoxImage.Error); }
  }

  public string RootUrl { get; set; } = "PRECIPET/GIF/WKR";
}